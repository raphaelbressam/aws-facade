using Amazon.SecretsManager;
using AWSFacade.SecretsManager.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AWSFacade.SecretsManager.Extensions
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddSecretsManagerCache(this IServiceCollection services, Func<IServiceProvider, AmazonSecretsManagerClient> setup)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddMemoryCache();
            services.AddSingleton<ISecretsCache, SecretsManager>();
            services.AddSingleton<IAmazonSecretsManager>(setup.Invoke(services.BuildServiceProvider()));

            return services;
        }
    }
}
