using Amazon;
using Microsoft.Extensions.Options;

namespace AWSFacade.SQS
{
    public class SqsOptions : IOptions<SqsOptions>
    {
        public string? QueueUrl { get; set; }
        public string? MessageGroupId { get; set; }
        public string? MessageDeduplicationId { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; } = RegionEndpoint.SAEast1;

        SqsOptions IOptions<SqsOptions>.Value { get { return this; } }
    }
}