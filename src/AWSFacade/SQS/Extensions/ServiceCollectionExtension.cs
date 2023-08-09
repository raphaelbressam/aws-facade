using AWSFacade.SQS.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AWSFacade.SQS.Extensions
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddSqsFacade(this IServiceCollection services, Action<SqsOptions> setupAction)
        {
            return services.AddSqsFacade(string.Empty, setupAction);
        }

        public static IServiceCollection AddSqsFacade(this IServiceCollection services, string name, Action<SqsOptions> setupAction)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (setupAction is null) throw new ArgumentNullException(nameof(setupAction));
            if (name is null) throw new ArgumentNullException(nameof(name));

            services.AddOptions();

            var options = new SqsOptions();
            setupAction(options);

            services.AddSingleton<ISqsFacade>((s) => new SqsFacade(name, options));
            services.AddSingleton<ISqsFacadeFactory, SqsFacadeFactory>();

            return services;
        }

        private static SqsOptions GetOptions(SqsOptions options) => options;
    }
}
