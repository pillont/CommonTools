using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    // SOURCE :https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
    public class RequestLogsMiddleware
    {
        private const string RECEPTION_PREFIX = "RECEPTION REQUEST";
    private const string ERROR_PREFIX = "FAILED REQUEST";
        private const string RESULT_PREFIX = "RESULT REQUEST";
        private readonly ILogger _logger;
        private readonly RequestLogFormater _formater;
        private readonly RequestDelegate _next;
        

        public RequestLogsMiddleware(
            RequestLogFormater formater,
            RequestDelegate next, 
            ILogger<RequestLogsMiddleware> logger)
        {
            _formater = formater;
            _next = next;
            _logger = logger;
        }

        // SOURCE : https://github.com/dotnet/aspnetcore/issues/7795#issuecomment-466150359
        // This helper method comes in 3.0. For now you'll need a copy in your app
        public static Endpoint GetEndpoint(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Features.Get<IEndpointFeature>()?.Endpoint;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // CASE : requete qui ne correspondent pas à des controllers
            // EXAMPLE : affichage de swagger / get qu'une resource (icon, ...)
            if (!IsValidRequest(context))
            {
                await _next.Invoke(context);
                return;
            }

            var executingData = CreateExecutingData(context);
            
            // CASE : Log executing 
            _logger.LogInformation($"{RECEPTION_PREFIX} " +
                $"IDENTIFIER: {context.TraceIdentifier}{Environment.NewLine}" +
                $"{_formater.FormatExecutingMessage(executingData)}");

            var resultData = new LogRequestResponse()
            {
                Method = context.Request.Method,
                Uri = $"{context.Request.Path} {context.Request.QueryString}",
            };

            var sw = new Stopwatch();
            sw.Start();

            try
            {
                // CASE : execution de la requete
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ERROR_PREFIX} IDENTIFIER: {context.TraceIdentifier}{Environment.NewLine}");
            }

            sw.Stop();
            resultData.Duration = sw.Elapsed;

            resultData.Headers = context.Response.Headers;
            resultData.StatusCode = context.Response.StatusCode;

            // NOTE : le endpoint est disponible seulement apres l'execution de la requete
            var endpoint = GetEndpoint(context);

            // CASE  : on ne log pas si l'attribut SkipLog est trouvé
            //         et que la fonctionalité n'est pas dans la config
            if (endpoint != null)
            {
                var skipLogAttribute = endpoint.Metadata.GetMetadata<SkipLogAttribute>();
                if (skipLogAttribute != null)
                        return;
            }

            // CASE : Log executed
            LogExecutedRequest(resultData, context.TraceIdentifier);
        }

        protected virtual bool IsValidRequest(HttpContext context)
        {
            // NOTE : on ne log pas les requetes qui affiche le swagger
            return !context.Request.Path.IsSwaggerDisplayRequest();
        }

        private LogRequestSending CreateExecutingData(HttpContext context)
        {
            var req = context.Request;
            req.EnableBuffering();


            return new LogRequestSending()
            {
                Headers = context.Request.Headers,
                Method = context.Request.Method,
                Uri = $"{context.Request.Path} {context.Request.QueryString}",
            };
        }

        private void LogExecutedRequest(LogRequestResponse data, string traceIdentifier)
        {
            var message = $"{RESULT_PREFIX} " +
                $"IDENTIFIER: {traceIdentifier} {Environment.NewLine}" +
                $"{_formater.FormatExecutedMessage(data)}";

            var statusCategory = data.StatusCode / 100;
            switch (statusCategory)
            {
                // StatusCode : 2XX : tout est ok
                case 2:
                    _logger.LogInformation(message);
                    break;
                // StatusCode : 4XX : erreur client
                case 4:
                    _logger.LogWarning(message);
                    break;
                // StatusCode : 5XX : erreur serveur
                case 5:
                    _logger.LogError(message);
                    break;
                // Autre
                default:
                    goto case 4;
            }
        }
    }
}