namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// Validates that an email is in a valid format.
    /// </summary>
    public static class ValidEmailRule
    {
        private static class Constants
        {
            public const string AtSign = "@";
            public const string Period = ".";
        }

        /// <summary>
        /// Validates that the specified email is in a valid format if not null or empty.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        public static ValidationResult Validate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!value.Contains(Constants.AtSign) ||
                !value.Contains(Constants.Period))
                {
                    return ValidationResult.Failed("Invalid email.");
                }
            }

            return ValidationResult.Success;
        }
    }
}