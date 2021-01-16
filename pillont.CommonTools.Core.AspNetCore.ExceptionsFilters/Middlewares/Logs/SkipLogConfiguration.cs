namespace pillont.CommonTools.Core.AspNetCore.ExceptionsFilters.Middlewares.Logs
{
    public class SkipLogConfiguration
    {
        public string[] Except { get; set; }
        public bool ForceAllLogs { get; set; }
    }
}
