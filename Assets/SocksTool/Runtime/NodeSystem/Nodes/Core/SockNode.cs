using System.Linq;
using System.Text;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes.Core
{
    public abstract class SockNode : Node
    {
        public const string InputFieldName = "_in";

        public virtual string Name => "Default";

        protected override void Init()
        {
            base.Init();
            name = Name;
        }

        public override object GetValue(NodePort port) => GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);

        public virtual void GetText(StringBuilder sb) { }

        public void AddIndent(StringBuilder sb)
        {
            int num = GetIndent();
            for (int i = 0; i < num; i++)
            {
                sb.Append("    ");
            }
        }
        
        public string GetName()
        {
            NodeInfo nodeInfo = GetValue(null) is NodeInfo ? (NodeInfo)GetValue(null) : default;
            return nodeInfo.NodeTitle + " " + nodeInfo.Indent + " " + nodeInfo.Count + " " + nodeInfo.Offset;
        }

        public virtual int GetIndent()
        {
            NodeInfo[] nodeInfos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);
            return nodeInfos.Length > 1 ? nodeInfos.Select(nodeInfo => nodeInfo.Indent).Prepend(int.MaxValue).Min() - 1 : nodeInfos[0].Indent;
        }
        

        protected void AddPositionTag(StringBuilder stringBuilder)
        {
            stringBuilder.Append("#_sock_position:");
            stringBuilder.Append(position.x);
            stringBuilder.Append(",");
            stringBuilder.Append(position.y);
        }
    }
}