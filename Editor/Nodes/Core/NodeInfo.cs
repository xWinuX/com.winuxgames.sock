using System;
using UnityEngine;

namespace WinuXGames.Sock.Editor.Nodes.Core
{
    [Serializable]
    internal struct NodeInfo
    {
        public static NodeInfo ErrorNodeInfo = new NodeInfo(null, null, -1, "-");

        [SerializeField] private StartNode _startNode;
        [SerializeField] private int       _indent;
        [SerializeField] private string    _identifier;

        private Type _sourceType;

        public NodeInfo(Type sourceType, StartNode startNode, int indent, string identifier)
        {
            _sourceType = sourceType;
            _startNode  = startNode;
            _indent     = indent;
            _identifier = identifier;
        }


        public StartNode StartNode  => _startNode;
        public Type      SourceType => _sourceType;
        public int       Indent     => _indent;
        public string    Identifier => _identifier;
    }
}