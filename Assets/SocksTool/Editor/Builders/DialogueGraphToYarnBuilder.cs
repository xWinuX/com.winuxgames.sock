﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.NodeSystem.Utility;
using UnityEngine;
using XNode;

namespace SocksTool.Editor.Builders
{
    public static class DialogueGraphToYarnBuilder
    {
        private class OpenPathInfo
        {
            public OpenPathInfo(SockNode node, NodePort nodePort, int index, bool lastInPath)
            {
                NodePort   = nodePort;
                Index      = index;
                Node       = node;
                LastInPath = lastInPath;
            }

            public SockNode Node       { get; }
            public NodePort NodePort   { get; }
            public int      Index      { get; }
            public bool     LastInPath { get; }
        }

        public static string Build(DialogueGraph dialogueGraph, bool includeSockTags = true)
        {
            StringBuilder sb = new StringBuilder();

            List<StartNode>     startNodes    = dialogueGraph.nodes.OfType<StartNode>().ToList();
            Stack<OpenPathInfo> openPathStack = new Stack<OpenPathInfo>();

            if (startNodes.Count(sn => sn.EndNode == null) != 0)
            {
                Debug.LogError("Parsing nodes back to yarn failed!");
                Debug.LogError("Some node paths are not closed!");
                return string.Empty;
            }

            // Cache
            List<NodePort> dynamicOutputs = new List<NodePort>();
            foreach (StartNode startNode in startNodes)
            {
                startNode.GetText(sb, 0, includeSockTags);
                SockNode currentNode  = null;
                NodePort output       = startNode.GetOutputPort(StartNode.OutputFieldName);
                NodePort connectedTo  = output.GetConnection(0);
                bool     isLastInPath = true;

                void Pop()
                {
                    if (!openPathStack.TryPop(out OpenPathInfo info)) { return; }

                    info.Node.GetText(sb, info.Index, includeSockTags);

                    currentNode  = info.Node;
                    connectedTo  = info.NodePort;
                    isLastInPath = info.LastInPath;
                }

                int  iterationLimiter = 1000;
                bool stop             = false;
                while (connectedTo != null && iterationLimiter > 0 && !stop)
                {
                    switch (connectedTo.node)
                    {
                        case LineNode lineNode:
                        {
                            lineNode.GetText(sb, 0, includeSockTags);
                            connectedTo = lineNode.GetOutputPort(LineNode.OutputFieldName).GetConnection(0);

                            // Add line merger tag if sock tags are included
                            if (includeSockTags)
                            {
                                NodePort       lineNodeInput       = lineNode.GetInputPort(LineNode.InputFieldName);
                                Node           connectedToLineNode = lineNodeInput.GetConnection(0).node;
                                LineNodeMerger lineNodeMerger      = connectedToLineNode as LineNodeMerger;
                                if (lineNodeMerger != null)
                                {
                                    Debug.Log("linemerger");
                                    string s = sb.ToString().TrimEnd();
                                    sb.Clear();
                                    sb.Append(s);
                                    lineNodeMerger.AddPositionTag(sb, SockTag.SockLineMergerNodePositionTag);
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

                            Pop();
                            break;
                        }

                        case LineNodeMerger lineNodeMerger:
                        {
                            if (isLastInPath)
                            {
                                NodePort connectedToPort = lineNodeMerger.GetOutputPort(LineNodeMerger.OutputFieldName).GetConnection(0);
                                openPathStack.Push(new OpenPathInfo(lineNodeMerger, connectedToPort, 0, true));
                            }

                            Pop();
                            break;
                        }

                        case EndNode endNode:
                            if (!isLastInPath)
                            {
                                Pop();
                                break;
                            }

                            endNode.GetText(sb, 0, includeSockTags);
                            stop = true;
                            break;

                        case StartNode sNode:
                            if (currentNode != null) { currentNode.AddIndent(sb, 1); }

                            sb.Append("<<jump ");
                            sb.Append(sNode.Title);
                            sb.Append(">>");
                            sb.AppendLine();
                            Pop();
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