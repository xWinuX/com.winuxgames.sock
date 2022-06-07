using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using UnityEditor;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.Settings
{
    internal class SO_SockSettings : ScriptableObject
    {
        [Header("Graph Settings")]
        [SerializeField] private Color _connectionColor = new Color(0.46f, 0.85f, 0.61f);
        [SerializeField] private NoodlePath _connectionStyle = NoodlePath.Curvy;

        [Header("Export Settings")]
        [SerializeField] private string _lineBreakReplacementString = "/n";

        [Header("Node Settings")]
        [SerializeField] private SO_SockNodeSettings _nodeSettings;

        private readonly NodeEditorPreferences.Settings _xNodeSettings = new NodeEditorPreferences.Settings();

        internal Color      ConnectionColor { get => _connectionColor; set => _connectionColor = value; }
        internal NoodlePath ConnectionStyle { get => _connectionStyle; set => _connectionStyle = value; }

        internal SO_SockNodeSettings NodeSettings
        {
            get
            {
                CheckForNodeSettings();
                return _nodeSettings;
            }
            set => _nodeSettings = value;
        }

        internal string LineBreakReplacementString { get => _lineBreakReplacementString; set => _lineBreakReplacementString = value; }

        private void CheckForNodeSettings()
        {
            if (_nodeSettings != null) { return; }

            // Check if node settings folder exists
            string       assetPath   = AssetDatabase.GetAssetPath(this);
            List<string> splitString = assetPath.Split("/").ToList();
            splitString.RemoveAt(splitString.Count - 1);
            string basePath         = string.Join("/", splitString);
            string nodeSettingsPath = basePath + "/NodeSettings";
            if (!AssetDatabase.IsValidFolder(basePath + "/NodeSettings"))
            {
                AssetDatabase.CreateFolder(basePath, "NodeSettings");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // Check if at least on node setting exists
            string[] foundAssets = AssetDatabase.FindAssets("t:" + nameof(SO_SockNodeSettings), new[] { nodeSettingsPath });
            if (foundAssets.Length == 0)
            {
                SO_SockNodeSettings nodeSettings = CreateInstance<SO_SockNodeSettings>();
                AssetDatabase.CreateAsset(nodeSettings, nodeSettingsPath + "/Default.asset");
                EditorUtility.SetDirty(nodeSettings);
                AssetDatabase.SaveAssetIfDirty(nodeSettings);
                AssetDatabase.Refresh();
                _nodeSettings = nodeSettings;
            }
        }

        internal NodeEditorPreferences.Settings GetReferencedXNodeSettings()
        {
            _xNodeSettings.typeColors[typeof(NodeInfo).FullName] = _connectionColor;
            _xNodeSettings.noodlePath                            = _connectionStyle;
            return _xNodeSettings;
        }

        internal void ResetGraphSettings(SO_SockSettings sockSettings)
        {
            _connectionColor = sockSettings.ConnectionColor;
            _connectionStyle = sockSettings.ConnectionStyle;
        }

        internal void ResetExportSettings(SO_SockSettings sockSettings) { _lineBreakReplacementString = sockSettings.LineBreakReplacementString; }
    }
}