using GHelper;
using GHelper.USB;
using System.Management;
using System.Runtime.InteropServices;

public enum AsusFan
{
    CPU = 0,
    GPU = 1,
    Mid = 2,
    XGM = 3
}

public enum AsusMode
{
    Balanced = 0,
    Turbo = 1,
    Silent = 2
}

public enum AsusGPU
{
    Eco = 0,
    Standard = 1,
    Ultimate = 2
}

/// <summary>
/// Stub implementation — all ACPI hardware calls are replaced with
/// no-ops / default return values so the UI works identically without
/// ASUS hardware.
/// </summary>
public class AsusACPI
{

    const string FILE_NAME = @"\\.\\ATKACPI";
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

    public const uint PerformanceMode = 0x00120075; // Performance modes
    public const uint VivoBookMode = 0x00110019; // Vivobook performance modes

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
    public const uint ScreenInit = 0x00050011; // ?

    public const uint DevsCPUFan = 0x00110022;
    public const uint DevsGPUFan = 0x00110023;

    public const uint DevsCPUFanCurve = 0x00110024;
    public const uint DevsGPUFanCurve = 0x00110025;
    public const uint DevsMidFanCurve = 0x00110032;

    public const uint FanHysteresis = 0x00110034;
    public const int Temp_CPU = 0x00120094;
    public const int Temp_GPU = 0x00120097;

    public const int PPT_APUA0 = 0x001200A0;  // sPPT (slow boost limit) / PL2
    public const int PPT_EDCA1 = 0x001200A1;  // CPU EDC
    public const int PPT_TDCA2 = 0x001200A2;  // CPU TDC
    public const int PPT_APUA3 = 0x001200A3;  // SPL (sustained limit) / PL1

    public const int PPT_CPUB0 = 0x001200B0;  // CPU PPT on 2022 (PPT_LIMIT_APU)
    public const int PPT_CPUB1 = 0x001200B1;  // Total PPT on 2022 (PPT_LIMIT_SLOW)

    public const int PPT_GPUC0 = 0x001200C0;  // NVIDIA GPU Boost
    public const int PPT_APUC1 = 0x001200C1;  // fPPT (fast boost limit)
    public const int PPT_GPUC2 = 0x001200C2;  // NVIDIA GPU Temp Target (75.. 87 C) 

    public const uint CORES_CPU = 0x001200D2; // Intel E-core and P-core configuration in a format 0x0[E]0[P]
    public const uint CORES_MAX = 0x001200D3; // Maximum Intel E-core and P-core availability

    public const uint GPU_BASE  = 0x00120099;  // Base part GPU TGP
    public const uint GPU_POWER = 0x00120098;  // Additonal part of GPU TGP

    public const int APU_MEM = 0x000600C1;

    public const int TUF_KB_BRIGHTNESS = 0x00050021;
    public const int KBD_BACKLIGHT_OOBE = 0x0005002F;

    public const int TUF_KB = 0x00100056;
    public const int TUF_KB2 = 0x0010005a;

    public const int TUF_KB_STATE = 0x00100057;

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

    private bool? _allAMD = null;
    private readonly Dictionary<uint, bool> _supportCache = new();

    public static uint GPUEco => AppConfig.IsVivoZenPro() ? GPUEcoVivo : GPUEcoROG;
    public static uint GPUMux => AppConfig.IsVivoZenPro() ? GPUMuxVivo : GPUMuxROG;

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

    // Event handling attempt

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

    private IntPtr eventHandle;
    private bool _connected = false;

    // still works only with asus optimization service on , if someone knows how to get ACPI events from asus without that - let me know
    public void RunListener()
    {
        Logger.WriteLine("ACPI listener stub — no hardware");
    }

    public bool IsConnected()
    {
        return _connected;
    }

    public AsusACPI()
    {
        _connected = true;
    }

    public void Control(uint dwIoControlCode, byte[] lpInBuffer, byte[] lpOutBuffer)
    {
    }

    public void Close()
    {
    }


    protected byte[] CallMethod(uint MethodID, byte[] args)
    {
        return new byte[16];
    }

    public byte[] DeviceInit()
    {
        return new byte[16];
    }

    public byte[] DeviceWatchDog()
    {
        return new byte[16];
    }

    public int DeviceSet(uint DeviceID, int Status, string? logName)
    {
        return 1;
    }


    public int DeviceSet(uint DeviceID, byte[] Params, string? logName)
    {
        return 1;
    }


    public int DeviceGet(uint DeviceID)
    {
        return -1;
    }

    public byte[] DeviceGetBuffer(uint DeviceID, uint Status = 0)
    {
        return new byte[16];
    }


    public decimal? GetBatteryDischarge()
    {
        return null;
    }


    public int SetVivoMode(int mode)
    {
        return 1;
    }

    public int SetGPUEco(int eco)
    {
        // Pretend success without changing hardware
        return 1;
    }

    public int GetFan(AsusFan device)
    {
        return -1;
    }

    public bool IsMidFanSupported()
    {
        return false;
    }

    public int SetFanRange(AsusFan device, byte[] curve)
    {
        return 1;
    }


    public int SetFanCurve(AsusFan device, byte[] curve)
    {
        return 1;
    }

    public byte[] GetFanCurve(AsusFan device, int mode = 0)
    {
        // Return empty/zero curve
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
        return (-1, -1);
    }

    public int SetFanHysteresis(int up, int down)
    {
        return 1;
    }

    public static byte[] FixFanCurve(byte[] curve)
    {
        if (curve.Length != 16) throw new Exception("Incorrect curve");

        var points = new Dictionary<byte, byte>();
        byte old = 0;

        for (int i = 0; i < 8; i++)
        {
            if (curve[i] <= old) curve[i] = (byte)Math.Min(100, old + 6); // preventing 2 points in same spot from default asus profiles
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

    public bool IsXGConnected()
    {
        return false;
    }

    public bool IsAllAmdPPT()
    {
        if (_allAMD is null) _allAMD = false;
        return (bool)_allAMD;
    }

    public bool IsOverdriveSupported()
    {
        return false;
    }

    public bool IsSupported(uint DeviceID)
    {
        if (!_supportCache.TryGetValue(DeviceID, out bool supported))
        {
            supported = false;
            _supportCache[DeviceID] = supported;
        }
        return supported;
    }

    public bool IsNVidiaGPU()
    {
        return false;
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

    public string ScanRange()
    {
        return string.Empty;
    }

    private byte[] DeviceGetLarge(uint DeviceID, int extraIn = 8, int outSize = 40)
    {
        return new byte[outSize];
    }

    public void TUFKeyboardBrightness(int brightness, string log = "TUF Backlight")
    {
    }

    public void TUFKeyboardRGB(AuraMode mode, Color color, int speed, string? log = "TUF RGB")
    {
    }

    const int ASUS_WMI_KEYBOARD_POWER_BOOT = 0x03 << 16;
    const int ASUS_WMI_KEYBOARD_POWER_AWAKE = 0x0C << 16;
    const int ASUS_WMI_KEYBOARD_POWER_SLEEP = 0x30 << 16;
    const int ASUS_WMI_KEYBOARD_POWER_SHUTDOWN = 0xC0 << 16;
    public void TUFKeyboardPower(bool awake = true, bool boot = false, bool sleep = false, bool shutdown = false)
    {
    }

    private ManagementEventWatcher? watcher;

    public void SubscribeToEvents(Action<object, EventArrivedEventArgs> EventHandler)
    {
    }


}
