using System;
using UnityEngine;

namespace WinuXGames.Sock.Editor
{
    [Serializable]
    public class SockNodeSettings
    {
        [SerializeField] private Color _color;
        [SerializeField] private int   _width;

        public Color Color => _color;
        public int   Width => _width;
    }
}