using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp.Data
{
    /// <summary>
    /// Response type from Graph API
    /// </summary>
    /// <typeparam name="T">Object type to cast the response to</typeparam>
    public class GraphResponse<T> : GraphCallResponse<T>
    {
        /// <summary>
        /// JSON String representation of the response
        /// </summary>
        public string? Response { get; set; }
    }
}