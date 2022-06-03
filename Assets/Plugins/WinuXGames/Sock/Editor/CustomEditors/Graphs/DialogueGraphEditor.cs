using System;
using System.IO;
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

            if (GUILayout.Button("Save", EditorStyles.toolbarButton)) { SaveFile(true); }

            if (GUILayout.Button("Save Without Sock Tags", EditorStyles.toolbarButton)) { SaveFile(false); }

            if (GUILayout.Button("Preview", EditorStyles.toolbarButton))
            {
                DialoguePreviewWindow previewWindow = ScriptableObject.CreateInstance<DialoguePreviewWindow>();
                string                exportedYarn  = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);

                previewWindow.Show();
                previewWindow.StartPreview(exportedYarn, _previewNode);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("_Popup@2x"), EditorStyles.toolbarButton)) { SockSettingsWindow.ShowWindow(); }

            GUILayout.EndHorizontal();
        }

        private void SaveFile(bool includeSockTags)
        {
            // Try to parse the node graph to a .yarn
            string exportedYarn;
            try { exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph, includeSockTags); }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Error during parsing!");
                return;
            }
            
            string path = _dialogueGraph.FileSourcePath;
            // Check if .yarn file exists and if not try to create a new one
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (textAsset == null)
            {
                Debug.LogWarning($"Source Yarn File {path} couldn't be found!");
                Debug.LogWarning("Trying to create a new one...");

                try { File.Create(Application.dataPath + path.Remove(0, 6)).Close(); }
                catch (Exception e0)
                {
                    Debug.LogError(e0);
                    Debug.LogWarning($"Yarn File couldn't be created at {path}!");
                    Debug.LogWarning("Trying to create a new one on the root level...");

                    try
                    {
                        path = "Assets/" + path.Split("/")[^1];
                        File.Create(Application.dataPath + path.Remove(0, 6)).Close();
                    }
                    catch (Exception e1)
                    {
                        Debug.LogError(e1);
                        Debug.LogError("Yarn file couldn't be created, it either already exists at the root level or something unexpected happened!");
                        throw;
                    }
                }
            }
            
            // Write the data to the file
            File.WriteAllText(path, exportedYarn);
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}