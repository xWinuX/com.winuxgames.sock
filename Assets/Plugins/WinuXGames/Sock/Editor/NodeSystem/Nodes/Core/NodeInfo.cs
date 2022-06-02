using System;
using UnityEngine;

namespace SocksTool.Runtime.NodeSystem.Nodes.Core
{
    [Serializable]
    public struct NodeInfo
    {
        public NodeInfo(Type sourceType, StartNode startNode, int indent, string identifier)
        {
            _sourceType = sourceType;
            _startNode  = startNode;
            _indent     = indent;
            _identifier = identifier;
        }

        [SerializeField] private StartNode _startNode;
        [SerializeField] private int       _indent;
        [SerializeField] private string    _identifier;

        private Type _sourceType;

        public static NodeInfo ErrorNodeInfo = new NodeInfo(null, null, -1, "-");


        public StartNode StartNode  => _startNode;
        public Type      SourceType => _sourceType;
        public int       Indent     => _indent;
        public string    Identifier => _identifier;

        public bool CanConnectTo(NodeInfo other) => !StartNode.Equals(other.StartNode) && Indent == other.Indent;
    }
}