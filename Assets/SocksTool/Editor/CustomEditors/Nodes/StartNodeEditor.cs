using SocksTool.Editor.Utility;
using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : SockNodeEditor<StartNode>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(StartNode.TitleFieldName), GUIContent.none);
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(StartNode.OutputFieldName), GUILayout.MaxWidth(0));
            EditorGUILayout.EndHorizontal();
        }

        public override Color GetTint() => NodeColor.StartNodeColor;
    }
}