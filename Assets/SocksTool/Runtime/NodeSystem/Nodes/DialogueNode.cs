using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    public abstract class DialogueNode : SockNode
    {
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Multiple)]
        private NodeInfo _in;

        public NodeInfo In => _in;

        public override object GetValue(NodePort port) => GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);

        protected override void Init()
        {
            base.Init();
            name = Name;
        }
    }
}