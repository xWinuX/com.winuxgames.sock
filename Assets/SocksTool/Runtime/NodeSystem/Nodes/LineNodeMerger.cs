using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(150)]
    [CreateNodeMenu("Dialogue/Line Node Merger")]
    public class LineNodeMerger : SockNode
    {
        public const string OutputFieldName = nameof(_out);
        
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Multiple)]
        private NodeInfo _in;
        
        [SerializeField]
        [Output(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        public override string Name => "Line Node Merger";

        public override object GetValue(NodePort port) => GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
    }
}