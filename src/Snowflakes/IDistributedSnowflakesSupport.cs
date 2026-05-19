namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 分布式雪花Id支持接口
/// </summary>
public interface IDistributedSnowflakesSupport
{
    /// <summary>
    /// 获取下一个可用的机器Id
    /// </summary>
    Task<int> GetNextWorkIdAsync();

    /// <summary>
    /// 刷新机器Id的存活状态
    /// </summary>
    Task RefreshAliveAsync();

    /// <summary>
    /// 移除未按心跳时间刷新的机器Id
    /// </summary>
    Task RemoveNotAliveWorkNodeAsync();
}
