using System;
using UnityEditor;
using UnityEngine;
using Yarn;
using Yarn.Compiler;

namespace SocksTool.Editor
{
    public static class YarnParser
    {
        [MenuItem("Socks/Test")]
        private static void Run()
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(EditorUtility.AssetPath + "/Test.yarn");
            
            try
            {
                CompilationResult result = Compiler.Compile(compilationJob);
                foreach ((string key, Node value) in result.Program.Nodes)
                {
                    Debug.Log("Key " + key);
                    Debug.Log(value.Name);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}