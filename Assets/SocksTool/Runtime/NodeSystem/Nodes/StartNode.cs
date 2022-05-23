using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Node Start")]
    public class StartNode : DialogueNode
    {
        public const string OutputFieldName = nameof(_out);
        public const string TitleFieldName  = nameof(_title);

        [SerializeField]
        [Output(connectionType = ConnectionType.Override)]
        private LineNode _out;

        [SerializeField] private string _title;

        protected override void Init()
        {
            base.Init();
            name = "Start";
        }

        public string Title { get => _title; set => _title = value; }

        public override object GetValue(NodePort port) => In;
    }
}