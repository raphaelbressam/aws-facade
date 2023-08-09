using Amazon;
using AWSFacade.SQS.Contracts;
using AWSFacade.SQS.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AWSFacadeTest
{
    public class SqsFacadeUnitTest
    {
        [Theory(DisplayName = "Given that I added 2 or more Sqs's. When executing create, be prompted. Then the sqs facade should be created with right name.")]
        [InlineData("A", "B", "C", "D", "")]
        public async Task SuccessCaseWithNames(params string[] list)
        {
            ServiceCollection services = new ServiceCollection();
            foreach (string item in list)
            {
                services.AddSqsFacade(item, config =>
                {
                    config.MessageGroupId = string.IsNullOrWhiteSpace(item) ? "default" : item;
                    config.QueueUrl = string.IsNullOrWhiteSpace(item) ? "default" : item;
                    config.RegionEndpoint = RegionEndpoint.SAEast1;
                });
            }

            var serviceProvider = services.BuildServiceProvider();

            var sqsFacadeFactory = serviceProvider.GetService<ISqsFacadeFactory>();

            foreach (string item in list)
            {
                var sqsFacade = sqsFacadeFactory!.Create(item);

                sqsFacade.Should().NotBeNull();
                sqsFacade.Name.Should().Be(item);
            }
        }

        [Fact(DisplayName = "Given that I added Sqs without name. When executing create, be prompted. Then the sqs facade should be created.")]
        public async Task SuccessCaseWithoutName()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSqsFacade(config =>
            {
                config.MessageGroupId = "default";
                config.QueueUrl = "default";
                config.RegionEndpoint = RegionEndpoint.SAEast1;
            });

            var serviceProvider = services.BuildServiceProvider();

            var sqsFacadeFactory = serviceProvider.GetService<ISqsFacadeFactory>();
            var sqsFacade = sqsFacadeFactory!.Create();

            sqsFacade.Should().NotBeNull();
            sqsFacade.Name.Should().BeEmpty();
        }
    }

    public class Customer
    {
        private readonly ISqsFacade _sqsFacade;
        public Customer(ISqsFacadeFactory sqsFacadeFactory)
        {
            _sqsFacade = sqsFacadeFactory.Create();
        }
    }
}