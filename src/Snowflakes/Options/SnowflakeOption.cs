using System;

namespace Lycoris.Common.Snowflakes.Options;

/// <summary>
/// 雪花Id配置
/// </summary>
public class SnowflakeOption
{
    /// <summary>
    /// 工作机器Id，默认从1开始，用于防止时钟回拨导致的Id重复
    /// </summary>
    public int? WorkId { get; set; } = 1;

    /// <summary>
    /// 工作机器Id所占用的长度，最大10，默认10
    /// </summary>
    public int WorkIdLength { get; set; } = 10;

    /// <summary>
    /// 用于计算时间戳的开始时间，默认 UTC 2020-01-01
    /// </summary>
    public DateTime StartTimeStamp { get; set; } = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
