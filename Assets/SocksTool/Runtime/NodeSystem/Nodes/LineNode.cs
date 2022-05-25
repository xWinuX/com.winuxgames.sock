using System.Linq;
using System.Text;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Line")]
    public class LineNode : DialogueNode
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

        public override string Name => HasMultipleInputs ? "Line Merger" : "Line";

        public bool HasMultipleInputs => GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo)?.Length > 1;

        public override object GetValue(NodePort port) => GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);

        public override void GetText(StringBuilder sb)
        {
            if (!string.IsNullOrWhiteSpace(_character))
            {
                sb.Append(Character);
                sb.Append(": ");
            }

            sb.Append(_text);

            AddPositionTag(sb);

            sb.AppendLine();
        }
    }
}