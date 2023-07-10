namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public class LogRequestSending
    {
        public object Headers { get; set; }
        public string Method { get; set; }
        public string Uri { get; set; }
    }
}