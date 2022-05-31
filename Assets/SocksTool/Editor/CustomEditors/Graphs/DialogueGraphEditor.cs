using SocksTool.Editor.CustomEditors.Builders;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using UnityEngine;
using XNodeEditor;

namespace SocksTool.Editor.CustomEditors.Graphs
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    public class DialogueGraphEditor : NodeGraphEditor
    {
        private DialogueGraph _dialogueGraph;
        public override void OnGUI()
        {
            if (_dialogueGraph == null) { _dialogueGraph = target as DialogueGraph; }

            if (GUILayout.Button("Export", GUILayout.MaxWidth(100)))
            {
                string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph);
                Debug.Log(exportedYarn);
            }
            
            
            if (GUILayout.Button("Export Without Sock Tags", GUILayout.MaxWidth(400)))
            {
                string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);
                Debug.Log(exportedYarn);
            }
        }
    }
}