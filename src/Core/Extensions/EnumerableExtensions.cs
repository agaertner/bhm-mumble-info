using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekres.Mumble_Info {
    internal static class EnumerableExtensions {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) {
            return !source?.Any() ?? true;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }
    }
}
