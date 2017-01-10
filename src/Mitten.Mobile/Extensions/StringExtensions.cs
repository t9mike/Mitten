using System.Collections.Generic;
using System.IO;

namespace Mitten.Mobile.Extensions
{
    /// <summary>
    /// Contains extension methods for a string.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Reads each line for the current string and returns as an enumerable. 
        /// </summary>
        /// <param name="value">A string value.</param>
        /// <returns>The lines.</returns>
        public static IEnumerable<string> ReadLines(this string value)
        {
            string line;
            using (StringReader sr = new StringReader(value))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
