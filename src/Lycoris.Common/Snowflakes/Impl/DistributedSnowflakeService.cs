using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes.Impl;

public class DistributedSnowflakeService : ISnowflakeMaker
{
    private readonly SnowflakeIdGenerator _generator;

    public DistributedSnowflakeService(IOptions<DistributedSnowflakeOption> options, IDistributedSnowflakesSupport distributedSupport)
    {
        _generator = new SnowflakeIdGenerator(options.Value, async () => await distributedSupport.GetNextWorkIdAsync());
    }

    public long GetNextId() => _generator.Next();
    public Task<long> GetNextIdAsync() => _generator.NextAsync();
    public long GetNextId(int? workId) => _generator.Next(workId);
    public Task<long> GetNextIdAsync(int? workId) => _generator.NextAsync(workId);
}
