using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.CustomEditors.Nodes.Core;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineMergerNode))]
    internal class LineMergerNodeEditor : SockNodeEditor<LineMergerNode>
    {
        protected override SockNodeSettings Settings { get; } = SockSettings.GetSettings().NodeSettings.LineMergerNodeSettings;

        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineMergerNode.OutputFieldName), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }
    }
}