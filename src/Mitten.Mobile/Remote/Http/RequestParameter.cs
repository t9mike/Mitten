using System.Collections.Generic;

namespace Mitten.Mobile.Remote.Http
{
    /// <summary>
    /// Represents a parameter when making a request to the server.
    /// </summary>
    public class RequestParameter
    {
        private RequestParameter(string parameterName, string value)
        {
            this.ParameterName = parameterName;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Converts this parameter into a key value pair where the parameter name is used as the key.
        /// </summary>
        /// <returns>A new key value pair.</returns>
        internal KeyValuePair<string, string> AsKeyValuePair()
        {
            return new KeyValuePair<string, string>(this.ParameterName, this.Value);
        }

        /// <summary>
        /// Creates a new request parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value.</param>
        public static RequestParameter Create(string parameterName, string value)
        {
            return new RequestParameter(parameterName, value);
        }

        /// <summary>
        /// Creates a new request parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value.</param>
        public static RequestParameter Create(string parameterName, object value)
        {
            return new RequestParameter(parameterName, RequestParameter.GetString(value));
        }

        private static string GetString(object value)
        {
            return 
                value != null 
                ? value.ToString() 
                : null;
        }
    }
}

