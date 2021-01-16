using System;

namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public class SkipLogAttribute : Attribute
    {
        public string FunctionName { get; set; }
    }
}
