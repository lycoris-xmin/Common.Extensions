namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 雪花Id生成器接口
/// </summary>
public interface ISnowflakeMaker
{
    /// <summary>
    /// 获取下一个Id
    /// </summary>
    long GetNextId();

    /// <summary>
    /// 异步获取下一个Id
    /// </summary>
    Task<long> GetNextIdAsync();

    /// <summary>
    /// 使用指定工作机器Id获取下一个Id
    /// </summary>
    /// <param name="workId">工作机器Id</param>
    long GetNextId(int? workId);

    /// <summary>
    /// 使用指定工作机器Id异步获取下一个Id
    /// </summary>
    /// <param name="workId">工作机器Id</param>
    Task<long> GetNextIdAsync(int? workId);
}
