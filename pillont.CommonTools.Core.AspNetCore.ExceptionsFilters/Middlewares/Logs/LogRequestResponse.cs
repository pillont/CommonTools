using System;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public class LogRequestResponse : LogRequestSending
    {
        public TimeSpan Duration { get; set; }
        public int StatusCode { get; set; }
    }
}