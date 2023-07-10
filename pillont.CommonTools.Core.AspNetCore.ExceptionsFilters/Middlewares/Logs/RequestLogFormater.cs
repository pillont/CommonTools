using System;
using System.Text.Json;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public class RequestLogFormater
    {
        public string FormatExecutedMessage(LogRequestResponse data)
        {
            return string.Join(Environment.NewLine,
                               $"URI: {data.Method} {data.Uri}",
                               $"STATUS CODE: {data.StatusCode}",
                               $"DURATION: {data.Duration}",
                               $"HEADERS: {JsonSerializer.Serialize(data.Headers)}");
        }

        public string FormatExecutingMessage(LogRequestSending data)
        {
            return string.Join(Environment.NewLine,
                               $"URI: {data.Method} {data.Uri}",
                               $"HEADERS: {JsonSerializer.Serialize(data.Headers)}");
        }
    }
}