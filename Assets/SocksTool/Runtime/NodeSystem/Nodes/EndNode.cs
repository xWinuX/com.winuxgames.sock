using System.Linq;
using System.Text;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(100)]
    [CreateNodeMenu("Dialogue/End Node")]
    public class EndNode : MultiInputNode
    {
        public override string    Name      => "End Node";
        public          StartNode StartNode { get; set; }

        public override object GetValue(NodePort port) => NodeInfo.ErrorNodeInfo;

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true)
        {
            sb.Append("===");
            sb.AppendLine();
            sb.AppendLine();
        }
        
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            
            NodeInfo nodeInfo = GetInputValue<NodeInfo>(InputFieldName);
            if (StartNode == null)
            {
                StartNode                  = nodeInfo.StartNode;
                nodeInfo.StartNode.EndNode = this;
            }
            else
            {
                if (!StartNode.Equals(nodeInfo.StartNode)) { from.Disconnect(to); }
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            if (Inputs.Count() != 0) { return; }

            StartNode.EndNode = null;
            StartNode         = null;
        }
        
        protected override int GetIndent() => 0;
    }
}