using UnityEngine;
using XNode;

namespace WinuXGames.Sock.Editor.NodeSystem.Nodes.Core
{
    public abstract class MultiInputNode : SockNode
    {
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Multiple)]
        private NodeInfo _in;

        public NodeInfo In => _in;
        
        [SerializeField]
        [HideInInspector]
        private StartNode _startNode;
        
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Ignore for Start Node
            if (GetType() == typeof(StartNode)) { return; }

            if (GetInputPort(InputFieldName).ConnectionCount == 1) { _startNode = GetProcessedValue().StartNode; }

            if (_startNode == null) { return; }

            SockNode fromSockNode = from.node as SockNode;
            if (fromSockNode != null &&!_startNode.Equals(fromSockNode.GetProcessedValue().StartNode))
            {
                from.Disconnect(to);
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            if (GetInputPort(InputFieldName).ConnectionCount == 0) { _startNode = null; }
        }
    }
}