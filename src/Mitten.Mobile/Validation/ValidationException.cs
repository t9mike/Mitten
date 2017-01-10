using System;
using System.Collections.Generic;
using System.Text;

namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// Represents an error that occurred because state failed to pass validation. 
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ValidationException class.
        /// </summary>
        /// <param name="errors">One or more errors that occurred.</param>
        public ValidationException(IEnumerable<string> errors)
            : base(ValidationException.GetMessage(errors))
        {
        }

        private static string GetMessage(IEnumerable<string> errors)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("The following validation error(s) occurred:");

            foreach (string error in errors)
            {
                sb.AppendLine(error);
            }

            return sb.ToString();
        }
    }
}

