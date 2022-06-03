using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes.Core
{
    [CustomNodeEditor(typeof(SockNode))]
    internal abstract class SockNodeEditor<T> : NodeEditor where T : SockNode
    {
        protected abstract SockNodeSettings Settings { get; }

        protected T TargetNode;

        public override void OnBodyGUI()
        {
            if (TargetNode == null) { TargetNode = target as T; }

            serializedObject.Update();

            if (TargetNode != null) { DrawNode(); }

            serializedObject.ApplyModifiedProperties();
        }

        public override Color GetTint() => Settings.Color;

        public override int GetWidth() => Settings.Width;

        protected virtual void DrawNode() { DrawInputNodePort(); }

        protected void DrawInputNodePort() { NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetInputPort(SockNode.InputFieldName), GUILayout.MaxWidth(0)); }
    }
}