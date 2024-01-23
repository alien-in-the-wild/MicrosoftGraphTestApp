using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.GraphData
{
    /// <summary>
    /// Original Response type from Graph API
    /// </summary>
    /// <typeparam name="T">Object type to cast the response to</typeparam>
    public class GraphCallResponse<T>
    {
        /// <summary>
        /// Deserialized response object
        /// </summary>
        [JsonPropertyName("value")]
        public T? Value { get; init; }
    }
}