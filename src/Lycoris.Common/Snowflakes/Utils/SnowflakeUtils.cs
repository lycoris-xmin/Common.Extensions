namespace Lycoris.Common.Snowflakes.Utils;

internal static class SnowflakeUtils
{
    public static long SnowflakeTimeStamp(long startTicks, long lastTimestamp = 0L)
    {
        var current = (DateTime.Now.Ticks - startTicks) / 10000;

        if (lastTimestamp == current)
        {
            Thread.Sleep(0);
            return SnowflakeTimeStamp(startTicks, lastTimestamp);
        }

        return current;
    }
}
