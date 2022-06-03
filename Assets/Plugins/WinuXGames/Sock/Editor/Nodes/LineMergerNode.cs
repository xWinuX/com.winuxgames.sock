using System;
using System.Linq;
using System.Text;
using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using XNode;

namespace WinuXGames.Sock.Editor.Nodes
{
    [CreateNodeMenu("Dialogue/Line Merger", 2)]
    internal class LineMergerNode : MultiInputNode
    {
        public const string OutputFieldName = nameof(_out);

        [SerializeField]
        [Output(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        public override string Name => "Line Merger";

        public override Type[] AllowedInputTypes { get; } = { typeof(LineNode) };

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true) { }

        public override object GetValue(NodePort port)
        {
            NodeInfo[] infos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);

            int    lowestIndent = infos.Min(nodeInfo => nodeInfo.Indent) - 1;
            int    minLength    = infos.Min(nodeInfo => nodeInfo.Identifier.Length); // this gets you the shortest length of all elements in names
            string shortest     = infos.FirstOrDefault(nodeInfo => nodeInfo.Identifier.Length == minLength).Identifier;

            LastValidNodeInfo = new NodeInfo(typeof(LineMergerNode), infos[0].StartNode, lowestIndent, shortest);
            return LastValidNodeInfo;
        }
    }
}