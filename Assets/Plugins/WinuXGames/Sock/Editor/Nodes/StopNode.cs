using System;
using System.Text;
using WinuXGames.Sock.Editor.Nodes.Core;
using XNode;

namespace WinuXGames.Sock.Editor.Nodes
{
    [CreateNodeMenu("Dialogue/Stop", 4)]
    internal class StopNode : MultiInputNode
    {
        public override string Name => "Stop";

        public override Type[] AllowedInputTypes { get; } =
        {
            typeof(LineNode), 
            typeof(StartNode)
        };

        public override object GetValue(NodePort port)
        {
            LastValidNodeInfo = GetInputValue<NodeInfo>(InputFieldName);
            return LastValidNodeInfo;
        }

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true)
        {
            base.GetText(sb, index, includeSockTags);
            sb.Append("<<stop>>");
            sb.AppendLine();
        }
    }
}