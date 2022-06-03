using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Utility;
using Yarn;
using Yarn.Compiler;

namespace WinuXGames.Sock.Editor.Windows
{
    public class DialoguePreviewWindow : EditorWindow
    {
        private Dialogue                        _currentDialogue;
        private IDictionary<string, StringInfo> _currentStringTable;
        private Action                          _drawPreview;

        private void OnGUI() { _drawPreview?.Invoke(); }

        internal static void ShowWindow(string yarnString, string startNode)
        {
            DialoguePreviewWindow window = CreateInstance<DialoguePreviewWindow>();
            window.position = new Rect(0, 0, 400, 430);
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
            string actualText = _currentStringTable[line.ID].text;
            _drawPreview = () =>
            {
                GUILayout.Label(actualText);
                if (GUILayout.Button("Next")) { _currentDialogue.Continue(); }
            };
        }

        private void OptionsHandler(OptionSet options)
        {
            _drawPreview = () =>
            {
                for (int i = 0; i < options.Options.Length; i++)
                {
                    OptionSet.Option option     = options.Options[i];
                    string           actualText = _currentStringTable[option.Line.ID].text;
                    
                    if (!GUILayout.Button(actualText)) { continue; }

                    _currentDialogue.SetSelectedOption(i);
                    _currentDialogue.Continue();
                    break;
                }
            };
        }
    }
}