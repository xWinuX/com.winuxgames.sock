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
            DrawNodeSettings();
        }

        [MenuItem("WinuXGames/Sock/Settings")]
        public static void ShowWindow() { GetWindowWithRect<SockSettingsWindow>(new Rect(0, 0, 400, 430)); }

        private GUIStyle GetHeaderStyle() => _headerStyle ??= new GUIStyle
        {
            fontSize  = 20,
            fontStyle = FontStyle.Bold,
            normal =
            {
                textColor = EditorStyles.label.normal.textColor
            }
        };

        private void DrawGraphSettings()
        {
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 5, 0) }, GUILayout.MaxWidth(400));
            GUILayout.Label("Graph Settings", GetHeaderStyle());
            GUILayout.Space(10f);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Connection Color", GUILayout.MaxWidth(110));
            _sockSettings.ConnectionColor = EditorGUILayout.ColorField(_sockSettings.ConnectionColor);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Connection Style", GUILayout.MaxWidth(110));
            _sockSettings.ConnectionStyle = (NoodlePath)EditorGUILayout.EnumPopup(_sockSettings.ConnectionStyle);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                NodeEditorWindow.RepaintAll();
                EditorUtility.SetDirty(_sockSettings);
            }

            GUILayout.Space(10f);
            if (GUILayout.Button("Reset to default"))
            {
                SockSettings.ResetGraphSettings();
                GetNodeSettings();
            }

            GUILayout.EndVertical();
            GUILayout.Space(10f);
        }

        private void GetNodeSettings()
        {
            _allNodeSettingsDictionary = new Dictionary<string, SockNodeSettings>();
            _sockSettings.NodeSettings.GetAllNodeSettings(_allNodeSettingsDictionary);
        }

        private void DrawNodeSettings()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(10, 10, 5, 0) }, GUILayout.MaxWidth(400));
            GUILayout.Label("Node Settings", GetHeaderStyle());
            GUILayout.Space(10f);
            foreach ((string nodeName, SockNodeSettings settings) in _allNodeSettingsDictionary)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(nodeName, EditorStyles.boldLabel);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Color", GUILayout.MaxWidth(40));
                settings.Color = EditorGUILayout.ColorField(settings.Color);
                GUILayout.Label("Width", GUILayout.MaxWidth(40));
                settings.Width = EditorGUILayout.IntField(settings.Width, GUILayout.MaxWidth(50));
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
            }

            if (GUILayout.Button("Reset to default"))
            {
                SockSettings.ResetNodeSettings();
                GetNodeSettings();
            }

            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                // Set asset dirty and save 
                NodeEditorWindow.RepaintAll();
                EditorUtility.SetDirty(_sockSettings.NodeSettings);
            }
        }
    }
}