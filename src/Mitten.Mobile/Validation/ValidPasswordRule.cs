namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// Validates that a password meets minimum requirements.
    /// </summary>
    public static class ValidPasswordRule
    {
        private static class Constants
        {
            public const int MinimumPasswordLength = 5;
        }

        /// <summary>
        /// Validates that the specified password meets minimum requirements.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        public static ValidationResult Validate(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length < Constants.MinimumPasswordLength)
                {
                    return ValidationResult.Failed("Password cannot be less than " + Constants.MinimumPasswordLength + " characters.");
                }
            }

            return ValidationResult.Success;
        }
    }
}