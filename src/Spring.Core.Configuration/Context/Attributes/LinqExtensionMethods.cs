using System;
using System.Collections.Generic;

namespace Spring.Context.Attributes
{
    /// <summary>
    /// Limited extension methods reproducing the small subset of LINQ that is needed in the code; required b/c the project targets .NET 2.0 where LINQ is not available.
    /// </summary>
    internal static class LinqExtensionMethods
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            int counter = 0;
            foreach (TSource obj in source)
            {
                counter++;
            }

            return counter;
        }

        internal static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value) where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");
            
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
            if (source == null) throw new ArgumentNullException("source");

            IList<TSource> results = new List<TSource>();

            foreach (TSource obj in source)
            {
                results.Add(obj);
            }

            return results;
        }


        internal static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source,
                                                            Predicate<TSource> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

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
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

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