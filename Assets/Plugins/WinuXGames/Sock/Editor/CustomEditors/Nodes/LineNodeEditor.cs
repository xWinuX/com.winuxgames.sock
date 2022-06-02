using UnityEngine;
using WinuXGames.Sock.Editor.NodeSystem.Nodes;
using WinuXGames.Sock.Editor.NodeSystem.Utility;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineNode))]
    public class LineNodeEditor : SockNodeEditor<LineNode>
    {
        public override void OnHeaderGUI()
        {
            if (TargetNode == null) { return; }

            GUILayout.Label(TargetNode.Name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

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