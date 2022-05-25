using System.Text;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Start Node")]
    public class StartNode : DialogueNode
    {
        public const string OutputFieldName = nameof(_out);
        public const string TitleFieldName  = nameof(_title);

        [SerializeField]
        [Output(connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        [SerializeField] private string _title;

        public override string Name => "Start Node";

        public string Title { get => _title; set => _title = value; }

        public override object GetValue(NodePort port) => new NodeInfo(_title, 0);

        public override int GetIndent() => 0;

        public override void GetText(StringBuilder sb)
        {
            sb.Append("title: ");
            sb.Append(_title);
            sb.AppendLine();
            sb.AppendLine("---");
        }
    }
}