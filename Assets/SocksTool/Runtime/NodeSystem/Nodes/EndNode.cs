using System.Text;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(100)]
    [CreateNodeMenu("Dialogue/End Node")]
    public class EndNode : DialogueNode
    {
        public override string Name => "End Node";

        public override object GetValue(NodePort port) => new NodeInfo(string.Empty, 0);

        public override int GetIndent() => 0;

        public override void GetText(StringBuilder sb)
        {
            sb.AppendLine("===");
        }
    }
}