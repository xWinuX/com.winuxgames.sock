using System.Collections.Generic;
using UnityEngine;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Options")]
    public class OptionNode : DialogueNode
    {
        public const string OptionsFieldName = nameof(_options);
        
        [SerializeField] 
        [Output(dynamicPortList = true)] 
        private List<string> _options = new List<string>();

        public List<string> Options => _options;
    }
}