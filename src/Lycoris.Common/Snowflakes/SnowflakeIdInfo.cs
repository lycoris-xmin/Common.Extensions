namespace Lycoris.Common.Snowflakes;

public readonly struct SnowflakeIdInfo
{
    public DateTime Timestamp { get; }
    public int WorkId { get; }
    public int Sequence { get; }

    public SnowflakeIdInfo(DateTime timestamp, int workId, int sequence)
    {
        Timestamp = timestamp;
        WorkId = workId;
        Sequence = sequence;
    }

    public override string ToString() => $"Timestamp={Timestamp:O}, WorkId={WorkId}, Sequence={Sequence}";
}
