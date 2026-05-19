using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Common.Snowflakes.Options;

public class SnowflakeOptionBuilder : SnowflakeOption
{
    internal readonly IServiceCollection Services;

    public SnowflakeOptionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
