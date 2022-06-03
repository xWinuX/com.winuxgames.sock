using WinuXGames.Sock.Editor.CustomEditors.Nodes.Core;
using WinuXGames.Sock.Editor.Nodes;
using WinuXGames.Sock.Editor.Settings;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(StopNode))]
    internal class StopNodeEditor : SockNodeEditor<StopNode>
    {
        protected override SockNodeSettings Settings { get; } = SockSettings.GetSettings().NodeSettings.StopNodeSettings;

        protected override void DrawNode()
        {
            DrawInputNodePort();
        }
    }
}