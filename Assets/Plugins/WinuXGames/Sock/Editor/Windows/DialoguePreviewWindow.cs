using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Utility;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;

namespace WinuXGames.Sock.Editor.Windows
{
    internal class DialoguePreviewWindow : EditorWindow
    {
        private Dialogue                        _currentDialogue;
        private IDictionary<string, StringInfo> _currentStringTable;
        private Action                          _drawPreview;

        private void OnGUI() { _drawPreview?.Invoke(); }

        internal static void ShowWindow(string yarnString, string startNode)
        {
            DialoguePreviewWindow window = CreateInstance<DialoguePreviewWindow>();
            window.position = new Rect(0, 0, 300, 200);
            window.minSize  = window.position.size;
            window.maxSize  = window.position.size;
            window.StartPreview(yarnString, startNode);
            window.Show();
        }

        private void StartPreview(string yarnString, string startNode)
        {
            _currentDialogue = new Dialogue(new MemoryVariableStore());

            CompilationResult result = YarnUtility.CompileYarnString(yarnString);
            _currentDialogue.AddProgram(result.Program);

            _currentStringTable = result.StringTable;

            _currentDialogue.NodeCompleteHandler     += NodeCompleteHandler;
            _currentDialogue.CommandHandler          += CommandHandler;
            _currentDialogue.NodeStartHandler        += NodeStartHandler;
            _currentDialogue.DialogueCompleteHandler += DialogueCompleteHandler;
            _currentDialogue.LineHandler             += LineHandler;
            _currentDialogue.OptionsHandler          += OptionsHandler;

            _currentDialogue.SetNode(startNode);
        }

        private void NodeCompleteHandler(string completedNodeName) { _currentDialogue.Continue(); }

        private void CommandHandler(Command command) { _currentDialogue.Continue(); }

        private void DialogueCompleteHandler()
        {
            _drawPreview = null;
            _currentDialogue.Stop();
            Close();
        }

        private void NodeStartHandler(string startedNodeName) { _currentDialogue.Continue(); }

        private void LineHandler(Line line)
        {
            string            actualText           = _currentStringTable[line.ID].text;
            MarkupParseResult result               = YarnUtility.ParseMarkup(actualText);
            string            textWithoutCharacter = YarnUtility.TextWithoutCharacterName(result, out string characterName);
            _drawPreview = () =>
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(characterName, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.TextArea(
                    textWithoutCharacter,
                    EditorStyles.textArea,
                    GUILayout.MinWidth(200),
                    GUILayout.MinHeight(100),
                    GUILayout.MaxWidth(200),
                    GUILayout.MaxHeight(100)
                );
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Next")) { _currentDialogue.Continue(); }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            };
        }

        private void OptionsHandler(OptionSet options)
        {
            _drawPreview = () =>
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                for (int i = 0; i < options.Options.Length; i++)
                {
                    OptionSet.Option option     = options.Options[i];
                    string           actualText = _currentStringTable[option.Line.ID].text;
                    bool             clicked    = false;

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(actualText, GUILayout.MinWidth(200), GUILayout.MaxWidth(200)))
                    {
                        _currentDialogue.SetSelectedOption(i);
                        _currentDialogue.Continue();
                        clicked = true;
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (clicked) { break; }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            };
        }
    }
}