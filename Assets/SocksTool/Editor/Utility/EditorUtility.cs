using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SocksTool.Editor.Utility
{
    public static class EditorUtility
    {
        public static string AssetPath => "Assets/_" + GetProjectName();
        
        public static string GetProjectName()
        {
            string[] split       = Application.dataPath.Split('/');
            string   projectName = split[^2];
            return projectName;
        }

        public static string AbsoluteToRelative(string path)
        {
            path = path.Replace('\\', '/');

            if (path.StartsWith(Application.dataPath)) { return "Assets" + path[Application.dataPath.Length..]; }

            return "";
        }

        public static void InstantiatePrefabFromResources(string path)
        {
            Object     o          = Resources.Load(path);
            GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(o);
            if (Selection.gameObjects.Length > 0) { gameObject.transform.SetParent(Selection.gameObjects[0].transform, false); }

            EditorSceneManager.MarkAllScenesDirty();
        }

        public static bool PathIsRelative(string path) => path.StartsWith("Assets");
    }
}