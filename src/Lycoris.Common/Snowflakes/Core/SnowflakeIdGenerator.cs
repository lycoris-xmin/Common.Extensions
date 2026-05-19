using Lycoris.Common.Snowflakes.Options;
using Lycoris.Common.Snowflakes.Utils;

namespace Lycoris.Common.Snowflakes.Core;

internal sealed class SnowflakeIdGenerator
{
    private readonly int _workIdLength;
    private readonly int _maxWorkId;
    private readonly int _indexLength;
    private readonly int _maxIndex;
    private readonly long _startTicks;

    private readonly object _locker = new();
    private long _lastTimestamp = -1L;
    private uint _lastIndex = 0;
    private int? _workId;

    private readonly Func<Task<int>>? _asyncWorkIdRefresher;

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
