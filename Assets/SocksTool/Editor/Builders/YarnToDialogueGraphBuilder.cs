using System;
using System.Collections.Generic;
using System.Linq;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.NodeSystem.Utility;
using SocksTool.Runtime.Utility;
using UnityEngine;
using XNode;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;
using Node = XNode.Node;

namespace SocksTool.Editor.Builders
{
    public class YarnToDialogueGraphBuilder
    {
        private const int IterationLimit = 10000;

        /// <summary>
        /// Horizontal and vertical spacing between nodes automatically created nodes
        /// </summary>
        public Vector2 Spacing { get; set; } = new Vector2(350, 150);

        private readonly Dictionary<string, Node>           _nodeLookup           = new Dictionary<string, Node>();
        private readonly Dictionary<string, List<NodePort>> _jumpLookup           = new Dictionary<string, List<NodePort>>();
        private readonly Dictionary<Node, StringInfo>       _nodeStringInfoLookup = new Dictionary<Node, StringInfo>();
        private          DialogueGraph                      _dialogueGraph;
        private          Vector2                            _nodeCursor;

        /// <summary>
        /// Build dialogue graph out of given yarn asset
        /// </summary>
        /// <param name="yarnAssetPath">Path of yarn asset to use</param>
        /// <returns>A dialogue graph populated with nodes read from the yarn file</returns>
        /// <exception cref="OverflowException">This is usually caused by an invalid yarn file, but can also occur if the given file is really large</exception>
        public DialogueGraph Build(string yarnAssetPath)
        {
            ResetState();
           
            _dialogueGraph = ScriptableObject.CreateInstance<DialogueGraph>();

            CompilationResult result = YarnUtility.CompileYarnFile(yarnAssetPath);
            foreach ((string _, Yarn.Node node) in result.Program.Nodes)
            {
                DebugLabels(node);

                // Convert node metadata to string info for easier tag usage
                StringInfo nodeStringInfo = new StringInfo { metadata = node.Tags.ToArray() };

                // Add and setup start node
                TryAddNode("Start_" + node.Name, out StartNode startNode, nodeStringInfo, SockTag.SockStartNodePositionTag);
                startNode.Title = node.Name;

                // Add tags to start node
                foreach (string tag in node.Tags)
                {
                    if (tag.StartsWith(SockTag.SockTagPrefix)) { continue; }

                    startNode.Tags.Add(tag);
                }

                // Are there any outputs waiting for this node, if yes connect them to it
                if (_jumpLookup.TryGetValue(node.Name, out List<NodePort> jumpOutputs))
                {
                    NodePort startNodeInput = startNode.GetInputPort(StartNode.InputFieldName);
                    foreach (NodePort jumpOutput in jumpOutputs) { jumpOutput.Connect(startNodeInput); }
                }

                Stack<string>       stringStack       = new Stack<string>();
                Stack<OpenPathInfo> openPaths         = new Stack<OpenPathInfo>();
                OptionNode          currentOptionNode = null;
                NodePort            currentOutput     = startNode.GetOutputPort(StartNode.OutputFieldName);

                bool stop           = false;
                int  programCounter = 0;
                int  iterationLimit = IterationLimit;
                while (iterationLimit > 0 && !stop)
                {
                    Instruction  instruction = node.Instructions[programCounter];
                    OpenPathInfo openPathInfo;

                    DebugInstruction(instruction, result.StringTable, programCounter);
                    
                    
                    // Stops current execution path and checks if another can be started, if not end loop
                    void StopCurrentExecutionPath()
                    {
                        if (openPaths.Count > 0)
                        {
                            openPathInfo = openPaths.Pop();

                            currentOutput  = openPathInfo.NodePort;
                            programCounter = GetProgramCounterFromLabel(node.Labels, openPathInfo.Label);
                        }
                        else { stop = true; }
                    }

                    switch (instruction.Opcode)
                    {
                        case Instruction.Types.OpCode.JumpTo:
                            programCounter = GetProgramCounterFromLabel(node.Labels, instruction.Operands[0].StringValue);
                            break;
                        case Instruction.Types.OpCode.RunLine:
                        {
                            string     stringKey  = instruction.Operands[0].StringValue;
                            StringInfo stringInfo = result.StringTable[stringKey];
                            string     actualText = stringInfo.text;
                            GetNodePositionFromStringInfo(stringInfo);
                            if (TryAddNode(stringKey, out LineNode lineNode, stringInfo))
                            {
                                // Parse text and look for character attribute and assign it to node if it's there
                                MarkupParseResult markupParseResult = YarnUtility.ParseMarkup(actualText);
                                if (markupParseResult.TryGetAttributeWithName("character", out MarkupAttribute characterNameAttribute))
                                {
                                    lineNode.Character = YarnUtility.GetPropertyStringValue(characterNameAttribute.Properties, "name");
                                    lineNode.Text      = actualText.Remove(characterNameAttribute.Position, characterNameAttribute.Length);
                                }
                                else { lineNode.Text = markupParseResult.Text; }

                                // Connect old output to new input
                                currentOutput.Connect(lineNode.GetInputPort(SockNode.InputFieldName));
                                currentOutput = lineNode.GetOutputPort(LineNode.OutputFieldName);
                            }
                            else // If line node already exists check if a merger already exists and create it if not
                            {
                                // Try to create a merger
                                if (TryAddNode(stringKey + "_Merger", out LineNodeMerger lineNodeMerger))
                                {
                                    NodePort lineMergerOutput   = lineNodeMerger.GetOutputPort(LineNodeMerger.OutputFieldName);
                                    NodePort lineNodeInput      = lineNode.GetInputPort(SockNode.InputFieldName);
                                    NodePort previousConnection = lineNodeInput.GetConnection(0);

                                    // Get position from line node if it has the corresponding tag
                                    if (_nodeStringInfoLookup.TryGetValue(lineNodeInput.node, out StringInfo lineNodeStringInfo))
                                    {
                                        lineNodeMerger.position = GetNodePositionFromStringInfo(lineNodeStringInfo, SockTag.SockLineMergerNodePositionTag);
                                    }
                                    
                                    // Connect merger to already existing line node (previous connection will automatically be disconnected)
                                    lineMergerOutput.Connect(lineNodeInput);

                                    // Connect the node that was previously connected to the line node to the  merger
                                    previousConnection.Connect(lineNodeMerger.GetInputPort(SockNode.InputFieldName));
                                }
                                
                                currentOutput.Connect(lineNodeMerger.GetInputPort(SockNode.InputFieldName));

                                StopCurrentExecutionPath();
                                break;
                            }

                            programCounter++;
                            break;
                        }
                        case Instruction.Types.OpCode.AddOption:
                        {
                            string     stringKey  = instruction.Operands[0].StringValue;
                            StringInfo stringInfo = result.StringTable[stringKey];

                            // Check if there's already an option node active
                            if (currentOptionNode == null)
                            {
                                // Try to add a new option node
                                if (TryAddNode(stringKey, out currentOptionNode, stringInfo)) { currentOutput.Connect(currentOptionNode.GetInputPort(OptionNode.InputFieldName)); }
                                else // If the option node already exists end the current execution path and break out of the switch flow
                                {
                                    StopCurrentExecutionPath();
                                    break;
                                }
                            }

                            // Add new option to option node and add the resulting output port to the open paths stack
                            NodePort dynamicOutput = currentOptionNode.AddOption(stringInfo.text);
                            openPaths.Push(new OpenPathInfo(dynamicOutput, instruction.Operands[1].StringValue));

                            _nodeCursor.y++;
                            programCounter++;
                            break;
                        }
                        case Instruction.Types.OpCode.ShowOptions:
                            currentOptionNode = null;

                            openPathInfo = openPaths.Pop();

                            currentOutput  = openPathInfo.NodePort;
                            programCounter = GetProgramCounterFromLabel(node.Labels, openPathInfo.Label);
                            break;
                        case Instruction.Types.OpCode.PushString:
                            stringStack.Push(instruction.Operands[0].StringValue);
                            programCounter++;
                            break;
                        case Instruction.Types.OpCode.Pop:
                            programCounter++;
                            break;
                        case Instruction.Types.OpCode.Stop:
                            StopCurrentExecutionPath();
                            break;
                        case Instruction.Types.OpCode.RunNode:
                            string key = stringStack.Pop();
                            if (!_jumpLookup.ContainsKey(key)) { _jumpLookup.Add(key, new List<NodePort>()); }

                            _jumpLookup[key].Add(currentOutput);
                            StopCurrentExecutionPath();
                            break;
                        default:
                            programCounter++;
                            break;
                    }

                    iterationLimit--;
                }

                if (iterationLimit == 0)
                {
                    throw new OverflowException("Iteration Limit has been reached! you are either trying to open an invalid yarn file or one that is too big!");
                }
            }

            return _dialogueGraph;
        }

        private void ResetState()
        {
            _nodeLookup.Clear();
            _jumpLookup.Clear();
            _nodeStringInfoLookup.Clear();
            _nodeCursor    = Vector2.zero;
            _dialogueGraph = null;
        }

        /// <summary>
        /// This will try to add a new node with given string key
        /// </summary>
        /// <param name="stringKey">String key to create node with</param>
        /// <param name="node">Will output either the newly created node or the existing one from the lookup table</param>
        /// <param name="stringInfo">String info of current node, default is the default struct</param>
        /// <param name="positionTagFilter">Tag filter for position</param>
        /// <typeparam name="T">A Node Type to search for</typeparam>
        /// <returns>Returns true if the Node was added or false if the node was already found in the lookup</returns>
        private bool TryAddNode<T>(string stringKey, out T node, StringInfo stringInfo = default, string positionTagFilter = SockTag.SockPositionTag) where T : Node, new()
        {
            if (_nodeLookup.TryGetValue(stringKey, out Node lookupNode))
            {
                node = lookupNode as T;
                return false;
            }

            node          = ScriptableObject.CreateInstance<T>();
            node.position = GetNodePositionFromStringInfo(stringInfo, positionTagFilter);

            _nodeStringInfoLookup.Add(node, stringInfo);

            _nodeLookup.Add(stringKey, node);
            AddNodeToGraph(node);

            _nodeCursor.x++;
            return true;
        }

        private Vector2 GetNodePositionFromStringInfo(StringInfo stringInfo, string tagFilter = SockTag.SockPositionTag)
        {
            Vector2 position = _nodeCursor * Spacing;
            if (stringInfo.metadata == null) { return position; }

            string tag = stringInfo.metadata.FirstOrDefault(s => s.StartsWith(tagFilter + ":"));

            if (tag == null) { return position; }

            try
            {
                string   positionString    = tag[(tag.IndexOf(':') + 1)..];
                string[] positionDelimited = positionString.Split(',');
                string   xString           = positionDelimited[0];
                string   yString           = positionDelimited[1];

                position = new Vector2(Convert.ToSingle(xString), Convert.ToSingle(yString));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Malformed position tag on line " + stringInfo.lineNumber + ", using default position");
                Debug.LogWarning(e);
            }

            return position;
        }

        private void AddNodeToGraph(Node node)
        {
            _dialogueGraph.nodes.Add(node);
            Node.graphHotfix = _dialogueGraph;
            node.graph       = _dialogueGraph;
        }

        private int GetProgramCounterFromLabel(IReadOnlyDictionary<string, int> labels, string label)
        {
            if (labels.TryGetValue(label, out int programCounter)) { return programCounter; }

            throw new IndexOutOfRangeException($"Label {label} Does not exist!");
        }

        private class OpenPathInfo
        {
            public OpenPathInfo(NodePort nodePort, string label)
            {
                NodePort = nodePort;
                Label    = label;
            }

            public NodePort NodePort { get; }
            public string   Label    { get; }
        }
        
        private static void DebugLabels(Yarn.Node node)
        {
            Debug.Log("Labels:");
            foreach ((string labelName, int instructionNumber) in node.Labels) { Debug.Log("- " + labelName + " at instruction " + instructionNumber); }
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