using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(LineNode))]
    public class LineNodeEditor : NodeEditor
    {
        private LineNode _lineNode;
        
        public override void OnBodyGUI()
        {
            if (_lineNode == null) { _lineNode = target as LineNode; }

            serializedObject.Update();

            GUILayout.BeginHorizontal();
            NodeEditorGUILayout.PortField(new GUIContent(""), _lineNode!.GetInputPort("_in"), GUILayout.MinWidth(0));
            NodeEditorGUILayout.PortField(new GUIContent(""), _lineNode!.GetOutputPort("_out"), GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_character"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_text"), GUIContent.none);

            serializedObject.ApplyModifiedProperties();
        }
    }
}