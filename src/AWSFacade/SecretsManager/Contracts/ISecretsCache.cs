using System.Threading.Tasks;

namespace AWSFacade.SecretsManager.Contracts
{
    public interface ISecretsCache
    {
        string? GetSecretValue(string key);
        Task<string?> GetSecretValueAsync(string key);
    }
}
