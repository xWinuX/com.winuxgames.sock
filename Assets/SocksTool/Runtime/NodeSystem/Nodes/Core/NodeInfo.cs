using System;
using UnityEngine;
using Yarn;

namespace SocksTool.Runtime.NodeSystem.Nodes.Core
{
    [Serializable]
    public struct NodeInfo
    {
        public NodeInfo(StartNode startNode, int indent, int count, int offset)
        {
            _startNode = startNode;
            _indent    = indent;
            _count     = count;
            _offset    = offset;
        }

        [SerializeField] private StartNode _startNode;
        [SerializeField] private int    _indent;
        [SerializeField] private int    _count;
        [SerializeField] private int    _offset;


        public static NodeInfo ErrorNodeInfo = new NodeInfo(null, -1, -1, -1);

        public StartNode StartNode => _startNode;
        public int       Indent    => _indent;
        public int       Count     => _count;
        public int       Offset    => _offset;

        public bool CanConnectTo(NodeInfo other) => !StartNode.Equals(other.StartNode) && Indent == other.Indent;
    }
}