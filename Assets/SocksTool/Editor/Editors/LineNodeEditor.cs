using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(LineNode))]
    public class LineNodeEditor : DialogueNodeEditor<LineNode>
    {
        protected override void DrawNode()
        {
            GUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_character"));
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineNode.OutputFieldName), GUILayout.MaxWidth(0));
            GUILayout.EndHorizontal();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_text"), GUIContent.none);
        }

        public override Color GetTint() => NodeColor.LineNodeColor;
    }
}