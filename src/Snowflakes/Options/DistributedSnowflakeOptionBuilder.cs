using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Common.Snowflakes.Options;

/// <summary>
/// 分布式雪花Id配置构建器
/// </summary>
public class DistributedSnowflakeOptionBuilder : DistributedSnowflakeOption
{
    internal readonly IServiceCollection Services;
    internal Type? RedisType;
    internal IDistributedSnowflakesRedis? RedisHelper;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services">服务集合</param>
    public DistributedSnowflakeOptionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
