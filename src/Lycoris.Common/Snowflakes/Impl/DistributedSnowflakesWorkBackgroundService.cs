using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lycoris.Common.Snowflakes.Impl;

public class DistributedSnowflakesWorkBackgroundService : BackgroundService
{
    private readonly DistributedSnowflakeOption _option;
    private readonly IDistributedSnowflakesSupport? _distributedSupport;
    private readonly int _refreshAliveInterval;
    private readonly ILogger? _logger;

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
