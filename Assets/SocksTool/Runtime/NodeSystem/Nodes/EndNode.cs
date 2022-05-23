namespace SocksTool.Runtime.NodeSystem.Nodes
{
    [NodeWidth(100)]
    [CreateNodeMenu("Dialogue/Node End")]
    public class EndNode : DialogueNode
    {
        protected override void Init()
        {
            base.Init();
            name = "End";
        }
    }
}