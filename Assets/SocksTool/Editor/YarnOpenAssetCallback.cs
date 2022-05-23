using System;
using System.Collections.Generic;
using System.Linq;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.Utility;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;
using Node = Yarn.Node;

namespace SocksTool.Editor
{
    public static class YarnOpenAssetCallback
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenYarnAsset(int instanceID, int line)
        {
            if (Selection.activeObject == null) { return false; }

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!path.EndsWith(".yarn")) { return false; }

            OpenYarnFile(path);
            return true;
        }

        public static void OpenYarnFile(string assetPath)
        {
            CompilationResult result = CompileYarnFile(assetPath);

            DialogueGraph dialogueGraph = ScriptableObject.CreateInstance<DialogueGraph>();
            
            foreach ((string key, Node node) in result.Program.Nodes)
            {
                Debug.Log("Key: " + key);
                Debug.Log("labels"); 
                foreach ((string s, int value) in node.Labels)
                {
                    Debug.Log(s + " " + value);
                }
                ProcessNode(node, dialogueGraph, result);
            }

            NodeEditorWindow.Open(dialogueGraph);
        }


        private static CompilationResult CompileYarnFile(string path)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(path);

            CompilationResult result;
            try
            {
                result = Compiler.Compile(compilationJob);
                foreach ((string key, Node value) in result.Program.Nodes)
                {
                    Debug.Log("Key " + key);
                    Debug.Log(value.Name);
                }
            }
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

        private static OptionNode CreateAddOptionNode(Instruction instruction, IDictionary<string, StringInfo> stringTable)
        {
            OptionNode optionNode = ScriptableObject.CreateInstance<OptionNode>();

            string operandStringValue = YarnUtility.GetOperandStringValue(instruction.Operands);
            optionNode.Options.Add(stringTable[operandStringValue].text);
            
            return optionNode;
        }

        private static void DebugInstruction(Instruction instruction, IDictionary<string, StringInfo> stringTable)
        {
            Debug.Log("OP Code: " + instruction.Opcode);
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
                        if (stringTable.TryGetValue(operand.StringValue, out StringInfo stringInfo))
                        {
                            Debug.Log("--- " + stringInfo.text);
                        }
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

        private static void ProcessNode(Node node, NodeGraph nodeGraph, CompilationResult result)
        {
            StartNode startNode = ScriptableObject.CreateInstance<StartNode>();
            startNode.Title = node.Name;
            nodeGraph.nodes.Add(startNode);

            int                   nodeCount         = 1;
            LineNode              previousLineNode  = null;
            OptionNode            currentOptionNode = null;
            Stack<List<NodePort>> nodePortStack     = new Stack<List<NodePort>>();
            Stack<int>            stack             = new Stack<int>();
            foreach (Instruction instruction in node.Instructions)
            {
                DebugInstruction(instruction, result.StringTable);
                
                NodePort input;
                NodePort output;
                switch (instruction.Opcode)
                {
                    case Instruction.Types.OpCode.RunLine:
                        LineNode lineNode = CreateRunLineNode(instruction, result.StringTable);
                        lineNode.position.x = nodeCount * 350;
                        
                        input  = lineNode.GetInputPort(LineNode.InputFieldName);
                        if (stack.Count > 0)
                        {
                            Debug.Log(stack.Peek());
                            Debug.Log(nodePortStack.Peek()[stack.Peek()]);
                            output = nodePortStack.Peek()[stack.Peek()];
                        }
                        else
                        {
                            output = previousLineNode == null ? startNode.GetOutputPort(StartNode.OutputFieldName) : previousLineNode.GetOutputPort(LineNode.OutputFieldName);
                        }
                        
                        output.Connect(input);
                        
                        nodeGraph.nodes.Add(lineNode);
                        nodeCount++;
                        previousLineNode = lineNode;
                        break;
                    case Instruction.Types.OpCode.AddOption:
                        if (currentOptionNode == null)
                        {
                            currentOptionNode            = ScriptableObject.CreateInstance<OptionNode>();
                            currentOptionNode.position.x = nodeCount * 350;
                            input                        = currentOptionNode.GetInputPort(OptionNode.InputFieldName);
                            output                       = previousLineNode.GetOutputPort(LineNode.OutputFieldName);
                            output.Connect(input);
                            nodePortStack.Push(new List<NodePort>());
                            nodeGraph.nodes.Add(currentOptionNode);
                        }

                        currentOptionNode.AddDynamicOutput(typeof(string), XNode.Node.ConnectionType.Override, XNode.Node.TypeConstraint.None,
                            OptionNode.OptionsFieldName + " " + currentOptionNode.Options.Count);
                        
                        string operandStringValue = YarnUtility.GetOperandStringValue(instruction.Operands);
                        currentOptionNode.Options.Add(result.StringTable[operandStringValue].text);
                        currentOptionNode.UpdatePorts();

                        Debug.Log("Dynamic output ports");
                        foreach (NodePort dynamicOutput in currentOptionNode.DynamicOutputs)
                        {
                            Debug.Log(dynamicOutput);
                        }
                        
                        Debug.Log(currentOptionNode.DynamicOutputs.GetEnumerator().Current);
                        
                        nodePortStack.Peek().Add(currentOptionNode.DynamicOutputs.ToList()[currentOptionNode.DynamicOutputs.Count()-1]);
                        break;
                    case Instruction.Types.OpCode.JumpTo:
                        int number = stack.Pop();
                        stack.Push(number+1);
                        break;
                    case Instruction.Types.OpCode.ShowOptions: 
                        stack.Push(0);
                        break;
                    case Instruction.Types.OpCode.Pop:
                        stack.Pop();
                        nodePortStack.Pop();
                        break;
                }
            }
            
            
        }
    }
}