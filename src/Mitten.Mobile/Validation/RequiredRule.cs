namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// Rule that validates data is required.
    /// </summary>
    public static class RequiredRule
    {
        /// <summary>
        /// Validates that the specified string is not empty or null.
        /// </summary>
        /// <param name="name">A user-friendly name identifying the value that is missing.</param>
        /// <param name="value">The value to validate.</param>
        public static ValidationResult Validate(string name, string value)
        {
            return 
                string.IsNullOrWhiteSpace(value) 
                ? RequiredRule.GetFailedResult(name) 
                : ValidationResult.Success;

        }

        /// <summary>
        /// Validates that the specified object is not null.
        /// </summary>
        /// <param name="name">A user-friendly name identifying the value that is missing.</param>
        /// <param name="value">The value to validate.</param>
        public static ValidationResult Validate(string name, object value)
        {
            return 
                value == null 
                ? RequiredRule.GetFailedResult(name) 
                : ValidationResult.Success;

        }

        private static ValidationResult GetFailedResult(string name)
        {
            return ValidationResult.Failed(name + " is required."); 
        }
    }
}