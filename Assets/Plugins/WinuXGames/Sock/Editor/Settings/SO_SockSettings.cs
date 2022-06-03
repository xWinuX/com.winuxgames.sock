using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using XNodeEditor;

namespace WinuXGames.Sock.Editor.Settings
{
    internal class SO_SockSettings : ScriptableObject
    {
        [SerializeField] private Color      _connectionColor;
        [SerializeField] private NoodlePath _connectionStyle;

        [SerializeField] private SO_SockNodeSettings _nodeSettings;

        private readonly NodeEditorPreferences.Settings _xNodeSettings = new NodeEditorPreferences.Settings();

        internal Color               ConnectionColor { get => _connectionColor; set => _connectionColor = value; }
        internal NoodlePath          ConnectionStyle { get => _connectionStyle; set => _connectionStyle = value; }
        internal SO_SockNodeSettings NodeSettings    { get => _nodeSettings;    set => _nodeSettings = value; }

        public NodeEditorPreferences.Settings GetReferencedXNodeSettings()
        {
            _xNodeSettings.typeColors[typeof(NodeInfo).FullName] = _connectionColor;
            _xNodeSettings.noodlePath                            = _connectionStyle;
            return _xNodeSettings;
        }

        public void ReplaceValuesWith(SO_SockSettings sockSettings)
        {
            _connectionColor = sockSettings._connectionColor;
            _connectionStyle = sockSettings._connectionStyle;
        }
    }
}