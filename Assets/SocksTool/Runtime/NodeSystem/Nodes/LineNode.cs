using System.Text;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.NodeSystem.Utility;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Line", 0)]
    public class LineNode : SingleInputNode
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
        
        public override object GetValue(NodePort port)
        {
            NodeInfo nodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            return new NodeInfo(nodeInfo.StartNode, nodeInfo.Indent, nodeInfo.Count+1, nodeInfo.Offset);
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

            if (includeSockTags) { AddPositionTag(sb, SockTag.SockPositionTag); }

            sb.AppendLine();
        }
    }
}