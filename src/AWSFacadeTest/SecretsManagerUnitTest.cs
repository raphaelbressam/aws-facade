using Amazon;
using Amazon.SecretsManager;
using AWSFacade.SecretsManager.Contracts;
using AWSFacade.SecretsManager.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AWSFacadeTest
{
    public class SecretsManagerUnitTest
    {
        [Fact(DisplayName = "Given that secret manager has keys. When executing get secret value, be prompted. Then the secret string should be null.")]
        public void SuccessCase()
        {
            var cancellationToken = new CancellationTokenSource();
            var secretsManagerClientMock = new Mock<AmazonSecretsManagerClient>(new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.SAEast1 });

            ServiceCollection services = new ServiceCollection();
            services.AddSecretsManagerCache((sp) => secretsManagerClientMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            var secretsCache = serviceProvider.GetRequiredService<ISecretsCache>();

            var result = secretsCache.GetSecretValue("key1");

            result.Should().BeNull();
        }

        [Fact(DisplayName = "Given that secret manager has keys. When executing get secret value async, be prompted. Then the secret string should be null")]
        public async Task SuccessCaseAsync()
        {
            var cancellationToken = new CancellationTokenSource();
            var secretsManagerClientMock = new Mock<AmazonSecretsManagerClient>(new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.SAEast1 });

            ServiceCollection services = new ServiceCollection();
            services.AddSecretsManagerCache((sp) => secretsManagerClientMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            var secretsCache = serviceProvider.GetRequiredService<ISecretsCache>();

            var result = await secretsCache.GetSecretValueAsync("key1");

            result.Should().BeNull();
        }
    }
}