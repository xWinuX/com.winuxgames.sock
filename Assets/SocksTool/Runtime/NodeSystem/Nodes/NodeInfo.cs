using System;
using UnityEngine;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [Serializable]
    public struct NodeInfo
    {
        public NodeInfo(string nodeTitle, int indent)
        {
            _nodeTitle = nodeTitle;
            _indent    = indent;
        }

        [SerializeField] private string _nodeTitle;
        [SerializeField] private int    _indent;

        public static NodeInfo ErrorNodeInfo = new NodeInfo("ERROR", -1);
        
        public string NodeTitle => _nodeTitle;
        public int    Indent    => _indent;

        
        public bool CanConnectTo(NodeInfo other) => NodeTitle != other.NodeTitle && Indent == other.Indent;
    }
}