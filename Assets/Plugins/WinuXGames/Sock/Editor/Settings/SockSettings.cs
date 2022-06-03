﻿using UnityEditor;

namespace WinuXGames.Sock.Editor.Settings
{
    public static class SockSettings
    {
        public const string SourceSettingsPath         = "Assets/Plugins/WinuXGames/Sock/Settings";
        public const string SourceSockSettingsPath     = SourceSettingsPath + "/_SockSettings.asset";
        public const string SourceSockNodeSettingsPath = SourceSettingsPath + "/NodeSettings/_NodeSettings.asset";

        public const string CopySettingsPath         = "Assets/Plugins/WinuXGames/SockSettings";
        public const string CopySockSettingsPath     = CopySettingsPath + "/SockSettings.asset";
        public const string CopySockNodeSettingsPath = CopySettingsPath + "/NodeSettings/NodeSettings.asset";

        private static SO_SockSettings _sockSettings;

        public static void ResetSettings()
        {
            SO_SockNodeSettings source = AssetDatabase.LoadAssetAtPath<SO_SockNodeSettings>(SourceSockNodeSettingsPath);
            SO_SockNodeSettings copy = AssetDatabase.LoadAssetAtPath<SO_SockNodeSettings>(CopySockNodeSettingsPath);

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
    }
}