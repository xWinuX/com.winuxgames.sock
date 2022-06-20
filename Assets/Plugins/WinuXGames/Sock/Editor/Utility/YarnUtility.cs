using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;

namespace WinuXGames.Sock.Editor.Utility
{
    /// <summary>
    /// Holds Utility for processing Yarn Code
    /// </summary>
    public static class YarnUtility
    {
        private static readonly Dialogue Dialogue = new Dialogue(new MemoryVariableStore());

        /// <summary>
        /// Returns Text without character name
        /// </summary>
        /// <param name="result">MarkupParseResult of text to remove character name from</param>
        /// <param name="characterName">Returns either the removed name or null if there wasn't a name</param>
        /// <returns>MarkupParseResult text without the character name</returns>
        public static string TextWithoutCharacterName(MarkupParseResult result, out string characterName)
        {
            // Parse text and look for character attribute and assign it to node if it's there
            if (result.TryGetAttributeWithName("character", out MarkupAttribute characterNameAttribute))
            {
                characterName = GetPropertyStringValue(characterNameAttribute.Properties, "name");
                return result.Text.Remove(characterNameAttribute.Position, characterNameAttribute.Length);
            }

            characterName = null;
            return result.Text;
        }

        /// <summary>
        /// Parses Markup in given text
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Result of parsing</returns>
        public static MarkupParseResult ParseMarkup(string text) => Dialogue.ParseMarkup(text);
        
        /// <summary>
        /// Tries to get number value from given property dictionary
        /// </summary>
        /// <param name="property">Property Dictionary</param>
        /// <param name="key">Key to parse</param>
        /// <param name="defaultValue">Value to return if value getting failed</param>
        /// <returns>Either the found value or the default value if nothing was found</returns>
        public static float GetPropertyNumberValue(IReadOnlyDictionary<string, MarkupValue> property, string key, float defaultValue = 0f)
        {
            if (!property.TryGetValue(key, out MarkupValue markupValue)) { return defaultValue; }

            defaultValue = markupValue.FloatValue;
            if (defaultValue == 0f) { defaultValue = markupValue.IntegerValue; }

            return defaultValue;
        }

        /// <summary>
        /// Tries to get string value from given property dictionary
        /// </summary>
        /// <param name="property">Property Dictionary</param>
        /// <param name="key">Key to parse</param>
        /// <param name="defaultValue">Value to return if value getting failed</param>
        /// <returns>Either the found value or the default value if nothing was found</returns>
        public static string GetPropertyStringValue(IReadOnlyDictionary<string, MarkupValue> property, string key, string defaultValue = "") =>
            !property.TryGetValue(key, out MarkupValue markupValue) ? defaultValue : markupValue.StringValue;

        /// <summary>
        /// Compiles yarn file at given path
        /// </summary>
        /// <param name="path">Path to yarn file</param>
        /// <returns>Compilation results</returns>
        public static CompilationResult CompileYarnFile(string path)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromFiles(path);

            return CompileYarn(compilationJob);
        }

        /// <summary>
        /// Compiles given yarn code
        /// </summary>
        /// <param name="yarnString">Yarn code to parse</param>
        /// <returns>Compilation results</returns>
        public static CompilationResult CompileYarnString(string yarnString)
        {
            CompilationJob compilationJob = CompilationJob.CreateFromString("preview", yarnString);

            return CompileYarn(compilationJob);
        }

        /// <summary>
        /// Gets program counter from given label
        /// </summary>
        /// <param name="labels">All labels of node</param>
        /// <param name="label">Label to search</param>
        /// <returns>Program counter of given label</returns>
        /// <exception cref="IndexOutOfRangeException">Is caused by the given label not existing</exception>
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