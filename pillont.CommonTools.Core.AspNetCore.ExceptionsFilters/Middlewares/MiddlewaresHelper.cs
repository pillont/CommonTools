using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares
{
    public static class MiddlewaresHelper
    {
        public static bool IsSwaggerDisplayRequest(this PathString pathStr)
        {
            var path = pathStr.ToString();
            return path == "/"
                || path.EndsWith(".html")
                || path.EndsWith(".png")
                || path.EndsWith(".css")
                || path.EndsWith(".ico")
                || path.EndsWith(".json")
                || path.Contains("swagger");
        }

        public static async Task<string> ReadBodyDuringExecutionAsync(this HttpContext context, Func<Task> requestExecution)
        {
            using (var responseBody = new MemoryStream())
            {
                // NOTE : le body est en ecriture seule
                //        on fait donc un stream tampon pour lire le contenu et remettre le body en etat après lecture
                var originalBodyStream = context.Response.Body;
                context.Response.Body = responseBody;

                try
                {
                    await requestExecution();

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //NOTE : pas de Dispose sur le reader pour ne pas Dispose le stream
                    var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    return text;
                }
                finally
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }
}