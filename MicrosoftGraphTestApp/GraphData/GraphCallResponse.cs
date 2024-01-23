using System.Text.Json.Serialization;

namespace MicrosoftGraphTestApp.GraphData
{
    public class GraphCallResponse<T>
    {
        [JsonPropertyName("value")]
        public T? Value { get; init; }
    }
}