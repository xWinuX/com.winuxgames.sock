using System.Linq;
using System.Text;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    public abstract class SockNode : Node
    {
        public const string InputFieldName = "_in";

        public virtual string Name => "Default";

        public virtual void GetText(StringBuilder sb) { }

        public string GetName()
        {
            NodeInfo nodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            return nodeInfo.NodeTitle + " " + nodeInfo.Indent;
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