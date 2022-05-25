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
        public static string Build(DialogueGraph dialogueGraph)
        {
            StringBuilder sb = new StringBuilder();

            FileParseResult result;
            

            List<StartNode> startNodes = dialogueGraph.nodes.OfType<StartNode>().ToList();

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
                            lineNode.GetText(sb);
                            connectedTo = lineNode.GetOutputPort(LineNode.OutputFieldName).GetConnection(0);
                            break;
                        case OptionNode optionNode: 
                            
                            break;
                        case StartNode sNode:
                            Debug.Log("startNode");
                            sb.Append("<<jump ");
                            sb.Append(sNode.Title);
                            sb.Append(">>");
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