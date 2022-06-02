using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.Utility;
using XNodeEditor;

namespace WInuXGames.Sock.Plugins.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : SockNodeEditor<StartNode>
    {
        private ReorderableList _tagList;
        
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(StartNode.TitleFieldName), GUIContent.none);
            NodeEditorGUILayout.PortField(new GUIContent(""), TargetNode.GetOutputPort(StartNode.OutputFieldName), GUILayout.MaxWidth(0));
            EditorGUILayout.EndHorizontal();
            
            if (_tagList == null)
            {
                _tagList                     =  new ReorderableList(serializedObject, serializedObject.FindProperty(StartNode.TagsFieldName));
                
                _tagList.drawHeaderCallback  += rect 
                    => GUI.Label(rect, "Tags"); 
                
                _tagList.drawElementCallback =  (rect, index, active, focused) 
                    => TargetNode.Tags[index] = EditorGUI.TextField(rect, TargetNode.Tags[index].Replace(" ", ""));;
            }

            _tagList.DoLayoutList();
        }
        
        public override Color GetTint() => NodeColor.StartNodeColor;
    }
}