using UnityEditor;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Settings
{
    internal static class SockSettings
    {
        public const string SettingsPath = "Assets/Plugins/WinuXGames/SockSettings/SockSettings.asset";
        
        private static readonly string[] FoldersToCreate =
        {
            "Assets",
            "Assets/Plugins",
            "Assets/Plugins/WinuXGames",
            "Assets/Plugins/WinuXGames/SockSettings",
        };

        private static SO_SockSettings _sockSettings;

        public static SO_SockSettings GetSettings()
        {
            if (_sockSettings != null) { return _sockSettings; }

            // Try to load sock setting
            SO_SockSettings loadedSockSettings = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(SettingsPath);
            if (loadedSockSettings != null)
            {
                _sockSettings = loadedSockSettings;
                return _sockSettings;
            }

            // Create folders
            for (int i = 0; i < FoldersToCreate.Length; i++)
            {
                string path = FoldersToCreate[i];
                if (AssetDatabase.IsValidFolder(path)) { continue; }

                AssetDatabase.CreateFolder(FoldersToCreate[i - 1], path.Split("/")[^1]);
            }

            // Create Settings
            if (!System.IO.File.Exists(SettingsPath))
            {
                SO_SockSettings sockSettings = ScriptableObject.CreateInstance<SO_SockSettings>();
                AssetDatabase.CreateAsset(sockSettings, SettingsPath);
                EditorUtility.SetDirty(sockSettings);
                AssetDatabase.SaveAssetIfDirty(sockSettings);
                AssetDatabase.Refresh();
                _sockSettings = sockSettings;
                return _sockSettings;
            }
            
            return null;
        }
    }
}