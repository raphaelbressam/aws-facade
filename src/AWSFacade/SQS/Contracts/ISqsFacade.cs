using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace AWSFacade.SQS.Contracts
{
    public interface ISqsFacade
    {
        public string Name { get; }
        public string QueueUrl { get; }
        public string? MessageGroupId { get; }

        Task<SendMessageResponse> PublishMessageAsync(string message);
        Task<Message> ReceiveMessageAsync();
        Task<bool> DeleteMessageAsync(string receiptHandle);
        Task<bool> DeleteLastMessageAsync();
    }
}
