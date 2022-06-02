using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes.Core;
using WInuXGames.Sock.Plugins.Editor.Utility;
using XNode;

namespace WInuXGames.Sock.Plugins.Editor.NodeSystem.Nodes
{
    [NodeWidth(300)]
    [CreateNodeMenu("Dialogue/Start", 3)]
    public class StartNode : MultiInputNode
    {
        public const string OutputFieldName = nameof(_out);
        public const string TitleFieldName  = nameof(_title);
        public const string TagsFieldName   = nameof(_tags);

        [SerializeField]
        [Output(connectionType = ConnectionType.Override)]
        private NodeInfo _out;

        [SerializeField] private string       _title = "NodeTitle";
        [SerializeField] private List<string> _tags  = new List<string>();

        public override string Name => "Start";

        public override Type[] AllowedInputTypes { get; } =
        {
            typeof(LineNode),
            typeof(OptionNode)
        };

        public List<string> Tags  => _tags;
        public string       Title { get => _title; set => _title = value; }

        public override object GetValue(NodePort port)
        {
            LastValidNodeInfo = new NodeInfo(typeof(StartNode), this, 0, "0");
            return LastValidNodeInfo;
        } 

        protected override int GetIndent() => 0;

        public override void GetText(StringBuilder sb, int index = 0, bool includeSockTags = true)
        {
            // Header
            sb.Append("title: ").Append(_title);
            sb.AppendLine();
            sb.Append("tags: ");

            // User tags
            foreach (string tag in _tags) { sb.Append(tag).Append(' '); }

            // Internal Sock Tags
            if (includeSockTags)
            {
                sb.Append(SockConstants.SockStartNodePositionTag).Append(':');
                GetPositionString(sb);
                sb.Append(' ');
            }

            sb.AppendLine();

            // Start Node
            sb.Append("---");
            sb.AppendLine();
        }
    }
}