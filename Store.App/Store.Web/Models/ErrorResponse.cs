namespace Store.Web.Models
{
    public class ErrorResponse
    {
        public Dictionary<string, string[]> errors { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string traceId { get; set; }
    }
}
