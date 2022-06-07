using UnityEditor;

namespace WinuXGames.Sock.Editor.Settings
{
    internal static class SockSettings
    {
        public const string SettingsPath = "Assets/Plugins/WinuXGames/SockSettings/SockSettings.asset";
        
        public const string SourceSettingsPath         = "Assets/Plugins/WinuXGames/Sock/Settings";
        public const string SourceSockSettingsPath     = SourceSettingsPath + "/_SockSettings.asset";
        public const string SourceSockNodeSettingsPath = SourceSettingsPath + "/NodeSettings/_NodeSettings.asset";

        public const string CopySettingsPath         = "Assets/Plugins/WinuXGames/SockSettings";
        public const string CopySockSettingsPath     = CopySettingsPath + "/SockSettings.asset";
        public const string CopySockNodeSettingsPath = CopySettingsPath + "/NodeSettings/NodeSettings.asset";

        private static SO_SockSettings _sockSettings;
        
        public static void ResetGraphSettings()
        {
            SO_SockSettings source = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(SourceSockSettingsPath);
            SO_SockSettings copy   = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(CopySockSettingsPath);

            copy.ResetGraphSettings(source);
        }
        
        public static void ResetExportSettings()
        {
            (SO_SockSettings source, SO_SockSettings copy) = GetSourceAndCopySockSettings();
            copy.ResetExportSettings(source);
        }

        public static void ResetNodeSettings()
        {
            (SO_SockNodeSettings source, SO_SockNodeSettings copy) = GetSourceAndCopySockNodeSettings();
            copy.ReplaceValuesWith(source);
        }

        public static SO_SockSettings GetSettings()
        {
            if (_sockSettings == null)
            {
                SO_SockSettings sockSettings = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(CopySockSettingsPath);
                _sockSettings = sockSettings;
            }

            if (_sockSettings.NodeSettings == null)
            {
                SO_SockNodeSettings sockNodeSettings = AssetDatabase.LoadAssetAtPath<SO_SockNodeSettings>(CopySockNodeSettingsPath);
                _sockSettings.NodeSettings = sockNodeSettings;
            }

            return _sockSettings;
        }
        
        private static (SO_SockSettings source, SO_SockSettings copy) GetSourceAndCopySockSettings()
        {
            SO_SockSettings source = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(SourceSockSettingsPath);
            SO_SockSettings copy   = AssetDatabase.LoadAssetAtPath<SO_SockSettings>(CopySockSettingsPath);

            return (source, copy);
        }

        private static (SO_SockNodeSettings source, SO_SockNodeSettings copy) GetSourceAndCopySockNodeSettings()
        {
            SO_SockNodeSettings source = AssetDatabase.LoadAssetAtPath<SO_SockNodeSettings>(SourceSockNodeSettingsPath);
            SO_SockNodeSettings copy   = AssetDatabase.LoadAssetAtPath<SO_SockNodeSettings>(CopySockNodeSettingsPath);

            return (source, copy);
        }

    }
}