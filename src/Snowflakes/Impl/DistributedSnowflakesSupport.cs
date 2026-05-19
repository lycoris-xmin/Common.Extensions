using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes.Impl;

/// <summary>
/// 分布式雪花Id Redis支持实现
/// </summary>
public sealed class DistributedSnowflakesSupport : IDistributedSnowflakesSupport
{
    private readonly IDistributedSnowflakesRedis _distributedRedis;
    private readonly string _currentWorkIndex;
    private readonly string _inUse;
    private int _workId;
    private readonly DistributedSnowflakeOption _option;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="option">分布式雪花Id配置</param>
    /// <param name="distributedRedis">分布式Redis操作接口</param>
    public DistributedSnowflakesSupport(DistributedSnowflakeOption option, IDistributedSnowflakesRedis distributedRedis)
    {
        _option = option;
        _distributedRedis = distributedRedis;
        _currentWorkIndex = $"{_option.RedisPrefix}:CurrentWorkIndex";
        _inUse = $"{_option.RedisPrefix}:Use";
    }

    /// <inheritdoc />
    public async Task<int> GetNextWorkIdAsync()
    {
        var cache = await StringIncrementAsync(_currentWorkIndex);
        _workId = (int)cache - 1;

        if (_workId > (1 << _option.WorkIdLength) - 1)
        {
            var startScore = DateTime.Now.AddSeconds(-1800).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
            var endScore = DateTime.Now.AddMinutes(-5);
            var availableWorkIds = await SortedRangeByScoreAsync(_inUse, GetTimestamp(startScore), GetTimestamp(endScore), offset: 1);
            if (availableWorkIds.Count == 0)
                throw new Exception("no available work nodes");

            _workId = int.Parse(availableWorkIds.First().Key);
        }

        await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));
        return _workId;
    }

    /// <inheritdoc />
    public async Task RefreshAliveAsync()
    {
        await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));
    }

    /// <inheritdoc />
    public async Task RemoveNotAliveWorkNodeAsync()
    {
        var startScore = DateTime.Now.AddSeconds(-1801).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
        var notAliveList = await SortedRangeByScoreAsync(_inUse, 0, GetTimestamp(startScore), count: 20);
        if (notAliveList is { Count: > 0 })
        {
            foreach (var item in notAliveList)
                await _distributedRedis.ZRemAsync(_inUse, item.Key);
        }
    }

    private static long GetTimestamp(DateTime? time = null)
    {
        time ??= DateTime.Now;
        return (time.Value.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000;
    }

    private async Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? timeSpan = null)
    {
        var cache = await _distributedRedis.IncrByAsync(key, value);
        if (timeSpan != null)
            await _distributedRedis.ExpireAsync(key, timeSpan.Value);
        return cache;
    }

    private async Task<Dictionary<string, decimal>> SortedRangeByScoreAsync(string key, decimal min, decimal max, long? count = null, long offset = 0)
    {
        var cache = await _distributedRedis.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);
        if (cache is not { Length: > 0 })
            return new Dictionary<string, decimal>();

        var dic = new Dictionary<string, decimal>();
        foreach (var (member, score) in cache)
            dic.Add(member, score);

        return dic.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }
}
