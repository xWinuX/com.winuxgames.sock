using SocksTool.Editor.Utility;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Utility;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineMergerNode))]
    public class LineMergerNodeEditor : SockNodeEditor<LineMergerNode>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineMergerNode.OutputFieldName), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }
        
        public override Color GetTint() => NodeColor.LineNodeMergerColor;
    }
}