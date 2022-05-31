using SocksTool.Runtime.Extensions;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.NodeSystem.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(OptionNode))]
    public class OptionNodeEditor : SockNodeEditor<OptionNode>
    {
        private int _selectIndex = -1;

        protected override void DrawNode()
        {
            EditorGUILayout.BeginHorizontal();
            DrawInputNodePort();
            NodeEditorGUILayout.DynamicPortList(
                OptionNode.OutputFieldName,
                typeof(NodeInfo),
                serializedObject,
                NodePort.IO.Output,
                Node.ConnectionType.Override,
                Node.TypeConstraint.Strict,
                OnCreateReorderableList
            );
            EditorGUILayout.EndHorizontal();
        }

        private void OnCreateReorderableList(ReorderableList list)
        {
            list.headerHeight = 0;
            list.multiSelect  = false;
            list.draggable    = true;

            // Save selected index for reordering
            list.onSelectCallback += OnListOnSelectCallback;

            // Move the string element with the dynamic output
            // This would be easier with the reorderWithDetailsCallback, but that doesn't work with xNode
            list.onReorderCallback += OnListOnReorderCallback;

            // Add new element to string list
            list.onAddCallback += OnListOnAddCallback;

            // Remove Selected element or last element if nothing is selected
            list.onRemoveCallback += OnListOnRemoveCallback;

            // Element drawer
            list.drawElementCallback = ListDrawElementCallback;
        }

        private void OnListOnAddCallback(ReorderableList _) { TargetNode.OptionStringList.Add(""); }

        private void OnListOnSelectCallback(ReorderableList reorderableList) { _selectIndex = reorderableList.index; }

        private void ListDrawElementCallback(Rect rect, int index, bool active, bool focused)
        {
            TargetNode.OptionStringList[index] = EditorGUI.TextField(rect, TargetNode.OptionStringList[index]);

            NodePort port = TargetNode.GetPort(OptionNode.OutputFieldName + " " + index);

            rect.width += NodeEditorWindow.current.graphEditor.GetPortStyle(port).padding.right;
            rect.width += NodeEditorWindow.current.graphEditor.GetPortStyle(port).margin.right;
            Vector2 position = rect.position + new Vector2(rect.width + 3, 2);
            NodeEditorGUILayout.PortField(position, port);
        }

        private void OnListOnRemoveCallback(ReorderableList reorderableList)
        {
            if (reorderableList.selectedIndices.Count > 0) { TargetNode.OptionStringList.RemoveAt(reorderableList.selectedIndices[0]); }
            else if (TargetNode.OptionStringList.Count > 0) { TargetNode.OptionStringList.RemoveAt(TargetNode.OptionStringList.Count - 1); }
        }

        private void OnListOnReorderCallback(ReorderableList reorderableList)
        {
            // Move up
            if (reorderableList.index > _selectIndex)
            {
                for (int i = _selectIndex; i < reorderableList.index; ++i) { TargetNode.OptionStringList.Swap(i, i + 1); }
            }
            else // Move down
            {
                for (int i = _selectIndex; i > reorderableList.index; --i) { TargetNode.OptionStringList.Swap(i, i - 1); }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public override Color GetTint() => NodeColor.OptionNodeColor;
    }
}