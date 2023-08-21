using Amazon.SecretsManager;
using AWSFacade.SecretsManager.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AWSFacade.SecretsManager
{
    public class SecretsManager : ISecretsCache
    {
        readonly IAmazonSecretsManager _amazonSecretsManager;
        readonly IMemoryCache _memoryCache;
        readonly ILogger? _logger;

        public SecretsManager(IAmazonSecretsManager amazonSecretsManager, IMemoryCache memoryCache, ILogger<SecretsManager>? logger = null)
        {
            _amazonSecretsManager = amazonSecretsManager;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public string? GetSecretValue(string key)
        {
            return GetSecretValueAsync(key).GetAwaiter().GetResult();
        }

        public async Task<string?> GetSecretValueAsync(string key)
        {
            LogInformation("Get secret value from key:{key}", key);

            if (_memoryCache.TryGetValue(key, out string? cachedValue)
                && !string.IsNullOrWhiteSpace(cachedValue))
            {
                LogInformation("Found in cache");
                LogDebug("Cache value: {cachedValue}", cachedValue);
                return cachedValue;
            }

            var secretValueResponse = await _amazonSecretsManager.GetSecretValueAsync(new Amazon.SecretsManager.Model.GetSecretValueRequest { SecretId = key });
            var secretString = secretValueResponse?.SecretString;

            LogInformation("HttpStatusCode: {secretValueResponse.HttpStatusCode}", secretValueResponse?.HttpStatusCode ?? 0);
            LogDebug("Value:{secretString}", secretString ?? "");

            if (!string.IsNullOrWhiteSpace(secretString))
                _memoryCache.Set(key, secretString);

            return secretString;
        }

        private void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger?.LogDebug(message, args);
        }
    }
}
