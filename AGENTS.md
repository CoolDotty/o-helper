# G-Helper — Agent Guide

## What This Is

G-Helper is a **lightweight Windows Forms (WinForms) tray application** written in C# (.NET 8.0) that replaces ASUS Armoury Crate. It runs as a single EXE with a system tray icon and a settings form, communicating with ASUS hardware via ACPI (the `\\.\ATKACPI` device) and HID (the `\\.\ASUSINPUT` device).

## Essential Commands

| Command | Purpose |
|---------|---------|
| `dotnet build app/GHelper.sln` | Build (Debug, AnyCPU) |
| `dotnet publish app/GHelper.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true --no-self-contained` | Publish release EXE |
| `dotnet build app/GHelper.sln -c Release -r win-x64` | Build release |

- **No test project** exists. No unit tests, no test framework.
- **Build kills running GHelper processes** before building (see `.csproj` line 94 — only applies locally, not on CI).

## Project Structure

```
app/
├── Program.cs             # Entry point — sets up singletons, tray icon, event subs
├── AsusACPI.cs            # ACPI driver interface (DEVS/DSTS/INIT) — core HW comm
├── AppConfig.cs           # JSON config store (debounced write, atomic file ops)
├── HardwareControl.cs     # Static class — sensor polling, native battery API, fan/GpuControl refs
├── NativeMethods.cs       # P/Invoke helpers (idle time, screen off, lock, error lookup)
├── Settings.cs            # Main SettingsForm (partial class + .Designer.cs)
├── Fans.cs / Fans.Designer.cs   # Fan curve editor form
├── Extra.cs / Extra.Designer.cs  # Services/extra settings form
├── Matrix.cs / Matrix.Designer.cs # Anime Matrix/Slash display form
├── Updates.cs / Updates.Designer.cs # BIOS/driver updater form
├── Handheld.cs / Handheld.Designer.cs # ROG Ally controller form
├── AsusMouseSettings.cs   # Mouse settings form
│
├── UI/                    # Custom WinForms controls (RButton, RComboBox, RForm, Slider, etc.)
├── Mode/                  # Performance mode system (ModeControl, Modes, PowerNative)
├── GPU/                   # GPU mode switching + Nvidia/AMD control
├── Display/               # Screen brightness, refresh rate, overdrive, color profiles
├── Fan/                   # Fan sensor calibration + curve handling
├── Battery/               # Battery charge limit control
├── Input/                 # Keyboard hook, hotkey dispatcher, ACPI event listener
├── Overlay/               # Hardware overlay (FPS, temps, usage) with ETW FPS monitor
├── USB/                   # HID device access (Aura RGB, XG Mobile, AsusHid)
├── AnimeMatrix/           # Anime Matrix / Slash display + communication protocol
├── Peripherals/           # ASUS mouse detection + per-model configs
├── Pawn/                  # PawnIO — Ryzen SMU & Intel MSR access (embedded binaries)
├── Ally/                  # ROG Ally-specific controls
├── AutoUpdate/            # Auto-update checker
└── Helpers/               # Logger, AsusService, Audio, ClamshellMode, Startup, etc.
```

## Architecture & Control Flow

### Startup (`Program.cs`)

1. Parse CLI args (supports `charge`, `cpu`, `gpu`, `services`, etc.)
2. Set up localization from `config.json`
3. Create **global static singletons**: `settingsForm`, `modeControl`, `gpuControl`, `allyControl`, `clamshellControl`, `toast`, `hardwareOverlay`, `acpi`, `trayIcon`, `inputDispatcher`
4. Initialize ACPI — if `AsusACPI.IsConnected()` fails on an ASUS machine, show error and exit
5. Set up tray icon, context menu, input dispatcher (keyboard hook), XGM, aura, matrix
6. Subscribe to system events: `PowerModeChanged`, `SessionSwitch`, `DisplaySettingsChanged`, power setting notifications
7. Start sensor refresh timers

### Global Singletons (accessed via `Program.*`)

- `Program.acpi` — `AsusACPI` instance, all raw ACPI device communication
- `Program.modeControl` — `ModeControl`, performance/power/fan/UV mode application
- `Program.gpuControl` — `GPUModeControl`, GPU eco/standard/ultimate switching
- `Program.settingsForm` — `SettingsForm`, main UI window
- `Program.trayIcon` — `NotifyIcon`, system tray presence
- `Program.inputDispatcher` — `InputDispatcher`, keyboard/hotkey handling
- `Program.toast` — `ToastForm`, OSD toast notifications
- `HardwareControl.GpuControl` — `IGpuControl?`, Nvidia or AMD GPU sensor/OC interface

### Hardware Communication

Two main channels:

1. **ACPI** (`AsusACPI.cs`): Uses `\\.\ATKACPI` device with `DeviceSet` (DEVS) and `DeviceGet` (DSTS) IOCTL calls. Device IDs are `uint` constants (e.g., `0x00120075` for PerformanceMode, `0x00120057` for BatteryLimit).
2. **HID** (`AsusHid.cs` / `Aura.cs`): Uses `\\.\ASUSINPUT` for keyboard backlight, Aura RGB, and per-key lighting.

### Configuration (`AppConfig.cs`)

- JSON file stored at `%APPDATA%\GHelper\config.json` (with fallback to startup dir and `%COMMON_APPDATA%\GHelper\config.json`)
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

- Applied via ACPI `PerformanceMode` + fan curves + power limits + CPU boost + GPU clocks
- Mode settings stored as `{key}_{modeID}` in config (e.g., `limit_total_0`, `fan_profile_cpu_1`)
- Auto-mode switching on power state change (AC ↔ battery) is configurable
- Reapply timer for Ryzen CPUs that silently reset temp limits under load

### GPU Modes (`GPU/`)

Four GPU modes communicated via ACPI:
| Mode | Value | ACPI call |
|------|-------|-----------|
| Eco (iGPU only) | 0 | `SetGPUEco(1)` |
| Standard | 1 | `SetGPUEco(0)` |
| Ultimate (dGPU only, MUX switch) | 2 | `GPUMux = 0` |
| Optimized (auto-switch) | — | Switches based on power state |

- Ultimate mode requires **restart** (MUX switch)
- Eco mode stops NV services, kills GPU apps
- Standard mode restarts NV services after delay

### Custom UI Controls (`UI/`)

All custom WinForms controls in the `GHelper.UI` namespace:
- `RForm` — base form with dark/light theme, color constants, DWM titlebar
- `RButton`, `RBadgeButton` — custom-painted buttons with border colors
- `RComboBox` — themed combobox
- `RCheckBox`, `RTrackBar`, `RNumericUpDown`, `RTextBox`, `Slider` — themed controls
- `CustomContextMenu` — drawn context menu

### Localization

Labels use `Properties.Strings.{Key}` — resources are in `.resx` files. Localization is handled via standard .NET satellite assemblies (Crowdin-managed).

## Key Patterns & Gotchas

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

### ACPI Quirk Handling
The codebase has model-specific workarounds:
- `AppConfig.IsASUS()`, `AppConfig.IsTUF()`, `IsVivoZenPro()`, `IsAlly()`, `IsDUO()`, `IsZ13()`, `IsPZ13()` — model detection based on WMI `Win32_ComputerSystem.Model`
- Vivo/Zenbook models use different ACPI device IDs (e.g., `GPUEcoVivo` vs `GPUEcoROG`)
- Vivobook has fallback `SetVivoMode()` when standard performance mode ACPI calls fail
- G14 2024 has a workaround (`IsResetRequired`) for power limits not resetting properly
- Some Ryzen families (Renoir, Mobile, Raphael) have timer-based reapply workarounds for temp limits and power settings that silently reset under load

### Fan Profiles
Fan curves are stored as byte arrays serialized to config strings (`fan_profile_cpu_{mode}`, etc.). The `FanSensorControl.cs` contains **per-model fan max tables** — every model has different max RPM values. Calibration runs the fans at full speed for ~15 seconds to measure actual max.

### Mouse Peripheral Models
Mice are modeled as individual classes under `Peripherals/Mouse/Models/` with per-model feature support (DPI, polling rate, buttons, etc.). Detection is HID-based via `HidSharp`.

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
- Do not hardcode model names in new code without adding them to the switch tables in `FanSensorControl.cs` and `AppConfig.cs`
- Do not try to build or run this on anything except Windows — it uses WinForms, P/Invoke, and NT device paths
- Do not change `.Designer.cs` files unless absolutely necessary, and if you do, keep the designer code in the `#region Windows Form Designer generated code` block
