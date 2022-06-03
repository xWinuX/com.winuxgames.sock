using UnityEngine;
using XNode;

namespace WinuXGames.Sock.Editor.NodeSystem.Nodes.Core
{
    public abstract class SingleInputNode : SockNode
    {
        [SerializeField]
        [Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
        private NodeInfo _in;

        public NodeInfo In => _in;
        
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            
            SockNode fromSockNode = from.node as SockNode;
            SockNode toSockNode   = to.node as SockNode;
            
            // Don't allow non sock nodes to be connected to sock nodes
            if (fromSockNode == null || toSockNode == null)
            {
                Disconnect(from, to, fromSockNode, toSockNode);
                Debug.LogError("You tried one of the nodes you tried to connect wasn't a sock node!");
                return;
            }

            // Check if node isn't connected to a node that hasn't the same start node
            // IMPORTANT: this can only be called after the loop check because getting a node value with a loop present crashes the application 
            NodeInfo fromInfo = fromSockNode.LastValidNodeInfo;
            NodeInfo toInfo   = toSockNode.LastValidNodeInfo;
            if (fromInfo.StartNode != null && toInfo.StartNode != null)
            {
                Debug.Log(fromInfo.StartNode.GetInstanceID());
                Debug.Log(toInfo.StartNode.GetInstanceID());
                if (!fromInfo.StartNode.Equals(toInfo.StartNode))
                {
                    toSockNode.Looped = true;
                    Disconnect(from, to, fromSockNode, toSockNode);
                    Debug.LogError("You can't connect nodes with different start nodes together!");
                    Debug.LogError("You can however connect nodes to the input of start nodes to execute a jump");
                }
            }
        }
    }
}