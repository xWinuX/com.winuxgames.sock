using System.Linq;
using System.Text;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(150)]
    [CreateNodeMenu("Dialogue/Line Node Merger", 2)]
    public class LineNodeMerger : MultiInputNode
    {
        public const string OutputFieldName = nameof(_out);
        
        [SerializeField]
        [Output(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        public override string Name => "Line Node Merger";

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true) { }

        public override object GetValue(NodePort port)
        {
            NodeInfo[] infos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);

            int offset       = infos.Sum(nodeInfo => nodeInfo.Offset);
            int lowestIndent = infos.Min(nodeInfo => nodeInfo.Indent)-1;

            return new NodeInfo(infos[0].StartNode, lowestIndent, 0, offset);
        }
    }
}