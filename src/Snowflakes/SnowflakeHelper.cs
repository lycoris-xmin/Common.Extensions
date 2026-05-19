using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 单机雪花Id静态帮助类
/// </summary>
public static class SnowflakeHelper
{
    private static SnowflakeIdGenerator? _generator;
    internal static bool HelperEnabled = false;

    internal static void Init(SnowflakeOption option)
    {
        _generator = new SnowflakeIdGenerator(option);
        HelperEnabled = true;
    }

    /// <summary>
    /// 获取下一个雪花Id
    /// </summary>
    public static long GetNextId() => GetGenerator().Next();

    /// <summary>
    /// 异步获取下一个雪花Id
    /// </summary>
    public static Task<long> GetNextIdAsync() => GetGenerator().NextAsync();

    /// <summary>
    /// 使用指定工作机器Id获取下一个雪花Id
    /// </summary>
    /// <param name="workId">工作机器Id</param>
    public static long GetNextId(int? workId) => GetGenerator().Next(workId);

    /// <summary>
    /// 使用指定工作机器Id异步获取下一个雪花Id
    /// </summary>
    /// <param name="workId">工作机器Id</param>
    public static Task<long> GetNextIdAsync(int? workId) => GetGenerator().NextAsync(workId);

    /// <summary>
    /// 批量获取雪花Id
    /// </summary>
    /// <param name="count">批量数量</param>
    public static long[] GetNextIds(int count) => GetGenerator().NextBatch(count);

    /// <summary>
    /// 解析雪花Id为组成部分
    /// </summary>
    /// <param name="id">雪花Id</param>
    public static SnowflakeIdInfo Parse(long id) => GetGenerator().Parse(id);

    private static SnowflakeIdGenerator GetGenerator()
    {
        if (_generator == null)
            throw new InvalidOperationException("SnowflakeHelper is not initialized. Call AsHelper() during service registration.");
        return _generator;
    }
}
