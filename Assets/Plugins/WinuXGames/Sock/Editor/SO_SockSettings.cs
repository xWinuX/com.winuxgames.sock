using System;
using UnityEngine;

namespace WInuXGames.Sock.Plugins.Editor
{

    
    [Serializable]
    public class SockNodeSettings
    {
        [SerializeField] private Color _color;
        [SerializeField] private int   _width;

        public Color Color => _color;
        public int   Width => _width;
    }

    public class SO_SockNodeSettings : ScriptableObject
    {
        [SerializeField] private SockNodeSettings _lineNodeSettings;
        [SerializeField] private SockNodeSettings _optionNodeSettings;
        [SerializeField] private SockNodeSettings _startNodeSettings;
        [SerializeField] private SockNodeSettings _lineMergerNodeSettings;
    }
    
    public class SO_SockSettings : ScriptableObject
    {
        [SerializeField] private SO_SockNodeSettings _nodeSettings;
    }
}