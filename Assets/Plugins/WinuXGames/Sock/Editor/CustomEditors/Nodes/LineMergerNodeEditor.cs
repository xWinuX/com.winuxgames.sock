using UnityEditor;
using UnityEngine;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.Utility;
using XNodeEditor;

namespace WInuXGames.Sock.Plugins.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(LineMergerNode))]
    public class LineMergerNodeEditor : SockNodeEditor<LineMergerNode>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(LineMergerNode.OutputFieldName), GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();
        }
        
        public override Color GetTint() => NodeColor.LineNodeMergerColor;
    }
}