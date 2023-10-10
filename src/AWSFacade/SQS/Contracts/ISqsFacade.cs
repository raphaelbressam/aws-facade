using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace AWSFacade.SQS.Contracts
{
    public interface ISqsFacade
    {
        public string Name { get; }
        public string QueueUrl { get; }
        public string? MessageGroupId { get; }

        Task<SendMessageResponse> PublishMessageAsync(string message, string? messageDeduplicationId = null);
        Task<SendMessageResponse> PublishMessageAsync(string message, string messageGroupId, string? messageDeduplicationId = null);
        Task<Message> ReceiveMessageAsync();
        Task<bool> DeleteMessageAsync(string receiptHandle);
        Task<bool> DeleteLastMessageAsync();
    }
}
