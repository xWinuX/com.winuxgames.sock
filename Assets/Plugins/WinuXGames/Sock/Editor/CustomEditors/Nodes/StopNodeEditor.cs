using WinuXGames.Sock.Editor.NodeSystem.Nodes;
using WinuXGames.Sock.Editor.Settings;

namespace WinuXGames.Sock.Editor.CustomEditors.Nodes
{
    [CustomNodeEditor(typeof(StopNode))]
    public class StopNodeEditor : SockNodeEditor<StopNode>
    {
        protected override SockNodeSettings Settings { get; } = SockSettings.GetSettings().NodeSettings.StopNodeSettings;

        protected override void DrawNode()
        {
            DrawInputNodePort();
        }
    }
}