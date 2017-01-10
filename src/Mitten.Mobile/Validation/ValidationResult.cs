using System;
using System.Collections.Generic;

namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// The result of a validation check.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Represents a successful validation check.
        /// </summary>
        public static readonly ValidationResult Success = new ValidationResult(false, new string[0]);

        private ValidationResult(bool hasErrors, IEnumerable<string> messages)
        {
            this.HasErrors = hasErrors;
            this.Messages = messages;
        }

        /// <summary>
        /// Gets whether or not there were any errors.
        /// </summary>
        public bool HasErrors { get; private set; }

        /// <summary>
        /// Gets a list of error messages in the event a validation rule failed.
        /// </summary>
        public IEnumerable<string> Messages { get; private set; }

        /// <summary>
        /// Gets a validation result for a failed validation check.
        /// </summary>
        /// <param name="messages">One or more error messages describing the reason for the failure.</param>
        public static ValidationResult Failed(params string[] messages)
        {
            return ValidationResult.Failed((IEnumerable<string>)messages);
        }

        /// <summary>
        /// Gets a validation result for a failed validation check.
        /// </summary>
        /// <param name="messages">One or more error messages describing the reason for the failure.</param>
        public static ValidationResult Failed(IEnumerable<string> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            return new ValidationResult(true, messages);
        }
    }
}