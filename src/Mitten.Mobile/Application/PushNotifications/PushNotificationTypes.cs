using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mitten.Mobile.Application.PushNotifications
{
    /// <summary>
    /// Represents a collection of known PushNoficiation types.
    /// </summary>
    public class PushNotificationTypes
    {
        private readonly IDictionary<string, Type> knownTypes;

        /// <summary>
        /// Initializes a new instance of the PushNotificationTypes class.
        /// </summary>
        /// <param name="knownTypes">A dictionary of known push notification types keyed by the name of the notification.</param>
        public PushNotificationTypes(IDictionary<string, Type> knownTypes)
        {
            Throw.IfArgumentNull(knownTypes, nameof(knownTypes));
            this.knownTypes = knownTypes;
        }

        /// <summary>
        /// Gets the notification Type based on a given name or null if no known notification Types were found.
        /// </summary>
        /// <param name="notificationName">The name of the notification.</param>
        /// <returns>A Type or null.</returns>
        public Type TryGetType(string notificationName)
        {
            Throw.IfArgumentNullOrWhitespace(notificationName, nameof(notificationName));

            Type type = null;
            this.knownTypes.TryGetValue(notificationName, out type);
            return type;
        }

        /// <summary>
        /// Scans the specified assembly for all classes with a PushNotificationAttribute.
        /// </summary>
        /// <param name="assembly">An assembly to scan.</param>
        /// <returns>A PushNotificationTypes instance.</returns>
        public static PushNotificationTypes FromAssembly(Assembly assembly)
        {
            return new PushNotificationTypes(PushNotificationTypes.LoadNotificationTypes(assembly));
        }

        private static IDictionary<string, Type> LoadNotificationTypes(Assembly assembly)
        {
            IEnumerable<Tuple<PushNotificationAttribute, Type>> attributes =
                assembly.DefinedTypes
                    .Select(typeInfo =>
                    {
                        PushNotificationAttribute attribute = (PushNotificationAttribute)typeInfo.GetCustomAttribute(typeof(PushNotificationAttribute));

                        return
                            attribute != null
                            ? new Tuple<PushNotificationAttribute, Type>(attribute, typeInfo.AsType())
                            : null;
                    })
                    .Where(item => item != null);


            IDictionary<string, Type> types = new Dictionary<string, Type>();
            TypeInfo pushNotificationType = typeof(PushNotification).GetTypeInfo();

            foreach (Tuple<PushNotificationAttribute, Type> item in attributes)
            {
                if (types.ContainsKey(item.Item1.NotificationName))
                {
                    throw new InvalidOperationException("A class already defines a push notification attribute with name (" + item.Item1.NotificationName + ").");
                }

                if (!pushNotificationType.IsAssignableFrom(item.Item2.GetTypeInfo()))
                {
                    throw new InvalidOperationException("Type (" + item.Item2.Name + ") must inhert from (" + typeof(PushNotification).Name + ").");
                }

                types.Add(item.Item1.NotificationName, item.Item2);
            }

            return types;
        }
    }
}
