using System.Collections.Generic;
using System.Linq;

namespace Mitten.Mobile.Extensions
{
    /// <summary>
    /// Contains extensions for an enumerable.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether or not the list starts with the same elements as the second list. 
        /// </summary>
        /// <param name="lhs">Left-hand side to compare.</param>
        /// <param name="rhs">Right-hand side to compare.</param>
        /// <returns>True if the first list starts with the same elements as the second list, otherwise false.</returns>
        public static bool StartsWith<TValue>(this IEnumerable<TValue> lhs, IEnumerable<TValue> rhs)
        {
            Throw.IfArgumentNull(lhs, "lhs");
            Throw.IfArgumentNull(rhs, "rhs");

            int count = rhs.Count();

            if (lhs.Count() < count)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                if (!object.Equals(lhs.ElementAt(i), rhs.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether or not the list ends with the same elements as the second list. 
        /// </summary>
        /// <param name="lhs">Left-hand side to compare.</param>
        /// <param name="rhs">Right-hand side to compare.</param>
        /// <returns>True if the first list ends with the same elements as the second list, otherwise false.</returns>
        public static bool EndsWith<TValue>(this IEnumerable<TValue> lhs, IEnumerable<TValue> rhs)
        {
            Throw.IfArgumentNull(lhs, "lhs");
            Throw.IfArgumentNull(rhs, "rhs");

            int rhsCount = rhs.Count();
            int lhsCount = lhs.Count();

            if (lhsCount < rhsCount)
            {
                return false;
            }

            for (int i = rhsCount; i > 0; i--)
            {
                if (!object.Equals(lhs.ElementAt(lhsCount - i), rhs.ElementAt(rhsCount - i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Splits the current enumerable into multiple enumerable objects of the specified size.
        /// </summary>
        /// <param name="items">The items to split.</param>
        /// <param name="chunkSize">The size for each chunk.</param>
        public static IEnumerable<IEnumerable<TValue>> Split<TValue>(this IEnumerable<TValue> items, int chunkSize)
        {
            List<IEnumerable<TValue>> chunks = new List<IEnumerable<TValue>>();

            using (IEnumerator<TValue> iterator = items.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    List<TValue> chunk = new List<TValue>();

                    chunk.Add(iterator.Current);
                    for (int i = 1; i < chunkSize && iterator.MoveNext(); i++)
                    {
                        chunk.Add(iterator.Current);
                    }

                    chunks.Add(chunk);
                }
            }

            return chunks;
        }
    }
}