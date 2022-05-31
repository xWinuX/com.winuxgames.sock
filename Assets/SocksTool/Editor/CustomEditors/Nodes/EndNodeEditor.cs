using SocksTool.Editor.Utility;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Utility;
using UnityEngine;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(EndNode))]
    public class EndNodeEditor : SockNodeEditor<EndNode>
    {
        public override Color GetTint() => NodeColor.EndNodeColor;
    }
}