using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Common.Snowflakes.Options;

/// <summary>
/// 雪花Id配置构建器
/// </summary>
public class SnowflakeOptionBuilder : SnowflakeOption
{
    internal readonly IServiceCollection Services;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services">服务集合</param>
    public SnowflakeOptionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
