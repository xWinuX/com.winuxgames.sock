using XNode;

namespace SocksTool.Runtime
{
    public class DialogueNode : Node
    {
        [Input]  public DialogueNode In;
        [Output] public DialogueNode Out;

        public override object GetValue(NodePort port) => port.fieldName == "Out" ? Out : null;
    }
}