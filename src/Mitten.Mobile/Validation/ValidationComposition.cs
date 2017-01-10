using System.Collections.Generic;
using System.Linq;

namespace Mitten.Mobile.Validation
{
    /// <summary>
    /// Supports composing multiple rules into a single validation result.
    /// </summary>
    public static class ValidationComposition
    {
        /// <summary>
        /// Gets a single validation result composed of multiple results. A failed result will 
        /// be returned if at least one result in the list failed.
        /// </summary>
        /// <param name="results">One or more results to compose into a single result.</param>
        /// <returns>A new validation result.</returns>
        public static ValidationResult FromResults(params ValidationResult[] results)
        {
            IEnumerable<string> messages = 
                results
                    .Where(result => result.HasErrors)
                    .SelectMany(result => result.Messages);

            return 
                messages.Any() 
                ? ValidationResult.Failed(messages) 
                : ValidationResult.Success;
        }
    }
}