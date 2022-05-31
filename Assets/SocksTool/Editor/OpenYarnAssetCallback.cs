using SocksTool.Editor.Builders;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using UnityEditor;
using XNodeEditor;

namespace SocksTool.Editor
{
    public static class OpenYarnAssetCallback
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenYarnAsset(int instanceID, int line)
        {
            if (Selection.activeObject == null) { return false; }

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!path.EndsWith(".yarn")) { return false; }

            YarnToDialogueGraphBuilder builder       = new YarnToDialogueGraphBuilder();
            DialogueGraph              dialogueGraph = builder.Build(path);
            NodeEditorWindow.Open(dialogueGraph);
            
            return true;
        }
    }
}