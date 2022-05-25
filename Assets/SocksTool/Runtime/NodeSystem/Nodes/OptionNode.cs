using System.Collections.Generic;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Option")]
    public class OptionNode : SingleInputNode
    {
        public const string OutputFieldName = nameof(_outputList);

        [SerializeField]
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        private List<NodeInfo> _outputList = new List<NodeInfo>();

        public override string Name => "Option";

        public List<string> OptionStringList { get; } = new List<string>();

        public override object GetValue(NodePort port)
        {
            NodeInfo nodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            return new NodeInfo(nodeInfo.NodeTitle, nodeInfo.Indent + 1, 0, nodeInfo.Offset + nodeInfo.Count);
        }
        
        public NodePort AddOption(string option)
        {
            NodePort output = AddDynamicOutput(
                typeof(NodeInfo),
                ConnectionType.Override,
                TypeConstraint.Strict,
                OutputFieldName + " " + _outputList.Count
            );

            _outputList.Add(new NodeInfo());
            OptionStringList.Add(option);

            UpdatePorts();

            return output;
        }
    }
}