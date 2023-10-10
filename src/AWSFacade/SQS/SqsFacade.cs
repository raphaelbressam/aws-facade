using Amazon.SQS;
using Amazon.SQS.Model;
using AWSFacade.SQS.Contracts;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AWSFacade.SQS
{
    public class SqsFacade : ISqsFacade
    {
        private AmazonSQSClient _amazonSQSClient;
        public string Name { get; } = string.Empty;
        public string QueueUrl { get; }
        public string? MessageGroupId { get; }
        public string? MessageDeduplicationId { get; }

        private string? LastReceiptHandle;

        public SqsFacade(string name, IOptions<SqsOptions> options, AmazonSQSClient? amazonSQSClient = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.QueueUrl)) throw new ArgumentNullException(nameof(options.Value.QueueUrl));

            Name = name;
            QueueUrl = options.Value.QueueUrl;
            MessageGroupId = options.Value.MessageGroupId;
            MessageDeduplicationId = options.Value.MessageDeduplicationId;

            _amazonSQSClient = amazonSQSClient != null ? amazonSQSClient : new AmazonSQSClient(options.Value.RegionEndpoint);
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string message)
        {
            var response = await PublishMessageAsync(message, MessageGroupId ?? "");
            return response;
        }

        public async Task<SendMessageResponse> PublishMessageAsync(string message, string messageGroupId)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = QueueUrl,
                MessageBody = message
            };

            if (!string.IsNullOrWhiteSpace(messageGroupId))
                sendMessageRequest.MessageGroupId = messageGroupId;

            if (!string.IsNullOrWhiteSpace(MessageDeduplicationId))
                sendMessageRequest.MessageDeduplicationId = MessageDeduplicationId;

            var response = await _amazonSQSClient.SendMessageAsync(sendMessageRequest);
            return response;
        }

        public async Task<Message> ReceiveMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(MessageGroupId))
                await DeleteLastMessageAsync();

            var receiveMessageRequest = new ReceiveMessageRequest(QueueUrl) { MaxNumberOfMessages = 1 };
            var receiveMessageResponse = await _amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

            if (receiveMessageResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new AmazonSQSException($"Receive message from SQS:\"{QueueUrl}\" was failed!");

            var message = receiveMessageResponse.Messages.FirstOrDefault();

            CleanLastReceiptHandle();
            if (message != null)
                SetLastReceiptHandle(message.ReceiptHandle);

            return receiveMessageResponse.Messages.FirstOrDefault();
        }

        public async Task<bool> DeleteMessageAsync(string receiptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest(QueueUrl, receiptHandle);
            var deleteMessageResponse = await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);

            return deleteMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> DeleteLastMessageAsync()
        {
            var deleteMessageRequest = new DeleteMessageRequest(QueueUrl, LastReceiptHandle);
            var deleteMessageResponse = await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);

            if (deleteMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                CleanLastReceiptHandle();
                return true;
            }

            return false;
        }

        private void CleanLastReceiptHandle()
            => LastReceiptHandle = null;
        private void SetLastReceiptHandle(string value)
            => LastReceiptHandle = value;
    }
}
