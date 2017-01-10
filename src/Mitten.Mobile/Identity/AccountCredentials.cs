using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mitten.Mobile.Identity
{
    /// <summary>
    /// Contains credentials for authentication against an account service.
    /// </summary>
    public class AccountCredentials
    {
        private static class Constants
        {
            public const char ValueSeperator = '&';
            public const char ValueIdentifier = '=';
        }

        private readonly IDictionary<string, string> credentials;

        /// <summary>
        /// Initializes a new instance of the AccountCredentials class.
        /// </summary>
        public AccountCredentials()
            : this (new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
        }

        private AccountCredentials(IDictionary<string, string> credentials)
        {
            this.credentials = credentials;
        }

        /// <summary>
        /// Gets the value of a credential by the specified name. 
        /// </summary>
        public string this[string name]
        {
            get { return this.credentials[name]; }
            set { this.credentials[name] = value; }
        }

        /// <summary>
        /// Serializes the account credentials into a string.
        /// </summary>
        public string Serialize()
        {
            StringBuilder serializedString = new StringBuilder();

            foreach (KeyValuePair<string, string> item in this.credentials)
            {
                this.SerializeValue(serializedString, item.Key, item.Value);
            }

            return serializedString.ToString().Trim(Constants.ValueSeperator);
        }

        /// <summary>
        /// Attempts to deserializes a set of account credentials and returns null if the serialized credentials are invalid.
        /// </summary>
        /// <param name="serializedCredentials">Serialized credentials.</param>
        public static AccountCredentials TryDeserialize(string serializedCredentials)
        {
            if (!string.IsNullOrWhiteSpace(serializedCredentials))
            {
                string[] values = serializedCredentials.Split(Constants.ValueSeperator);
                IEnumerable<Tuple<string, string>> parsedValues = values.Select(value => AccountCredentials.TryParseSerializedValue(value));
                Dictionary<string, string> credentials = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (Tuple<string, string> item in parsedValues)
                {
                    if (item == null)
                    {
                        return null;
                    }

                    credentials.Add(item.Item1, item.Item2);
                }

                return new AccountCredentials(credentials);
            }

            return null;
        }

        private void SerializeValue<TValue>(StringBuilder serializedString, string valueName, TValue value)
        {
            serializedString.Append(
                valueName + 
                Constants.ValueIdentifier + 
                Uri.EscapeDataString(value.ToString()) + 
                Constants.ValueSeperator);
        }

        private static Tuple<string, string> TryParseSerializedValue(string value)
        {
            string[] items = value.Split(new [] { Constants.ValueIdentifier }, StringSplitOptions.RemoveEmptyEntries);

            return 
                items.Length == 2 
                ? new Tuple<string, string>(items[0], Uri.UnescapeDataString(items[1]))
                : null;
        }
    }
}