namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 分布式雪花Id Redis操作接口
/// </summary>
public interface IDistributedSnowflakesRedis
{
    /// <summary>
    /// 将 key 所储存的值加上给定的增量值
    /// </summary>
    /// <param name="key">redis键</param>
    /// <param name="value">增量值</param>
    Task<long> IncrByAsync(string key, long value);

    /// <summary>
    /// 为给定 key 设置过期时间
    /// </summary>
    /// <param name="key">redis键</param>
    /// <param name="expire">过期时间</param>
    Task<bool> ExpireAsync(string key, TimeSpan expire);

    /// <summary>
    /// 向有序集合添加一个或多个成员
    /// </summary>
    /// <param name="key">redis键</param>
    /// <param name="scoreMembers">分数和成员元组数组</param>
    Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers);

    /// <summary>
    /// 移除有序集合中的一个或多个成员
    /// </summary>
    /// <typeparam name="T">成员类型</typeparam>
    /// <param name="key">redis键</param>
    /// <param name="member">一个或多个成员</param>
    Task<long> ZRemAsync<T>(string key, params T[] member);

    /// <summary>
    /// 通过分数返回有序集合指定区间内的成员和分数
    /// </summary>
    /// <param name="key">redis键</param>
    /// <param name="min">分数最小值</param>
    /// <param name="max">分数最大值</param>
    /// <param name="count">返回多少成员</param>
    /// <param name="offset">返回条件偏移位置</param>
    Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0L);
}
