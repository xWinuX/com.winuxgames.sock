﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Settings
{
    internal class SO_SockNodeSettings : ScriptableObject
    {
        [SerializeField] private SockNodeSettings _lineNodeSettings       = new SockNodeSettings(new Color(0.61f, 0.44f, 0.18f), 300);
        [SerializeField] private SockNodeSettings _optionNodeSettings     = new SockNodeSettings(new Color(0.21f, 0.42f, 0.61f), 300);
        [SerializeField] private SockNodeSettings _startNodeSettings      = new SockNodeSettings(new Color(0.22f, 0.61f, 0.26f), 300);
        [SerializeField] private SockNodeSettings _lineMergerNodeSettings = new SockNodeSettings(new Color(0.61f, 0.54f, 0.15f), 100);
        [SerializeField] private SockNodeSettings _stopNodeSettings       = new SockNodeSettings(new Color(0.6f, 0.27f, 0.21f), 80);

        internal SockNodeSettings LineNodeSettings       => _lineNodeSettings;
        internal SockNodeSettings OptionNodeSettings     => _optionNodeSettings;
        internal SockNodeSettings StartNodeSettings      => _startNodeSettings;
        internal SockNodeSettings LineMergerNodeSettings => _lineMergerNodeSettings;
        internal SockNodeSettings StopNodeSettings       => _stopNodeSettings;

        internal void GetAllNodeSettings(Dictionary<string, SockNodeSettings> dictionary)
        {
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(LineNodeSettings)), LineNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(OptionNodeSettings)), OptionNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(StartNodeSettings)), StartNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(LineMergerNodeSettings)), LineMergerNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(StopNodeSettings)), StopNodeSettings);
        }

        internal void ResetValues()
        {
            SO_SockNodeSettings sockNodeSettings = CreateInstance<SO_SockNodeSettings>();
         
            Dictionary<string, SockNodeSettings> oldSettings = new Dictionary<string, SockNodeSettings>();
            GetAllNodeSettings(oldSettings);

            Dictionary<string, SockNodeSettings> newSettings = new Dictionary<string, SockNodeSettings>();
            sockNodeSettings.GetAllNodeSettings(newSettings);

            foreach ((string key, SockNodeSettings nodeSettings) in oldSettings)
            {
                nodeSettings.Color = newSettings[key].Color;
                nodeSettings.Width = newSettings[key].Width;
            }
        }
    }
}