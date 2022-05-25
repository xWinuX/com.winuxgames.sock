using SocksTool.Editor.Utility;
using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineNodeMerger))]
    public class LineNodeMergerEditor : SockNodeEditor<LineNodeMerger>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineNodeMerger.OutputFieldName), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }
        
        public override Color GetTint() => NodeColor.LineNodeMergerColor;
    }
}