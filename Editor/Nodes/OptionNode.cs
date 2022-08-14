using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using WinuXGames.Sock.Editor.Nodes.Core;
using WinuXGames.Sock.Editor.Utility;
using XNode;

namespace WinuXGames.Sock.Editor.Nodes
{
    [CreateNodeMenu("Dialogue/Option", 1)]
    internal class OptionNode : SingleInputNode
    {
        public const string OutputFieldName = nameof(_outputList);

        [SerializeField]
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)]
        private List<NodeInfo> _outputList = new List<NodeInfo>();

        [SerializeField] // The options list shouldn't loose it's contents on reloads
        [HideInInspector] // But it shouldn't be displayed since that's what the dynamic port list above does
        private List<string> _optionStringList = new List<string>();

        public override string Name              => "Option";
        public override Type[] AllowedInputTypes { get; } = { typeof(LineNode) };

        public List<string> OptionStringList => _optionStringList;

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