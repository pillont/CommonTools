using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters;

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

    protected virtual object GetContent(TException exception)
    {
        return exception is IContentHolder contentHolder ? contentHolder.Content : exception.Message;
    }

    private void TryFormatCustomException(ActionExecutedContext context)
    {
        if (context.Exception is TException exception)
        {
            var content = GetContent(exception);
            context.Result = GetResultObject(content);
            context.ExceptionHandled = true;
            return;
        }
    }

    protected virtual TResult GetResultObject(object content)
    {
        return (TResult)Activator.CreateInstance(typeof(TResult), content);
    }
}
