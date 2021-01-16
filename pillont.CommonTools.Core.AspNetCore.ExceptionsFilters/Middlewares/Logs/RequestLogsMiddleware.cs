using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    // SOURCE :https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
    public class RequestLogsMiddleware
    {
        private const string RECEPTION_PREFIX = "RECEPTION REQUEST";
        private const string RESULT_PREFIX = "RESULT REQUEST";
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly SkipLogConfiguration _skipLogConfig;

        public RequestLogsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<SkipLogConfiguration> skipLogConfig)
        {
            _next = next;
            _skipLogConfig = skipLogConfig?.Value ?? throw new ArgumentNullException(nameof(skipLogConfig));
            _logger = loggerFactory.CreateLogger<RequestLogsMiddleware>();
        }

        // SOURCE : https://github.com/dotnet/aspnetcore/issues/7795#issuecomment-466150359
        // This helper method comes in 3.0. For now you'll need a copy in your app
        public static Endpoint GetEndpoint(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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

            var executingData = await CreateExecutingDataAsync(context);

            var resultData = new LogRequestResponse()
            {
                Method = context.Request.Method,
                Uri = $"{context.Request.Path} {context.Request.QueryString.ToString()}",
            };

            resultData.Body = await context.ReadBodyDuringExecutionAsync(async () =>
            {
                var sw = new Stopwatch();
                sw.Start();

                // CASE : execution de la requete
                await _next.Invoke(context);

                sw.Stop();
                resultData.Duration = sw.Elapsed;
            });

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
                {
                    if (!NeedLogFor(skipLogAttribute.FunctionName))
                    {
                        return;
                    }
                }
            }

            // CASE : Log executing et executed
            _logger.LogInformation($"{RECEPTION_PREFIX} IDENTIFIER: {context.TraceIdentifier}{Environment.NewLine}{RequestLogFormater.FormatExecutingMessage(executingData)}");
            LogExecutedRequest(resultData, context.TraceIdentifier);
        }

        protected virtual bool IsValidRequest(HttpContext context)
        {
            // NOTE : on ne log pas les requetes qui affiche le swagger
            return !context.Request.Path.IsSwaggerDisplayRequest();
        }

        private async Task<LogRequestSending> CreateExecutingDataAsync(HttpContext context)
        {
            string bodyStr;
            var req = context.Request;
            req.EnableBuffering();

            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync();
                req.Body.Position = 0;
            }

            return new LogRequestSending()
            {
                Body = bodyStr,
                Headers = context.Request.Headers,
                Method = context.Request.Method,
                Uri = $"{context.Request.Path} {context.Request.QueryString.ToString()}",
            };
        }

        private void LogExecutedRequest(LogRequestResponse data, string traceIdentifier)
        {
            var message = $"{RESULT_PREFIX} IDENTIFIER: {traceIdentifier} {Environment.NewLine}{RequestLogFormater.FormatExecutedMessage(data)}";

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

        private bool NeedLogFor(string currentFunctionName)
        {
            return _skipLogConfig.ForceAllLogs
                   || (!string.IsNullOrWhiteSpace(currentFunctionName)
                      && _skipLogConfig.Except.Contains(currentFunctionName));
        }
    }
}