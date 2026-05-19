namespace Lycoris.Common.Snowflakes;

public interface IDistributedSnowflakesRedis
{
    Task<long> IncrByAsync(string key, long value);
    Task<bool> ExpireAsync(string key, TimeSpan expire);
    Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers);
    Task<long> ZRemAsync<T>(string key, params T[] member);
    Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0L);
}
