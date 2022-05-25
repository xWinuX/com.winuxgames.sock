using System;
using UnityEngine;
using Yarn;

namespace SocksTool.Runtime.NodeSystem.Nodes.Core
{
    [Serializable]
    public struct NodeInfo
    {
        public NodeInfo(string nodeTitle, int indent, int count, int offset)
        {
            _nodeTitle = nodeTitle;
            _indent    = indent;
            _count     = count;
            _offset    = offset;
        }

        [SerializeField] private string _nodeTitle;
        [SerializeField] private int    _indent;
        [SerializeField] private int    _count;
        [SerializeField] private int    _offset;


        public static NodeInfo ErrorNodeInfo = new NodeInfo("ERROR", -1, -1, -1);

        public string NodeTitle => _nodeTitle;
        public int    Indent    => _indent;
        public int    Count     => _count;
        public int    Offset    => _offset;

        public bool CanConnectTo(NodeInfo other) => NodeTitle != other.NodeTitle && Indent == other.Indent;
    }
}