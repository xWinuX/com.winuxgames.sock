using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(DialogueNode))]
    public abstract class DialogueNodeEditor<T> : NodeEditor where T : Node
    {
        protected T TargetNode;

        public override void OnBodyGUI()
        {
            if (TargetNode == null) { TargetNode = target as T; }

            serializedObject.Update();

            DrawNode();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawNode() { DrawInputNodePort(); }

        protected void DrawInputNodePort() { NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetInputPort(DialogueNode.InputFieldName), GUILayout.MaxWidth(0)); }
    }
}