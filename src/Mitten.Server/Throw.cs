using System;
using System.ComponentModel;

namespace Mitten.Server
{
    /// <summary>
    /// Utility class for simplifying basic invariant checks and exception throwing.
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Checks that the specified value is not null and throws an exception if the check fails.
        /// This is intended for checking arguments passed into a method call.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void IfArgumentNull(object value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Checks that the specified value is not null and throws an exception if the check fails.
        /// This is intended for checking arguments passed into a method call.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void IfArgumentNullOrWhitespace(string value, string argumentName)
        {
            Throw.IfArgumentNull(value, argumentName);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The value must not be empty or contain only whitespace.", argumentName);
            }
        }

        /// <summary>
        /// Checks if the specified value is not empty and throws an exception if the check fails.
        /// This is intended for checking arguments passed into a method call.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="argumentName">The argument name.</param>
        public static void IfArgumentEmpty(Guid value, string argumentName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("The Guid must not be empty.", argumentName);
            }
        }

        /// <summary>
        /// Checks if the specified Enum value is invalid and throws an exception if the check fails.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum being checked.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="invalidValue">The invalid value to check against.</param>
        public static void IfEnumArgumentInvalid<TEnum>(TEnum value, string argumentName, TEnum invalidValue)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("Generic type argument (" + typeof(TEnum).Name + ") is not an enum.");
            }

            int valueAsInt = Convert.ToInt32(value);
            int invalidValueAsInt = Convert.ToInt32(invalidValue);

            if (valueAsInt == invalidValueAsInt)
            {
                throw new InvalidEnumArgumentException(argumentName, invalidValueAsInt, typeof(TEnum));
            }
        }
    }
}