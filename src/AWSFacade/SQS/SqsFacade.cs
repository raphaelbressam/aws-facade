using Amazon.SQS;
using Amazon.SQS.Model;
using AWSFacade.SQS.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AWSFacade.SQS
{
    public class SqsFacade : ISqsFacade
    {
        private AmazonSQSClient _amazonSQSClient;
        public string Name { get; } = string.Empty;
        public string QueueUrl { get; }
        public string? MessageGroupId { get; }

        public SqsFacade(string name, IOptions<SqsOptions> options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.QueueUrl)) throw new ArgumentNullException(nameof(options.Value.QueueUrl));

            Name = name;
            QueueUrl = options.Value.QueueUrl;
            MessageGroupId = options.Value.MessageGroupId;

            _amazonSQSClient = new AmazonSQSClient(options.Value.RegionEndpoint);
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = QueueUrl,
                MessageBody = message
            };

            if (!string.IsNullOrWhiteSpace(MessageGroupId))
                sendMessageRequest.MessageGroupId = MessageGroupId;

            var response = await _amazonSQSClient.SendMessageAsync(sendMessageRequest);
            return response;
        }
    }
}
