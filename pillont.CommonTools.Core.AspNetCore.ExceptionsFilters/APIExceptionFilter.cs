using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using pillont.CommonTools.Core.AspNetCore.Core.Exceptions;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters;

public class APIExceptionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Nothing to do
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not APIException apiException)
        {
            return;
        }

        context.Result = new ObjectResult(apiException.ErrorBody) 
        { 
            StatusCode = apiException.StatusCode 
        };
        context.ExceptionHandled = true;
    }
}
