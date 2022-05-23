using System.Text;
using UnityEngine;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Line")]
    public class LineNode : DialogueNode
    {
        public const string OutputFieldName = nameof(_out);
        
        [SerializeField]
        [Output(connectionType = ConnectionType.Override)]
        private LineNode _out;
        
        [SerializeField]                  
        private string _character;
        
        [SerializeField] [TextArea(5, 5)] 
        private string _text;

        public string Character { get => _character; set => _character = value; }
        
        public string Text { get => _text; set => _text = value; }

        public override string GetText()
        {
            StringBuilder sb = new StringBuilder();

            if (_character != string.Empty)
            {
                sb.Append(Character);
                sb.Append(": ");
            }

            sb.Append(_text);

            return sb.ToString();
        }
    }
}