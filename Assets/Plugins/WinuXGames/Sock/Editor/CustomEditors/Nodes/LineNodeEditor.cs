using System;
using UnityEngine;
using WinuXGames.Sock.Editor.CustomEditors.Nodes.Core;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineNode))]
    internal class LineNodeEditor : SockNodeEditor<LineNode>
    {
        protected override SockNodeSettings Settings { get; } = SockSettings.GetSettings().NodeSettings.LineNodeSettings;

        protected override void DrawNode()
        {
            // Check if line node text is not empty/null
            if (string.IsNullOrEmpty(TargetNode.Text))
            {
                HasError  = true;
                ErrorText = "A Line Node needs to have at least 1 character of text";
            }
            else
            {
                HasError  = false;
                ErrorText = string.Empty;
            }

            GUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_character"));
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineNode.OutputFieldName), GUILayout.MaxWidth(0));
            GUILayout.EndHorizontal();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_text"), GUIContent.none);
        }
    }
}