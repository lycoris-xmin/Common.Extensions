using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lycoris.Common.Snowflakes.Impl;

/// <summary>
/// 分布式雪花Id心跳后台服务，用于刷新机器存活状态和清理过期节点
/// </summary>
public class DistributedSnowflakesWorkBackgroundService : BackgroundService
{
    private readonly DistributedSnowflakeOption _option;
    private readonly IDistributedSnowflakesSupport? _distributedSupport;
    private readonly int _refreshAliveInterval;
    private readonly ILogger? _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="option">分布式雪花Id配置</param>
    /// <param name="distributedSupport">分布式支持服务</param>
    /// <param name="factory">日志工厂</param>
    public DistributedSnowflakesWorkBackgroundService(
        DistributedSnowflakeOption option,
        IDistributedSnowflakesSupport? distributedSupport,
        ILoggerFactory? factory)
    {
        _option = option;
        _distributedSupport = distributedSupport;
        _refreshAliveInterval = (int)Math.Ceiling(_option.RefreshAliveInterval.Add(TimeSpan.FromMinutes(1)).TotalMilliseconds);
        _logger = factory?.CreateLogger<DistributedSnowflakesWorkBackgroundService>();
    }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">取消令牌</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var support = _option.Type == DistributedSnowflakeType.AsService
                    ? _distributedSupport
                    : DistributedSnowflakeHelper._distributedSupport;

                if (support != null)
                {
                    await support.RefreshAliveAsync();
                    await support.RemoveNotAliveWorkNodeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "refresh machine alive time failed");
            }

            await Task.Delay(_refreshAliveInterval, stoppingToken);
        }
    }
}
