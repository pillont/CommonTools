using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HelloWork.WebApiCustomers.Injector;
using Microsoft.Extensions.DependencyInjection;

namespace pillont.MSDependencyInjectionTools
{
    public static class ServiceInjector
    {
        /// <summary>
        /// collect all modules in assembly and populate services collection with there
        /// </summary>
        /// <param name="targetAssembly">assembly where look for module</param>
        /// <param name="services">to populate</param>
        public static void InjectModules(Assembly targetAssembly, IServiceCollection services)
        {
            var allModules = targetAssembly.GetTypes()                                                  // all types
                                            .Where(t => t.IsClass                                       // where is class
                                                    && typeof(IInjectorModule).IsAssignableFrom(t))     // and injection module
                                            .Select(t => Activator.CreateInstance(t))                   // to create instance
                                            .Cast<IInjectorModule>()                                    // of generic type
                                            .ToList();

            foreach (var module in allModules)
            {
                module.Register(services);
            }
        }
    }
}