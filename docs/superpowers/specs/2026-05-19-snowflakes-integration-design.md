# Snowflakes Integration Design

## Goal
Integrate Snowflakes ID generation (from `E:\Github\Snowflakes`) into `Lycoris.Common` project, fixing bugs and adding convenience extensions.

## Directory Structure
```
src/Lycoris.Common/Snowflakes/
├── ISnowflakeMaker.cs
├── SnowflakeHelper.cs
├── DistributedSnowflakeHelper.cs
├── IDistributedSnowflakesRedis.cs
├── IDistributedSnowflakesSupport.cs
├── SnowflakesBuilderExtensions.cs
├── Core/
│   └── SnowflakeIdGenerator.cs          ← extracted shared algorithm
├── Impl/
│   ├── SnowflakesMakerService.cs
│   ├── DistributedSnowflakeService.cs
│   ├── DistributedSnowflakesSupport.cs
│   └── DistributedSnowflakesWorkBackgroundService.cs
├── Options/
│   ├── SnowflakeOption.cs
│   ├── SnowflakeOptionBuilder.cs
│   ├── DistributedSnowflakeOption.cs
│   └── DistributedSnowflakeOptionBuilder.cs
└── Utils/
    └── SnowflakeUtils.cs
```

## Core: SnowflakeIdGenerator
Internal class containing the ID generation algorithm. Handles timestamps, workId management, sequence within millisecond, and clock drift recovery. A single implementation shared by all four callers:
- `SnowflakeHelper` (static standalone)
- `SnowflakesMakerService` (DI standalone)
- `DistributedSnowflakeHelper` (static distributed)
- `DistributedSnowflakeService` (DI distributed)

## Bug Fixes
1. **ChangeWorkId comparison**: `>` → `<` so workId actually increments on each call
2. **SnowflakeUtils recursive call**: pass original `ticks` parameter, not millisecond value
3. **RemoveNotAliveWorkNodeAsync**: remove only current item per iteration, not all items

## Convenience Extensions
- `SnowflakeIdInfo` readonly struct (Timestamp, WorkId, Sequence fields)
- `ParseSnowflakeId(long id)` → `SnowflakeIdInfo`
- `GetNextIds(int count)` → `long[]` — batch generation, single lock acquisition

## NuGet Dependencies (added to Lycoris.Common.csproj)
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.5
- Microsoft.Extensions.Hosting.Abstractions 9.0.5
- Microsoft.Extensions.Logging.Abstractions 9.0.5
- Microsoft.Extensions.Options 9.0.5
