using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes;

public static class DistributedSnowflakeHelper
{
    private static SnowflakeIdGenerator? _generator;
    internal static IDistributedSnowflakesSupport? _distributedSupport;
    internal static bool HelperEnabled = false;

    internal static void Init(DistributedSnowflakeOption option, IDistributedSnowflakesSupport distributedSupport)
    {
        _distributedSupport = distributedSupport;
        _generator = new SnowflakeIdGenerator(option, async () => await _distributedSupport.GetNextWorkIdAsync());
        HelperEnabled = true;
    }

    public static long GetNextId() => GetGenerator().Next();
    public static Task<long> GetNextIdAsync() => GetGenerator().NextAsync();
    public static long GetNextId(int? workId) => GetGenerator().Next(workId);
    public static Task<long> GetNextIdAsync(int? workId) => GetGenerator().NextAsync(workId);
    public static long[] GetNextIds(int count) => GetGenerator().NextBatch(count);
    public static SnowflakeIdInfo Parse(long id) => GetGenerator().Parse(id);

    private static SnowflakeIdGenerator GetGenerator()
    {
        if (_generator == null)
            throw new InvalidOperationException("DistributedSnowflakeHelper is not initialized.");
        return _generator;
    }
}
