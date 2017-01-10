using System;

namespace Mitten.Server.Extensions
{
    /// <summary>
    /// Contains extensions for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Attempts to parse an integer value from a string.
        /// </summary>
        /// <param name="valueToParse">The value to parse.</param>
        /// <returns>The integer value or null if the string could not be parsed.</returns>
        public static int? TryParseAsInt(this string valueToParse)
        {
            return StringExtensions.Parse<int>(valueToParse, int.TryParse);
        }

        /// <summary>
        /// Attempts to parse a bool value from a string.
        /// </summary>
        /// <param name="valueToParse">The value to parse.</param>
        /// <returns>The boolean value or null if the string could not be parsed.</returns>
        public static bool? TryParseAsBool(this string valueToParse)
        {
            return StringExtensions.Parse<bool>(valueToParse, bool.TryParse);
        }

        /// <summary>
        /// Attempts to parse a guid value from a string.
        /// </summary>
        /// <param name="valueToParse">The value to parse.</param>
        /// <returns>The guid value or null if the string could not be parsed.</returns>
        public static Guid? TryParseAsGuid(this string valueToParse)
        {
            return StringExtensions.Parse<Guid>(valueToParse, Guid.TryParse);
        }

        /// <summary>
        /// Attempts to parse an enum value from a string.
        /// </summary>
        /// <typeparam name="TEnum">The Type of emum to return.</typeparam>
        /// <param name="valueToParse">The value to parse.</param>
        /// <param name="ignoreCase">True if the case should be ignored, otherwise false; the default is false.</param>
        /// <returns>The enum value or null if the string could not be parsed.</returns>
        public static TEnum? TryParseEnum<TEnum>(this string valueToParse, bool ignoreCase = false)
             where TEnum : struct
        {
            TEnum result;
            if (Enum.TryParse(valueToParse, ignoreCase, out result))
            {
                return result;
            }

            return null;
        }

        private delegate bool TryParse<TValue>(string value, out TValue result) where TValue : struct;
        private static TValue? Parse<TValue>(string valueToParse, TryParse<TValue> tryParse)
            where TValue : struct
        {
            TValue result;
            if (tryParse(valueToParse, out result))
            {
                return result;
            }

            return null;
        }
    }
}
