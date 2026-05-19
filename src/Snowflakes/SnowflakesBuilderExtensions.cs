using Lycoris.Common.Snowflakes.Impl;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 雪花Id服务注册扩展
/// </summary>
public static class SnowflakesBuilderExtensions
{
    // === Standalone ===

    /// <summary>
    /// 添加单机雪花Id服务
    /// </summary>
    /// <param name="services">服务集合</param>
    public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services)
        => new(services);

    /// <summary>
    /// 添加单机雪花Id服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置操作</param>
    public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services, Action<SnowflakeOptionBuilder> configure)
    {
        var builder = new SnowflakeOptionBuilder(services);
        configure(builder);
        return builder;
    }

    /// <summary>
    /// 注册为单例服务
    /// </summary>
    /// <param name="builder">配置构建器</param>
    public static IServiceCollection AsService(this SnowflakeOptionBuilder builder)
    {
        if (SnowflakeHelper.HelperEnabled)
            throw new InvalidOperationException("cannot register as a singleton service and a static instance at the same time");

        builder.Services.Configure<SnowflakeOption>(opt =>
        {
            opt.WorkId = builder.WorkId;
            opt.WorkIdLength = builder.WorkIdLength;
            opt.StartTimeStamp = builder.StartTimeStamp;
        });

        builder.Services.TryAddSingleton<ISnowflakeMaker, SnowflakesMakerService>();
        return builder.Services;
    }

    /// <summary>
    /// 注册为静态实例
    /// </summary>
    /// <param name="builder">配置构建器</param>
    public static IServiceCollection AsHelper(this SnowflakeOptionBuilder builder)
    {
        if (builder.Services.Any(s => s.ImplementationType == typeof(SnowflakesMakerService)))
            throw new InvalidOperationException("cannot register as a singleton service and a static instance at the same time");

        SnowflakeHelper.Init(builder);
        return builder.Services;
    }

    // === Distributed ===

    /// <summary>
    /// 添加分布式雪花Id服务
    /// </summary>
    /// <param name="services">服务集合</param>
    public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services)
        => new(services);

    /// <summary>
    /// 添加分布式雪花Id服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configure">配置操作</param>
    public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services, Action<DistributedSnowflakeOptionBuilder> configure)
    {
        var builder = new DistributedSnowflakeOptionBuilder(services);
        configure(builder);
        return builder;
    }

    /// <summary>
    /// 添加分布式雪花Id Redis服务（单例模式）
    /// </summary>
    /// <typeparam name="T">Redis服务类型</typeparam>
    /// <param name="builder">配置构建器</param>
    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisService<T>(this DistributedSnowflakeOptionBuilder builder)
        where T : IDistributedSnowflakesRedis
    {
        builder.RedisType = typeof(T);
        return builder;
    }

    /// <summary>
    /// 注册为单例服务
    /// </summary>
    /// <param name="builder">配置构建器</param>
    public static IServiceCollection AsService(this DistributedSnowflakeOptionBuilder builder)
    {
        if (DistributedSnowflakeHelper.HelperEnabled)
            throw new InvalidOperationException("cannot register as a singleton service and a static instance at the same time");

        if (builder.RedisType == null)
            throw new InvalidOperationException("can not find redis tool service");

        builder.Services.Configure<DistributedSnowflakeOption>(opt =>
        {
            opt.WorkId = builder.WorkId;
            opt.WorkIdLength = builder.WorkIdLength;
            opt.StartTimeStamp = builder.StartTimeStamp;
            opt.RedisPrefix = string.IsNullOrEmpty(builder.RedisPrefix) ? Guid.NewGuid().ToString("N") : builder.RedisPrefix;
            opt.RefreshAliveInterval = builder.RefreshAliveInterval;
        });

        builder.Services.TryAddSingleton(builder.RedisType);
        builder.Services.TryAddSingleton<IDistributedSnowflakesSupport>(sp =>
            new DistributedSnowflakesSupport(
                sp.GetRequiredService<IOptions<DistributedSnowflakeOption>>().Value,
                (IDistributedSnowflakesRedis)sp.GetRequiredService(builder.RedisType)));
        builder.Services.TryAddSingleton<ISnowflakeMaker, DistributedSnowflakeService>();

        builder.Services.AddHostedService(sp =>
        {
            var option = sp.GetRequiredService<IOptions<DistributedSnowflakeOption>>();
            option.Value.Type = DistributedSnowflakeType.AsService;
            return new DistributedSnowflakesWorkBackgroundService(
                option.Value,
                sp.GetRequiredService<IDistributedSnowflakesSupport>(),
                sp.GetService<ILoggerFactory>());
        });

        return builder.Services;
    }

    /// <summary>
    /// 添加分布式雪花Id Redis服务（静态实例模式）
    /// </summary>
    /// <typeparam name="T">Redis服务类型</typeparam>
    /// <param name="builder">配置构建器</param>
    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisHelper<T>(this DistributedSnowflakeOptionBuilder builder)
        where T : IDistributedSnowflakesRedis, new()
    {
        builder.RedisHelper = new T();
        return builder;
    }

    /// <summary>
    /// 添加分布式雪花Id Redis服务（静态实例模式）
    /// </summary>
    /// <typeparam name="T">Redis服务类型</typeparam>
    /// <param name="builder">配置构建器</param>
    /// <param name="redisHelper">Redis帮助类实例</param>
    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisHelper<T>(this DistributedSnowflakeOptionBuilder builder, T redisHelper)
        where T : IDistributedSnowflakesRedis
    {
        builder.RedisHelper = redisHelper;
        return builder;
    }

    /// <summary>
    /// 注册为静态实例
    /// </summary>
    /// <param name="builder">配置构建器</param>
    public static IServiceCollection AsHelper(this DistributedSnowflakeOptionBuilder builder)
    {
        if (builder.Services.Any(s => s.ImplementationType == typeof(DistributedSnowflakeService)))
            throw new InvalidOperationException("cannot register as a singleton service and a static instance at the same time");

        if (builder.RedisHelper == null)
            throw new InvalidOperationException("can not find redis tool service");

        var option = new DistributedSnowflakeOption
        {
            WorkId = builder.WorkId,
            WorkIdLength = builder.WorkIdLength,
            StartTimeStamp = builder.StartTimeStamp,
            RedisPrefix = string.IsNullOrEmpty(builder.RedisPrefix) ? Guid.NewGuid().ToString("N") : builder.RedisPrefix,
            RefreshAliveInterval = builder.RefreshAliveInterval,
            Type = DistributedSnowflakeType.AsHelper
        };

        var support = new DistributedSnowflakesSupport(option, builder.RedisHelper);
        DistributedSnowflakeHelper.Init(option, support);

        builder.Services.AddHostedService(sp =>
            new DistributedSnowflakesWorkBackgroundService(option, null, sp.GetService<ILoggerFactory>()));

        return builder.Services;
    }
}
