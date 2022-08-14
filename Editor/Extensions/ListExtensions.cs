using System.Collections.Generic;

namespace WinuXGames.Sock.Editor.Extensions
{
    /// <summary>
    /// Extension Methods for Lists
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Swap given indices with each other in given list
        /// </summary>
        /// <param name="list">List to swap indices in</param>
        /// <param name="indexA">Index to swap with indexB</param>
        /// <param name="indexB">Index to swap with indexA</param>
        /// <typeparam name="T">Type of list to swap</typeparam>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB) { (list[indexA], list[indexB]) = (list[indexB], list[indexA]); }
    }
}