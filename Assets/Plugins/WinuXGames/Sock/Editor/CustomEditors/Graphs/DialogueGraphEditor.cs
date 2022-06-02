using UnityEngine;
using WInuXGames.Sock.Plugins.Editor.Builders;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.NodeGraphs;
using XNodeEditor;

namespace WInuXGames.Sock.Plugins.Editor.CustomEditors.Graphs
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    public class DialogueGraphEditor : NodeGraphEditor
    {
        private DialogueGraph _dialogueGraph;

        private string _previewNode = "Test";
        
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

            _previewNode = GUILayout.TextField(_previewNode, GUILayout.MaxWidth(100));

            if (GUILayout.Button("Preview", GUILayout.MaxWidth(100)))
            {
                DialoguePreviewWindow previewWindow = ScriptableObject.CreateInstance<DialoguePreviewWindow>();
                string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);
                
                previewWindow.Show();
                previewWindow.StartPreview(exportedYarn, _previewNode);
            }
        }
    }
}