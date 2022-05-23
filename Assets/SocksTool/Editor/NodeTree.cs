using System.Collections.Generic;
using XNode;

namespace SocksTool.Editor
{
    public class NodeTree
    {
        public List<XNode.Node> Nodes          { get; }
        public NodePort         LastOutputPort { get; set; }
        public Stack<NodeTree>  SubNodes       { get; set; } = new Stack<NodeTree>();

        public NodeTree(List<XNode.Node> dialogueNodes, NodePort lastOutputPort)
        {
            Nodes          = dialogueNodes;
            LastOutputPort = lastOutputPort;
        }
    }
}