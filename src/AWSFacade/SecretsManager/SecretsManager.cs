using Amazon.SecretsManager;
using AWSFacade.SecretsManager.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace AWSFacade.SecretsManager
{
    public class SecretsManager : ISecretsCache
    {
        readonly IAmazonSecretsManager _amazonSecretsManager;
        readonly IMemoryCache _memoryCache;

        public SecretsManager(IAmazonSecretsManager amazonSecretsManager, IMemoryCache memoryCache)
        {
            _amazonSecretsManager = amazonSecretsManager;
            _memoryCache = memoryCache;
        }

        public string? GetSecretValue(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetSecretValueAsync(string key)
        {
            if (_memoryCache.TryGetValue(key, out string? cachedValue)
                && !string.IsNullOrWhiteSpace(cachedValue))
                return cachedValue;

            var secretValueResponse = await _amazonSecretsManager.GetSecretValueAsync(new Amazon.SecretsManager.Model.GetSecretValueRequest { SecretId = key });
            var secretString = secretValueResponse?.SecretString;

            if (!string.IsNullOrWhiteSpace(secretString))
                _memoryCache.Set(key, secretString);

            return secretString;
        }
    }
}
