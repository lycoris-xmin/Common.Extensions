using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes;

public static class SnowflakeHelper
{
    private static SnowflakeIdGenerator? _generator;
    internal static bool HelperEnabled = false;

    internal static void Init(SnowflakeOption option)
    {
        _generator = new SnowflakeIdGenerator(option);
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
            throw new InvalidOperationException("SnowflakeHelper is not initialized. Call AsHelper() during service registration.");
        return _generator;
    }
}
