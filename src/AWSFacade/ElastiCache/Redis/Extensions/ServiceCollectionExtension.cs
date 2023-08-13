using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AWSFacade.ElastiCache.Redis.Extensions
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddElasticacheRedis(this IServiceCollection services, Action<RedisGatewayOptions> setup)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddDistributedMemoryCache();
            services.AddOptions();

            services.Configure(setup);
            services.AddSingleton<IDistributedCache, RedisGateway>();

            return services;
        }
    }
}
