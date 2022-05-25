using System;
using System.Collections.Generic;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.Utility;
using UnityEngine;
using XNode;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;
using Node = Yarn.Node;

namespace SocksTool.Editor.CustomEditors.Builders
{
    public static class YarnToDialogueGraphBuilder
    {
        public static DialogueGraph Build(string yarnAssetPath)
        {
            CompilationResult result = CompileYarnFile(yarnAssetPath);

            Dictionary<string, List<NodePort>> jumpDictionary = new Dictionary<string, List<NodePort>>();

            DialogueGraph dialogueGraph = ScriptableObject.CreateInstance<DialogueGraph>();
            
            int nodeCount = 0;
            foreach ((string _, Node node) in result.Program.Nodes)
            {
                DebugLabels(node);
                
                OptionNode currentOptionNode = null;

                bool popped = false;

                int width    = 0;
                int maxWidth = 0;
                int depth    = 0;

                StartNode startNode = ScriptableObject.CreateInstance<StartNode>();
                startNode.Title = node.Name;

                if (jumpDictionary.TryGetValue(node.Name, out List<NodePort> nodePorts))
                {
                    NodePort input = startNode.GetInputPort(StartNode.InputFieldName);
                    foreach (NodePort nodePort in nodePorts) { nodePort.Connect(input); }
                }

                NodeTree        nodeTree        = new NodeTree(new List<XNode.Node> { startNode }, startNode.GetOutputPort(StartNode.OutputFieldName));
                List<NodeTree>  openNodeQueues  = new List<NodeTree>();
                List<NodePort>  openOutputPorts = new List<NodePort>();
                Stack<NodeTree> nodeTreeStack   = new Stack<NodeTree>();

                nodeTreeStack.Push(nodeTree);

                void AddWidth(int num = 1)
                {
                    width += num;
                    if (width > maxWidth) { maxWidth = width; }
                }

                void RemoveWidth(int num = 1) { width -= num; }

                void AddNewNode<T>(T newNode, string inputFieldName, string outputFieldName = null) where T : XNode.Node
                {
                    // Add nodes to graph
                    XNode.Node.graphHotfix = dialogueGraph;
                    newNode.graph          = dialogueGraph;
                    dialogueGraph.nodes.Add(newNode);

                    // Add nodes to compile structure
                    NodeTree currentNodeTree = nodeTreeStack.Peek();
                    currentNodeTree.Nodes.Add(newNode);

                    // Connect newly created node to last output if it exists
                    NodePort nodePort = currentNodeTree.LastOutputPort;
                    if (nodePort != null)
                    {
                        NodePort input = newNode.GetInputPort(inputFieldName);
                        nodePort.Connect(input);
                    }

                    // If there are open output port connect them to the new node as well
                    if (openOutputPorts.Count > 0)
                    {
                        List<NodePort> outputPorts = new List<NodePort>(openOutputPorts);
                        openOutputPorts.Clear();
                        XNode.Node nodeToConnectTo = newNode;
                        
                        width = maxWidth;
                        if (nodeToConnectTo as MultiInputNode == null)
                        {
                            nodeToConnectTo = ScriptableObject.CreateInstance<LineNodeMerger>();

                            AddWidth();
                            AddNewNode(nodeToConnectTo, LineNodeMerger.InputFieldName, LineNodeMerger.OutputFieldName);
                         
                            
                            NodePort newNodeInput = newNode.GetInputPort(inputFieldName);
                            NodePort output       = nodeToConnectTo.GetOutputPort(LineNodeMerger.OutputFieldName);
                            output.Connect(newNodeInput);
                        }

                        NodePort input = nodeToConnectTo.GetInputPort(LineNodeMerger.InputFieldName);
                        foreach (NodePort openOutputPort in outputPorts) { openOutputPort?.Connect(input); }
                    }

                    // Get new output port form new node
                    if (outputFieldName != null) { currentNodeTree.LastOutputPort = newNode.GetOutputPort(outputFieldName); }
                    else { nodeTreeStack.Peek().LastOutputPort                    = null; }

                    // Set positions of nodes
                    newNode.position.x = width * 350;
                    newNode.position.y = -depth * 200 + 800 * nodeCount;
                    AddWidth();
                }

                void PopHandler()
                {
                    if (!popped) { return; }

                    foreach (NodeTree openNodeQueue in openNodeQueues)
                    {
                        Debug.Log(openNodeQueue.LastOutputPort);
                        openOutputPorts.Add(openNodeQueue.LastOutputPort);
                    }

                    openNodeQueues.Clear();
                    popped = false;
                }

                AddNewNode(startNode, StartNode.InputFieldName, StartNode.OutputFieldName);

                int programCounter = 0;
                foreach (Instruction instruction in node.Instructions)
                {
                    DebugInstruction(instruction, result.StringTable, programCounter);

                    switch (instruction.Opcode)
                    {
                        case Instruction.Types.OpCode.RunLine:
                            PopHandler();

                            LineNode lineNode = CreateRunLineNode(instruction, result.StringTable);
                            AddNewNode(lineNode, LineNode.InputFieldName, LineNode.OutputFieldName);
                            break;
                        case Instruction.Types.OpCode.AddOption:
                            if (currentOptionNode == null)
                            {
                                currentOptionNode = ScriptableObject.CreateInstance<OptionNode>();
                                AddNewNode(currentOptionNode, OptionNode.InputFieldName);
                            }

                            string operandStringValue = YarnUtility.GetOperandStringValue(instruction.Operands);

                            NodePort output = currentOptionNode.AddOption(result.StringTable[operandStringValue].text);

                            nodeTreeStack.Peek().SubNodes.Push(new NodeTree(new List<XNode.Node>(), output));
                            depth++;
                            break;
                        case Instruction.Types.OpCode.JumpTo:
                            NodeTree tree = nodeTreeStack.Pop();
                            openNodeQueues.Add(tree);
                            popped =  false;
                            RemoveWidth(tree.Nodes.Count);
                            depth--;
                            break;

                        case Instruction.Types.OpCode.ShowOptions:
                            currentOptionNode = null;
                            foreach (NodeTree subNode in nodeTreeStack.Peek().SubNodes) { nodeTreeStack.Push(subNode); }

                            break;

                        case Instruction.Types.OpCode.Pop:
                            popped = true;
                            break;

                        case Instruction.Types.OpCode.PushString:
                            string jumpNode = YarnUtility.GetOperandStringValue(instruction.Operands);
                            if (!jumpDictionary.ContainsKey(jumpNode)) { jumpDictionary.Add(jumpNode, new List<NodePort>()); }

                            jumpDictionary[jumpNode].Add(nodeTreeStack.Peek().LastOutputPort);
                            break;

                        case Instruction.Types.OpCode.Stop:
                            PopHandler();
                            EndNode endNode = ScriptableObject.CreateInstance<EndNode>();
                            AddNewNode(endNode, EndNode.InputFieldName);
                            break;
                    }

                    programCounter++;
                }

                nodeCount++;
            }

            return dialogueGraph;
        }

        private static CompilationResult CompileYarnFile(string path)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(path);

            CompilationResult result;
            try { result = Compiler.Compile(compilationJob); }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Failed to compile Yarn Script: " + path);
                throw;
            }

            return result;
        }

        private static LineNode CreateRunLineNode(Instruction instruction, IDictionary<string, StringInfo> stringTable)
        {
            LineNode lineNode = ScriptableObject.CreateInstance<LineNode>();

            string operandStringValue = YarnUtility.GetOperandStringValue(instruction.Operands);

            MarkupParseResult markupParseResult = YarnUtility.ParseMarkup(stringTable[operandStringValue].text);

            if (markupParseResult.TryGetAttributeWithName("character", out MarkupAttribute characterNameAttribute))
            {
                lineNode.Character = YarnUtility.GetPropertyStringValue(characterNameAttribute.Properties, "name", "");
                lineNode.Text      = markupParseResult.DeleteRange(characterNameAttribute).Text;
            }
            else { lineNode.Text = markupParseResult.Text; }

            return lineNode;
        }

        private static void DebugLabels(Node node)
        {
            Debug.Log("Labels:");
            foreach ((string labelName, int instructionNumber) in node.Labels)
            {
                Debug.Log("- " + labelName + " at instruction " + instructionNumber);
            }
        }
        
        private static void DebugInstruction(Instruction instruction, IDictionary<string, StringInfo> stringTable, int count)
        {
            Debug.Log(count + " OP Code: " + instruction.Opcode);
            DebugOperands(instruction.Operands, stringTable);
        }

        private static void DebugOperands(IEnumerable<Operand> operands, IDictionary<string, StringInfo> stringTable)
        {
            foreach (Operand operand in operands)
            {
                switch (operand.ValueCase)
                {
                    case Operand.ValueOneofCase.None:
                        Debug.Log("--- None Value");
                        break;
                    case Operand.ValueOneofCase.StringValue:
                        Debug.Log("--- String Value: " + operand.StringValue);
                        if (stringTable.TryGetValue(operand.StringValue, out StringInfo stringInfo)) { Debug.Log("--- " + stringInfo.text); }

                        break;
                    case Operand.ValueOneofCase.BoolValue:
                        Debug.Log("--- Bool Value: " + operand.BoolValue);
                        break;
                    case Operand.ValueOneofCase.FloatValue:
                        Debug.Log("--- Float Value: " + operand.FloatValue);
                        break;
                }
            }
        }
    }
}