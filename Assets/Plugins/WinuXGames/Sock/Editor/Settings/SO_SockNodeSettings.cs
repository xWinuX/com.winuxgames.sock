using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Settings
{
    [CreateAssetMenu(menuName = "Create SO_SockNodeSettings", fileName = "SO_SockNodeSettings", order = 0)]
    public class SO_SockNodeSettings : ScriptableObject
    {
        [SerializeField] private SockNodeSettings _lineNodeSettings;
        [SerializeField] private SockNodeSettings _optionNodeSettings;
        [SerializeField] private SockNodeSettings _startNodeSettings;
        [SerializeField] private SockNodeSettings _lineMergerNodeSettings;
        [SerializeField] private SockNodeSettings _stopNodeSettings;

        public SockNodeSettings LineNodeSettings       => _lineNodeSettings;
        public SockNodeSettings OptionNodeSettings     => _optionNodeSettings;
        public SockNodeSettings StartNodeSettings      => _startNodeSettings;
        public SockNodeSettings LineMergerNodeSettings => _lineMergerNodeSettings;
        public SockNodeSettings StopNodeSettings       => _stopNodeSettings;

        public void GetAllNodeSettings(Dictionary<string, SockNodeSettings> dictionary) 
        {
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(LineNodeSettings)), LineNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(OptionNodeSettings)), OptionNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(StartNodeSettings)), StartNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(LineMergerNodeSettings)), LineMergerNodeSettings);
            dictionary.Add(ObjectNames.NicifyVariableName(nameof(StopNodeSettings)), StopNodeSettings);
        }

        public void ReplaceValuesWith(SO_SockNodeSettings sockNodeSettings)
        {
            Dictionary<string,SockNodeSettings> oldSettings = new Dictionary<string, SockNodeSettings>();
            GetAllNodeSettings(oldSettings);
            
            Dictionary<string,SockNodeSettings> newSettings = new Dictionary<string, SockNodeSettings>();
            sockNodeSettings.GetAllNodeSettings(newSettings);

            foreach ((string key, SockNodeSettings nodeSettings) in oldSettings)
            {
                nodeSettings.Color = newSettings[key].Color;
                nodeSettings.Width = newSettings[key].Width;
            }
        }
    }
}