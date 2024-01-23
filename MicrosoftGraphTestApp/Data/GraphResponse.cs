using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp.Data
{
    public class GraphResponse<T> : GraphCallResponse<T>
    {
        public string Response { get; set; }
    }
}