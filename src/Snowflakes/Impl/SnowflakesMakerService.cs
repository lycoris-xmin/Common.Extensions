using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes.Impl;

/// <summary>
/// 单机雪花Id生成服务
/// </summary>
public sealed class SnowflakesMakerService : ISnowflakeMaker
{
    private readonly SnowflakeIdGenerator _generator;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">雪花Id配置</param>
    public SnowflakesMakerService(IOptions<SnowflakeOption> options)
    {
        _generator = new SnowflakeIdGenerator(options.Value);
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
