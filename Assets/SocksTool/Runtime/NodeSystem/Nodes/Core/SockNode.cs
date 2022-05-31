using System.Globalization;
using System.Text;
using XNode;
using Yarn.Unity;

namespace SocksTool.Runtime.NodeSystem.Nodes.Core
{
    public abstract class SockNode : Node
    {
        public const string InputFieldName = "_in";

        public virtual string Name => "Default";

        public override object GetValue(NodePort port) => GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);

        public virtual void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true) { AddIndent(sb); }

        public void AddIndent(StringBuilder sb, int additional = 0)
        {
            int num = GetIndent();
            for (int i = 0; i < num + additional; i++) { sb.Append("    "); }
        }

        public string GetName()
        {
            NodeInfo nodeInfo = GetProcessedValue();
            if (nodeInfo.StartNode == null) { return "Error"; }

            return nodeInfo.StartNode.Title + " " + nodeInfo.Indent + " " + nodeInfo.Count + " " + nodeInfo.Offset;
        }


        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // Avoid self connecting
            if (from.node.Equals(to.node)) { from.Disconnect(to); }
        }

        protected virtual int GetIndent()
        {
            NodeInfo[] nodeInfos = GetInputValues(InputFieldName, NodeInfo.ErrorNodeInfo);
            return nodeInfos.Length > 1 ? GetProcessedValue().Indent : nodeInfos[0].Indent;
        }

        public NodeInfo GetProcessedValue() => GetValue(null) is NodeInfo ? (NodeInfo)GetValue(null) : default;

        protected override void Init()
        {
            base.Init();
            name = Name;
        }

        public void GetPositionString(StringBuilder sb)
        {
            sb.Append(position.x.ToString(CultureInfo.InvariantCulture)).Append(',').Append(position.y.ToString(CultureInfo.InvariantCulture));
        }

        public virtual void AddPositionTag(StringBuilder sb, string tag)
        {
            sb.Append(" #").Append(tag).Append(':');
            GetPositionString(sb);
        }
    }
}