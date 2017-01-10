using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mitten.Mobile.Remote.Http.Json
{
    /// <summary>
    /// A custom string enum converter that will return Unknown when deserializing json if the enum Type defines an Unknown value and 
    /// the value being deserialized is not defined for the enum Type.
    /// </summary>
    internal class CustomEnumConverter : StringEnumConverter
    {
        private static class Constants
        {
            public const string UnknownValue = "Unknown";
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type enumType = this.GetEnumType(objectType);
            object unknownValue = this.TryGetUnknownValue(enumType);

            if (unknownValue == null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            string enumStringValue = reader.Value.ToString();

            if (string.IsNullOrWhiteSpace(enumStringValue) ||
                !this.IsDefined(enumType, enumStringValue))
            {
                return unknownValue;
            }

            return Enum.Parse(enumType, enumStringValue);
        }

        private Type GetEnumType(Type objectType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(objectType);
            return underlyingType ?? objectType;
        }

        private object TryGetUnknownValue(Type enumType)
        {
            if (this.IsDefined(enumType, Constants.UnknownValue))
            {
                return Enum.Parse(enumType, Constants.UnknownValue);
            }

            return null;
        }

        private bool IsDefined(Type enumType, string valueName)
        {
            return Enum.GetNames(enumType).Any(name => name == valueName);
        }
    }
}