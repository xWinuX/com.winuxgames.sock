using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    public class LineNode : Node
    {
        [SerializeField] private string _character;
        [SerializeField] [TextArea(5,5)] private string _text;
        
        [SerializeField] [Input]  private LineNode _in;
        [SerializeField] [Output] private LineNode _out;

        public string Character { get => _character; set => _character = value; }
        public string Text      { get => _text;      set => _text = value; }

        public LineNode In  { get => _in;  set => _in = value; }
        public LineNode Out { get => _out; set => _out = value; }

        public override object GetValue(NodePort port) => port.fieldName == "_out" ? _out : null;
    }
}