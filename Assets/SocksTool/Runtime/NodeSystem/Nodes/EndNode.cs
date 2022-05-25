using System.Text;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(100)]
    [CreateNodeMenu("Dialogue/End Node")]
    public class EndNode : MultiInputNode
    {
        public override string Name => "End Node";

        public override object GetValue(NodePort port) => NodeInfo.ErrorNodeInfo;

        public override int GetIndent() => 0;

        public override void GetText(StringBuilder sb)
        {
            sb.AppendLine("===");
        }
    }
}