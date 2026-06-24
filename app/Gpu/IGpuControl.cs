namespace OHelper.Gpu;

public  interface IGpuControl : IDisposable {
    bool IsNvidia { get; }
    bool IsValid { get; }
    public string FullName { get; }
    bool SupportsGpuClockControl { get; }
    int MinGpuCoreOffset { get; }
    int MaxGpuCoreOffset { get; }
    int MinGpuMemoryOffset { get; }
    int MaxGpuMemoryOffset { get; }
    int MinGpuClockLimit { get; }
    int MaxGpuClockLimit { get; }
    int? GetCurrentTemperature();
    int? GetGpuUse();
    (long usedMb, long totalMb)? GetVramInfo();
    float? GetGpuPower();
    bool GetGpuClockOffsets(out int core, out int memory);
    int GetMaxGpuClock();
    int SetGpuClockOffsets(int core, int memory);
    int SetMaxGpuClock(int clock);
    void KillGPUApps();

}
