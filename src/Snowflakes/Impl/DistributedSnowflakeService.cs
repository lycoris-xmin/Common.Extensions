using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes.Impl;

/// <summary>
/// 分布式雪花Id生成服务
/// </summary>
public class DistributedSnowflakeService : ISnowflakeMaker
{
    private readonly SnowflakeIdGenerator _generator;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">分布式雪花Id配置</param>
    /// <param name="distributedSupport">分布式支持服务</param>
    public DistributedSnowflakeService(IOptions<DistributedSnowflakeOption> options, IDistributedSnowflakesSupport distributedSupport)
    {
        _generator = new SnowflakeIdGenerator(options.Value, async () => await distributedSupport.GetNextWorkIdAsync());
    }

    /// <inheritdoc />
    public long GetNextId() => _generator.Next();

    /// <inheritdoc />
    public Task<long> GetNextIdAsync() => _generator.NextAsync();

    /// <inheritdoc />
    public long GetNextId(int? workId) => _generator.Next(workId);

    /// <inheritdoc />
    public Task<long> GetNextIdAsync(int? workId) => _generator.NextAsync(workId);
}
