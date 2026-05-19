namespace Lycoris.Common.Snowflakes;

public interface IDistributedSnowflakesSupport
{
    Task<int> GetNextWorkIdAsync();
    Task RefreshAliveAsync();
    Task RemoveNotAliveWorkNodeAsync();
}
