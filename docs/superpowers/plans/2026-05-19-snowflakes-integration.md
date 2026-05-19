# Snowflakes Integration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Integrate Snowflakes ID generation into Lycoris.Common, fix 3 bugs in the original, add batch generation and ID parsing convenience APIs.

**Architecture:** Shared `SnowflakeIdGenerator` internal class encapsulates the core algorithm (timestamp + workId + sequence bit-packing, clock drift recovery, sequence exhaustion). Four callers — `SnowflakeHelper` (static), `SnowflakesMakerService` (DI), `DistributedSnowflakeHelper` (static + Redis), `DistributedSnowflakeService` (DI + Redis) — all delegate to a generator instance. DI registration via `SnowflakesBuilderExtensions`.

**Tech Stack:** C# / .NET 8.0, `Microsoft.Extensions.*` 9.0.5 packages for DI/Options/Hosting/Logging.

---

### Task 1: Add NuGet package references

**Files:**
- Modify: `src/Lycoris.Common/Lycoris.Common.csproj`

- [ ] **Step 1: Add four Microsoft.Extensions package references**

In the `<ItemGroup>` that already contains package references, append these four:

```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.5" />
<PackageReference Include="Microsoft.Extensions.Options" Version="9.0.5" />
```

- [ ] **Step 2: Restore packages and verify build**

```bash
dotnet restore src/Lycoris.Common/Lycoris.Common.csproj
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

Expected: Build succeeds (no code referencing new packages yet, so no changes needed).

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Lycoris.Common.csproj
git commit -m "feat: add Microsoft.Extensions packages for Snowflakes integration"
```

---

### Task 2: Create core options classes

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Options/SnowflakeOption.cs`
- Create: `src/Lycoris.Common/Snowflakes/Options/SnowflakeOptionBuilder.cs`
- Create: `src/Lycoris.Common/Snowflakes/Options/DistributedSnowflakeOption.cs`
- Create: `src/Lycoris.Common/Snowflakes/Options/DistributedSnowflakeOptionBuilder.cs`

- [ ] **Step 1: Create SnowflakeOption.cs**

```csharp
namespace Lycoris.Common.Snowflakes.Options;

public class SnowflakeOption
{
    /// <summary>
    /// 工作机器ID，默认从1开始，用于防止时钟回拨导致的Id重复
    /// </summary>
    public int? WorkId { get; set; } = 1;

    /// <summary>
    /// 工作机器id所占用的长度，最大10，默认10
    /// </summary>
    public int WorkIdLength { get; set; } = 10;

    /// <summary>
    /// 用于计算时间戳的开始时间，默认 UTC 2020-01-01
    /// </summary>
    public DateTime StartTimeStamp { get; set; } = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
```

- [ ] **Step 2: Create SnowflakeOptionBuilder.cs**

```csharp
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
```

- [ ] **Step 3: Create DistributedSnowflakeOption.cs**

```csharp
namespace Lycoris.Common.Snowflakes.Options;

public class DistributedSnowflakeOption : SnowflakeOption
{
    /// <summary>
    /// Redis分布式路由前缀
    /// </summary>
    public string? RedisPrefix { get; set; }

    /// <summary>
    /// 刷新存活状态的间隔时间，默认1小时
    /// </summary>
    public TimeSpan RefreshAliveInterval { get; set; } = TimeSpan.FromHours(1);

    internal DistributedSnowflakeType Type { get; set; }
}

public enum DistributedSnowflakeType
{
    AsService = 0,
    AsHelper = 1
}
```

- [ ] **Step 4: Create DistributedSnowflakeOptionBuilder.cs**

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Lycoris.Common.Snowflakes.Options;

public class DistributedSnowflakeOptionBuilder : DistributedSnowflakeOption
{
    internal readonly IServiceCollection Services;
    internal Type? RedisType;
    internal IDistributedSnowflakesRedis? RedisHelper;

    public DistributedSnowflakeOptionBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
```

- [ ] **Step 5: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 6: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Options/
git commit -m "feat: add Snowflakes options classes"
```

---

### Task 3: Create interface definitions

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/ISnowflakeMaker.cs`
- Create: `src/Lycoris.Common/Snowflakes/IDistributedSnowflakesRedis.cs`
- Create: `src/Lycoris.Common/Snowflakes/IDistributedSnowflakesSupport.cs`

- [ ] **Step 1: Create ISnowflakeMaker.cs**

```csharp
namespace Lycoris.Common.Snowflakes;

public interface ISnowflakeMaker
{
    long GetNextId();
    Task<long> GetNextIdAsync();
    long GetNextId(int? workId);
    Task<long> GetNextIdAsync(int? workId);
}
```

- [ ] **Step 2: Create IDistributedSnowflakesRedis.cs**

```csharp
namespace Lycoris.Common.Snowflakes;

public interface IDistributedSnowflakesRedis
{
    Task<long> IncrByAsync(string key, long value);
    Task<bool> ExpireAsync(string key, TimeSpan expire);
    Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers);
    Task<long> ZRemAsync<T>(string key, params T[] member);
    Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0L);
}
```

- [ ] **Step 3: Create IDistributedSnowflakesSupport.cs**

```csharp
namespace Lycoris.Common.Snowflakes;

public interface IDistributedSnowflakesSupport
{
    Task<int> GetNextWorkIdAsync();
    Task RefreshAliveAsync();
    Task RemoveNotAliveWorkNodeAsync();
}
```

- [ ] **Step 4: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 5: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/ISnowflakeMaker.cs src/Lycoris.Common/Snowflakes/IDistributedSnowflakesRedis.cs src/Lycoris.Common/Snowflakes/IDistributedSnowflakesSupport.cs
git commit -m "feat: add Snowflakes interface definitions"
```

---

### Task 4: Create SnowflakeUtils with bug fix

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Utils/SnowflakeUtils.cs`

- [ ] **Step 1: Write the implementation**

```csharp
namespace Lycoris.Common.Snowflakes.Utils;

internal static class SnowflakeUtils
{
    /// <summary>
    /// 计算从起始时间到当前的毫秒偏移量
    /// </summary>
    /// <param name="startTicks">起始时间的 Ticks</param>
    /// <param name="lastTimestamp">上一次的时间戳毫秒值，用于等待下一毫秒</param>
    /// <returns>毫秒级时间戳偏移量</returns>
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
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Utils/
git commit -m "feat: add fixed SnowflakeUtils (bugfix: correct recursive argument)"
```

---

### Task 5: Create shared SnowflakeIdGenerator core

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Core/SnowflakeIdGenerator.cs`

- [ ] **Step 1: Write the shared generator**

```csharp
using Lycoris.Common.Snowflakes.Options;
using Lycoris.Common.Snowflakes.Utils;

namespace Lycoris.Common.Snowflakes.Core;

/// <summary>
/// 雪花ID生成器核心算法，内部使用，被 Helper 和 Service 共享
/// </summary>
internal sealed class SnowflakeIdGenerator
{
    private readonly int _workIdLength;
    private readonly int _maxWorkId;     // inclusive max (fix: -1 from original)
    private readonly int _indexLength;
    private readonly int _maxIndex;      // inclusive max
    private readonly long _startTicks;

    private readonly object _locker = new();
    private long _lastTimestamp = -1L;
    private uint _lastIndex = 0;
    private int? _workId;

    private readonly Func<int>? _syncWorkIdRefresher;
    private readonly Func<Task<int>>? _asyncWorkIdRefresher;

    /// <summary>
    /// 单机构造函数
    /// </summary>
    public SnowflakeIdGenerator(SnowflakeOption option)
    {
        _workIdLength = option.WorkIdLength;
        _maxWorkId = (1 << _workIdLength) - 1;
        _indexLength = 22 - _workIdLength;
        _maxIndex = (1 << _indexLength) - 1;
        _startTicks = option.StartTimeStamp.Ticks;

        if (option.WorkId.HasValue)
            _workId = option.WorkId.Value;
    }

    /// <summary>
    /// 分布式构造函数，传入异步刷新 workId 的方法
    /// </summary>
    public SnowflakeIdGenerator(DistributedSnowflakeOption option, Func<Task<int>> asyncWorkIdRefresher)
    {
        _workIdLength = option.WorkIdLength;
        _maxWorkId = (1 << _workIdLength) - 1;
        _indexLength = 22 - _workIdLength;
        _maxIndex = (1 << _indexLength) - 1;
        _startTicks = option.StartTimeStamp.Ticks;
        _asyncWorkIdRefresher = asyncWorkIdRefresher;

        if (option.WorkId.HasValue)
            _workId = option.WorkId.Value;
    }

    /// <summary>
    /// 同步获取下一个Id
    /// </summary>
    public long Next(int? workId = null)
    {
        if (workId.HasValue)
            _workId = workId.Value;

        if (_workId > _maxWorkId)
            throw new ArgumentException($"workId value range is 0 - {_maxWorkId}");

        lock (_locker)
        {
            if (_workId == null)
                InitializeWorkIdSync();

            return GenerateCore();
        }
    }

    /// <summary>
    /// 异步获取下一个Id
    /// </summary>
    public async Task<long> NextAsync(int? workId = null)
    {
        if (workId.HasValue)
            _workId = workId.Value;

        if (_workId > _maxWorkId)
            throw new ArgumentException($"workId value range is 0 - {_maxWorkId}");

        if (_workId == null)
            await InitializeWorkIdAsync();

        lock (_locker)
        {
            return GenerateCore();
        }
    }

    /// <summary>
    /// 批量获取Id，单次加锁
    /// </summary>
    public long[] NextBatch(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "count must be greater than 0");

        var ids = new long[count];
        lock (_locker)
        {
            if (_workId == null)
                InitializeWorkIdSync();

            for (int i = 0; i < count; i++)
                ids[i] = GenerateCore();
        }
        return ids;
    }

    /// <summary>
    /// 解析雪花Id为组成部分
    /// </summary>
    public SnowflakeIdInfo Parse(long id)
    {
        var sequenceMask = (1L << _indexLength) - 1;
        var workIdMask = ((1L << _workIdLength) - 1) << _indexLength;

        var sequence = id & sequenceMask;
        var workId = (id & workIdMask) >> _indexLength;
        var timestamp = id >> (_indexLength + _workIdLength);
        var datetime = new DateTime(_startTicks + timestamp * 10000, DateTimeKind.Utc);

        return new SnowflakeIdInfo(datetime, (int)workId, (int)sequence);
    }

    public int WorkIdLength => _workIdLength;
    public int IndexLength => _indexLength;
    public DateTime StartTimestamp => new(_startTicks, DateTimeKind.Utc);

    private void InitializeWorkIdSync()
    {
        if (_asyncWorkIdRefresher != null)
            _workId = _asyncWorkIdRefresher().GetAwaiter().GetResult();
        else
            RefreshWorkIdLocal();
    }

    private async Task InitializeWorkIdAsync()
    {
        if (_asyncWorkIdRefresher != null)
            _workId = await _asyncWorkIdRefresher();
        else
            RefreshWorkIdLocal();
    }

    private void RefreshWorkIdLocal()
    {
        if (_workId == null)
            _workId = 0;
        else if (_workId < _maxWorkId)
            _workId++;
        // else: keep current (no more workIds available)
    }

    private long GenerateCore()
    {
        var currentTimeStamp = SnowflakeUtils.SnowflakeTimeStamp(_startTicks);

        if (_lastIndex > _maxIndex)
            currentTimeStamp = SnowflakeUtils.SnowflakeTimeStamp(_startTicks, _lastTimestamp);

        if (currentTimeStamp > _lastTimestamp)
        {
            _lastIndex = 0;
            _lastTimestamp = currentTimeStamp;
        }
        else if (currentTimeStamp < _lastTimestamp)
        {
            // 时钟回拨：刷新 workId
            if (_asyncWorkIdRefresher != null)
                _workId = _asyncWorkIdRefresher().GetAwaiter().GetResult();
            else
                RefreshWorkIdLocal();

            return GenerateCore();
        }

        if (_workId == null)
            throw new InvalidOperationException("WorkId is not initialized");

        long work = (long)_workId.Value << _indexLength;
        long time = currentTimeStamp << (_indexLength + _workIdLength);
        long id = time | work | _lastIndex;

        _lastIndex++;
        return id;
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Core/
git commit -m "feat: add SnowflakeIdGenerator shared core (bugfixes: ChangeWorkId, workId validation, timestamp recursive arg, batch generation)"
```

---

### Task 6: Create SnowflakeIdInfo struct

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/SnowflakeIdInfo.cs`

- [ ] **Step 1: Write the struct**

```csharp
namespace Lycoris.Common.Snowflakes;

/// <summary>
/// 雪花Id解析结果
/// </summary>
public readonly struct SnowflakeIdInfo
{
    /// <summary>
    /// ID生成时的时间戳
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// 工作机器ID
    /// </summary>
    public int WorkId { get; }

    /// <summary>
    /// 毫秒内序列号
    /// </summary>
    public int Sequence { get; }

    public SnowflakeIdInfo(DateTime timestamp, int workId, int sequence)
    {
        Timestamp = timestamp;
        WorkId = workId;
        Sequence = sequence;
    }

    public override string ToString() => $"Timestamp={Timestamp:O}, WorkId={WorkId}, Sequence={Sequence}";
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/SnowflakeIdInfo.cs
git commit -m "feat: add SnowflakeIdInfo struct for ID parsing"
```

---

### Task 7: Create SnowflakeHelper (static standalone)

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/SnowflakeHelper.cs`

- [ ] **Step 1: Write the helper**

```csharp
using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes;

public static class SnowflakeHelper
{
    private static SnowflakeIdGenerator? _generator;
    internal static bool HelperEnabled = false;

    internal static void Init(SnowflakeOption option)
    {
        _generator = new SnowflakeIdGenerator(option);
        HelperEnabled = true;
    }

    public static long GetNextId() => GetGenerator().Next();

    public static Task<long> GetNextIdAsync() => GetGenerator().NextAsync().AsTask();

    public static long GetNextId(int? workId) => GetGenerator().Next(workId);

    public static Task<long> GetNextIdAsync(int? workId) => GetGenerator().NextAsync(workId).AsTask();

    /// <summary>
    /// 批量获取雪花Id
    /// </summary>
    public static long[] GetNextIds(int count) => GetGenerator().NextBatch(count);

    /// <summary>
    /// 解析雪花Id为组成部分
    /// </summary>
    public static SnowflakeIdInfo Parse(long id) => GetGenerator().Parse(id);

    private static SnowflakeIdGenerator GetGenerator()
    {
        if (_generator == null)
            throw new InvalidOperationException("SnowflakeHelper is not initialized. Call AsHelper() during service registration.");
        return _generator;
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/SnowflakeHelper.cs
git commit -m "feat: add SnowflakeHelper static class"
```

---

### Task 8: Create SnowflakesMakerService (DI standalone)

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Impl/SnowflakesMakerService.cs`

- [ ] **Step 1: Write the service**

```csharp
using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes.Impl;

public sealed class SnowflakesMakerService : ISnowflakeMaker
{
    private readonly SnowflakeIdGenerator _generator;

    public SnowflakesMakerService(IOptions<SnowflakeOption> options)
    {
        _generator = new SnowflakeIdGenerator(options.Value);
    }

    public long GetNextId() => _generator.Next();

    public Task<long> GetNextIdAsync() => _generator.NextAsync().AsTask();

    public long GetNextId(int? workId) => _generator.Next(workId);

    public Task<long> GetNextIdAsync(int? workId) => _generator.NextAsync(workId).AsTask();
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Impl/SnowflakesMakerService.cs
git commit -m "feat: add SnowflakesMakerService DI service"
```

---

### Task 9: Create DistributedSnowflakesSupport

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesSupport.cs`

- [ ] **Step 1: Write the support class with bug fix**

```csharp
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes.Impl;

public sealed class DistributedSnowflakesSupport : IDistributedSnowflakesSupport
{
    private readonly IDistributedSnowflakesRedis _distributedRedis;
    private readonly string _currentWorkIndex;
    private readonly string _inUse;
    private int _workId;
    private readonly DistributedSnowflakeOption _option;

    public DistributedSnowflakesSupport(DistributedSnowflakeOption option, IDistributedSnowflakesRedis distributedRedis)
    {
        _option = option;
        _distributedRedis = distributedRedis;
        _currentWorkIndex = $"{_option.RedisPrefix}:CurrentWorkIndex";
        _inUse = $"{_option.RedisPrefix}:Use";
    }

    public async Task<int> GetNextWorkIdAsync()
    {
        var cache = await StringIncrementAsync(_currentWorkIndex);
        _workId = (int)cache - 1;

        if (_workId > (1 << _option.WorkIdLength) - 1)
        {
            var startScore = DateTime.Now.AddSeconds(-1800).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
            var endScore = DateTime.Now.AddMinutes(-5);
            var availableWorkIds = await SortedRangeByScoreAsync(_inUse, GetTimestamp(startScore), GetTimestamp(endScore), offset: 1);
            if (availableWorkIds.Count == 0)
                throw new Exception("no available work nodes");

            _workId = int.Parse(availableWorkIds.First().Key);
        }

        await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));
        return _workId;
    }

    public async Task RefreshAliveAsync()
    {
        await _distributedRedis.ZAddAsync(_inUse, (GetTimestamp(), _workId.ToString()));
    }

    public async Task RemoveNotAliveWorkNodeAsync()
    {
        var startScore = DateTime.Now.AddSeconds(-1801).AddSeconds(-(int)Math.Ceiling(_option.RefreshAliveInterval.TotalSeconds));
        var notAliveList = await SortedRangeByScoreAsync(_inUse, 0, GetTimestamp(startScore), count: 20);
        if (notAliveList is { Count: > 0 })
        {
            foreach (var item in notAliveList)
                await _distributedRedis.ZRemAsync(_inUse, item.Key);
        }
    }

    private static long GetTimestamp(DateTime? time = null)
    {
        time ??= DateTime.Now;
        return (time.Value.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000;
    }

    private async Task<long> StringIncrementAsync(string key, long value = 1, TimeSpan? timeSpan = null)
    {
        var cache = await _distributedRedis.IncrByAsync(key, value);
        if (timeSpan != null)
            await _distributedRedis.ExpireAsync(key, timeSpan.Value);
        return cache;
    }

    private async Task<Dictionary<string, decimal>> SortedRangeByScoreAsync(string key, decimal min, decimal max, long? count = null, long offset = 0)
    {
        var cache = await _distributedRedis.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);
        if (cache is not { Length: > 0 })
            return new Dictionary<string, decimal>();

        var dic = new Dictionary<string, decimal>();
        foreach (var (member, score) in cache)
            dic.Add(member, score);

        return dic.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesSupport.cs
git commit -m "feat: add DistributedSnowflakesSupport (bugfix: correct per-item removal in foreach)"
```

---

### Task 10: Create DistributedSnowflakeHelper (static distributed)

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/DistributedSnowflakeHelper.cs`

- [ ] **Step 1: Write the helper**

```csharp
using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;

namespace Lycoris.Common.Snowflakes;

public static class DistributedSnowflakeHelper
{
    private static SnowflakeIdGenerator? _generator;
    internal static IDistributedSnowflakesSupport? _distributedSupport;
    internal static bool HelperEnabled = false;

    internal static void Init(DistributedSnowflakeOption option, IDistributedSnowflakesSupport distributedSupport)
    {
        _distributedSupport = distributedSupport;
        _generator = new SnowflakeIdGenerator(option, async () => await _distributedSupport.GetNextWorkIdAsync());
        HelperEnabled = true;
    }

    public static long GetNextId() => GetGenerator().Next();
    public static Task<long> GetNextIdAsync() => GetGenerator().NextAsync().AsTask();
    public static long GetNextId(int? workId) => GetGenerator().Next(workId);
    public static Task<long> GetNextIdAsync(int? workId) => GetGenerator().NextAsync(workId).AsTask();
    public static long[] GetNextIds(int count) => GetGenerator().NextBatch(count);
    public static SnowflakeIdInfo Parse(long id) => GetGenerator().Parse(id);

    private static SnowflakeIdGenerator GetGenerator()
    {
        if (_generator == null)
            throw new InvalidOperationException("DistributedSnowflakeHelper is not initialized.");
        return _generator;
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/DistributedSnowflakeHelper.cs
git commit -m "feat: add DistributedSnowflakeHelper static class"
```

---

### Task 11: Create DistributedSnowflakeService (DI distributed)

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakeService.cs`

- [ ] **Step 1: Write the service**

```csharp
using Lycoris.Common.Snowflakes.Core;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes.Impl;

public class DistributedSnowflakeService : ISnowflakeMaker
{
    private readonly SnowflakeIdGenerator _generator;

    public DistributedSnowflakeService(IOptions<DistributedSnowflakeOption> options, IDistributedSnowflakesSupport distributedSupport)
    {
        _generator = new SnowflakeIdGenerator(options.Value, async () => await distributedSupport.GetNextWorkIdAsync());
    }

    public long GetNextId() => _generator.Next();
    public Task<long> GetNextIdAsync() => _generator.NextAsync().AsTask();
    public long GetNextId(int? workId) => _generator.Next(workId);
    public Task<long> GetNextIdAsync(int? workId) => _generator.NextAsync(workId).AsTask();
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakeService.cs
git commit -m "feat: add DistributedSnowflakeService DI service"
```

---

### Task 12: Create background service for distributed heartbeat

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesWorkBackgroundService.cs`

- [ ] **Step 1: Write the background service**

```csharp
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lycoris.Common.Snowflakes.Impl;

public class DistributedSnowflakesWorkBackgroundService : BackgroundService
{
    private readonly DistributedSnowflakeOption _option;
    private readonly IDistributedSnowflakesSupport? _distributedSupport;
    private readonly int _refreshAliveInterval;
    private readonly ILogger? _logger;

    public DistributedSnowflakesWorkBackgroundService(
        DistributedSnowflakeOption option,
        IDistributedSnowflakesSupport? distributedSupport,
        ILoggerFactory? factory)
    {
        _option = option;
        _distributedSupport = distributedSupport;
        _refreshAliveInterval = (int)Math.Ceiling(_option.RefreshAliveInterval.Add(TimeSpan.FromMinutes(1)).TotalMilliseconds);
        _logger = factory?.CreateLogger<DistributedSnowflakesWorkBackgroundService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var support = _option.Type == DistributedSnowflakeType.AsService
                    ? _distributedSupport
                    : DistributedSnowflakeHelper._distributedSupport;

                if (support != null)
                {
                    await support.RefreshAliveAsync();
                    await support.RemoveNotAliveWorkNodeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "refresh machine alive time failed");
            }

            await Task.Delay(_refreshAliveInterval, stoppingToken);
        }
    }
}
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesWorkBackgroundService.cs
git commit -m "feat: add distributed heartbeat background service"
```

---

### Task 13: Create DI registration extensions

**Files:**
- Create: `src/Lycoris.Common/Snowflakes/SnowflakesBuilderExtensions.cs`

- [ ] **Step 1: Write the extensions**

```csharp
using Lycoris.Common.Snowflakes.Impl;
using Lycoris.Common.Snowflakes.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lycoris.Common.Snowflakes;

public static class SnowflakesBuilderExtensions
{
    // === Standalone ===

    public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services)
        => new(services);

    public static SnowflakeOptionBuilder AddSnowflake(this IServiceCollection services, Action<SnowflakeOptionBuilder> configure)
    {
        var builder = new SnowflakeOptionBuilder(services);
        configure(builder);
        return builder;
    }

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

    public static IServiceCollection AsHelper(this SnowflakeOptionBuilder builder)
    {
        if (builder.Services.Any(s => s.ImplementationType == typeof(SnowflakesMakerService)))
            throw new InvalidOperationException("cannot register as a singleton service and a static instance at the same time");

        SnowflakeHelper.Init(builder);
        return builder.Services;
    }

    // === Distributed ===

    public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services)
        => new(services);

    public static DistributedSnowflakeOptionBuilder AddDistributedSnowflake(this IServiceCollection services, Action<DistributedSnowflakeOptionBuilder> configure)
    {
        var builder = new DistributedSnowflakeOptionBuilder(services);
        configure(builder);
        return builder;
    }

    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisService<T>(this DistributedSnowflakeOptionBuilder builder)
        where T : IDistributedSnowflakesRedis
    {
        builder.RedisType = typeof(T);
        return builder;
    }

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

    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisHelper<T>(this DistributedSnowflakeOptionBuilder builder)
        where T : IDistributedSnowflakesRedis, new()
    {
        builder.RedisHelper = new T();
        return builder;
    }

    public static DistributedSnowflakeOptionBuilder AddSnowflakesRedisHelper<T>(this DistributedSnowflakeOptionBuilder builder, T redisHelper)
        where T : IDistributedSnowflakesRedis
    {
        builder.RedisHelper = redisHelper;
        return builder;
    }

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
```

- [ ] **Step 2: Verify build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj --no-restore
```

- [ ] **Step 3: Commit**

```bash
git add src/Lycoris.Common/Snowflakes/SnowflakesBuilderExtensions.cs
git commit -m "feat: add Snowflakes DI registration extensions"
```

---

### Task 14: Final build verification

- [ ] **Step 1: Full build**

```bash
dotnet build src/Lycoris.Common/Lycoris.Common.csproj
```

Expected: Build succeeds with zero errors and zero warnings.

- [ ] **Step 2: Check all files are in correct locations**

```bash
find src/Lycoris.Common/Snowflakes -type f -name "*.cs" | sort
```

Expected output:
```
src/Lycoris.Common/Snowflakes/Core/SnowflakeIdGenerator.cs
src/Lycoris.Common/Snowflakes/DistributedSnowflakeHelper.cs
src/Lycoris.Common/Snowflakes/IDistributedSnowflakesRedis.cs
src/Lycoris.Common/Snowflakes/IDistributedSnowflakesSupport.cs
src/Lycoris.Common/Snowflakes/ISnowflakeMaker.cs
src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakeService.cs
src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesSupport.cs
src/Lycoris.Common/Snowflakes/Impl/DistributedSnowflakesWorkBackgroundService.cs
src/Lycoris.Common/Snowflakes/Impl/SnowflakesMakerService.cs
src/Lycoris.Common/Snowflakes/Options/DistributedSnowflakeOption.cs
src/Lycoris.Common/Snowflakes/Options/DistributedSnowflakeOptionBuilder.cs
src/Lycoris.Common/Snowflakes/Options/SnowflakeOption.cs
src/Lycoris.Common/Snowflakes/Options/SnowflakeOptionBuilder.cs
src/Lycoris.Common/Snowflakes/SnowflakeHelper.cs
src/Lycoris.Common/Snowflakes/SnowflakeIdInfo.cs
src/Lycoris.Common/Snowflakes/SnowflakesBuilderExtensions.cs
src/Lycoris.Common/Snowflakes/Utils/SnowflakeUtils.cs
```

- [ ] **Step 3: Commit** (if any final adjustments)

```bash
git status
```
