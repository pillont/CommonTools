using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using pillont.CommonTools.Core.AspNetCore.Core.Exceptions;

namespace pillont.CommonTools.RestFullApi.HttpFilters
{
    /// <summary>
    /// SOURCE : https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.0
    /// </summary>
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // HERE : action to apply before request
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            TryFormatCustomException(context);

            // HERE : action to apply after request
        }

        private void TryFormatCustomException(ActionExecutedContext context)
        {
            var apiException = context.Exception as APIException;
            if (apiException == null)
            {
                return;
            }

            context.Result = new ObjectResult(apiException.ErrorBody) { StatusCode = apiException.StatusCode };
            context.ExceptionHandled = true;
        }
    }
}