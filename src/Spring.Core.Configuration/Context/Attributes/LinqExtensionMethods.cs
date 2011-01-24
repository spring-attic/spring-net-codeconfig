using System;
using System.Collections.Generic;

namespace Spring.Context.Attributes
{
    internal static class LinqExtensionMethods
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            int counter = 0;
            foreach (TSource obj in source)
            {
                counter++;
            }

            return counter;
        }

        internal static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value) where TSource : class
        {
            foreach (TSource obj in source)
            {
                if (obj == value)
                {
                    return true;
                }
            }

            return false;
        }

        internal static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            return source.Where(delegate { return true; });
        }


        internal static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source,
                                                            Predicate<TSource> predicate)
        {
            IList<TSource> matching = new List<TSource>();

            foreach (TSource obj in source)
            {
                if (predicate(obj))
                {
                    matching.Add(obj);
                }
            }

            return matching;
        }


        internal static bool Any<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            foreach (TSource obj in source)
            {
                if (predicate(obj))
                {
                    return true;
                }
            }

            return false;
        }
    }
}