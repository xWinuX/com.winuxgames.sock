using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(EndNode))]
    public class EndNodeEditor : DialogueNodeEditor<EndNode>
    {
        public override Color GetTint() => NodeColor.EndNodeColor;
    }
}