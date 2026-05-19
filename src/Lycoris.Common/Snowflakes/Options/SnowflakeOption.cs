namespace Lycoris.Common.Snowflakes.Options;

public class SnowflakeOption
{
    public int? WorkId { get; set; } = 1;
    public int WorkIdLength { get; set; } = 10;
    public DateTime StartTimeStamp { get; set; } = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
