using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.Settings
{
    public class SockSettingsWindow : EditorWindow
    {
        private Dictionary<string, SockNodeSettings> _allNodeSettingsDictionary;

        private readonly GUIContent _titleContent = new GUIContent("Sock Settings");

        private SO_SockSettings _sockSettings;
        private int             _updateCounter;

        private GUIStyle _headerStyle;

        [MenuItem("WinuXGames/Sock/Settings")]
        public static void ShowWindow() { GetWindowWithRect<SockSettingsWindow>(new Rect(0, 0, 400, 300)); }

        private GUIStyle GetHeaderStyle() =>
            _headerStyle ??= new GUIStyle
            {
                fontSize  = 20,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = EditorStyles.label.normal.textColor
                }
            };

        private void OnGUI()
        {
            titleContent = _titleContent;

            _sockSettings = SockSettings.GetSettings();

            if (_allNodeSettingsDictionary == null) { GetNodeSettings(); }
            
            DrawNodeSettings();
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
                SockSettings.ResetSettings();
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