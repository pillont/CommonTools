using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters
{
    // SOURCE https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1
    public class UnexpectedErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var factory = context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = factory.CreateLogger<UnexpectedErrorFilter>();

            logger.LogError(context.Exception, $"Unexpected exception applied : {context.Exception.GetType().Name} {context.Exception.Message}");
        }
    }
}