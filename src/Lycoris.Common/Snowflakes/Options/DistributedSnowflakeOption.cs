namespace Lycoris.Common.Snowflakes.Options;

public class DistributedSnowflakeOption : SnowflakeOption
{
    public string? RedisPrefix { get; set; }
    public TimeSpan RefreshAliveInterval { get; set; } = TimeSpan.FromHours(1);
    internal DistributedSnowflakeType Type { get; set; }
}

public enum DistributedSnowflakeType
{
    AsService = 0,
    AsHelper = 1
}
