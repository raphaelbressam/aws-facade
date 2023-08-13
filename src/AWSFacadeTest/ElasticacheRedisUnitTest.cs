using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AWSFacade.ElastiCache.Redis.Extensions;
using AWSFacade.SecretsManager.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.Redis;

namespace AWSFacadeTest
{
    public class ElasticacheRedisUnitTest : IAsyncLifetime
    {
        const string SECRET_VALUE = "testsecret";
        private RedisContainer _redisContainer = new RedisBuilder().WithCommand($"--requirepass {SECRET_VALUE}").Build();

        public Task DisposeAsync()
        {
            return _redisContainer.DisposeAsync().AsTask();
        }

        public Task InitializeAsync()
        {
            return _redisContainer.StartAsync();
        }

        [Fact(DisplayName = "Given the key has value. When executing GetStringAsync, be prompted. The key value was found.")]
        public async Task GetKeySuccessCaseWithValue()
        {
            var secretsManagerClientMock = new Mock<AmazonSecretsManagerClient>(new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.SAEast1 });
            secretsManagerClientMock.Setup(s => s.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), CancellationToken.None)).ReturnsAsync(new GetSecretValueResponse() { SecretString = SECRET_VALUE });

            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddSecretsManagerCache((sp) => secretsManagerClientMock.Object);
            services.AddElasticacheRedis(config =>
            {
                config.AbortConnect = false;
                config.Endpoing = _redisContainer.GetConnectionString();
                config.InstanceName = "test";
                config.SecretKey = "key";
                config.Ssl = false;
            });

            var serviceProvider = services.BuildServiceProvider();


            var cache = serviceProvider.GetRequiredService<IDistributedCache>();
            await cache.SetStringAsync("test", "test");
            var result = await cache.GetStringAsync("test");

            result.Should().Be("test");
        }

        [Fact(DisplayName = "Given the key is nil. When executing GetStringAsync, be prompted. The return value be null.")]
        public async Task GetKeySuccessCaseNil()
        {
            var secretsManagerClientMock = new Mock<AmazonSecretsManagerClient>(new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.SAEast1 });
            secretsManagerClientMock.Setup(s => s.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), CancellationToken.None)).ReturnsAsync(new GetSecretValueResponse() { SecretString = SECRET_VALUE });

            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddSecretsManagerCache((sp) => secretsManagerClientMock.Object);
            services.AddElasticacheRedis(config =>
            {
                config.AbortConnect = false;
                config.Endpoing = _redisContainer.GetConnectionString();
                config.InstanceName = "test";
                config.SecretKey = "key";
                config.Ssl = false;
            });

            var serviceProvider = services.BuildServiceProvider();


            var cache = serviceProvider.GetRequiredService<IDistributedCache>();
            var result = await cache.GetStringAsync("test");

            result.Should().BeNullOrWhiteSpace();
        }
    }
}