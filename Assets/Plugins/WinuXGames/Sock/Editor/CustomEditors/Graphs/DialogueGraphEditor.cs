using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Builders;
using WinuXGames.Sock.Editor.NodeSystem.NodeGraphs;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Graphs
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    public class DialogueGraphEditor : NodeGraphEditor
    {
        private DialogueGraph _dialogueGraph;

        private string _previewNode = "Test";

        public override void OnGUI()
        {
            if (_dialogueGraph == null) { _dialogueGraph = target as DialogueGraph; }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
            {
                string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph);
                Debug.Log(exportedYarn);
            }

            if (GUILayout.Button("Save Without Sock Tags", EditorStyles.toolbarButton))
            {
                string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);
                Debug.Log(exportedYarn);
            }


            if (GUILayout.Button("Preview", EditorStyles.toolbarButton))
            {
                DialoguePreviewWindow previewWindow = ScriptableObject.CreateInstance<DialoguePreviewWindow>();
                string                exportedYarn  = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);

                previewWindow.Show();
                previewWindow.StartPreview(exportedYarn, _previewNode);
            }

            GUILayout.FlexibleSpace();

            ;

            if (GUILayout.Button(EditorGUIUtility.IconContent("_Popup@2x"), EditorStyles.toolbarButton)) { SockSettingsWindow.ShowWindow(); }

            GUILayout.EndHorizontal();
        }
    }
}