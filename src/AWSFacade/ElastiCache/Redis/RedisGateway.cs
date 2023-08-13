using AWSFacade.SecretsManager.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSFacade.ElastiCache.Redis
{
    public class RedisGateway : IDistributedCache
    {
        const string HASHKEY_DATA = "data";

        IDatabase? _dataBase = null;
        private readonly ISecretsCache _secretsCache;
        private readonly RedisGatewayOptions _options;
        private readonly ILogger _logger;

        public RedisGateway(ISecretsCache secretsCache, IOptions<RedisGatewayOptions> options, ILogger<RedisGateway> logger)
        {
            _secretsCache = secretsCache;
            _options = options.Value;
            _logger = logger;
        }

        private async Task<string> GetConnectionStringAsync()
        {
            var redisSecretKey = await _secretsCache.GetSecretValueAsync(_options.SecretKey);
            return $"{_options.Endpoing},password={redisSecretKey},ssl={_options.Ssl},abortConnect={_options.AbortConnect}";
        }

        private async Task InitDatabaseAsync()
        {
            if (_dataBase == null)
            {
                var connectionString = await GetConnectionStringAsync();
                var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
                _dataBase = connection.GetDatabase();
            }
        }

        private async Task<bool> TestConnectionAsync()
        {
            try
            {
                _ = await _dataBase!.PingAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string SetPrefixKey(string key)
        {
            return $"{_options.InstanceName}{key}";
        }

        public byte[]? Get(string key)
        {
            return GetAsync(key).GetAwaiter().GetResult();
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            await InitDatabaseAsync();
            var keyValue = await _dataBase!.HashGetAsync(SetPrefixKey(key), HASHKEY_DATA);
            return keyValue;
        }

        public void Refresh(string key)
        {
            RefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await InitDatabaseAsync();
            await _dataBase!.KeyTouchAsync(SetPrefixKey(key));
        }

        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await InitDatabaseAsync();
            _ = await _dataBase!.KeyDeleteAsync(SetPrefixKey(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await InitDatabaseAsync();
            string valueString = Encoding.UTF8.GetString(value, 0, value.Length);
            _ = await _dataBase!.HashSetAsync(SetPrefixKey(key), HASHKEY_DATA, valueString);
        }
    }
}
