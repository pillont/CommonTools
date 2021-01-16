using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace pillont.CommonTools.Core.AspNetCore.Injection
{
    public interface IInjectorModule
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}