using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Settings;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.Windows
{
    public class SockSettingsWindow : EditorWindow
    {
        private readonly GUIContent                           _titleContent = new GUIContent("Sock Settings");
        private          Dictionary<string, SockNodeSettings> _allNodeSettingsDictionary;

        private GUIStyle _headerStyle;

        private SO_SockSettings _sockSettings;
        private int             _updateCounter;

        private void OnGUI()
        {
            titleContent = _titleContent;

            _sockSettings = SockSettings.GetSettings();

            if (_allNodeSettingsDictionary == null) { GetNodeSettings(); }

            DrawGraphSettings();
            DrawExportSettings();
            DrawNodeSettings();
        }

        [MenuItem("WinuXGames/Sock/Settings")]
        public static void ShowWindow() { GetWindowWithRect<SockSettingsWindow>(new Rect(0, 0, 400, 505)); }

        private void BeginSection(string sectionName)
        {
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 5, 0) }, GUILayout.MaxWidth(400));
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(sectionName, GetHeaderStyle());
            GUILayout.Space(10f);
        }

        private void EndSection(Action onChange = null, Action onReset = null)
        {
            if (GUILayout.Button("Reset to default")) { onReset?.Invoke(); }

            if (EditorGUI.EndChangeCheck()) { onChange?.Invoke(); }

            GUILayout.EndVertical();
            GUILayout.Space(10f);
        }

        private void GetNodeSettings()
        {
            _allNodeSettingsDictionary = new Dictionary<string, SockNodeSettings>();
            _sockSettings.NodeSettings.GetAllNodeSettings(_allNodeSettingsDictionary);
        }

        private GUIStyle GetHeaderStyle() => _headerStyle ??= new GUIStyle
        {
            fontSize  = 20,
            fontStyle = FontStyle.Bold,
            normal =
            {
                textColor = EditorStyles.label.normal.textColor
            }
        };

        private void DrawExportSettings()
        {
            BeginSection("Export Settings");
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Line Break Replacement");
            _sockSettings.LineBreakReplacementString = EditorGUILayout.TextField(_sockSettings.LineBreakReplacementString);
            GUILayout.EndHorizontal();
            EndSection(
                () => { EditorUtility.SetDirty(_sockSettings); },
                () =>
                {
                    SockSettings.ResetExportSettings();
                    GetNodeSettings();
                });
        }

        private void DrawGraphSettings()
        {
            BeginSection("Graph Settings");
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Connection Color");
            _sockSettings.ConnectionColor = EditorGUILayout.ColorField(_sockSettings.ConnectionColor);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Connection Style");
            _sockSettings.ConnectionStyle = (NoodlePath)EditorGUILayout.EnumPopup(_sockSettings.ConnectionStyle);
            GUILayout.EndHorizontal();

            EndSection(
                () =>
                {
                    NodeEditorWindow.RepaintAll();
                    EditorUtility.SetDirty(_sockSettings);
                },
                () =>
                {
                    SockSettings.ResetGraphSettings();
                    GetNodeSettings();
                });
        }

        private void DrawNodeSettings()
        {
            float previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            BeginSection("Node Settings");
            foreach ((string nodeName, SockNodeSettings settings) in _allNodeSettingsDictionary)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(nodeName, EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Color");
                settings.Color = EditorGUILayout.ColorField(settings.Color);
                EditorGUILayout.PrefixLabel("Width");
                settings.Width = EditorGUILayout.IntField(settings.Width);
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }

            EndSection(
                () =>
                {
                    // Set asset dirty and save 
                    NodeEditorWindow.RepaintAll();
                    EditorUtility.SetDirty(_sockSettings.NodeSettings);
                },
                () =>
                {
                    SockSettings.ResetNodeSettings();
                    GetNodeSettings();
                });
            EditorGUIUtility.labelWidth = previousWidth;
        }
    }
}