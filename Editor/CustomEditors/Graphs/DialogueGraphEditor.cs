using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Builders;
using WinuXGames.Sock.Editor.NodeGraphs;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Nodes.Core;
using WinuXGames.Sock.Editor.Settings;
using WinuXGames.Sock.Editor.Windows;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.CustomEditors.Graphs
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    internal class DialogueGraphEditor : NodeGraphEditor
    {
        private DialogueGraph _dialogueGraph;

        public override void OnGUI()
        {
            if (_dialogueGraph == null) { _dialogueGraph = target as DialogueGraph; }

            // Reset prefs so it take the sock ones
            NodeEditorPreferences.ResetPrefs();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            // Save
            if (GUILayout.Button("Save", EditorStyles.toolbarButton)) { SaveFile(true); }

            // Save Without Sock Tags
            if (GUILayout.Button("Save Without Sock Tags", EditorStyles.toolbarButton)) { SaveFile(false); }

            // Preview
            if (EditorGUILayout.DropdownButton(new GUIContent("Preview", ""), FocusType.Passive, EditorStyles.toolbarButton))
            {
                if (CheckDialogueGraphIsValid())
                {
                    GenericMenu menu = new GenericMenu();

                    IEnumerable<StartNode> startNodes = _dialogueGraph.nodes.OfType<StartNode>();

                    foreach (StartNode startNode in startNodes)
                    {
                        menu.AddItem(new GUIContent(startNode.Title), false, _ =>
                        {
                            string exportedYarn = DialogueGraphToYarnBuilder.Build(_dialogueGraph, false);
                            DialoguePreviewWindow.ShowWindow(exportedYarn, startNode.Title);
                        }, null);
                    }

                    menu.ShowAsContext();
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("_Popup@2x"), EditorStyles.toolbarButton)) { SockSettingsWindow.ShowWindow(); }

            GUILayout.EndHorizontal();
        }

        public override NodeEditorPreferences.Settings GetDefaultPreferences() => SockSettings.GetSettings().GetReferencedXNodeSettings();

        private void SaveFile(bool includeSockTags)
        {
            // Check for graph errors
            if (!CheckDialogueGraphIsValid()) { return; }


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

        private bool CheckDialogueGraphIsValid()
        {
            try
            {
                List<SockNode> sockNodes = _dialogueGraph.nodes.Cast<SockNode>().ToList();
                List<string>   errors    = (from sockNode in sockNodes where sockNode.HasError select sockNode.ErrorText).ToList();
                if (errors.Count != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("There are errors in the node graph:").AppendLine();
                    foreach (string error in errors) { stringBuilder.Append(error).AppendLine(); }

                    EditorUtility.DisplayDialog("Dialogue Graph Error!", stringBuilder.ToString(), "Ok");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Failed to check for Dialogue Graph Validity");
                throw;
            }

            return true;
        }
    }
}