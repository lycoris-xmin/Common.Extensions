using System;

namespace Lycoris.Common.Snowflakes.Options;

/// <summary>
/// 分布式雪花Id配置
/// </summary>
public class DistributedSnowflakeOption : SnowflakeOption
{
    /// <summary>
    /// Redis分布式路由前缀，根据对应集群或者服务类别配置不同的前缀，未设置则随机生成
    /// </summary>
    public string? RedisPrefix { get; set; }

    /// <summary>
    /// 刷新存活状态的间隔时间，默认1小时
    /// </summary>
    public TimeSpan RefreshAliveInterval { get; set; } = TimeSpan.FromHours(1);

    internal DistributedSnowflakeType Type { get; set; }
}

/// <summary>
/// 分布式雪花Id类型
/// </summary>
public enum DistributedSnowflakeType
{
    /// <summary>
    /// 单例服务模式
    /// </summary>
    AsService = 0,

    /// <summary>
    /// 静态帮助类模式
    /// </summary>
    AsHelper = 1
}
