using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;

namespace WinuXGames.Sock.Editor.Utility
{
    public static class YarnUtility
    {
        private static readonly Dialogue          Dialogue = new Dialogue(new MemoryVariableStore());
        public static           MarkupParseResult ParseMarkup(string text) => Dialogue.ParseMarkup(text);

        public static float GetPropertyNumberValue(IReadOnlyDictionary<string, MarkupValue> property, string key, float defaultValue = 0f)
        {
            if (!property.TryGetValue(key, out MarkupValue markupValue)) { return defaultValue; }

            defaultValue = markupValue.FloatValue;
            if (defaultValue == 0f) { defaultValue = markupValue.IntegerValue; }

            return defaultValue;
        }

        public static string GetPropertyStringValue(IReadOnlyDictionary<string, MarkupValue> property, string key, string defaultValue = "") =>
            !property.TryGetValue(key, out MarkupValue markupValue) ? defaultValue : markupValue.StringValue;

        public static CompilationResult CompileYarnFile(string path)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(path);

            return CompileYarn(compilationJob);
        }

        public static CompilationResult CompileYarnString(string yarnString)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromString("preview", yarnString);

            return CompileYarn(compilationJob);
        }

        public static int GetProgramCounterFromLabel(IReadOnlyDictionary<string, int> labels, string label)
        {
            if (labels.TryGetValue(label, out int programCounter)) { return programCounter; }

            throw new IndexOutOfRangeException($"Label {label} Does not exist!");
        }

        private static CompilationResult CompileYarn(CompilationJob compilationJob)
        {
            CompilationResult result;
            try { result = Compiler.Compile(compilationJob); }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Failed to compile Yarn Script!");
                throw;
            }

            return result;
        }
    }
}