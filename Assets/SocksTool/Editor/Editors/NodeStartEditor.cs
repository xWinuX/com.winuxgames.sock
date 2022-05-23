using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(StartNode))]
    public class NodeStartEditor : DialogueNodeEditor<StartNode>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(StartNode.TitleFieldName), GUIContent.none);
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(StartNode.OutputFieldName), GUILayout.MaxWidth(0));
            EditorGUILayout.EndHorizontal();
        }

        public override Color GetTint() => new Color(0.84f, 0.68f, 0.2f);
    }
}