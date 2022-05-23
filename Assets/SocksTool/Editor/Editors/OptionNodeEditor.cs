using SocksTool.Runtime.NodeSystem.Nodes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SocksTool.Editor.Editors
{
    [CustomNodeEditor(typeof(OptionNode))]
    public class OptionNodeEditor : DialogueNodeEditor<OptionNode>
    {
        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.DynamicPortList(
                OptionNode.OptionsFieldName,
                typeof(LineNode),
                serializedObject,
                NodePort.IO.Output,
                Node.ConnectionType.Override,
                Node.TypeConstraint.Inherited,
                OnCreateReorderableList
            );
            EditorGUILayout.EndHorizontal();
        }
        
        private void OnCreateReorderableList(ReorderableList list)
        {
            list.headerHeight = 0;

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                TargetNode.Options[index] = EditorGUI.TextField(rect, TargetNode.Options[index]);

                NodePort port     = TargetNode.GetPort(OptionNode.OptionsFieldName + " " + index);

                rect.width += NodeEditorWindow.current.graphEditor.GetPortStyle(port).padding.right;
                rect.width += NodeEditorWindow.current.graphEditor.GetPortStyle(port).margin.right;
                Vector2 position = rect.position + new Vector2(rect.width+3, 2);
                NodeEditorGUILayout.PortField(position, port);
            };
        }

        public override Color GetTint() => NodeColor.OptionNodeColor;
    }
}