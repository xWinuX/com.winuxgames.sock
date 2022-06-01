using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocksTool.Runtime.NodeSystem.Nodes.Core;
using SocksTool.Runtime.NodeSystem.Utility;
using SocksTool.Runtime.Utility;
using UnityEngine;
using XNode;

namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Option", 1)]
    public class OptionNode : SingleInputNode
    {
        public const string OutputFieldName = nameof(_outputList);

        [SerializeField]
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        private List<NodeInfo> _outputList = new List<NodeInfo>();

        public override string Name              => "Option";
        public override Type[] AllowedInputTypes { get; } = { typeof(LineNode) };

        public List<string> OptionStringList { get; } = new List<string>();

        public override object GetValue(NodePort port)
        {
            NodeInfo nodeInfo = GetInputValue(InputFieldName, NodeInfo.ErrorNodeInfo);
            LastValidNodeInfo = new NodeInfo(typeof(OptionNode), nodeInfo.StartNode, nodeInfo.Indent + 1, nodeInfo.Identifier + Outputs.ToList().IndexOf(port));
            return LastValidNodeInfo;
        }

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true)
        {
            base.GetText(sb, index, includeSockTags);
            sb.Append("-> ");
            sb.Append(OptionStringList[index]);
            if (index == 0 && includeSockTags) { AddPositionTag(sb, SockConstants.SockPositionTag); }

            sb.AppendLine();
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