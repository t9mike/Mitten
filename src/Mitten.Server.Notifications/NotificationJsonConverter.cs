using System;
using Mitten.Server.Notifications.Push;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mitten.Server.Notifications
{
    /// <summary>
    /// Handles converting notification objects from json.
    /// </summary>
    internal class NotificationJsonConverter : JsonConverter
    {
        private static class Constants
        {
            public const string NotificationTypePropertyName = "NotificationType";
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Notification);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jObject = JObject.Load(reader);

            JToken resolutionTypeToken = jObject[Constants.NotificationTypePropertyName];
            string value = resolutionTypeToken.Value<string>();
            NotificationType notificationType = (NotificationType)Enum.Parse(typeof(NotificationType), value);

            switch (notificationType)
            {
                case NotificationType.Push:
                    return jObject.ToObject<PushNotification>(serializer);

                default:
                    throw new InvalidOperationException("Unexpected notification type (" + notificationType + ").");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}