using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters
{
    /// <summary>
    /// Filtre pour transformer les <see cref="TException"/> en reponse HTTP appropriée : <see cref="TResult"/>
    ///
    /// SOURCE : https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.0
    /// </summary>
    public class HttpResponseExceptionFilter<TException, TResult> : IActionFilter
        where TException : Exception
        where TResult : ObjectResult
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            TryFormatCustomException(context);

            // HERE : action to apply after request
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // HERE : action to apply before request
        }

        private object GetContent(TException exception)
        {
            if (exception is IContentHolder contentHolder)
            {
                return contentHolder.Content;
            }

            return exception.Message;
        }

        private void TryFormatCustomException(ActionExecutedContext context)
        {
            if (context.Exception is TException notExistsInStoreException)
            {
                object content = GetContent(notExistsInStoreException);
                context.Result = (TResult)Activator.CreateInstance(typeof(TResult), content);
                context.ExceptionHandled = true;
                return;
            }
        }
    }
}
