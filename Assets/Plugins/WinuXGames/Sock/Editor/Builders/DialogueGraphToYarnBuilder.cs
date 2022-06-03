using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinuXGames.Sock.Editor.NodeGraphs;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Nodes.Core;
using WinuXGames.Sock.Editor.Utility;
using XNode;

namespace WinuXGames.Sock.Editor.Builders
{
    internal static class DialogueGraphToYarnBuilder
    {
        public static string Build(DialogueGraph dialogueGraph, bool includeSockTags = true)
        {
            StringBuilder sb = new StringBuilder();

            List<StartNode>     startNodes    = dialogueGraph.nodes.OfType<StartNode>().ToList();
            Stack<OpenPathInfo> openPathStack = new Stack<OpenPathInfo>();

            // Cache
            List<NodePort> dynamicOutputs = new List<NodePort>();
            foreach (StartNode startNode in startNodes)
            {
                startNode.GetText(sb, 0, includeSockTags);
                SockNode currentNode  = null;
                NodePort output       = startNode.GetOutputPort(StartNode.OutputFieldName);
                NodePort connectedTo  = output.GetConnection(0);
                bool     isLastInPath = true;

                bool stop = false;

                void StopCurrentExecutionPath()
                {
                    if (!openPathStack.TryPop(out OpenPathInfo info))
                    {
                        stop = true; // If the stack is empty stop the loop
                        return;
                    }

                    info.Node.GetText(sb, info.Index, includeSockTags);

                    currentNode  = info.Node;
                    connectedTo  = info.NodePort;
                    isLastInPath = info.LastInPath;
                }

                int iterationLimiter = SockConstants.IterationLimit;
                while (iterationLimiter > 0 && !stop)
                {
                    switch (connectedTo.node)
                    {
                        case LineNode lineNode:
                        {
                            lineNode.GetText(sb, 0, includeSockTags);
                            NodePort lineNodeOutput = lineNode.GetOutputPort(LineNode.OutputFieldName);

                            // If this node has no output connection go to the next open path
                            if (lineNodeOutput.ConnectionCount == 0)
                            {
                                StopCurrentExecutionPath();
                                break;
                            }

                            connectedTo = lineNode.GetOutputPort(LineNode.OutputFieldName).GetConnection(0);

                            // Add line merger tag if sock tags are included
                            if (includeSockTags)
                            {
                                NodePort       lineNodeInput       = lineNode.GetInputPort(SockNode.InputFieldName);
                                Node           connectedToLineNode = lineNodeInput.GetConnection(0).node;
                                LineMergerNode lineMergerNode      = connectedToLineNode as LineMergerNode;
                                if (lineMergerNode != null)
                                {
                                    string s = sb.ToString().TrimEnd();
                                    sb.Clear();
                                    sb.Append(s);
                                    lineMergerNode.AddPositionTag(sb, SockConstants.SockLineMergerNodePositionTag);
                                    sb.AppendLine();
                                }
                            }

                            break;
                        }

                        case OptionNode optionNode:
                        {
                            dynamicOutputs.Clear();
                            dynamicOutputs.AddRange(optionNode.DynamicOutputs);

                            for (int i = dynamicOutputs.Count - 1; i >= 0; i--)
                            {
                                NodePort optionNodeDynamicOutput = dynamicOutputs[i];
                                NodePort connectedToPort         = optionNodeDynamicOutput.GetConnection(0);
                                openPathStack.Push(new OpenPathInfo(optionNode, connectedToPort, i, i == dynamicOutputs.Count - 1));
                            }

                            StopCurrentExecutionPath();
                            break;
                        }

                        case LineMergerNode lineNodeMerger:
                        {
                            if (isLastInPath)
                            {
                                NodePort connectedToPort = lineNodeMerger.GetOutputPort(LineMergerNode.OutputFieldName).GetConnection(0);
                                openPathStack.Push(new OpenPathInfo(lineNodeMerger, connectedToPort, 0, true));
                            }

                            StopCurrentExecutionPath();
                            break;
                        }

                        case StartNode sNode:
                            if (currentNode != null) { currentNode.AddIndent(sb, 1); }

                            sb.Append("<<jump ");
                            sb.Append(sNode.Title);
                            sb.Append(">>");
                            sb.AppendLine();
                            StopCurrentExecutionPath();
                            break;

                        case StopNode stopNode:
                        {
                            NodePort inputPort = stopNode.GetInputPort(SockNode.InputFieldName);
                            
                            // Only include tags if enabled and input node is not a start node (this only really happens when creating a new yarn file)
                            if (includeSockTags && inputPort.GetConnection(0).node as StartNode == null)
                            {
                                string s = sb.ToString().TrimEnd();
                                sb.Clear();
                                sb.Append(s);
                                sb.Append(' ');
                                stopNode.AddPositionTag(sb, SockConstants.SockStopNodePositionTag);
                                sb.AppendLine();
                            }

                            if (inputPort.ConnectionCount == 1)
                            {
                                if (openPathStack.Count != 0) { stopNode.GetText(sb); }
                            }

                            StopCurrentExecutionPath();
                            break;
                        }
                    }
                    iterationLimiter--;
                }

                // Close Node
                sb.Append("===");
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private class OpenPathInfo
        {
            public int  Index      { get; }
            public bool LastInPath { get; }

            public SockNode Node     { get; }
            public NodePort NodePort { get; }

            public OpenPathInfo(SockNode node, NodePort nodePort, int index, bool lastInPath)
            {
                NodePort   = nodePort;
                Index      = index;
                Node       = node;
                LastInPath = lastInPath;
            }
        }
    }
}