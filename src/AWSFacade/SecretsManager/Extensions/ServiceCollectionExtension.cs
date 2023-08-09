using AWSFacade.SecretsManager.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AWSFacade.SecretsManager.Extensions
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddSecretsManagerCache(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ISecretsCache, SecretsManager>();

            return services;
        }
    }
}
