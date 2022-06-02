using UnityEngine;

namespace WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes.Core
{
    public abstract class MultiInputNode : SockNode
    {
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Multiple)]
        private NodeInfo _in;

        public NodeInfo In => _in;
    }
}