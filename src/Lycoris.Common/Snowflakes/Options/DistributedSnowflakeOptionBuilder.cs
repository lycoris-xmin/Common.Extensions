using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Common.Snowflakes.Options;

public class DistributedSnowflakeOptionBuilder : DistributedSnowflakeOption
{
    internal readonly IServiceCollection Services;
    internal Type? RedisType;
    internal IDistributedSnowflakesRedis? RedisHelper;

    public DistributedSnowflakeOptionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
