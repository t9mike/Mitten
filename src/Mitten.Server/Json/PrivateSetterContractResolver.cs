using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mitten.Server.Json
{
    /// <summary>
    /// A contract resolver that allows deserializing properties that have private setters.
    /// </summary>
    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Creates a JsonProperty for the given MemberInfo.
        /// </summary>
        /// <param name="member">The member to create a JsonProperty for.</param>
        /// <param name="memberSerialization">The member's parent MemberSerialization.</param>
        /// <returns>A created JsonProperty for the given MemberInfo.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Writable)
            {
                PropertyInfo property = member as PropertyInfo;
                if (property != null)
                {
                    bool hasPrivateSetter = property.GetSetMethod(true) != null;
                    jsonProperty.Writable = hasPrivateSetter;
                }
            }

            return jsonProperty;
        }
    }
}