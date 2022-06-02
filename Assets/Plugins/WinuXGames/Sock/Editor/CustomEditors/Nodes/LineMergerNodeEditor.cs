﻿using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.NodeSystem.Nodes;
using WinuXGames.Sock.Editor.NodeSystem.Utility;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
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