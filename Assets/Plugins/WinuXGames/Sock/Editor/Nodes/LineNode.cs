using System;
using System.Text;
using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using WinuXGames.Sock.Editor.Utility;
using XNode;

namespace WinuXGames.Sock.Editor.Nodes
{
    [CreateNodeMenu("Dialogue/Line", 0)]
    internal class LineNode : SingleInputNode
    {
        public const string OutputFieldName = nameof(_out);

        [SerializeField]
        [Output(connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        [SerializeField]
        private string _character;

        [SerializeField] [TextArea(5, 5)]
        private string _text;

        public string Character { get => _character; set => _character = value; }

        public string Text { get => _text; set => _text = value; }

        public override string Name => "Line";

        public override Type[] AllowedInputTypes { get; } =
        {
            typeof(StartNode),
            typeof(LineNode),
            typeof(OptionNode),
            typeof(LineMergerNode)
        };

        public override object GetValue(NodePort port)
        {
            NodeInfo nodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            LastValidNodeInfo = new NodeInfo(typeof(LineNode), nodeInfo.StartNode, nodeInfo.Indent, nodeInfo.Identifier);
            return LastValidNodeInfo;
        }

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true)
        {
            base.GetText(sb, index, includeSockTags);

            if (!string.IsNullOrWhiteSpace(_character))
            {
                sb.Append(Character);
                sb.Append(": ");
            }

            sb.Append(_text);

            if (includeSockTags) { AddPositionTag(sb, SockConstants.SockPositionTag); }

            sb.AppendLine();
        }
    }
}