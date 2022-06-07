using UnityEditor;
using UnityEngine;
using WinuXGames.Sock.Editor.Settings;

namespace WinuXGames.Sock.Editor.Core
{
    [InitializeOnLoad]
    public static class SockSetup
    {
        private static readonly string[] FoldersToCreate =
        {
            "Assets",
            "Assets/Plugins",
            "Assets/Plugins/WinuXGames",
            "Assets/Plugins/WinuXGames/SockSettings",
        };

        static SockSetup()
        {
            for (int i = 0; i < FoldersToCreate.Length; i++)
            {
                string path = FoldersToCreate[i];
                if (AssetDatabase.IsValidFolder(path)) { continue; }

                AssetDatabase.CreateFolder(FoldersToCreate[i - 1], path.Split("/")[^1]);
            }

            if (!System.IO.File.Exists(SockSettings.SettingsPath))
            {
                SO_SockSettings sockSettings = ScriptableObject.CreateInstance<SO_SockSettings>();
                AssetDatabase.CreateAsset(sockSettings, SockSettings.SettingsPath);
                EditorUtility.SetDirty(sockSettings);
            }
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
    }
}