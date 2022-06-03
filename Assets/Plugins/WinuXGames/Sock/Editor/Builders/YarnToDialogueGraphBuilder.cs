using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WinuXGames.Sock.Editor.NodeSystem.NodeGraphs;
using WinuXGames.Sock.Editor.NodeSystem.Nodes;
using WinuXGames.Sock.Editor.NodeSystem.Nodes.Core;
using WinuXGames.Sock.Editor.Utility;
using XNode;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;
using Node = XNode.Node;

namespace WinuXGames.Sock.Editor.Builders
{
    public class YarnToDialogueGraphBuilder
    {
        /// <summary>
        /// Horizontal and vertical spacing between nodes automatically created nodes
        /// </summary>
        public Vector2 Spacing { get; set; } = new Vector2(350, 200);

        /// <summary>
        /// Offset is added each time a node is created (default is 8 on both axis zo align them to the xNode grid)
        /// </summary>
        public Vector2 SpacingOffset { get; set; } = new Vector2(8, 8);

        private readonly Dictionary<string, Node>           _nodeLookup              = new Dictionary<string, Node>();
        private readonly Dictionary<string, List<NodePort>> _jumpLookup              = new Dictionary<string, List<NodePort>>();
        private readonly Dictionary<Node, StringInfo>       _nodeStringInfoLookup    = new Dictionary<Node, StringInfo>();
        private readonly List<Node>                         _nodesWithoutPositionTag = new List<Node>();
        private          DialogueGraph                      _dialogueGraph;
        private          Vector2                            _nodeCursor;
        private          Vector2                            _nodeCursorMax;

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
                //DebugLabels(node);

                _nodeCursor.x = 0;
                _nodeCursor.y = _nodeCursorMax.y;

                // Convert node metadata to string info for easier tag usage
                StringInfo nodeStringInfo = new StringInfo { metadata = node.Tags.ToArray() };

                // Add and setup start node
                TryAddNode("Start_" + node.Name, out StartNode startNode, nodeStringInfo, SockConstants.SockStartNodePositionTag);
                startNode.Title = node.Name;

                // Add tags to start node
                foreach (string tag in node.Tags)
                {
                    if (tag.StartsWith(SockConstants.SockTagPrefix)) { continue; }

                    startNode.Tags.Add(tag);
                }

                // Are there any outputs waiting for this node, if yes connect them to it
                if (_jumpLookup.TryGetValue(node.Name, out List<NodePort> jumpOutputs))
                {
                    NodePort startNodeInput = startNode.GetInputPort(SockNode.InputFieldName);
                    foreach (NodePort jumpOutput in jumpOutputs) { jumpOutput.Connect(startNodeInput); }
                }

                Stack<string>        stringStack       = new Stack<string>();
                Stack<OpenPathInfo>  openPaths         = new Stack<OpenPathInfo>();
                List<LineMergerNode> lineMergers       = new List<LineMergerNode>();
                OptionNode           currentOptionNode = null;
                NodePort             currentOutput     = startNode.GetOutputPort(StartNode.OutputFieldName);

                bool stop           = false;
                int  programCounter = 0;
                int  depth          = 0;
                int  iterationLimit = SockConstants.IterationLimit;
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
                            depth          = openPathInfo.Depth;
                            _nodeCursor    = openPathInfo.NodeCursor;
                            programCounter = YarnUtility.GetProgramCounterFromLabel(node.Labels, openPathInfo.Label);
                        }
                        else { stop = true; }
                    }

                    switch (instruction.Opcode)
                    {
                        case Instruction.Types.OpCode.JumpTo:
                            programCounter = YarnUtility.GetProgramCounterFromLabel(node.Labels, instruction.Operands[0].StringValue);
                            break;
                        case Instruction.Types.OpCode.RunLine:
                        {
                            string     stringKey  = instruction.Operands[0].StringValue;
                            StringInfo stringInfo = result.StringTable[stringKey];
                            string     actualText = stringInfo.text;
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
                                if (TryAddNode(stringKey + "_Merger", out LineMergerNode lineMergerNode))
                                {
                                    NodePort lineMergerOutput   = lineMergerNode.GetOutputPort(LineMergerNode.OutputFieldName);
                                    NodePort lineNodeInput      = lineNode.GetInputPort(SockNode.InputFieldName);
                                    NodePort previousConnection = lineNodeInput.GetConnection(0);

                                    // Get position from line node if it has the corresponding tag
                                    if (_nodeStringInfoLookup.TryGetValue(lineNodeInput.node, out StringInfo lineNodeStringInfo))
                                    {
                                        lineMergerNode.position = GetNodePositionFromStringInfo(lineNodeStringInfo, out _, SockConstants.SockLineMergerNodePositionTag);
                                    }

                                    // Connect the node that was previously connected to the line node to the  merger
                                    previousConnection.Connect(lineMergerNode.GetInputPort(SockNode.InputFieldName));

                                    // Connect merger to already existing line node (previous connection will automatically be disconnected)
                                    lineMergerOutput.Connect(lineNodeInput);

                                    lineMergers.Add(lineMergerNode);
                                }

                                currentOutput.Connect(lineMergerNode.GetInputPort(SockNode.InputFieldName));

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
                                if (TryAddNode(stringKey, out currentOptionNode, stringInfo)) { currentOutput.Connect(currentOptionNode.GetInputPort(SockNode.InputFieldName)); }
                                else // If the option node already exists end the current execution path and break out of the switch flow
                                {
                                    StopCurrentExecutionPath();
                                    break;
                                }
                            }

                            // Add new option to option node and add the resulting output port to the open paths stack
                            NodePort dynamicOutput = currentOptionNode.AddOption(stringInfo.text);
                            openPaths.Push(new OpenPathInfo(dynamicOutput, _nodeCursor, depth+1, instruction.Operands[1].StringValue));
                            ModifyNodeCursor(new Vector2(0, 1));
                            
                            programCounter++;
                            break;
                        }
                        case Instruction.Types.OpCode.ShowOptions:
                            currentOptionNode = null;

                            StopCurrentExecutionPath();
                            break;
                        case Instruction.Types.OpCode.PushString:
                            stringStack.Push(instruction.Operands[0].StringValue);
                            programCounter++;
                            break;
                        case Instruction.Types.OpCode.Pop:
                            depth--;
                            programCounter++;
                            break;
                        case Instruction.Types.OpCode.Stop:
                            TryAddNode($"Stop_{node.Name}_{depth}", out StopNode stopNode);
                            currentOutput.Connect(stopNode.GetInputPort(SockNode.InputFieldName));

                            if (currentOutput.node.position.x >= stopNode.position.x)
                            {
                                stopNode.position.x = currentOutput.node.position.x + Spacing.x;
                            }
                            
                            if (currentOutput.node.position.y > stopNode.position.y)
                            {
                                stopNode.position.y = currentOutput.node.position.y + Spacing.y;
                            }
                            
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

                if (iterationLimit == 0) { throw new OverflowException("Iteration limit has been reached, the yarn file is either invalid or too big"); }

                PositionLineMergersAndChildren(lineMergers, openPaths);
            }

            _dialogueGraph.Ready = true;
            return _dialogueGraph;
        }

        private void PositionLineMergersAndChildren(List<LineMergerNode> lineMergers, Stack<OpenPathInfo> openPaths)
        {
            foreach (LineMergerNode lineMergerNode in lineMergers)
            {
                Vector2 position = new Vector2(float.MinValue, float.MaxValue);

                NodePort lineMergerInput = lineMergerNode.GetInputPort(SockNode.InputFieldName);
                foreach (Vector2 inputPosition in lineMergerInput.GetConnections().Select(nodePort => nodePort.node.position))
                {
                    if (inputPosition.x > position.x) { position.x = inputPosition.x; }

                    if (inputPosition.y < position.y) { position.y = inputPosition.y; }
                }

                // Only position line mergers without a position tag
                if (_nodesWithoutPositionTag.Contains(lineMergerNode))
                {
                    _nodeCursor             = new Vector2(Mathf.Floor(position.x / Spacing.x), Mathf.Floor(position.y / Spacing.y));
                    lineMergerNode.position = GetPositionFromNodeCursor();
                }

                NodePort connectedTo = lineMergerNode.GetOutputPort(LineMergerNode.OutputFieldName);
                openPaths.Clear();
                bool stop = false;

                void Pop()
                {
                    if (!openPaths.TryPop(out OpenPathInfo openPathInfo))
                    {
                        stop = true;
                        return;
                    }

                    connectedTo = openPathInfo.NodePort;

                    _nodeCursor = openPathInfo.NodeCursor;
                }

                int iterationLimit = SockConstants.IterationLimit;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse (it just lies, stop is being set in the Pop method)
                while (!stop && iterationLimit > 0)
                {
                    // Only position nodes without a position tag
                    if (_nodesWithoutPositionTag.Contains(connectedTo.node))
                    {
                        ModifyNodeCursor(new Vector2(1, 0));
                        connectedTo.node.position = GetPositionFromNodeCursor();
                    }

                    if (connectedTo.node is OptionNode optionNode)
                    {
                        foreach (NodePort optionNodeDynamicOutput in optionNode.DynamicOutputs)
                        {
                            openPaths.Push(new OpenPathInfo(optionNodeDynamicOutput, _nodeCursor));
                            ModifyNodeCursor(new Vector2(0, 1));
                        }

                        Pop();
                        break;
                    }

                    if (!connectedTo.node.Outputs.Any())
                    {
                        Pop();
                        break;
                    }

                    NodePort nodePort = connectedTo.node.Outputs.First();
                    if (nodePort == null || nodePort.ConnectionCount == 0)
                    {
                        Pop();
                        break;
                    }

                    connectedTo = nodePort.GetConnection(0);

                    iterationLimit--;
                }

                if (iterationLimit == 0) { throw new OverflowException("There appears to be a circular reference inside the node tree"); }
            }
        }

        private void ResetState()
        {
            _nodeLookup.Clear();
            _jumpLookup.Clear();
            _nodeStringInfoLookup.Clear();

            _nodesWithoutPositionTag.Clear();
            _nodeCursor    = Vector2.zero;
            _nodeCursorMax = Vector2.zero;
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
        private bool TryAddNode<T>(string stringKey, out T node, StringInfo stringInfo = default, string positionTagFilter = SockConstants.SockPositionTag) where T : Node, new()
        {
            if (_nodeLookup.TryGetValue(stringKey, out Node lookupNode))
            {
                node = lookupNode as T;
                return false;
            }

            node          = ScriptableObject.CreateInstance<T>();
            node.position = GetNodePositionFromStringInfo(stringInfo, out bool hasTag, positionTagFilter);

            if (!hasTag) { _nodesWithoutPositionTag.Add(node); }

            _nodeStringInfoLookup.Add(node, stringInfo);

            _nodeLookup.Add(stringKey, node);
            AddNodeToGraph(node);

            ModifyNodeCursor(new Vector2(1, 0));
            return true;
        }

        private Vector2 GetNodePositionFromStringInfo(StringInfo stringInfo, out bool tagWasFound, string tagFilter = SockConstants.SockPositionTag)
        {
            Vector2 position = GetPositionFromNodeCursor();
            if (stringInfo.metadata == null)
            {
                tagWasFound = false;
                return position;
            }

            string tag = stringInfo.metadata.FirstOrDefault(s => s.StartsWith(tagFilter + ":"));

            if (tag == null)
            {
                tagWasFound = false;
                return position;
            }

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

            tagWasFound = true;
            return position;
        }

        private Vector2 GetPositionFromNodeCursor() => (_nodeCursor * Spacing) + SpacingOffset;

        private void AddNodeToGraph(Node node)
        {
            _dialogueGraph.nodes.Add(node);
            Node.graphHotfix = _dialogueGraph;
            node.graph       = _dialogueGraph;
        }

        private void ModifyNodeCursor(Vector2 vector2)
        {
            _nodeCursor += vector2;

            if (_nodeCursor.x > _nodeCursorMax.x) { _nodeCursorMax.x = _nodeCursor.x; }

            if (_nodeCursor.y > _nodeCursorMax.y) { _nodeCursorMax.y = _nodeCursor.y; }
        }

        private class OpenPathInfo
        {
            public OpenPathInfo(NodePort nodePort, Vector2 nodeCursor, int depth = 0, string label = "")
            {
                NodePort   = nodePort;
                NodeCursor = nodeCursor;
                Depth      = depth;
                Label      = label;
            }

            public NodePort NodePort   { get; }
            public Vector2  NodeCursor { get; set; }
            public int      Depth      { get; }
            public string   Label      { get; }
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