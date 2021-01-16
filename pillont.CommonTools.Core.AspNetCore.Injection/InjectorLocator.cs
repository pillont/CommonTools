using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pillont.CommonTools.Core.Enumerables;

namespace pillont.CommonTools.Core.AspNetCore.Injection
{
    /// <example>
    /// in startup.cs file :
    ///
    /// <code>
    ///   using pillont.CommonTools.Core.AspNetCore.Injection;
    ///
    ///   public void ConfigureServices(IServiceCollection services)
    ///   {
    ///       [...]
    ///       services.InjectModules(Configuration, typeof(MyModule).Assembly);
    ///   }
    /// </code>
    /// </example>
    public static class InjectorLocator
    {
        /// <summary>
        /// genère une configuration
        ///   -> peut charger un appSetting.json si voulu
        ///
        /// genère un ServiceCollection
        ///   -> instancie un startup avec la config pour appliquer la fonction ConfigureServices
        ///
        ///  Check toutes les injections
        /// </summary>
        /// <typeparam name="StartupT">
        /// type du startup pour définir les injections
        /// /!\ le startup doit avoir un ctor avec uniquement la config /!\
        /// </typeparam>
        /// <param name="configAction">
        /// action pour customiser la config
        /// </param>
        /// <param name="serviceAction">
        /// action pour customiser les services
        /// </param>
        /// <param name="useDefaultAppSettingJson">
        /// si true => charge l'appSetting au root du test dans le config builder
        /// </param>
        public static void AssertInjectionIsValid<StartupT>(Action<ConfigurationBuilder> configAction = null,
                                                          Action<ServiceCollection> serviceAction = null,
                                                          Action<ServiceProvider> providerAction = null,
                                                          bool useDefaultAppSettingJson = true)
        {
            var config = GenerateConfig(configAction, useDefaultAppSettingJson);
            var collection = GenerateServicesCollection<StartupT>(config);
            serviceAction?.Invoke(collection);

            ServiceProvider provider = collection.BuildServiceProvider();
            providerAction?.Invoke(provider);

            var assembly = typeof(StartupT).Assembly;
            CheckControllersInjections(provider, assembly);
            CheckFactoryPatterns(collection, provider);
        }

        /// <example>
        /// in test
        ///
        /// <code>
        /// var collection = new ServiceCollection();
        /// collection.InjectModules(conf);
        /// collection.AssertInjectionIsValid(typeof(Startup).Assembly);
        ///</code>
        /// </example>
        public static void AssertInjectionIsValid(this IServiceCollection services, Assembly startupAssembly)
        {
            ServiceProvider provider = services.BuildServiceProvider();

            var a = startupAssembly;
            CheckControllersInjections(provider, a);
            CheckFactoryPatterns(services, provider);
        }

        public static ServiceCollection GenerateServicesCollection<StartupT>(IConfiguration config)
        {
            var collection = new ServiceCollection();
            StartupT startup = default;
            // appelle la fonction ConfigureServices du startup
            try
            {
                startup = (StartupT)Activator.CreateInstance(typeof(StartupT), config);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("StartUp must contains ctor with only IConfiguration in parameter", e);
            }

            try
            {
                var fnc = typeof(StartupT).GetMethod("ConfigureServices");
                fnc.Invoke(startup, collection.AsArray());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("StartUp must contains function ConfigureServices with only IServiceCollection in parameter", e);
            }

            return collection;
        }

        public static void InjectModules(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var allModules = assembly                                       // search in injection assembly
                    .GetTypes()                                             // all types
                    .Where(t => t.IsClass                                   // where is class
                            && typeof(IInjectorModule).IsAssignableFrom(t)) // and injection module
                    .Select(t => Activator.CreateInstance(t))               // to create instance
                    .Cast<IInjectorModule>()                                // of generic type
                    .ToList();

            foreach (var module in allModules)
            {
                module.Register(services, configuration);
            }
        }

        private static void CheckControllersInjections(ServiceProvider provider, Assembly a)
        {
            var allControllerTypes = a.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t)
                         && !t.IsAbstract)
                .ToList();

            // On vérifie que la collecte à fonctionnée
            if (!allControllerTypes.Any())
            {
                throw new Exception("pas de services trouvés par la collecte");
            }

            // on instantie tous les controllers
            foreach (var t in allControllerTypes)
            {
                var result = ActivatorUtilities.CreateInstance(provider, t);
                if (result is null)
                {
                    throw new Exception($"{t} is null after creation");
                }

                CheckFunctionsArguments(provider, t);
            }
        }

        /// <summary>
        /// parcours tous les services de type Func<>
        /// </summary>
        private static void CheckFactoryPatterns(IServiceCollection services, ServiceProvider provider)
        {
            var allFuncDescriptions = services.Where(c => c.ServiceType.IsGenericType
                                                         && c.ServiceType.GetGenericTypeDefinition() == typeof(Func<>));
            foreach (var desc in allFuncDescriptions)
            {
                var funcImplem = provider.GetService(desc.ServiceType) as Func<object>;
                var funcResult = funcImplem();

                if (funcResult is null)
                {
                    throw new Exception($"provider could not implement {desc.ServiceType}");
                }
            }
        }

        private static void CheckFunctionsArguments(ServiceProvider provider, Type t)
        {
            // NOTE : on teste aussi les injection dans les fonctions
            //        qui sont faites grace au 'FromServicesAttribute'
            var fromServiceParameterTypes = t.GetMethods()
                                            .SelectMany(method => method.GetParameters())
                                            .Where(p => p.GetCustomAttribute<FromServicesAttribute>() != null)
                                            .Select(param => param.ParameterType)
                                            .Distinct();

            foreach (var paramType in fromServiceParameterTypes)
            {
                var service = provider.GetService(paramType);

                if (service is null)
                {
                    throw new Exception($"provider could not implement {paramType}");
                }
            }
        }

        private static IConfigurationRoot GenerateConfig(Action<ConfigurationBuilder> configAction, bool useDefaultAppSettingJson)
        {
            var builder = new ConfigurationBuilder();
            if (useDefaultAppSettingJson)
            {
                builder.AddJsonFile("appsettings.json");
            }

            configAction?.Invoke(builder);
            var config = builder.Build();
            return config;
        }
    }
}