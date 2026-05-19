namespace Lycoris.Common.Snowflakes;

public interface ISnowflakeMaker
{
    long GetNextId();
    Task<long> GetNextIdAsync();
    long GetNextId(int? workId);
    Task<long> GetNextIdAsync(int? workId);
}
