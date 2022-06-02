using UnityEditor;

namespace WinuXGames.Sock.Editor
{
    [InitializeOnLoad]
    public static class SockSetup
    {
        private const string SourceSettingsPath     = "Assets/Plugins/WinuXGames/Sock/Settings";
        private const string SourceSockSettingsPath = SourceSettingsPath + "/_SockSettings.asset";
        private const string SourceNodeSettingsPath = SourceSettingsPath + "/NodeSettings/_NodeSettings.asset";

        private const string CopySettingsPath     = "Assets/Plugins/WinuXGames/SockSettings";
        private const string CopySockSettingsPath = CopySettingsPath + "/SockSettings.asset";
        private const string CopyNodeSettingsPath = CopySettingsPath + "/NodeSettings/NodeSettings.asset";

        private static readonly string[] FoldersToCreate =
        {
            "Assets",
            "Assets/Plugins",
            "Assets/Plugins/WinuXGames",
            "Assets/Plugins/WinuXGames/SockSettings",
            "Assets/Plugins/WinuXGames/SockSettings/NodeSettings",
        };

        static SockSetup()
        {
            for (int i = 0; i < FoldersToCreate.Length; i++)
            {
                string path = FoldersToCreate[i];
                if (AssetDatabase.IsValidFolder(path)) { continue; }

                AssetDatabase.CreateFolder(FoldersToCreate[i - 1], path.Split("/")[^1]);
            }

            if (!System.IO.File.Exists(CopySockSettingsPath)) { AssetDatabase.CopyAsset(SourceSockSettingsPath, CopySockSettingsPath); }

            if (!System.IO.File.Exists(CopyNodeSettingsPath))
            {
                AssetDatabase.CopyAsset(SourceNodeSettingsPath, CopyNodeSettingsPath)
            }
        }
    }
}