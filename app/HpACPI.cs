using OHelper;
using System.Management;
using System.Runtime.InteropServices;

public enum HpFan
{
    CPU = 0,
    GPU = 1,
    Mid = 2,
    XGM = 3
}

public enum HpMode
{
    Balanced = 0,
    Turbo = 1,
    Silent = 2,
    Manual = 4,
    Unleashed = 4
}

public enum HpGPU
{
    Eco = 0,
    Standard = 1,
    Ultimate = 2
}

public enum HpBiosCommand : uint
{
    Default = 0x20008,
    Keyboard = 0x20009,
    Legacy = 0x00001,
    GpuMode = 0x00002,
}

public enum HpBiosCommandType : int
{
    FanGetCount = 0x10,
    PerformanceMode = 0x1A,
    FanSetLevel = 0x2E,
    FanGetLevel = 0x2D,
    FanGetLevelV2 = 0x37,
    FanGetRpm = 0x38,
    FanMaxGet = 0x26,
    FanMaxSet = 0x27,
    SystemGetData = 0x28,
    GpuGetPower = 0x21,
    GpuSetPower = 0x22,
    TempGet = 0x23,
    BatteryCare = 0x24,
    OverdriveGet = 0x35,
    OverdriveSet = 0x36,
    MaxFanRead = 0x38,
    MaxFanWrite = 0x39,
    SystemDesignData = 0x40,
    Tpptdp = 0x41,
    StatusRead = 0x45,
    StatusWrite = 0x46,
    GpuModeGet = 0x52,
    GpuModeSet = 0x52,
    IdleSet = 0x31,
}

public class HpACPI
{
    const uint CONTROL_CODE = 0x0022240C;

    const uint DSTS = 0x53545344;
    const uint DEVS = 0x53564544;
    const uint INIT = 0x54494E49;
    const uint WDOG = 0x474F4457;

    public const uint UniversalControl = 0x00100021;

    public const int Airplane = 0x88;
    public const int KB_Light_Up = 0xc4;
    public const int KB_Light_Down = 0xc5;
    public const int Brightness_Down = 0x10;
    public const int Brightness_Up = 0x20;
    public const int KB_Sleep = 0x6c;

    public const int KB_TouchpadToggle = 0x6b;
    public const int KB_MuteToggle = 0x7c;
    public const int KB_FNlockToggle = 0x4e;

    public const int KB_DUO_PgUpDn = 0x4B;
    public const int KB_DUO_SecondDisplay = 0x6A;

    public const int Touchpad_Toggle = 0x6B;

    public const int ChargerMode = 0x0012006C;

    public const int ChargerUSB = 2;
    public const int ChargerBarrel = 1;

    public const uint CPU_Fan = 0x00110013;
    public const uint GPU_Fan = 0x00110014;
    public const uint Mid_Fan = 0x00110031;

    public const uint BatteryDischarge = 0x0012005A;

    public const uint StatusMode = 0x00090031;
    public const uint PowerSavingMode = 0x00090032;

    public const uint PerformanceMode = 0x00120075;

    public const uint GPUEcoROG = 0x00090020;
    public const uint GPUEcoVivo = 0x00090120;

    public const uint GPUXGConnected = 0x00090018;
    public const uint GPUXG = 0x00090019;

    public const uint GPUMuxROG = 0x00090016;
    public const uint GPUMuxVivo = 0x00090026;

    public const uint BatteryLimit = 0x00120057;

    public const uint ScreenOverdrive = 0x00050019;
    public const uint ScreenMiniled1 = 0x0005001E;
    public const uint ScreenMiniled2 = 0x0005002E;
    public const uint ScreenFHD = 0x0005001C;
    public const uint ScreenHDRControl = 0x00050071;

    public const uint ScreenOptimalBrightness = 0x0005002A;
    public const uint ScreenInit = 0x00050011;

    public const uint DevsCPUFan = 0x00110022;
    public const uint DevsGPUFan = 0x00110023;

    public const uint DevsCPUFanCurve = 0x00110024;
    public const uint DevsGPUFanCurve = 0x00110025;
    public const uint DevsMidFanCurve = 0x00110032;

    public const uint FanHysteresis = 0x00110034;
    public const int Temp_CPU = 0x00120094;
    public const int Temp_GPU = 0x00120097;

    public const int PPT_APUA0 = 0x001200A0;
    public const int PPT_EDCA1 = 0x001200A1;
    public const int PPT_TDCA2 = 0x001200A2;
    public const int PPT_APUA3 = 0x001200A3;

    public const int PPT_CPUB0 = 0x001200B0;
    public const int PPT_CPUB1 = 0x001200B1;

    public const int PPT_GPUC0 = 0x001200C0;
    public const int PPT_APUC1 = 0x001200C1;
    public const int PPT_GPUC2 = 0x001200C2;

    public const uint CORES_CPU = 0x001200D2;
    public const uint CORES_MAX = 0x001200D3;

    public const uint GPU_BASE = 0x00120099;
    public const uint GPU_POWER = 0x00120098;

    public const int APU_MEM = 0x000600C1;

    public const int MicMuteLed = 0x00040017;
    public const int SoundMuteLed = 0x0004001C;

    public const int SlateMode = 0x00120063;
    public const int TabletState = 0x00060077;
    public const int TentState = 0x00060062;
    public const int FnLock = 0x00100023;

    public const int ScreenPadToggle = 0x00050031;
    public const int ScreenPadBrightness = 0x00050032;

    public const int CameraShutter = 0x00060078;
    public const int CameraLed = 0x00060079;
    public const int StatusLed = 0x000600C2;

    public const int BootSound = 0x00130022;

    public const int Tablet_Notebook = 0;
    public const int Tablet_Tablet = 1;
    public const int Tablet_Tent = 2;
    public const int Tablet_Rotated = 3;

    public const int PerformanceBalanced = 0;
    public const int PerformanceTurbo = 1;
    public const int PerformanceSilent = 2;
    public const int PerformanceManual = 4;

    public const int GPUModeEco = 0;
    public const int GPUModeStandard = 1;
    public const int GPUModeUltimate = 2;

    public const int WmiGpuModeHybrid = 0;
    public const int WmiGpuModeDiscrete = 1;
    public const int WmiGpuModeUma = 3;

    public const int MinTotal = 5;

    public static int MaxTotal = 150;
    public static int DefaultTotal = 80;

    public const int MinCPU = 5;
    public static int MaxCPU = 100;
    public const int DefaultCPU = 80;

    public const int MinGPUBoost = 5;
    public static int MaxGPUBoost = 25;

    public static int MinGPUPower = 0;
    public static int MaxGPUPower = 70;

    public const int MinGPUTemp = 75;
    public const int MaxGPUTemp = 87;

    public const int PCoreMin = 4;
    public const int ECoreMin = 0;

    public const int PCoreMax = 16;
    public const int ECoreMax = 16;

    private readonly Dictionary<uint, bool> _supportCache = new();

    public static uint GPUEco => AppConfig.IsVivoZenPro() ? GPUEcoVivo : GPUEcoROG;
    public static uint GPUMux => AppConfig.IsVivoZenPro() ? GPUMuxVivo : GPUMuxROG;

    public bool SupportsGpuModeSwitching()
    {
        var sdd = GetSystemDesignData();
        if (sdd.ReadSucceeded && sdd.SupportsGraphicsSwitching)
            return true;

        var modelCaps = AppConfig.GetModelCapabilities();
        if (modelCaps.HasMuxSwitch)
            return true;

        if (!sdd.ReadSucceeded && ProbeSupport(GPUEco))
        {
            Logger.WriteLine("HpACPI: GPU mode switching enabled via WMI probe (SystemDesignData unavailable)");
            return true;
        }

        return false;
    }

    public bool SupportsGpuMode(int mode)
    {
        if (!SupportsGpuModeSwitching()) return false;

        var sdd = GetSystemDesignData();
        if (sdd.ReadSucceeded)
        {
            return mode switch
            {
                GPUModeEco => sdd.HasIntegratedSlot,
                GPUModeStandard => sdd.HasHybridSlot,
                GPUModeUltimate => sdd.HasDedicatedSlot,
                _ => false
            };
        }

        return mode switch
        {
            GPUModeEco => true,
            GPUModeStandard => true,
            GPUModeUltimate => false,
            _ => false
        };
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        byte[] lpInBuffer,
        uint nInBufferSize,
        byte[] lpOutBuffer,
        uint nOutBufferSize,
        ref uint lpBytesReturned,
        IntPtr lpOverlapped
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private const uint GENERIC_READ = 0x80000000;
    private const uint GENERIC_WRITE = 0x40000000;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
    private const uint FILE_SHARE_READ = 1;
    private const uint FILE_SHARE_WRITE = 2;

    private IntPtr handle;
    private IntPtr eventHandle;
    private bool _connected = false;

    #region WMI BIOS Interface

    private const string WMI_NAMESPACE = @"\\.\root\wmi";
    private const string WMI_DATA_CLASS = "hpqBDataIn";
    private const string WMI_METHODS_CLASS = "hpqBIntM";
    private const string WMI_INSTANCE_NAME = @"ACPI\PNP0C14\0_0";
    private static readonly byte[] WMI_SIGN = { 0x53, 0x45, 0x43, 0x55 };

    private ManagementScope _wmiScope;
    private ManagementClass _wmiDataClass;
    private ManagementObject _wmiMethodsObject;
    private bool _wmiInitialized;
    private bool _wmiDisabled;
    private bool _useLegacyWmi;
    private int _consecutiveFailures;
    private const int MAX_CONSECUTIVE_FAILURES = 5;
    private DateTime _lastErrorLog = DateTime.MinValue;
    private static readonly TimeSpan ERROR_LOG_INTERVAL = TimeSpan.FromSeconds(30);

    private System.Timers.Timer _heartbeatTimer;
    private const int HEARTBEAT_INTERVAL_MS = 60000;

    // Thermal policy version from SystemGetData byte[3]
    // V1 = krpm scale (0-55), V2 = percentage scale (0-100)
    private byte _thermalPolicyVersion = 1;
    public int ThermalPolicyVersion => _thermalPolicyVersion;
    public int MaxFanLevel => _thermalPolicyVersion >= 2 ? 100 : 55;

    #endregion

    public void RunListener()
    {
        Logger.WriteLine("ACPI listener stub - no hardware");
    }

    public bool IsConnected()
    {
        return _connected;
    }

    public bool IsWmiReady() => _wmiInitialized && !_wmiDisabled;

    public HpACPI()
    {
        _connected = true;
        try
        {
            InitializeWmi();
            StartHeartbeat();
            Logger.WriteLine("HpACPI: WMI BIOS interface initialized");

            var probe = ExecuteBiosCommand((uint)HpBiosCommand.Default, (int)HpBiosCommandType.SystemGetData, null, 128);
            Logger.WriteLine($"HpACPI: WMI probe SystemGetData: success={probe.Success} rc={probe.ReturnCode} len={probe.Data.Length}");
            if (probe.Success && probe.Data.Length > 3)
            {
                _thermalPolicyVersion = probe.Data[3];
                Logger.WriteLine($"HpACPI: ThermalPolicy V{_thermalPolicyVersion} (MaxFanLevel={MaxFanLevel})");
            }

            DetectCapabilities();
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HpACPI: WMI init failed, running without hardware: {ex.Message}");
        }
    }

    #region WMI Core

    private void InitializeWmi()
    {
        if (_wmiInitialized) return;

        try
        {
            _wmiScope = new ManagementScope(WMI_NAMESPACE, new ConnectionOptions
            {
                EnablePrivileges = true,
                Impersonation = ImpersonationLevel.Impersonate
            });
            _wmiScope.Connect();

            _wmiDataClass = new ManagementClass(_wmiScope, new ManagementPath(WMI_DATA_CLASS), null);

            using (var instances = new ManagementClass(_wmiScope, new ManagementPath(WMI_METHODS_CLASS), null).GetInstances())
            {
                foreach (ManagementObject instance in instances)
                {
                    string instanceName = Convert.ToString(instance["InstanceName"]);
                    if (string.Equals(instanceName, WMI_INSTANCE_NAME, StringComparison.OrdinalIgnoreCase))
                    {
                        _wmiMethodsObject = instance;
                        break;
                    }
                    instance.Dispose();
                }
            }

            if (_wmiMethodsObject == null)
            {
                Logger.WriteLine("HpACPI: hpqBIntM instance not found");
                return;
            }

            _wmiInitialized = true;
            _wmiDisabled = false;
            _consecutiveFailures = 0;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HpACPI: WMI connection failed: {ex.Message}");
        }
    }

    private void InitializeLegacyWmi()
    {
        if (_useLegacyWmi) return;

        try
        {
            var scope = new ManagementScope(@"\\.\root\wmi", new ConnectionOptions
            {
                EnablePrivileges = true,
                Impersonation = ImpersonationLevel.Impersonate
            });
            scope.Connect();

            var dataClass = new ManagementClass(scope, new ManagementPath(WMI_DATA_CLASS), null);

            using (var instances = new ManagementClass(scope, new ManagementPath(WMI_METHODS_CLASS), null).GetInstances())
            {
                foreach (ManagementObject instance in instances)
                {
                    string instanceName = Convert.ToString(instance["InstanceName"]);
                    if (string.Equals(instanceName, WMI_INSTANCE_NAME, StringComparison.OrdinalIgnoreCase))
                    {
                        _wmiMethodsObject?.Dispose();
                        _wmiMethodsObject = instance;
                        break;
                    }
                    instance.Dispose();
                }
            }

            if (_wmiMethodsObject != null)
            {
                _wmiDataClass?.Dispose();
                _wmiDataClass = dataClass;
                _wmiScope = scope;
                _useLegacyWmi = true;
                Logger.WriteLine("HpACPI: Switched to legacy WMI fallback");
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HpACPI: Legacy WMI fallback also failed: {ex.Message}");
        }
    }

    public WmiBiosResult ExecuteBiosCommand(uint command, int commandType, byte[] inputData, int returnDataSize)
    {
        if (_wmiDisabled)
            return WmiBiosResult.Failure(returnDataSize);

        if (!_wmiInitialized)
        {
            InitializeWmi();
            if (!_wmiInitialized)
                return WmiBiosResult.Failure(returnDataSize);
        }

        lock (this)
        {
            try
            {
                using (ManagementObject input = _wmiDataClass.CreateInstance())
                {
                    input["Sign"] = WMI_SIGN;
                    input["Command"] = (uint)command;
                    input["CommandType"] = (uint)commandType;
                    input["Size"] = (uint)(inputData?.Length ?? 0);
                    input["hpqBData"] = inputData ?? Array.Empty<byte>();

                    string methodName = GetWmiMethodName(returnDataSize);
                    ManagementBaseObject inParams = _wmiMethodsObject.GetMethodParameters(methodName);
                    inParams["InData"] = input;

                    ManagementBaseObject outParams = _wmiMethodsObject.InvokeMethod(methodName, inParams, null);
                    ManagementBaseObject outData = outParams?["OutData"] as ManagementBaseObject;

                    if (outData == null)
                    {
                        OnCommandFailure("Missing OutData");
                        return WmiBiosResult.Failure(returnDataSize);
                    }

                    int returnCode = Convert.ToInt32(outData["rwReturnCode"]);
                    byte[] returnData = CopyReturnData(outData["Data"] as byte[], returnDataSize);

                    OnCommandSuccess();
                    return new WmiBiosResult(true, returnCode, returnData);
                }
            }
            catch (Exception ex)
            {
                if (!_useLegacyWmi)
                {
                    InitializeLegacyWmi();
                    if (_useLegacyWmi)
                        return ExecuteBiosCommand(command, commandType, inputData, returnDataSize);
                }
                OnCommandFailure(ex.Message);
                return WmiBiosResult.Failure(returnDataSize);
            }
        }
    }

    private void OnCommandSuccess()
    {
        _consecutiveFailures = 0;
        if (_wmiDisabled)
        {
            _wmiDisabled = false;
            Logger.WriteLine("HpACPI: WMI re-enabled after successful command");
        }
    }

    private void OnCommandFailure(string reason)
    {
        _consecutiveFailures++;
        if (_consecutiveFailures >= MAX_CONSECUTIVE_FAILURES && !_wmiDisabled)
        {
            _wmiDisabled = true;
            Logger.WriteLine($"HpACPI: WMI disabled after {MAX_CONSECUTIVE_FAILURES} consecutive transport failures");
            return;
        }

        if (DateTime.Now - _lastErrorLog > ERROR_LOG_INTERVAL)
        {
            _lastErrorLog = DateTime.Now;
            Logger.WriteLine($"HpACPI: WMI transport error ({_consecutiveFailures}/{MAX_CONSECUTIVE_FAILURES}): {reason}");
        }
    }

    private static string GetWmiMethodName(int returnDataSize)
    {
        if (returnDataSize <= 0) return "hpqBIOSInt0";
        if (returnDataSize <= 4) return "hpqBIOSInt4";
        if (returnDataSize <= 128) return "hpqBIOSInt128";
        if (returnDataSize <= 1024) return "hpqBIOSInt1024";
        return "hpqBIOSInt4096";
    }

    private static byte[] CopyReturnData(byte[] source, int returnDataSize)
    {
        if (returnDataSize <= 0) return Array.Empty<byte>();
        byte[] result = new byte[returnDataSize];
        if (source == null || source.Length == 0) return result;
        Array.Copy(source, result, Math.Min(source.Length, result.Length));
        return result;
    }

    #endregion

    #region Heartbeat

    private void StartHeartbeat()
    {
        if (!_wmiInitialized) return;
        if (_heartbeatTimer != null) return;

        _heartbeatTimer = new System.Timers.Timer(HEARTBEAT_INTERVAL_MS)
        {
            AutoReset = true,
            Enabled = false
        };
        _heartbeatTimer.Elapsed += (s, e) =>
        {
            try
            {
                var result = ExecuteBiosCommand(
                    (uint)HpBiosCommand.Default,
                    (int)HpBiosCommandType.SystemGetData,
                    null,
                    128);
                Logger.WriteLine($"HpACPI heartbeat: success={result.Success} rc={result.ReturnCode}");
            }
            catch { }
        };
        _heartbeatTimer.Start();
    }

    #endregion

    #region DeviceSet/DeviceGet — WMI-mapped implementations

    public void Control(uint dwIoControlCode, byte[] lpInBuffer, byte[] lpOutBuffer)
    {
    }

    public void Close()
    {
        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;

        _wmiInitialized = false;
        try { _wmiMethodsObject?.Dispose(); } catch { }
        try { _wmiDataClass?.Dispose(); } catch { }
        _wmiMethodsObject = null;
        _wmiDataClass = null;
        _wmiScope = null;
    }

    protected byte[] CallMethod(uint MethodID, byte[] args)
    {
        return new byte[16];
    }

    public byte[] DeviceInit()
    {
        return new byte[16];
    }

    public int DeviceSet(uint DeviceID, int Status, string? logName)
    {
        if (logName != null)
            Logger.WriteLine($"HpACPI DeviceSet: {logName} (ID=0x{DeviceID:X}, Status={Status})");

        if (DeviceID == PerformanceMode)
            return SetPerformanceMode(Status);

        if (DeviceID == BatteryLimit)
            return SetBatteryCare(Status != 0);

        if (DeviceID == ScreenOverdrive)
            return SetOverdrive(Status != 0);

        if (DeviceID == GPUEco || DeviceID == GPUEcoROG || DeviceID == GPUEcoVivo
            || DeviceID == GPUMux || DeviceID == GPUMuxROG || DeviceID == GPUMuxVivo)
            return SetGpuModeValue(Status);

        if (DeviceID == GPUXG)
            return SetGpuXg(Status);

        if (DeviceID == StatusMode)
            return 1;

        if (DeviceID == UniversalControl)
            return 1;

        if (DeviceID == MicMuteLed || DeviceID == SoundMuteLed)
            return 1;

        if (DeviceID == ScreenMiniled1 || DeviceID == ScreenMiniled2 || DeviceID == ScreenFHD
            || DeviceID == ScreenHDRControl || DeviceID == ScreenOptimalBrightness)
            return 1;

        if (DeviceID == PPT_APUA0 || DeviceID == PPT_APUA3 || DeviceID == PPT_APUC1
            || DeviceID == PPT_CPUB0 || DeviceID == PPT_CPUB1)
            return SetPowerLimit(DeviceID, Status);

        if (DeviceID == PPT_GPUC0 || DeviceID == PPT_GPUC2)
            return SetGpuPowerLimit(DeviceID, Status);

        if (DeviceID == GPU_POWER || DeviceID == GPU_BASE)
            return SetGpuPowerLimit(DeviceID, Status);

        if (DeviceID == BootSound)
            return 1;

        if (DeviceID == CameraShutter || DeviceID == StatusLed
            || DeviceID == ScreenPadToggle || DeviceID == ScreenPadBrightness || DeviceID == ScreenInit)
            return 1;

        Logger.WriteLine($"HpACPI: Unmapped DeviceSet 0x{DeviceID:X} = {Status}");
        return 1;
    }

    public int DeviceSet(uint DeviceID, byte[] Params, string? logName)
    {
        if (logName != null)
            Logger.WriteLine($"HpACPI DeviceSet(buf): {logName} (ID=0x{DeviceID:X}, Len={Params?.Length ?? 0})");

        if (DeviceID == CPU_Fan || DeviceID == GPU_Fan || DeviceID == Mid_Fan)
            return 1;

        if (DeviceID == DevsCPUFanCurve || DeviceID == DevsGPUFanCurve || DeviceID == DevsMidFanCurve)
        {
            if (Params != null && Params.Length >= 2)
                return SetFanLevel(Params[0], Params[1]);
            return 1;
        }

        if (DeviceID == StatusMode)
        {
            if (Params != null && Params.Length >= 2)
            {
                int modeByte = Params[1];
                var result = ExecuteBiosCommand((uint)HpBiosCommand.Default, (int)HpBiosCommandType.PerformanceMode,
                    new byte[] { 0xFF, (byte)modeByte, 0x01, 0x00 }, 4);
                return result.Success && result.ReturnCode == 0 ? 1 : 0;
            }
            return 1;
        }

        if (DeviceID == FanHysteresis)
            return 1;

        Logger.WriteLine($"HpACPI: Unmapped DeviceSet(buf) 0x{DeviceID:X}");
        return 1;
    }

    public int DeviceGet(uint DeviceID)
    {
        if (DeviceID == ChargerMode)
            return GetChargerMode();

        if (DeviceID == ScreenOverdrive)
            return GetOverdrive() ? 1 : 0;

        if (DeviceID == GPUEco || DeviceID == GPUEcoROG || DeviceID == GPUEcoVivo
            || DeviceID == GPUMux || DeviceID == GPUMuxROG || DeviceID == GPUMuxVivo)
            return GetGpuMode();

        if (DeviceID == GPUXG)
#pragma warning disable CS0618
            return IsXGConnected() ? 1 : 0;
#pragma warning restore CS0618

        if (DeviceID == Temp_CPU)
            return GetCpuTemp();

        if (DeviceID == Temp_GPU)
            return GetGpuTemp();

        if (DeviceID == GPU_BASE)
            return GetGpuBasePower();

        if (DeviceID == ScreenMiniled1 || DeviceID == ScreenMiniled2)
            return 0;

        if (DeviceID == ScreenFHD || DeviceID == ScreenHDRControl || DeviceID == ScreenOptimalBrightness)
            return 0;

        if (DeviceID == BootSound)
            return 0;

        if (DeviceID == FnLock)
            return 0;

        if (DeviceID == SlateMode || DeviceID == TabletState || DeviceID == TentState)
            return Tablet_Notebook;

        if (DeviceID == CameraShutter || DeviceID == CameraLed || DeviceID == StatusLed)
            return 0;

        Logger.WriteLine($"HpACPI: Unmapped DeviceGet 0x{DeviceID:X}");
        return -1;
    }

    public byte[] DeviceGetBuffer(uint DeviceID, uint Status = 0)
    {
        if (DeviceID == DevsCPUFanCurve || DeviceID == DevsGPUFanCurve || DeviceID == DevsMidFanCurve)
            return GetFanCurve(DeviceID == DevsCPUFanCurve ? HpFan.CPU : DeviceID == DevsGPUFanCurve ? HpFan.GPU : HpFan.Mid);

        return new byte[16];
    }

    #endregion

    #region Performance Mode

    private int SetPerformanceMode(int modeValue)
    {
        byte modeByte;
        switch (modeValue)
        {
            case PerformanceBalanced:
                modeByte = 0x30;
                break;
            case PerformanceTurbo:
                modeByte = 0x31;
                break;
            case PerformanceSilent:
                modeByte = 0x50;
                break;
            case PerformanceManual:
                modeByte = 0x04;
                break;
            default:
                modeByte = (byte)modeValue;
                break;
        }

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.PerformanceMode,
            new byte[] { 0xFF, modeByte, 0x01, 0x00 },
            4);

        Logger.WriteLine($"HpACPI SetPerformanceMode: mode={modeValue} byte=0x{modeByte:X2} success={result.Success} rc={result.ReturnCode}");

        if (!result.Success || result.ReturnCode != 0)
        {
            var retry = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.PerformanceMode,
                new byte[] { 0xFF, modeByte, 0x00, 0x00 },
                0);
            Logger.WriteLine($"HpACPI SetPerformanceMode retry: success={retry.Success} rc={retry.ReturnCode}");
            return retry.Success && retry.ReturnCode == 0 ? 1 : 0;
        }

        return 1;
    }

    #endregion

    #region Battery

    public decimal? GetBatteryDischarge()
    {
        return null;
    }

    private int SetBatteryCare(bool enabled)
    {
        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.BatteryCare,
            new byte[] { enabled ? (byte)1 : (byte)0, 0, 0, 0 },
            4);
        Logger.WriteLine($"HpACPI SetBatteryCare: enabled={enabled} success={result.Success} rc={result.ReturnCode}");
        return result.Success && result.ReturnCode == 0 ? 1 : 0;
    }

    private int GetChargerMode()
    {
        if (!IsWmiReady()) return ChargerBarrel;

        try
        {
            var designData = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.SystemGetData,
                null,
                128);

            if (designData.Success && designData.ReturnCode == 0 && designData.Data.Length >= 8)
            {
                return ChargerBarrel;
            }
        }
        catch { }

        return ChargerBarrel;
    }

    #endregion

    #region Display

    private bool _overdriveSupportedCached = false;
    private bool _overdriveSupportedValue = false;

    public bool IsOverdriveSupported()
    {
        if (_overdriveSupportedCached) return _overdriveSupportedValue;
        if (!IsWmiReady()) return false;

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.OverdriveGet,
                null,
                4);

            if (result.Success && result.ReturnCode == 0)
            {
                _overdriveSupportedValue = true;
            }
            else
            {
                _overdriveSupportedValue = false;
            }
        }
        catch
        {
            _overdriveSupportedValue = false;
        }

        _overdriveSupportedCached = true;
        return _overdriveSupportedValue;
    }

    private bool GetOverdrive()
    {
        if (!IsWmiReady()) return false;

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.OverdriveGet,
            null,
            4);

        if (result.Success && result.ReturnCode == 0 && result.Data.Length > 0)
            return result.Data[0] != 0;

        return false;
    }

    private int SetOverdrive(bool enabled)
    {
        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.OverdriveSet,
            new byte[] { enabled ? (byte)1 : (byte)0, 0, 0, 0 },
            4);
        return result.Success && result.ReturnCode == 0 ? 1 : 0;
    }

    #endregion

    #region GPU Mode

    public int GetGpuMode()
    {
        if (!IsWmiReady()) return GPUModeStandard;

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Legacy,
            (int)HpBiosCommandType.GpuModeGet,
            null,
            4);

        if (result.Success && result.ReturnCode == 0 && result.Data.Length > 0)
        {
            int wmiMode = result.Data[0];
            int mode = WmiModeToAppMode(wmiMode);
            Logger.WriteLine($"HpACPI: GPU mode WMI={wmiMode} -> app={mode}");
            return mode;
        }

        int fallback = DetectGpuModeFromVideoControllers();
        if (fallback >= 0)
        {
            Logger.WriteLine($"HpACPI: GPU mode WMI failed, fallback video controller detection -> {fallback}");
            return fallback;
        }

        return GPUModeStandard;
    }

    private int WmiModeToAppMode(int wmiMode)
    {
        return wmiMode switch
        {
            WmiGpuModeHybrid => GPUModeStandard,
            WmiGpuModeDiscrete => GPUModeUltimate,
            WmiGpuModeUma => GPUModeEco,
            _ => GPUModeStandard
        };
    }

    private int AppModeToWmiMode(int appMode)
    {
        return appMode switch
        {
            GPUModeStandard => WmiGpuModeHybrid,
            GPUModeUltimate => WmiGpuModeDiscrete,
            GPUModeEco => WmiGpuModeUma,
            _ => WmiGpuModeHybrid
        };
    }

    private int DetectGpuModeFromVideoControllers()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            bool hasDgpu = false;
            bool hasIgpu = false;

            foreach (var obj in searcher.Get())
            {
                string name = obj["Name"]?.ToString() ?? "";
                int status = Convert.ToInt32(obj["Status"] ?? 0);
                if (status != 0) continue;

                if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase) ||
                    (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) && !name.Contains("Radeon Graphics", StringComparison.OrdinalIgnoreCase)))
                    hasDgpu = true;
                else
                    hasIgpu = true;
            }

            if (hasDgpu && hasIgpu) return GPUModeStandard;
            if (hasDgpu && !hasIgpu) return GPUModeUltimate;
            if (hasIgpu && !hasDgpu) return GPUModeEco;
        }
        catch { }

        return -1;
    }

    public int SetGpuModeValue(int appMode)
    {
        if (!SupportsGpuMode(appMode))
        {
            Logger.WriteLine($"HpACPI: GPU mode {appMode} not supported on this model");
            return 0;
        }

        int wmiMode = AppModeToWmiMode(appMode);
        Logger.WriteLine($"HpACPI: Setting GPU mode app={appMode} wmi={wmiMode}");

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.GpuMode,
            (int)HpBiosCommandType.GpuModeSet,
            new byte[] { (byte)wmiMode, 0, 0, 0 },
            4);

        bool success = result.Success && result.ReturnCode == 0;
        Logger.WriteLine($"HpACPI: GPU mode set result: success={success} rc={result.ReturnCode}");
        return success ? 1 : 0;
    }

    private int SetGpuXg(int status)
    {
        return 1;
    }

    #endregion

    #region Fan Control

    // Countdown timer to extend WMI fan settings lifetime to prevent BIOS reversion
    private static System.Timers.Timer? _fanSettingsTimer;
    private static int _fanSettingsCountdown = 0;

    private static void StartFanSettingsTimer()
    {
        if (_fanSettingsTimer == null)
        {
            _fanSettingsTimer = new System.Timers.Timer(1000);
            _fanSettingsTimer.Elapsed += (_, _) =>
            {
                _fanSettingsCountdown--;
                if (_fanSettingsCountdown <= 0)
                {
                    _fanSettingsTimer?.Stop();
                }
            };
        }
        _fanSettingsCountdown = 5; // 5-second countdown as specified in issue
        _fanSettingsTimer.Start();
    }

    private static void ResetFanSettingsTimer()
    {
        _fanSettingsCountdown = 5;
        if (_fanSettingsTimer != null && !_fanSettingsTimer.Enabled)
        {
            _fanSettingsTimer.Start();
        }
    }

    public int GetFan(HpFan device)
    {
        if (!IsWmiReady()) return -1;

        try
        {
            // Use command 0x38 for direct fan RPM reading
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.FanGetRpm,
                new byte[4],
                128);

            if (result.Success && result.ReturnCode == 0 && result.Data.Length >= 4)
            {
                int fanIndex = device switch { HpFan.GPU => 1, HpFan.Mid => 2, _ => 0 };
                int offset = fanIndex * 2;
                
                // Handle endianness detection - try little-endian first, then big-endian
                int rpm = result.Data[offset] | (result.Data[offset + 1] << 8);
                
                // If the RPM value seems invalid (likely big-endian), try reverse
                if (rpm > 8000 || rpm < 0)
                {
                    rpm = (result.Data[offset] << 8) | result.Data[offset + 1];
                }
                
                // Apply sanity checking (0-8000 RPM range)
                if (rpm > 8000) rpm = 0;
                if (rpm < 0) rpm = 0;
                
                // Apply 1300 RPM dead zone (as noted in issue)
                if (rpm > 0 && rpm < 1300) rpm = 0;
                
                // Return in units of 100 RPM to match FormatFan expectations
                return rpm / 100;
            }
        }
        catch { }

        try
        {
            // Fallback to command 0x2D for fan level reading  
            var statusResult = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.FanGetLevel,
                new byte[4],
                128);

            if (statusResult.Success && statusResult.ReturnCode == 0 && statusResult.Data.Length >= 2)
            {
                int fanIndex = device switch { HpFan.GPU => 1, HpFan.Mid => 2, _ => 0 };
                // Data is already a level byte (0-100 scale), return as-is
                int rpm = statusResult.Data[fanIndex];
                return rpm;
            }
        }
        catch { }

        return -1;
    }

    public bool IsMidFanSupported()
    {
        // Check model capabilities from database for fan zone count
        var modelCaps = AppConfig.GetModelCapabilities();
        if (modelCaps != null)
        {
            return modelCaps.FanZoneCount >= 3;
        }
        
        // Fallback to model detection for known models with mid fan
        return AppConfig.ContainsModel("16-ah") || AppConfig.ContainsModel("16-ak") || 
               AppConfig.ContainsModel("OMEN MAX") || AppConfig.ContainsModel("OMEN MAX 16");
    }

    public int SetFanRange(HpFan device, byte[] curve)
    {
        if (!IsWmiReady()) return 0;

        // SetFanRange was a fallback using a nonexistent command type.
        // Redirect to SetFanLevel — the curve evaluation loop handles mapping.
        return 0;
    }

    public int SetFanMode(byte mode)
    {
        if (!IsWmiReady()) return 0;

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.PerformanceMode,
                new byte[] { 0xFF, mode, 0x00, 0x00 },
                0);

            bool ok = result.Success && result.ReturnCode == 0;
            Logger.WriteLine($"HpACPI SetFanMode: mode=0x{mode:X2} success={ok} rc={result.ReturnCode}");
            return ok ? 1 : 0;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("SetFanMode exception: " + ex.Message);
        }

        return 0;
    }

    public int SetFanLevel(byte cpuLevel, byte gpuLevel)
    {
        if (!IsWmiReady()) return 0;

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.FanSetLevel,
                new byte[] { cpuLevel, gpuLevel, 0x00, 0x00 },
                0);

            return result.Success && result.ReturnCode == 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("SetFanLevel exception: " + ex.Message);
        }

        return 0;
    }

    public int SetFanMax(bool enable)
    {
        if (!IsWmiReady()) return 0;

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.FanMaxSet,
                new byte[] { (byte)(enable ? 1 : 0), 0, 0, 0 },
                4);

            return result.Success && result.ReturnCode == 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("SetFanMax exception: " + ex.Message);
        }

        return 0;
    }

    public byte[] GetFanCurve(HpFan device, int mode = 0)
    {
        // Return default fan curve for the model and mode
        // This should be expanded to fetch actual stored curves from WMI or config
        var modelCaps = AppConfig.GetModelCapabilities();
        if (modelCaps == null) return new byte[16];
        
        // For now, return a fallback - actual implementation will pull from model DB
        // This is the implementation of GetDefaultCurve method in AppConfig
        // For Transcend model, return appropriate curve
        if (AppConfig.IsOmenTranscend() && mode <= 3)
        {
            switch (mode)
            {
                case 0: // Eco
                    return new byte[] { 0x1E, 0x32, 0x3C, 0x44, 0x4B, 0x52, 0x5A, 0x64, 0x00, 0x00, 0x14, 0x1E, 0x28, 0x32, 0x3C, 0x44 };
                case 1: // Balanced
                    return new byte[] { 0x1E, 0x32, 0x3C, 0x46, 0x4E, 0x55, 0x5C, 0x64, 0x00, 0x00, 0x00, 0x00, 0x14, 0x1E, 0x26, 0x2D };
                case 2: // Performance 
                    return new byte[] { 0x1E, 0x32, 0x3A, 0x41, 0x48, 0x4E, 0x55, 0x64, 0x16, 0x1C, 0x23, 0x2D, 0x3A, 0x46, 0x52, 0x5C };
                case 3: // Unleashed
                    return new byte[] { 0x1E, 0x32, 0x3A, 0x41, 0x48, 0x4E, 0x55, 0x64, 0x1C, 0x26, 0x30, 0x3A, 0x46, 0x52, 0x5C, 0x64 };
            }
        }
        
        // Return empty/fallback curve
        return new byte[16];
    }

    public static bool IsInvalidCurve(byte[] curve)
    {
        return curve.Length != 16 || IsEmptyCurve(curve);
    }

    public static bool IsEmptyCurve(byte[] curve)
    {
        return curve.All(singleByte => singleByte == 0);
    }

    public (int up, int down) GetFanHysteresis()
    {
        // In HP BIOS, hysteresis is typically handled by the firmware
        // This is a stub - in reality, we'd read from WMI or device settings
        // For now, return defaults that match common HP behavior
        return (5, 3); // 5°C up, 3°C down hysteresis
    }

    public int SetFanHysteresis(int up, int down)
    {
        if (!IsWmiReady()) return 1;
        
        try
        {
            // For models that support direct hysteresis setting, we'd send a command here
            // HP BIOS doesn't typically expose hysteresis via WMI directly
            
            // For now, just return success since we can't set it via WMI
            return 1;
        }
        catch (Exception ex)
        {
            Logger.WriteLine("SetFanHysteresis exception: " + ex.Message);
        }
        
        return 1;
    }

    private static int NormalizeRpm(int rpm)
    {
        int clamped = Math.Max(0, Math.Min(6500, rpm));
        int quantized = (int)(Math.Round(clamped / 100.0) * 100.0);
        if (quantized > 0 && quantized < 1300)
            return 0;
        return quantized;
    }

    #endregion

    #region Temperature

    private int GetCpuTemp()
    {
        if (!IsWmiReady()) return -1;

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.TempGet,
            new byte[] { 0x01, 0, 0, 0 },
            4);

        if (result.Success && result.ReturnCode == 0 && result.Data.Length > 0)
            return result.Data[0];

        return -1;
    }

    private int GetGpuTemp()
    {
        if (!IsWmiReady()) return -1;

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.TempGet,
            new byte[] { 0x02, 0, 0, 0 },
            4);

        if (result.Success && result.ReturnCode == 0 && result.Data.Length > 0)
            return result.Data[0];

        return -1;
    }

    #endregion

    #region Power Limits

    private int SetPowerLimit(uint deviceId, int value)
    {
        if (!IsWmiReady()) return 0;

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.Tpptdp,
                new byte[] { 0xFF, 0xFF, 0xFF, (byte)value },
                4);
            return result.Success && result.ReturnCode == 0 ? 1 : 0;
        }
        catch { }

        return 0;
    }

    private int SetGpuPowerLimit(uint deviceId, int value)
    {
        if (!IsWmiReady()) return 0;

        try
        {
            bool customTgp = value > 0;
            bool ppab = value > 0;

            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.GpuSetPower,
                new byte[] { customTgp ? (byte)1 : (byte)0, ppab ? (byte)1 : (byte)0, 0x01, 0x00 },
                0);
            return result.Success && result.ReturnCode == 0 ? 1 : 0;
        }
        catch { }

        return 0;
    }

    private int GetGpuBasePower()
    {
        if (!IsWmiReady()) return -1;

        var result = ExecuteBiosCommand(
            (uint)HpBiosCommand.Default,
            (int)HpBiosCommandType.GpuGetPower,
            new byte[4],
            4);

        if (result.Success && result.ReturnCode == 0 && result.Data.Length >= 2)
            return result.Data[0]; // customTgp flag

        return -1;
    }

    #endregion

    #region Capability Detection

    private SystemDesignDataInfo? _systemDesignData;
    private bool? _isNvidiaGpu;
    private bool? _isAllAmd;
    private bool _capabilityDetectionComplete;

    public SystemDesignDataInfo GetSystemDesignData()
    {
        if (_systemDesignData != null) return _systemDesignData;

        if (!IsWmiReady()) return SystemDesignDataInfo.Empty;

        try
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                var result = ExecuteBiosCommand(
                    (uint)HpBiosCommand.Default,
                    (int)HpBiosCommandType.SystemDesignData,
                    null,
                    128);

                if (result.Success && result.ReturnCode == 0 && result.Data.Length >= 9)
                {
                    _systemDesignData = SystemDesignDataInfo.FromRaw(result.Data[7], true);
                    Logger.WriteLine($"HpACPI: SystemDesignData byte[7]=0x{_systemDesignData.RawGpuModeSwitch:X2} slots={_systemDesignData.GraphicsModeSlots} switching={_systemDesignData.SupportsGraphicsSwitching}");
                    return _systemDesignData;
                }

                Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HpACPI: SystemDesignData read failed: {ex.Message}");
        }

        _systemDesignData = SystemDesignDataInfo.Empty;
        return _systemDesignData;
    }

    public void DetectCapabilities()
    {
        if (_capabilityDetectionComplete) return;
        _capabilityDetectionComplete = true;

        Logger.WriteLine("HpACPI: Starting capability detection...");

        var sdd = GetSystemDesignData();
        Logger.WriteLine($"HpACPI: Graphics switching supported: {sdd.SupportsGraphicsSwitching} (slots: {sdd.GraphicsModeSlots})");

        DetectGpuVendor();
        Logger.WriteLine($"HpACPI: NVIDIA GPU: {IsNVidiaGPU()}, All-AMD PPT: {IsAllAmdPPT()}");

        Logger.WriteLine($"HpACPI: Overdrive supported: {IsOverdriveSupported()}");

        var modelCaps = AppConfig.GetModelCapabilities();
        Logger.WriteLine($"HpACPI: Model capabilities loaded: {modelCaps.ModelName} (ProductId: {modelCaps.ProductId}, Family: {modelCaps.Family})");
    }

    private void DetectGpuVendor()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            bool hasNvidia = false;
            bool hasAmd = false;

            foreach (var obj in searcher.Get())
            {
                string name = obj["Name"]?.ToString() ?? "";
                if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                    hasNvidia = true;
                else if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
                         name.Contains("Radeon", StringComparison.OrdinalIgnoreCase))
                    hasAmd = true;
            }

            _isNvidiaGpu = hasNvidia;
            _isAllAmd = !hasNvidia && hasAmd;
        }
        catch (Exception ex)
        {
            Logger.WriteLine($"HpACPI: GPU vendor detection failed: {ex.Message}");
            _isNvidiaGpu = false;
            _isAllAmd = false;
        }
    }

    [Obsolete("XG Mobile is an ASUS ROG proprietary eGPU dock. Use AppConfig.GetModelCapabilities() instead.")]
    public bool IsXGConnected()
    {
        return false;
    }

    public bool IsAllAmdPPT()
    {
        if (_isAllAmd.HasValue) return _isAllAmd.Value;
        DetectGpuVendor();
        return _isAllAmd ?? false;
    }

    public void SetAPUMem(int memory = 4)
    {
    }

    public int GetAPUMem()
    {
        return -1;
    }

    public (int, int) GetCores(bool max = false)
    {
        return (-1, -1);
    }

    public void SetCores(int eCores, int pCores)
    {
    }

    public bool IsNVidiaGPU()
    {
        if (_isNvidiaGpu.HasValue) return _isNvidiaGpu.Value;
        DetectGpuVendor();
        return _isNvidiaGpu ?? false;
    }

    public bool IsSupported(uint DeviceID)
    {
        if (!_supportCache.TryGetValue(DeviceID, out bool supported))
        {
            supported = ProbeSupport(DeviceID);
            _supportCache[DeviceID] = supported;
        }
        return supported;
    }

    private bool ProbeSupport(uint deviceId)
    {
        if (!IsWmiReady()) return false;

        if (deviceId == ScreenOverdrive)
            return IsOverdriveSupported();

        if (deviceId == DevsCPUFanCurve || deviceId == DevsGPUFanCurve || deviceId == DevsMidFanCurve)
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.FanGetCount,
                new byte[4],
                4);
            return result.Success && result.ReturnCode == 0;
        }

        if (deviceId == GPUEco || deviceId == GPUEcoROG || deviceId == GPUEcoVivo
            || deviceId == GPUMux || deviceId == GPUMuxROG || deviceId == GPUMuxVivo)
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Legacy,
                (int)HpBiosCommandType.GpuModeGet,
                null,
                4);
            return result.Success && result.ReturnCode == 0;
        }

        if (deviceId == GPU_POWER || deviceId == GPU_BASE)
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.GpuGetPower,
                new byte[4],
                4);
            return result.Success && result.ReturnCode == 0;
        }

        if (deviceId == PPT_GPUC0 || deviceId == PPT_GPUC2)
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.GpuGetPower,
                new byte[4],
                4);
            return result.Success && result.ReturnCode == 0;
        }

        if (deviceId == MicMuteLed || deviceId == SoundMuteLed)
            return false;

        if (deviceId == PPT_APUA0 || deviceId == PPT_APUA3 || deviceId == PPT_APUC1
            || deviceId == PPT_CPUB0 || deviceId == PPT_CPUB1)
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.Tpptdp,
                new byte[] { 0xFF, 0xFF, 0xFF, 0 },
                4);
            return result.Success && result.ReturnCode == 0;
        }

        return false;
    }

    public string ScanRange()
    {
        if (!IsWmiReady()) return string.Empty;

        var sb = new System.Text.StringBuilder();

        try
        {
            var result = ExecuteBiosCommand(
                (uint)HpBiosCommand.Default,
                (int)HpBiosCommandType.SystemDesignData,
                null,
                128);

            if (result.Success && result.ReturnCode == 0 && result.Data.Length >= 8)
            {
                sb.AppendLine($"SystemDesignData: {BitConverter.ToString(result.Data[..Math.Min(16, result.Data.Length)])}");
            }
        }
        catch { }

        try
        {
            for (int cmdType = 0x10; cmdType <= 0x50; cmdType++)
            {
                var result = ExecuteBiosCommand(
                    (uint)HpBiosCommand.Default,
                    cmdType,
                    new byte[4],
                    4);
                if (result.Success && result.ReturnCode == 0)
                    sb.AppendLine($"CMD 0x{cmdType:X2}: OK ({BitConverter.ToString(result.Data)})");
            }
        }
        catch { }

        return sb.ToString();
    }

    private byte[] DeviceGetLarge(uint DeviceID, int extraIn = 8, int outSize = 40)
    {
        return new byte[outSize];
    }

    #endregion

    #region Fan Curve Utility

    public static byte[] FixFanCurve(byte[] curve)
    {
        if (curve.Length != 16) throw new Exception("Incorrect curve");

        var points = new Dictionary<byte, byte>();
        byte old = 0;

        for (int i = 0; i < 8; i++)
        {
            if (curve[i] <= old) curve[i] = (byte)Math.Min(100, old + 6);
            points[curve[i]] = curve[i + 8];
            old = curve[i];
        }

        var pointsFixed = new Dictionary<byte, byte>();
        bool fix = false;

        int count = 0;
        foreach (var pair in points.OrderBy(x => x.Key))
        {
            if (count == 0 && pair.Key >= 40)
            {
                fix = true;
                pointsFixed.Add(30, 0);
            }

            if (count != 3 || !fix)
                pointsFixed.Add(pair.Key, pair.Value);
            count++;
        }

        count = 0;
        foreach (var pair in pointsFixed.OrderBy(x => x.Key))
        {
            int x = pair.Key;

            if (AppConfig.IsClampFanDots())
            {
                int minX = 30 + (count * 10);
                int maxX = minX + 10;
                x = Math.Max(minX, Math.Min(maxX, x));
            }

            curve[count] = (byte)x;
            curve[count + 8] = pair.Value;
            count++;
        }

        return curve;
    }

    #endregion

    #region WmiBiosResult

    public readonly struct WmiBiosResult
    {
        public WmiBiosResult(bool success, int returnCode, byte[] data)
        {
            Success = success;
            ReturnCode = returnCode;
            Data = data ?? Array.Empty<byte>();
        }

        public bool Success { get; }
        public int ReturnCode { get; }
        public byte[] Data { get; }

        public static WmiBiosResult Failure(int returnDataSize)
        {
            return new WmiBiosResult(false, -1,
                returnDataSize > 0 ? new byte[returnDataSize] : Array.Empty<byte>());
        }
    }

    #endregion
}
