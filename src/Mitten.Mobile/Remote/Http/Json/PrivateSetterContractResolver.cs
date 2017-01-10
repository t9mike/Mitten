using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mitten.Mobile.Remote.Http.Json
{
    /// <summary>
    /// A custom contract resolver that allows deserializing properties that have private setters.
    /// </summary>
    internal class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Writable)
            {
                PropertyInfo property = member as PropertyInfo;
                if (property != null)
                {
                    bool hasPrivateSetter = 
                        property.SetMethod != null &&
                        property.SetMethod.IsPrivate;
                    
                    jsonProperty.Writable = hasPrivateSetter;
                }
            }

            return jsonProperty;
        }
    }
}