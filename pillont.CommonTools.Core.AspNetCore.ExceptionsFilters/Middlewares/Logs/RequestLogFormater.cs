using System;
using System.Text.Json;
using pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public static class RequestLogFormater
    {
        public static string FormatExecutedMessage(LogRequestResponse data)
        {
            return string.Join(Environment.NewLine,
                               $"URI: {data.Method} {data.Uri}",
                               $"STATUS CODE: {data.StatusCode}",
                               $"DURATION: {data.Duration}",
                               $"HEADERS: {JsonSerializer.Serialize(data.Headers)}",
                               $"RESPONSE: {data.Body}");
        }

        public static string FormatExecutingMessage(LogRequestSending data)
        {
            return string.Join(Environment.NewLine,
                               $"URI: {data.Method} {data.Uri}",
                               $"HEADERS: {JsonSerializer.Serialize(data.Headers)}",
                               $"BODY: {data.Body}");
        }
    }
}