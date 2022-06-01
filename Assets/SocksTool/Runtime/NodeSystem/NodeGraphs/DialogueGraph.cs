using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.NodeGraphs
{
    [CreateAssetMenu]
    public class DialogueGraph : NodeGraph
    {
        public bool Ready { get; set; }
    }
}