using Microsoft.Extensions.DependencyInjection;

namespace HelloWork.WebApiCustomers.Injector
{
    /// <summary>
    /// base injection module
    /// to apply injection define all modules you want in the API
    /// In the same assembly to implement register function
    /// </summary>
    public interface IInjectorModule
    {
        /// <summary>
        /// add all injection dependencies in the service collection
        /// </summary>
        void Register(IServiceCollection services);
    }
}