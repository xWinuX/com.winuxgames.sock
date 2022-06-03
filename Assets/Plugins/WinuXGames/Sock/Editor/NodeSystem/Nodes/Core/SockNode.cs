using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using WinuXGames.Sock.Editor.NodeSystem.NodeGraphs;
using WinuXGames.Sock.Editor.Utility;
using XNode;

namespace WinuXGames.Sock.Editor.NodeSystem.Nodes.Core
{
    public abstract class SockNode : Node
    {
        public const string InputFieldName = "_in";

        public virtual string Name              => "Default";
        public virtual Type[] AllowedInputTypes => Type.EmptyTypes;

        public bool     Looped            { get; set; }
        public NodeInfo LastValidNodeInfo { get; protected set; }

        public override object GetValue(NodePort port)
        {
            LastValidNodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            return LastValidNodeInfo;
        }

        public virtual void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true) { AddIndent(sb); }

        public void AddIndent(StringBuilder sb, int additional = 0)
        {
            int num = GetIndent();
            for (int i = 0; i < num + additional; i++) { sb.Append("    "); }
        }

        protected void Disconnect(NodePort from, NodePort to, SockNode fromNode, SockNode toNode)
        {
            from.Disconnect(to);
            to.Disconnect(from);

            if (fromNode != null) { fromNode.LastValidNodeInfo = default; }

            if (toNode != null) { toNode.LastValidNodeInfo = default; }
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            SockNode fromSockNode = from.node as SockNode;
            SockNode toSockNode   = to.node as SockNode;

            // Don't allow non sock nodes to be connected to sock nodes
            if (fromSockNode == null || toSockNode == null)
            {
                Disconnect(from, to, fromSockNode, toSockNode);
                Debug.LogError("You tried one of the nodes you tried to connect wasn't a sock node!");
                return;
            }


            // Only allow sock nodes inside of DialogueGraphs
            DialogueGraph dialogueGraph = graph as DialogueGraph;
            if (dialogueGraph == null)
            {
                Disconnect(from, to, fromSockNode, toSockNode);
                Debug.LogError("You can only use sock nodes inside of a DialogueGraph!");
                return;
            }

            // Only allow specified types
            if (Ports.Contains(to))
            {
                if (!AllowedInputTypes.Contains(from.node.GetType()))
                {
                    Disconnect(from, to, fromSockNode, toSockNode);
                    Debug.LogWarning($"You can't connect {from.node.GetType().Name} to {to.node.GetType().Name}!");
                    Debug.LogWarning("You can check which nodes can be connected to each other in the docs");
                    return;
                }
            }

            if (!((DialogueGraph)graph).Ready) { return; }

            // Ignore Start Nodes for loop check
            if (fromSockNode as StartNode != null) { return; }

            NodePort        connectedTo  = to;
            Stack<NodePort> openPaths    = new Stack<NodePort>();
            bool            stop         = false;
            bool            disconnected = false;

            void Pop()
            {
                if (!openPaths.TryPop(out connectedTo)) { stop = true; }
            }

            int iterationLimit = SockConstants.IterationLimit;
            while (!stop && iterationLimit > 0)
            {
                if (connectedTo.node.GetInstanceID() == from.node.GetInstanceID())
                {
                    Debug.LogError("Loop detected, disconnected affected nodes!");
                    Disconnect(from, to, fromSockNode, toSockNode);
                    disconnected = true;
                    break;
                }

                switch (connectedTo.node)
                {
                    case OptionNode optionNode:
                        foreach (NodePort optionNodeOutput in optionNode.DynamicOutputs) { openPaths.Push(optionNodeOutput.GetConnection(0)); }

                        Pop();
                        break;
                    default:
                        NodePort nodePort = connectedTo.node.Outputs.First();
                        if (nodePort == null || nodePort.ConnectionCount == 0)
                        {
                            Pop();
                            break;
                        }

                        connectedTo = nodePort.GetConnection(0);
                        break;
                }

                if (connectedTo == null) { Pop(); }

                iterationLimit--;
            }

            if (disconnected) { return; }

            if (iterationLimit == 0) { Debug.LogError("Iteration limit of loop checker has been reached! The node tree is either too big or an unexpected bug occured"); }

            GetValue(null);
        }


        public override void OnRemoveConnection(NodePort port)
        {
            if (GetInputPort(InputFieldName).ConnectionCount == 0) { LastValidNodeInfo = default; }
        }
       

        protected virtual int GetIndent()
        {
            NodeInfo[] nodeInfos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);
            return nodeInfos.Length > 1 ? GetProcessedValue().Indent : nodeInfos[0].Indent;
        }

        public NodeInfo GetProcessedValue() => GetValue(null) is NodeInfo ? (NodeInfo)GetValue(null) : default;

        protected override void Init()
        {
            base.Init();
            name = Name;
        }

        protected void GetPositionString(StringBuilder sb)
        {
            sb.Append(position.x.ToString(CultureInfo.InvariantCulture)).Append(',').Append(position.y.ToString(CultureInfo.InvariantCulture));
        }

        public void AddPositionTag(StringBuilder sb, string tag)
        {
            sb.Append(" #").Append(tag).Append(':');
            GetPositionString(sb);
        }
    }
}