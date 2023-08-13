using Microsoft.Extensions.Options;

namespace AWSFacade.ElastiCache.Redis
{
    public class RedisGatewayOptions : IOptions<RedisGatewayOptions>
    {
        public string Endpoing { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string InstanceName { get; set; } = string.Empty;
        public bool Ssl { get; set; }
        public bool AbortConnect { get; set; }

        public RedisGatewayOptions Value => this;
    }
}
