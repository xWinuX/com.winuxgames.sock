using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WinuXGames.Sock.Editor.CustomEditors.Nodes.Core;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(StartNode))]
    internal class StartNodeEditor : SockNodeEditor<StartNode>
    {
        protected override SockNodeSettings Settings { get; } = SockSettings.GetSettings().NodeSettings.StartNodeSettings;
        private            ReorderableList  _tagList;

        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            GUILayout.Label("Title", GUILayout.MaxWidth(40));
            SerializedProperty titleProperty = serializedObject.FindProperty(StartNode.TitleFieldName);
            titleProperty.stringValue = EditorGUILayout.TextField(titleProperty.stringValue.Replace(" ", ""));
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(StartNode.OutputFieldName), GUILayout.MaxWidth(0));
            EditorGUILayout.EndHorizontal();

            if (_tagList == null)
            {
                _tagList = new ReorderableList(serializedObject, serializedObject.FindProperty(StartNode.TagsFieldName));

                _tagList.drawHeaderCallback += rect
                    => GUI.Label(rect, "Tags");

                _tagList.drawElementCallback = (rect, index, _, _)
                    => TargetNode.Tags[index] = EditorGUI.TextField(rect, TargetNode.Tags[index].Replace(" ", ""));
            }

            _tagList.DoLayoutList();
        }
    }
}