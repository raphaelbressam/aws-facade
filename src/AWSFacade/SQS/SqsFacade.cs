using Amazon.SQS;
using Amazon.SQS.Model;
using AWSFacade.SQS.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AWSFacade.SQS
{
    public class SqsFacade : ISqsFacade
    {
        readonly ILogger<SqsFacade>? _logger;
        private AmazonSQSClient _amazonSQSClient;
        public string Name { get; } = string.Empty;
        public string QueueUrl { get; }
        public string? MessageGroupId { get; }

        private string? LastReceiptHandle;

        public SqsFacade(string name, IOptions<SqsOptions> options, AmazonSQSClient? amazonSQSClient = null, ILogger<SqsFacade>? logger = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.QueueUrl)) throw new ArgumentNullException(nameof(options.Value.QueueUrl));

            Name = name;
            QueueUrl = options.Value.QueueUrl;
            MessageGroupId = options.Value.MessageGroupId;

            _amazonSQSClient = amazonSQSClient != null ? amazonSQSClient : new AmazonSQSClient(options.Value.RegionEndpoint);
            _logger = logger;
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string message, string? messageDeduplicationId = null)
        {
            var response = await PublishMessageAsync(message, MessageGroupId ?? "", messageDeduplicationId);
            return response;
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string message, string messageGroupId, string? messageDeduplicationId = null)
        {
            _logger?.LogDebug("Publishing message {message}", message);
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = QueueUrl,
                MessageBody = message
            };

            if (!string.IsNullOrWhiteSpace(messageGroupId))
                sendMessageRequest.MessageGroupId = messageGroupId;

            if (!string.IsNullOrWhiteSpace(messageDeduplicationId))
                sendMessageRequest.MessageDeduplicationId = messageDeduplicationId;

            var response = await _amazonSQSClient.SendMessageAsync(sendMessageRequest);
            _logger?.LogDebug("Publish response: {HttpStatusCode}", response.HttpStatusCode);
            return response;
        }

        public async Task<Message> ReceiveMessageAsync()
        {
            _logger?.LogDebug("Receiving message from queue {QueueUrl}", QueueUrl);

            if (!string.IsNullOrWhiteSpace(MessageGroupId))
                await DeleteLastMessageAsync();

            var receiveMessageRequest = new ReceiveMessageRequest(QueueUrl) { MaxNumberOfMessages = 1 };
            var receiveMessageResponse = await _amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

            _logger?.LogDebug("Receive message response: {HttpStatusCode}", receiveMessageResponse.HttpStatusCode);

            if (receiveMessageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger?.LogWarning("Receive message from SQS:\"{QueueUrl}\" was failed!", QueueUrl);
                return default;
            }

            var message = receiveMessageResponse.Messages.FirstOrDefault();

            _logger?.LogDebug("Receive message: {Body}", message?.Body);

            CleanLastReceiptHandle();
            if (message != null)
                SetLastReceiptHandle(message.ReceiptHandle);

            return receiveMessageResponse.Messages.FirstOrDefault();
        }

        public async Task<bool> DeleteMessageAsync(string receiptHandle)
        {
            if (string.IsNullOrWhiteSpace(receiptHandle))
            {
                _logger?.LogWarning("The receipt handle is null, nothing to do!");
                return true;
            }

            _logger?.LogDebug("Deleting message {receiptHandle} from queue {QueueUrl}", receiptHandle, QueueUrl);

            var deleteMessageRequest = new DeleteMessageRequest(QueueUrl, receiptHandle);
            var deleteMessageResponse = await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);

            _logger?.LogDebug("Deleting message response: {HttpStatusCode}", deleteMessageResponse.HttpStatusCode);

            return deleteMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeleteLastMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(LastReceiptHandle))
            {
                _logger?.LogWarning("The last receipt handle is null, nothing to do!");
                return true;
            }

            _logger?.LogDebug("Deleting last message {LastReceiptHandle} from queue {QueueUrl}", LastReceiptHandle, QueueUrl);

            var deleteMessageRequest = new DeleteMessageRequest(QueueUrl, LastReceiptHandle);
            var deleteMessageResponse = await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);

            _logger?.LogDebug("Deleting message response: {HttpStatusCode}", deleteMessageResponse.HttpStatusCode);

            if (deleteMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                CleanLastReceiptHandle();
                return true;
            }

            return false;
        }

        private void CleanLastReceiptHandle()
        {
            _logger?.LogDebug("Cleaning last receipt handle");
            LastReceiptHandle = null;
        }
        private void SetLastReceiptHandle(string value)
        {
            _logger?.LogDebug("Set last receipt handle value: {value}", value);
            LastReceiptHandle = value;
        }
    }
}
