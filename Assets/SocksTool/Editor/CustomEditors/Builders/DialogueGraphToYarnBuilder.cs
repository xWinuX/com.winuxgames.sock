using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;
using XNode;
using Yarn.Compiler;

namespace SocksTool.Editor.CustomEditors.Builders
{
    public static class DialogueGraphToYarnBuilder
    {
        private class OpenPathInfo
        {
            public OpenPathInfo(OptionNode node, NodePort nodePort, string optionString)
            {
                NodePort     = nodePort;
                OptionString = optionString;
                Node    = node;
            }

            public OptionNode Node         { get; }
            public NodePort   NodePort     { get; }
            public string     OptionString { get; }
        }
        
        public static string Build(DialogueGraph dialogueGraph)
        {
            StringBuilder sb = new StringBuilder();
            
            List<StartNode>     startNodes      = dialogueGraph.nodes.OfType<StartNode>().ToList();
            Stack<OpenPathInfo> openOptionPaths = new Stack<OpenPathInfo>();

            NodePort Pop()
            {
                if (!openOptionPaths.TryPop(out OpenPathInfo info)) { return null; }

                info.Node.AddIndent(sb);
                sb.Append("-> ");
                sb.Append(info.OptionString);
                sb.AppendLine();
                
                return info.NodePort;
            }
            
            foreach (StartNode startNode in startNodes)
            {
                startNode.GetText(sb);
                NodePort output = startNode.GetOutputPort(StartNode.OutputFieldName);
                NodePort connectedTo = output.GetConnection(0);

                int iterationLimiter = 1000;
                while (connectedTo != null && iterationLimiter > 0)
                {
                    switch (connectedTo.node)
                    {
                        case LineNode lineNode:
                            Debug.Log("lineNode");
                            lineNode.AddIndent(sb);
                            lineNode.GetText(sb);
                            connectedTo = lineNode.GetOutputPort(LineNode.OutputFieldName).GetConnection(0);
                            break;
                        case OptionNode optionNode:
                            int connectionIndex = 0;
                            foreach (NodePort optionNodeDynamicOutput in optionNode.DynamicOutputs)
                            {
                                Debug.Log("dynamic node");
                                openOptionPaths.Push(new OpenPathInfo(optionNode,optionNodeDynamicOutput.GetConnection(0), optionNode.OptionStringList[connectionIndex]));
                                connectionIndex++;
                            }

                            connectedTo = Pop();
                            break;
                        case LineNodeMerger lineNodeMerger:
                            connectedTo = Pop(); 
                            break;
                        case StartNode sNode:
                            Debug.Log("startNode");
                            sb.Append("<<jump ");
                            sb.Append(sNode.Title);
                            sb.Append(">>");
                            sb.AppendLine();
                            connectedTo = Pop();
                            break;
                    }

                    Debug.Log(iterationLimiter);
                    iterationLimiter--;
                }

            }
            
            return sb.ToString(); 
        }
    }
}