using System.Collections.Generic;
using Yarn;
using Yarn.Markup;

namespace SocksTool.Runtime.Utility
{
    public static class YarnUtility
    {
        private static readonly Dialogue     Dialogue = new Dialogue(new MemoryVariableStore());
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
    }
}