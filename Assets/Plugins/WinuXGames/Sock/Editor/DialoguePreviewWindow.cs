using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WInuXGames.Sock.Plugins.Editor.Utility;
using Yarn;
using Yarn.Compiler;
using Yarn.Unity;

namespace WInuXGames.Sock.Plugins.Editor
{
    public class DialoguePreviewWindow : EditorWindow
    {
        private Dialogue                        _currentDialogue;
        private IDictionary<string, StringInfo> _currentStringTable;
        private Action                          _drawPreview;

        private void OnGUI() { _drawPreview?.Invoke(); }

        public void StartPreview(string yarnString, string startNode)
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

            LocalizedLine localizedLine = new LocalizedLine();
        }

        private void NodeCompleteHandler(string completedNodeName)
        {
            Debug.Log($"Completed Node {completedNodeName}");
            _currentDialogue.Continue();
        }

        private void CommandHandler(Command command)
        {
            Debug.Log("Command");
            _currentDialogue.Continue();
        }

        private void DialogueCompleteHandler()
        {
            Debug.Log("Dialogue Over!");
            _drawPreview = null;
            _currentDialogue.Stop();
            Close();
        }

        private void NodeStartHandler(string startedNodeName)
        {
            Debug.Log($"Started Previewing {startedNodeName}");
            _currentDialogue.Continue();
        }

        private void LineHandler(Line line)
        {
            string actualText = _currentStringTable[line.ID].text;
            Debug.Log(actualText);

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
                    if (GUILayout.Button(actualText))
                    {
                        _currentDialogue.SetSelectedOption(i);
                        _currentDialogue.Continue();
                        break;
                    }
                }
            };
        }
    }
}