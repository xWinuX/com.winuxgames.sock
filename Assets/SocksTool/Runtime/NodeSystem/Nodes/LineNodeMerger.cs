using System.Linq;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
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

        public override object GetValue(NodePort port)
        {
            NodeInfo[] infos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);

            int offset       = infos.Sum(nodeInfo => nodeInfo.Offset);
            int lowestIndent = infos.Min(nodeInfo => nodeInfo.Indent);

            return new NodeInfo(infos[0].NodeTitle, lowestIndent, 0, offset);
        }
    }
}