using NUnit.Framework;
using UnityEditor;
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

        protected bool   HasError;
        protected string ErrorText;

        private readonly GUIStyle _headerStyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);

        public override void OnBodyGUI()
        {
            if (TargetNode == null) { TargetNode = target as T; }

            serializedObject.Update();

            if (TargetNode != null)
            {
                DrawNode();
                TargetNode.HasError  = HasError;
                TargetNode.ErrorText = ErrorText;
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnHeaderGUI()
        {
            _headerStyle.normal.textColor = HasError ? Color.red : NodeEditorResources.styles.nodeHeader.normal.textColor;

            GUILayout.Label(
                new GUIContent(target.name, HasError ? ErrorText : ""),
                _headerStyle,
                GUILayout.Height(30)
            );
        }

        public override Color GetTint()
        {
            Color baseColor = Settings.Color;

            if (TargetNode == null) { return Color.magenta; }

            if (HasError) { return baseColor * 0.5f; }

            return baseColor;
        }

        public override int GetWidth() => Settings.Width;

        protected virtual void DrawNode() { DrawInputNodePort(); }

        protected void DrawInputNodePort() { NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetInputPort(SockNode.InputFieldName), GUILayout.MaxWidth(0)); }
    }
}