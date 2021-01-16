using System;
using Microsoft.AspNetCore.Mvc;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters
{
    public class NotFoundExceptionFilter<TException> : HttpResponseExceptionFilter<TException, NotFoundObjectResult>
        where TException : Exception
    {
    }
}
