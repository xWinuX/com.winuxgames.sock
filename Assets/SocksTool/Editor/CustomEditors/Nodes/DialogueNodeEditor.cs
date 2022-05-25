using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(DialogueNode))]
    public abstract class DialogueNodeEditor<T> : NodeEditor where T : SockNode
    {
        protected T TargetNode;
        
        public override void OnBodyGUI()
        {
            if (TargetNode == null) { TargetNode = target as T; }

            serializedObject.Update();

            if (TargetNode != null)
            {
                GUILayout.Label(TargetNode.GetName());
            
            }

            DrawNode();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawNode() { DrawInputNodePort(); }

        protected void DrawInputNodePort() { NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetInputPort(SockNode.InputFieldName), GUILayout.MaxWidth(0)); }
    }
}