namespace Mitten.Server.Json
{
    /// <summary>
    /// Defines an item that supports serializing into Json.
    /// </summary>
    public interface IJsonSerializable
    {
        /// <summary>
        /// Converts the current object into a json string.
        /// </summary>
        /// <returns>A json string.</returns>
        string ToJson();
    }
}