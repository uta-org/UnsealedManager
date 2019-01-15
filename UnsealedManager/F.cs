using System;
using System.Linq;

namespace UnsealedManager
{
    public static class F
    {
        public static string CleanLineBreak(this string str, string replacement = "")
        {
            return str.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", replacement);
        }

        //public static bool HasContent<T>(this T[] arr, T emptyComparer)
        //{
        //    return HasContent(arr, emptyComparer);
        //}

        public static bool HasContent<T>(this T[] arr, params T[] emptyComparers)
            where T : IEquatable<T>
        {
            emptyComparers = emptyComparers.Distinct().ToArray();
            return arr.Length > 0 && arr.All(item => !emptyComparers.Any(c => item.Equals(c)));
        }
    }
}