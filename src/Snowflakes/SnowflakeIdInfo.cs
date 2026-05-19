namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 雪花Id解析结果
/// </summary>
public readonly struct SnowflakeIdInfo
{
    /// <summary>
    /// Id生成时的时间戳
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// 工作机器Id
    /// </summary>
    public int WorkId { get; }

    /// <summary>
    /// 毫秒内序列号
    /// </summary>
    public int Sequence { get; }

    /// <summary>
    /// 雪花Id解析结果
    /// </summary>
    /// <param name="timestamp">时间戳</param>
    /// <param name="workId">工作机器Id</param>
    /// <param name="sequence">序列号</param>
    public SnowflakeIdInfo(DateTime timestamp, int workId, int sequence)
    {
        Timestamp = timestamp;
        WorkId = workId;
        Sequence = sequence;
    }

    /// <summary>
    /// 返回雪花Id解析结果的字符串表示
    /// </summary>
    public override string ToString() => $"Timestamp={Timestamp:O}, WorkId={WorkId}, Sequence={Sequence}";
}
