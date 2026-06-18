# O-Helper — Agent Guide

## What This Is

G-Helper is a **lightweight Windows Forms (WinForms) tray application** written in C# (.NET 8.0), being refactored from an ASUS Armoury Crate replacement into an **HP Omen controller**. The original ASUS ACPI/HID hardware communication has been **gutted** — all hardware calls are stubbed so the UI runs identically without ASUS hardware.

## Essential Commands

| Command | Purpose |
|---------|---------|
| `dev.bat` | Build debug + launch |
| `prod.bat` | Publish single-file release + launch |
| `dotnet build app/OHelper.sln` | Build only (Debug) |
| `dotnet publish app/OHelper.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true --no-self-contained` | Publish release EXE |

- **No test project** exists. No unit tests, no test framework.
- **Build kills running OHelper processes** before building (see `.csproj` line 94 — only applies locally, not on CI).

## Project Structure

```
app/
├── Program.cs             # Entry point — sets up singletons, tray icon, event subs
├── HpACPI.cs              # STUBBED — all HP WMI methods return defaults, no HW access
├── AppConfig.cs           # JSON config store (debounced write, atomic file ops)
├── HardwareControl.cs     # Static class — sensor polling, native battery API, fan/GpuControl refs
├── NativeMethods.cs       # P/Invoke helpers (idle time, screen off, lock, error lookup)
├── Settings.cs            # Main SettingsForm (partial class + .Designer.cs)
├── Fans.cs / Fans.Designer.cs   # Fan curve editor form
├── Extra.cs / Extra.Designer.cs  # Services/extra settings form
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
4. Initialize ACPI — stub connects always, no longer blocks startup on non-ASUS hardware
5. Set up tray icon, context menu, input dispatcher (keyboard hook), XGM, aura, matrix
6. Subscribe to system events: `PowerModeChanged`, `SessionSwitch`, `DisplaySettingsChanged`, power setting notifications
7. Start sensor refresh timers

### Global Singletons (accessed via `Program.*`)

- `Program.acpi` — `HpACPI` instance, **all methods stubbed** (DeviceSet/DeviceGet/GetFan/SendWmiSetting/etc. return defaults)
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

### Custom UI Controls (`UI/`)

All custom WinForms controls in the `OHelper.UI` namespace:
- `RForm` — base form with dark/light theme, color constants, DWM titlebar
- `RButton`, `RBadgeButton` — custom-painted buttons with border colors
- `RComboBox` — themed combobox
- `RCheckBox`, `RTrackBar`, `RNumericUpDown`, `RTextBox`, `Slider` — themed controls
- `CustomContextMenu` — drawn context menu

### Localization

Labels use `Properties.Strings.{Key}` — resources are in `.resx` files. Localization is handled via standard .NET satellite assemblies (Crowdin-managed).

## Stubbed Hardware Layer

The following files are modified to safely no-op on non-ASUS hardware:

| File | What was changed |
|------|-----------------|
| `AsusACPI.cs` → renamed to `HpACPI.cs` | All ACPI methods stubbed — `DeviceGet()` returns -1, `DeviceSet()` returns 1, `GetFan()` returns -1, `IsSupported()` returns false, `IsConnected()` returns true, etc. |
| `AnimeMatrix/.../WindowsUsbProvider.cs` | No longer throws on missing HID devices; all IO methods null-guard the stream |
| `Program.cs` | ACPI connection check no longer blocks startup |

All other files (`AsusHid.cs`, `Aura.cs`, `AnimeMatrixDevice.cs`, `Peripherals/*`) already have try-catch guards that prevent crashes when hardware isn't found.

## Model Differentiation Architecture

Hardware quirks, different laptop series, and one-off BIOS workarounds are handled via **centralized string-matching boolean gates** in `AppConfig.cs`, with callers branching on them across ~15 files.

### Model Detection (`AppConfig.cs`)

Two WMI queries at startup, cached in `Lazy<string>`:

| Source | Field | Example |
|--------|-------|---------|
| `Win32_ComputerSystem` | `Model` | `"HP OMEN 16-am2020ca"` |
| `Win32_BIOS` | `SMBIOSBIOSVersion` | `"F.12"` |

Key accessors: `GetModel()`, `GetModelShort()`, `ContainsModel("pattern")` (case-insensitive substring match).

### Boolean Gate Pattern (Centralized Quirks)

~20+ methods in `AppConfig.cs` return `true`/`false` based on `ContainsModel()`. No interfaces, no strategy pattern — just if/else branching.

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

`AsusACPI.cs` renamed to `HpACPI.cs` — all methods remain **stubbed** (return defaults, no HW access).

When real hardware support is added, HP uses the `root\WMI` namespace with HP-specific BIOS setting methods:

```
HPBIOS_BIOSSetting → get/set individual BIOS options
HPBIOS_BIOSSettingEnum → list available values for a setting
HPBIOS_BIOSSettingInterface → apply settings
```

Where ASUS used `DeviceSet(0x00120075, value)`, Omen would set a WMI BIOS setting like `"Performance Mode" → "Enabled"`. No ACPI constant selection needed — HP uses string-based setting names, not magic device IDs.

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
| `force_family` | Override family detection ("omen", "omen_slim", "omen_max", "transcend") — useful for demo/clearance units with weird model strings |

### Adding a New Model

1. Add `ContainsModel("NEW")` calls in the appropriate `AppConfig.cs` gate methods
2. If a new quirk, add a new boolean method in `AppConfig.cs` and branch in the relevant feature file
3. If new fan limits, add an entry in `FanSensorControl.cs`
4. If new GPU power defaults, add an entry in `NvidiaSmi.cs`
5. If existing config flags don't cover it, consider adding a new one as an escape hatch

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

- **Namespace**: `GHelper.{Subfolder}` (e.g., `GHelper.Display`, `GHelper.Mode`)
- **Naming**: PascalCase for methods/properties, `_camelCase` for private fields, hungarian notation for WinForms controls (`buttonSilent`, `panelGPU`, `comboMatrix`)
- **Logging**: `Logger.WriteLine()` — writes to `%APPDATA%\GHelper\log.txt`, truncated to ~2000 lines, sampled cleanup (1% chance per write)
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
