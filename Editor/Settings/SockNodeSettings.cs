using System;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Settings
{
    [Serializable]
    internal class SockNodeSettings
    {
        public SockNodeSettings(Color color, int width)
        {
            _color = color;
            _width = width;
        }
        
        [SerializeField] private Color _color;
        [SerializeField] private int   _width;

        public Color Color { get => _color; set => _color = value; }
        public int   Width { get => _width; set => _width = value; }
    }
}