using System;
using SocksTool.Runtime;
using SocksTool.Runtime.NodeSystem.NodeGraphs;
using SocksTool.Runtime.NodeSystem.Nodes;
using SocksTool.Runtime.Utility;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;
using Node = Yarn.Node;

namespace SocksTool.Editor
{
    public static class YarnOpenAssetCallback
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenYarnAsset(int instanceID, int line)
        {
            if (Selection.activeObject == null) { return false; }

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!path.EndsWith(".yarn")) { return false; }

            OpenYarnFile(path);
            return true;
        }


        public static void OpenYarnFile(string assetPath)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(assetPath);

            CompilationResult result;
            try
            {
                result = Compiler.Compile(compilationJob);
                foreach ((string key, Node value) in result.Program.Nodes)
                {
                    Debug.Log("Key " + key);
                    Debug.Log(value.Name);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Failed to compile Yarn Script: " + assetPath);
                throw;
            }

            DialogueGraph dialogueGraph = ScriptableObject.CreateInstance<DialogueGraph>();
            
            foreach ((string key, Node node) in result.Program.Nodes)
            {
                Debug.Log("Node: " + node.Name);
                Debug.Log("- Instructions: ");

                int      nodeCount = 0;
                LineNode previousNode = null;
                foreach (Instruction instruction in node.Instructions)
                {
                    Debug.Log("-- OP Code: " + instruction.Opcode);

                    switch (instruction.Opcode)
                    {
                        case Instruction.Types.OpCode.RunLine:
                            LineNode lineNode = ScriptableObject.CreateInstance<LineNode>();
                            lineNode.name       = "Line";
                            lineNode.position.x = nodeCount * 350;
                            foreach (Operand operand in instruction.Operands)
                            {
                                if (operand.ValueCase == Operand.ValueOneofCase.StringValue)
                                {
                                    if (result.StringTable.TryGetValue(operand.StringValue, out StringInfo stringInfo))
                                    {
                                        MarkupParseResult markupParseResult = YarnUtility.ParseMarkup(stringInfo.text);
                                        
                                        if (markupParseResult.TryGetAttributeWithName("character", out MarkupAttribute characterNameAttribute))
                                        {
                                            if (characterNameAttribute.Properties.TryGetValue("name", out MarkupValue value)) { lineNode.Character = value.StringValue; }

                                            lineNode.Text = markupParseResult.DeleteRange(characterNameAttribute).Text;
                                        }
                                        else { lineNode.Text = markupParseResult.Text; }
                                    }
                                }
                            }

                            if (previousNode != null)
                            {
                                NodePort input  = lineNode.GetInputPort("_in");
                                NodePort output = previousNode.GetOutputPort("_out");

                                output.Connect(input);
                            }

                            dialogueGraph.nodes.Add(lineNode);
                            nodeCount++;
                            previousNode = lineNode;
                            break;
                    }


                    foreach (Operand operand in instruction.Operands)
                    {
                        Debug.Log("--- Operand: " + operand.ValueCase);
                        switch (operand.ValueCase)
                        {
                            case Operand.ValueOneofCase.None:
                                Debug.Log("---- None");
                                break;
                            case Operand.ValueOneofCase.StringValue:
                                Debug.Log("----  " + operand.StringValue);
                                if (result.StringTable.TryGetValue(operand.StringValue, out StringInfo stringInfo)) { Debug.Log("---- " + stringInfo.text); }

                                break;
                            case Operand.ValueOneofCase.BoolValue:
                                Debug.Log("---- " + operand.BoolValue);
                                break;
                            case Operand.ValueOneofCase.FloatValue:
                                Debug.Log("---- " + operand.FloatValue);
                                break;
                        }
                    }
                }
            }

            NodeEditorWindow.Open(dialogueGraph);
        }
    }
}