using UnityEditor;
using WinuXGames.Sock.Editor.Builders;
using WinuXGames.Sock.Editor.NodeGraphs;
using XNodeEditor;

namespace WinuXGames.Sock.Editor
{
    public static class OpenYarnAssetCallback
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        private static bool OnOpenYarnAsset(int instanceID, int line)
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