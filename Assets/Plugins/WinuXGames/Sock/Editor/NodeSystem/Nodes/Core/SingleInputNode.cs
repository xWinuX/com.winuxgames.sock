using UnityEngine;

namespace WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes.Core
{
    public abstract class SingleInputNode : SockNode
    {
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        private NodeInfo _in;

        public NodeInfo In => _in;
    }
}