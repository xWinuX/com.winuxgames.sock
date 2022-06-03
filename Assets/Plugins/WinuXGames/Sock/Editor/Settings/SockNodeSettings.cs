using System;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Settings
{
    [Serializable]
    internal class SockNodeSettings
    {
        [SerializeField] private Color _color;
        [SerializeField] private int   _width;

        public Color Color { get => _color; set => _color = value; }
        public int   Width { get => _width; set => _width = value; }
    }
}