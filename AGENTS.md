# O-Helper — Agent Guide

## What This Is

O-Helper is a **lightweight Windows Forms (WinForms) tray application** written in C# (.NET 8.0) for controlling supported **HP OMEN** systems. It uses HP WMI BIOS communication via `hpqBIntM`/`hpqBDataIn` in `root\wmi`, with feature availability determined by model capabilities and runtime hardware probes.

## Essential Commands

| Command | Purpose |
|---------|---------|
| `dotnet build app/OHelper.sln` | Build only (Debug) |
| `dotnet publish app/OHelper.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true --no-self-contained` | Publish release EXE |

- **No test project** exists. No unit tests, no test framework.
- **Admin elevation required for runtime hardware operations** — WMI `root\wmi` + `EnablePrivileges=true` needs admin. Without elevation, firmware calls can return "Access denied". Build commands do not require elevation.
- **Build kills running OHelper processes** before building (see `.csproj` line 94 — only applies locally, not on CI).

## Project Structure

```
app/
├── Program.cs             # Entry point — sets up singletons, tray icon, event subs
├── HpACPI.cs              # WMI BIOS interface — hpqBIntM/hpqBDataIn in root\wmi
├── AppConfig.cs           # JSON config store (debounced write, atomic file ops, model detection)
├── HardwareControl.cs     # Static class — sensor polling, native battery API, fan/GpuControl refs
├── NativeMethods.cs       # P/Invoke helpers (idle time, screen off, lock, error lookup)
├── Settings.cs            # Main SettingsForm (partial class + .Designer.cs)
├── Fans.cs / Fans.Designer.cs   # Fan curve editor form
├── Extra.cs / Extra.Designer.cs  # Services/extra settings form
├── Hardware/              # Capability detection infrastructure
│   ├── SystemDesignDataInfo.cs   # BIOS type-40 blob parsing, GPU mode support flags
│   └── ModelCapabilityDatabase.cs # Per-ProductID model feature flags (30+ OMEN models)
├── Overlay/              # Hardware overlay (FPS, temps, usage) with ETW FPS monitor
├── USB/                  # HID device access (Omen keyboard RGB, accessories) — safe-falls if no HW
├── Peripherals/          # HP Omen mouse/keyboard detection — safe-falls if no HW
├── Pawn/                 # PawnIO — Ryzen SMU & Intel MSR access (embedded binaries)
├── AutoUpdate/           # Auto-update checker
└── Helpers/              # Logger, Audio, ClamshellMode, Startup, etc.
```

## Architecture & Control Flow

### Startup (`Program.cs`)

1. Parse CLI args (supports `charge`, `cpu`, `gpu`, `services`, etc.)
2. Set up localization from `config.json`
3. Create **global static singletons**: `settingsForm`, `modeControl`, `gpuControl`, `allyControl`, `clamshellControl`, `toast`, `hardwareOverlay`, `acpi`, `trayIcon`, `inputDispatcher`
4. Initialize HpACPI — connects to `root\wmi` via `hpqBIntM`, probes `SystemGetData`, reads `SystemDesignData` (type 40), runs `DetectCapabilities()` (GPU vendor, overdrive, model DB), starts heartbeat timer
5. Set up tray icon, context menu, input dispatcher (keyboard hook), XGM, aura, matrix
6. Subscribe to system events: `PowerModeChanged`, `SessionSwitch`, `DisplaySettingsChanged`, power setting notifications
7. Start sensor refresh timers

### Global Singletons (accessed via `Program.*`)

- `Program.acpi` — `HpACPI` instance, WMI BIOS transport via `hpqBIntM`/`hpqBDataIn` (reads/writes real firmware, graceful no-op fallback on failure)
- `Program.modeControl` — `ModeControl`, performance/power/fan/UV mode application
- `Program.gpuControl` — `GPUModeControl`, GPU eco/standard/ultimate switching
- `Program.settingsForm` — `SettingsForm`, main UI window
- `Program.trayIcon` — `NotifyIcon`, system tray presence
- `Program.inputDispatcher` — `InputDispatcher`, keyboard/hotkey handling
- `Program.toast` — `ToastForm`, OSD toast notifications
- `HardwareControl.GpuControl` — `IGpuControl?`, Nvidia or AMD GPU sensor/OC interface

### Configuration (`AppConfig.cs`)

- JSON file stored at `%APPDATA%\OHelper\config.json` (fallback to startup dir and `%COMMON_APPDATA%\OHelper\config.json`)
- **Debounced writes** — 2-second timer, atomic file replacement (`.tmp` → `.bak` → final)
- **Robust recovery** — can reconstruct config from regex-scavenged key-value pairs if JSON is corrupt
- Thread-safe via `configLock`
- Config values accessed by key name string — **no typed settings class**
- Use `AppConfig.Get("key")`, `AppConfig.Set("key", value)`, `AppConfig.GetString("key")`, `AppConfig.Is("key")`, `AppConfig.GetMode("key")` for mode-specific values (`key_modeID`)

### Performance Modes (`Mode/`)

Three built-in modes + up to 20 custom:
| ID | Name |
|----|------|
| 0 | Balanced |
| 1 | Turbo |
| 2 | Silent |
| 3+ | Custom |

- Mode settings stored as `{key}_{modeID}` in config (e.g., `limit_total_0`, `fan_profile_cpu_1`)
- Auto-mode switching on power state change (AC ↔ battery) is configurable

#### Power Mode Cache Gotchas

HP performance mode is effectively write-only for this app: do **not** assume BIOS can be queried for the active mode. The app must treat its config cache as the source of truth and, on startup, explicitly call the BIOS mode set for the expected cached mode before showing it as current.

- Main/current mode cache: `performance_mode`, `performance_mode_base`, and `performance_{Program.PerformanceKey()}`.
- Advanced Fans + Power auto defaults: `auto_mode_ac`, `auto_mode_dc`, and `auto_mode_enabled`.
- `auto_mode_ac` / `auto_mode_dc` are user-owned advanced settings. Do not update them from ordinary main-mode button/menu/hotkey changes.
- Startup with Auto Power Source Mode enabled should apply `auto_mode_ac` or `auto_mode_dc` directly and force the `SetPerformanceMode(...)` path even if `performance_mode` already matches. That both writes BIOS and calls `Settings.ShowMode(...)`.
- WinForms combo boxes fire `SelectedValueChanged` during data binding and layout. Never persist AC/DC auto defaults from `SelectedValueChanged`; use `SelectionChangeCommitted` or an equivalent user-initiated path.
- For settings that matter across immediate app restart, call `AppConfig.Flush()` after `AppConfig.Set(...)`. The normal write path is debounced.
- `Application.StartupPath\config.json` can exist beside a launched EXE. Avoid letting an EXE-local config silently override `%APPDATA%\OHelper\config.json` unless it is the only available config.

### Custom UI Controls (`UI/`)

All custom WinForms controls in the `OHelper.UI` namespace:
- `RForm` — base form with dark/light theme, color constants, DWM titlebar
- `RButton`, `RBadgeButton` — custom-painted buttons with border colors
- `RComboBox` — themed combobox
- `RCheckBox`, `RTrackBar`, `RNumericUpDown`, `RTextBox`, `Slider` — themed controls
- `CustomContextMenu` — drawn context menu

### Localization

Labels use `Properties.Strings.{Key}` — resources are in `.resx` files. Localization is handled via standard .NET satellite assemblies (Crowdin-managed).

## WMI Hardware Layer

`HpACPI.cs` communicates with HP firmware via WMI:

- **Namespace**: `root\wmi`
- **Classes**: `hpqBIntM` (methods), `hpqBDataIn` (input data)
- **Instance**: `ACPI\PNP0C14\0_0`
- **Signature**: `SECU` (`0x53454355`)
- **Method dispatch**: `hpqBIOSInt4` (out ≤4), `hpqBIOSInt128` (out ≤128), `hpqBIOSInt1024`, `hpqBIOSInt4096` — always use `returnDataSize ≥ 4` (HP firmware has no `hpqBIOSInt0`)
- **Admin required**: runtime firmware calls to `root\wmi` with `EnablePrivileges=true` need UAC elevation

### Reliability
- **60-second heartbeat**: sends `SystemGetData` (0x20008/0x28) to prevent WMI silence on 2023+ models
- **Graceful degradation**: auto-disables WMI after 5 consecutive transport failures, re-enables on success
- **Error throttling**: rate-limits repeated error logs (30s interval)
- **Legacy fallback**: auto-switches WMI connection on exception (BIOS F.15+ compatibility)
- **ReturnCode semantics**: 0=success, 5=NotSupported (firmware rejected command, not a transport failure — does not count toward disable threshold)

### WMI Command Map

| Feature | DeviceID | WMI (command, commandType) |
|---------|----------|----------------------------|
| Performance Mode Set | `PerformanceMode` | `0x20008 / 0x1A`, payload `{0xFF, modeByte, 0x01, 0x00}` |
| Battery Care | `BatteryLimit` | `0x20008 / 0x24`, payload `{enabled, 0, 0, 0}` |
| Overdrive Get/Set | `ScreenOverdrive` | `0x20008 / 0x35` (get), `/ 0x36` (set) |
| GPU Mode Get | `GPUEco/GPUMux` | `0x00001 / 0x52`, null input |
| GPU Mode Set | `GPUEco/GPUMux` | `0x00002 / 0x52`, payload `{mode, 0, 0, 0}` |
| GPU Power Get | `GPU_BASE/GPU_POWER` | `0x20008 / 0x21`, 4 bytes out |
| GPU Power Set | `PPT_GPUC0` | `0x20008 / 0x22`, payload `{tgp, ppab, 0x01, 0x00}` |
| System Design Data | — | `0x20008 / 0x28`, 128 bytes out |
| System Design Data (type 40) | — | `0x20008 / 0x40`, 128 bytes out — byte[7] = GPU mode support flags |
| Fan RPM | `GetFan()` | `0x20008 / 0x38` (direct RPM), fallback `0x20008 / 0x45` (status blob) |
| Fan Target Blob | `DevsCPUFanCurve` | `0x20008 / 0x46`, 128-byte blob (bytes 0,1 = CPU/GPU RPM÷100) |
| CPU/GPU Temp | `Temp_CPU/Temp_GPU` | `0x20008 / 0x23`, input `{0x01,...}`/`{0x02,...}` |
| Power Limits | `PPT_*` | `0x20008 / 0x41`, payload `{0xFF, 0xFF, 0xFF, value}` |

### Mode Byte Mapping

| Mode Value | Mode Byte | Firmware Name |
|------------|-----------|---------------|
| `PerformanceBalanced` (0) | `0x30` | Default/Balanced |
| `PerformanceTurbo` (1) | `0x31` | Performance |
| `PerformanceSilent` (2) | `0x50` | Cool/Quiet |
| `PerformanceManual` (4) | `0x04` | Unleashed/Extreme |

### Still No-Op (no confirmed WMI commandType)

`UniversalControl`, `MicMuteLed`, `SoundMuteLed` (HID-driven), `ScreenMiniled`, `ScreenFHD`, `ScreenHDRControl` (model-specific), `SlateMode`, `TabletState`, `TentState`, `FnLock`, `CameraShutter`, `BootSound` (ASUS-only sensors). These can be filled in when commandTypes are identified via `omen-bios-sniffer.ps1` or OmenCore expands coverage.

## Model Differentiation Architecture

Hardware quirks, different laptop series, and one-off BIOS workarounds are handled via **centralized string-matching boolean gates** in `AppConfig.cs`, with callers branching on them across ~15 files.

### Model Detection (`AppConfig.cs`)

Three WMI queries at startup, cached in `Lazy<string>`:

| Source | Field | Example |
|--------|-------|---------|
| `Win32_ComputerSystem` | `Model` | `"HP OMEN 16-am2020ca"` |
| `Win32_BIOS` | `SMBIOSBIOSVersion` | `"F.12"` |
| `Win32_BaseBoard` | `Product` | `"8E41"` |

Key accessors: `GetModel()`, `GetModelShort()`, `GetProductId()`, `ContainsModel("pattern")` (case-insensitive substring match), `GetModelCapabilities()` (returns `ModelCapabilities` from database), `GetModelFamily()` (returns `OmenModelFamily` enum).

### Boolean Gate Pattern (Centralized Quirks)

~20+ methods in `AppConfig.cs` return `true`/`false` based on `ContainsModel()`. No interfaces, no strategy pattern — just if/else branching. These are **coarse, fast** checks based on model string.

For **precise, per-ProductId** feature flags, use `AppConfig.GetModelCapabilities()` which returns a `ModelCapabilities` object from the `ModelCapabilityDatabase` (see `Hardware/ModelCapabilityDatabase.cs`).

### Capability Detection (`HpACPI.cs`)

`DetectCapabilities()` runs during `HpACPI` construction:

1. **SystemDesignData (type 40)** — Reads 128-byte BIOS blob, parses byte[7] as `GraphicsModeSupportSlot` flags (Integrated/Hybrid/Dedicated/Optimus). Access via `Program.acpi.GetSystemDesignData()`.
2. **GPU Vendor Detection** — `Win32_VideoController` WMI query for NVIDIA/AMD/Intel. Results cached in `_isNvidiaGpu` / `_isAllAmd`.
3. **Overdrive Probe** — WMI 0x35 command success check (existing, pre-warmed by `DetectCapabilities()`).
4. **Model Database Lookup** — `AppConfig.GetModelCapabilities()` resolves ProductId → per-model feature flags.

| Gate Method | Detection Source | Previous |
|-------------|-----------------|----------|
| `IsOverdriveSupported()` | WMI 0x35 probe (unchanged) | WMI 0x35 probe |
| `IsNVidiaGPU()` | `Win32_VideoController` WHERE Name LIKE '%NVIDIA%' | GpuGetPower WMI success |
| `IsAllAmdPPT()` | `Win32_VideoController` (no NVIDIA + has AMD) | Always `false` |
| `IsXGConnected()` | `[Obsolete]` — always returns `false` (ASUS-only) | Always `false` |

**Family Gates** (select ACPI/WMI constants, available features, UI visibility):

| Method | Matches | Effect |
|--------|---------|--------|
| `IsOmen()` | `"OMEN"` | Base gate — enables all Omen features and controls |
| `IsOmenSlim()` | `"OMEN Slim"`, `"Slim 16"` | Slim chassis — different fan curve defaults, keyboard layout, thermal profile |
| `IsOmenMax()` | `"OMEN MAX"`, `"OMEN MAX 16"`, `"16-ah"`, `"16-ak"` | Flagship tier — different TDP targets, mux switch config, higher default power limits |
| `IsTranscend()` | `"OMEN Transcend"`, `"14-fb"` | Thin-and-light — reduced fan allowance, different keyboard lighting UI (per-key vs zone), no numpad |

**One-Off Workaround Gates** (model-specific BIOS bugs, hardware quirks):

| Method | Affected Models | What It Does |
|--------|----------------|--------------|
| `HasLightBoost()` | Specific panel SKUs (TBD per model) | Enables display overdrive / LightBoost toggle in settings |
| `IsFanRequired()` | Models where fan curve is controllable via WMI | Enables custom fan curve UI |
| `IsPowerRequired()` | Models where PPT limits need re-application after fan curve | Re-applies power limits (BIOS workaround) |
| `IsCPULight()` | Lower-TDP AMD/Intel configs | Lower default CPU PPT values |
| `IsAlwaysUltimate()` | Select MAX or high-end SKUs | Forces GPU dGPU-only mode, hides mux switch |
| `IsSleepReset()` | Models with GPU state lost on sleep | Resets GPU mode after resume |
| `NoWMI()` | Models where WMI BIOS settings are read-only | Skips WMI-based power calls, uses native Windows power plans only |
| `IsChargeLimit6080()` | Models where HP firmware uses 60/80% charge limit scheme vs 80/100% | Adjusts charge limit radio buttons |

### HP WMI Interface (`HpACPI.cs`)

See **WMI Hardware Layer** section above for full details. `HpACPI.cs` uses the `hpqBIntM`/`hpqBDataIn` WMI interface with numeric `(command, commandType)` pairs — not `HPBIOS_BIOSSetting` string-based settings. The `DeviceSet(uint, int, string?)` / `DeviceGet(uint)` API is preserved for backward compatibility; internally, device IDs are mapped to WMI command pairs.

**RPM dead zone**: 1–1299 RPM snaps to 0 (firmware instability below 1300 RPM, confirmed by both OmenCore and omen-helper).

### Per-Model Fan Max Values (`Fan/FanSensorControl.cs`)

Fan speed scaling curves and max RPM values are hardcoded per model string — same structure as ASUS, but entries keyed on Omen model codes:

```
16-am2020ca → [55, 55, 58]     // HyperX OMEN 16
16-ah0000ca → [60, 60, 80]     // OMEN MAX 16 (has mid fan)
14-fb1047nr → [50, 50, 58]     // OMEN Transcend 14
Slim 16     → [52, 52, 58]     // OMEN Slim 16
default     → [55, 55, 58]     // generic OMEN
```

### Per-Model GPU Power Defaults (`GPU/NvidiaSmi.cs`)

Same structure as ASUS, but entries keyed on Omen family:

```
OMEN MAX 16     → 140W    // flagship tier
OMEN 16         → 115W    // standard
OMEN Slim 16    → 100W    // slim chassis
OMEN Transcend  →  90W    // thin-and-light
default         → 115W    // fallback
```

### Display Modes (`Display/VisualControl.cs`)

**Stubbed** — single stub path only. No ASUS ROG vs Vivobook dual pipeline. To be filled in when HP-specific display gamut control is understood.

### Config Override Escape Hatches

Users can override model detection or force behavior via config flags checked with `AppConfig.Is("flag")`:

| Config Key | Effect |
|------------|--------|
| `manual_mode` | Force manual performance mode on supported models |
| `no_overdrive` | Disable display overdrive / LightBoost |
| `no_gpu` | Disable GPU mode switching (mux) |
| `gpu_mode_force_set` | Force GPU mode instead of toggling |
| `no_brightness` | Disable brightness control |
| `force_family` | Override family detection ("omen", "omen_slim", "omen_max", "transcend", "victus", "desktop") — overrides `ModelCapabilityDatabase` lookup |

### Adding a New Model

1. Add an entry to `ModelCapabilityDatabase` in `Hardware/ModelCapabilityDatabase.cs` with the ProductId and feature flags
2. If coarse model-string matching is needed, add `ContainsModel("NEW")` calls in the appropriate `AppConfig.cs` gate methods
3. If a new quirk, add a new boolean method in `AppConfig.cs` and branch in the relevant feature file
4. If new fan limits, add an entry in `FanSensorControl.cs`
5. If new GPU power defaults, add an entry in `NvidiaSmi.cs`
6. If existing config flags don't cover it, consider adding a new one as an escape hatch

### Static Everything
Almost all core services are **static classes** or accessed via static references on `Program`. There's no dependency injection. This works for single-instance tray apps but makes testing impossible.

### Threading Patterns
- `System.Timers.Timer` used extensively (sensors, power settle, mode toggle, reapply)
- `Task.Run` with `CancellationToken` for mode switching (cancellable to handle rapid mode changes)
- `Control.Invoke`/`Control.BeginInvoke` for UI updates from background threads
- Raw `Thread.Sleep` used in several places (GPU mode switching, sensor polling)

### Config Is Untyped Strings/Ints
`AppConfig` stores everything as `object` (JSON root types). Keys are manually type-checked — `GetString` vs `Get`. Mode-specific keys follow the pattern `{key}_{modeID}`. Always check `AppConfig.Exists(key)` before reading.

### Embedded Firmware Blobs
Two binaries are embedded resources in `Pawn/`:
- `RyzenSMU.bin` — PawnIO SMU driver for AMD CPU control
- `IntelMSR.bin` — PawnIO MSR driver for Intel CPU control

These are loaded at runtime by `RyzenSmuService` and `IntelMsrService` to access CPU power management registers.

### Mouse Peripheral Models

Mice are modeled as individual classes under `Peripherals/Mouse/Models/` with per-model feature support. Detection is HID-based via `HidSharp` — safe-falls gracefully when no mouse is connected.

## Conventions

- **Namespace**: `OHelper.{Subfolder}` (e.g., `OHelper.Display`, `OHelper.Mode`)
- **Naming**: PascalCase for methods/properties, `_camelCase` for private fields, hungarian notation for WinForms controls (`buttonSilent`, `panelGPU`, `comboMatrix`)
- **Logging**: `Logger.WriteLine()` — writes to `%APPDATA%\OHelper\log.txt`, truncated to ~2000 lines, sampled cleanup (1% chance per write)
- **Colors**: Defined as static `Color` constants in `RForm` (e.g., `colorEco`, `colorStandard`, `colorTurbo`, `colorCustom`, `colorGray`)
- **String Resources**: All user-facing strings via `Properties.Strings.{Key}` — never hardcode displayed text
- **Error handling**: Catch + log pattern throughout, minimal user-facing error messages

## File Organization Notes

- `.Designer.cs` files are the Windows Forms designer output — they contain `InitializeComponent()` with all the control layout code. Do NOT hand-edit these beyond what the designer would produce.
- `.resx` files are resource XML — strings, images, icons. `Resources.resx` and `Strings.resx` in `Properties/`.
- `app.manifest` enables DPI awareness and long paths.

## What NOT To Do

- Do not add a test framework — the architecture (massive static dependencies, WinForms tight coupling) doesn't support it
- Do not refactor away from static singletons unless the entire architecture changes — `Program.{thing}` is the consistent pattern
- Do not try to build or run this on anything except Windows — it uses WinForms, P/Invoke, and NT device paths
- Do not change `.Designer.cs` files unless absolutely necessary, and if you do, keep the designer code in the `#region Windows Form Designer generated code` block
