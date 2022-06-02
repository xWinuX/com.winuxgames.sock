using System.Collections.Generic;

namespace WInuXGames.Sock.Plugins.Editor.Extensions
{
    public static class ListExtensions
    {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }
    }
}