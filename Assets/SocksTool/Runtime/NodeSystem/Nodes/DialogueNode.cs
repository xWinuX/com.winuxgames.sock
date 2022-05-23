using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    public abstract class DialogueNode : Node
    {
        public const string InputFieldName = nameof(_in);
        
        [SerializeField]
        [Input]
        private LineNode _in;
        
        public LineNode In => _in;

        public override object GetValue(NodePort port) => _in;
    }
}