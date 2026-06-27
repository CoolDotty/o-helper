# OMEN Feature Audit

This audit records the current HP OMEN implementation state for hardware controls, unsupported features, and guarded compatibility paths.

## GPU Tab

| Control | Backend | State | Notes |
| --- | --- | --- | --- |
| GPU max clock | `NvidiaGpuControl.SetMaxGPUClock()` | NVIDIA-only | Hidden unless the active GPU backend is NVIDIA. |
| GPU core offset | `NvidiaGpuControl.SetClocks()` | NVIDIA-only | Hidden unless the active GPU backend is NVIDIA. |
| GPU memory offset | `NvidiaGpuControl.SetClocks()` | NVIDIA-only | Hidden unless the active GPU backend is NVIDIA. |
| GPU variable TGP | HP WMI `0x20008 / 0x22` | Implemented when supported | Vendor-independent on OMEN models that expose GPU Power Boost. Hidden when `ModelCapabilities.SupportsGpuPowerBoost` is false, even if the firmware status blob is readable. |
| Dynamic Boost | HP WMI `PPT_GPUC0` through `0x20008 / 0x41` | Implemented when supported | Visible only when ACPI support probing succeeds. |
| GPU thermal target | HP WMI `PPT_GPUC2` through `0x20008 / 0x41` | Implemented when supported | Visible only when ACPI support probing succeeds. |

The GPU tab should be visible when any vendor-independent WMI GPU control is supported, even when NVIDIA clock controls are unavailable.

## Advanced Tab

| Control | Backend | State | Notes |
| --- | --- | --- | --- |
| CPU temperature limit | PawnIO Ryzen SMU | AMD-only | Hidden unless Ryzen/PawnIO support is present. |
| CPU undervolt | PawnIO Ryzen SMU | AMD-only | Hidden unless supported by `CpuInfo`. |
| iGPU undervolt | PawnIO Ryzen SMU | AMD-only | Hidden unless supported by `CpuInfo`. |
| PawnIO install/download | PawnIO installer path | AMD-only | Shown only for AMD platforms that can use the Advanced tab. |
| Intel undervolt | Intel MSR | Missing | `IntelMSR.bin` exists, but no Intel UV UI/backend is wired in this app. Intel systems show a status note instead of a dead tab. |

## Display / Dynamic Refresh

| Control | Backend | State | Notes |
| --- | --- | --- | --- |
| Auto/60Hz/120Hz/Dynamic refresh | Windows CCD virtual refresh path flags | Implemented for known supported models | Gated by `ModelCapabilities.SupportsDynamicRefresh`, with a Transcend 14 string fallback and `force_dynamic_refresh` config escape hatch. |
| MiniLED / multizone toggle | HP display WMI stubs | Hidden unless supported | Dynamic refresh has its own button; it no longer shares the MiniLED control. |

## Auto Performance Mode

Auto switching is controlled by `auto_mode_enabled`, `auto_mode_ac`, and `auto_mode_dc`. When enabled, startup and power-source changes apply the configured AC or battery mode after the `auto_mode_delay` debounce.

`manual_mode` has precedence over auto switching. If `manual_mode` is set, the app skips startup and power-source auto mode changes so users can keep direct manual control. Manual mode selection still uses the normal `SetPerformanceMode()` path, including Windows power mode updates, unless `no_windows_power_mode` or the legacy `skip_powermode` flag is set.

Windows power mode is synchronized through `PowerSetActiveOverlayScheme` unless disabled with `no_windows_power_mode` or `skip_powermode`:

| O-Helper mode | Windows power mode |
| --- | --- |
| Turbo / Performance (`1`) | Best Performance |
| Balanced (`0`) | Balanced |
| Silent / Eco (`2`) | Best Power Efficiency |
| Unleashed (`4`) | Best Performance |
| Custom (`3+`, except `4`) | Balanced unless the mode has a `powermode` override |

## Fan Curves

| Control | Backend | State | Notes |
| --- | --- | --- | --- |
| Custom fan curves | Software loop + HP WMI `0x20008 / 0x46` target blob | Implemented | Direct curve upload is not confirmed on HP. `SetFanCurve()` and `SetFanRange()` return unsupported so `ModeControl` starts the software loop, evaluates the saved curve, and writes CPU/GPU target levels through the 128-byte performance-status blob. |
| Max fans | HP WMI `0x20008 / 0x27` | Implemented | Software fan curve reapply is paused while max fans is active. |

## Removed Or Disabled ASUS Paths

| Feature | State | Notes |
| --- | --- | --- |
| XG Mobile toggle/fan/light | Disabled | `AppConfig.IsASUS()` is hard false for HP OMEN systems, XGM startup/hotkey routes are removed, and HP systems do not scan or write ASUS XGM HID devices. |
| Legacy utility actions | Removed | No OMEN UI path launches vendor-specific uninstallers or utility managers. |
| ASUS services | Disabled | The services panel is hidden and the command-line services action is skipped. |
| ASUS driver/BIOS updater tab | Disabled | The tab is hidden and the old ROG web API calls have been removed until an HP source is implemented. |
| ASUS mouse peripheral scanner | Disabled | Startup detection, HID device-change registration, and follow-up scans do not run because ASUS detection is hard-disabled. |
| ASUS HID hotkey listener | Disabled | The `KeyboardListener` initialization and XGM hotkey routes are removed; Omen keyboard/backlight actions use WMI paths. |
| ASUS Aura keyboard fallback | Disabled | Backlight timeout/startup/hotkey/tent-mode fallback calls are unreachable because ASUS detection is hard-disabled. |

## Unsupported HP WMI Stubs

| Method | State | Rationale |
| --- | --- | --- |
| `SetGpuXg()` | Unsupported | XG Mobile is ASUS-only and has no HP OMEN equivalent. |
| `SetAPUMem()` | Unsupported | No confirmed HP WMI command is known. The APU memory panel stays hidden because `GetAPUMem()` returns `-1`. |
| `SetCores()` | Unsupported | No confirmed HP WMI command is known. The CPU cores panel stays hidden because `GetCores()` returns `(-1, -1)`. |
| `SetFanHysteresis()` | Unsupported in firmware | The software fan loop applies local hysteresis; the firmware panel stays hidden because `GetFanHysteresis()` returns `(-1, -1)`. |

## GPU Power Payload

`HpACPI.SetGpuPowerLimit()` maps the existing GPU power value onto HP's preset-style payload:

- `0` sends `{ customTgp: 0, ppab: 0 }` for base TGP.
- The minimum nonzero UI value sends `{ customTgp: 1, ppab: 0 }` for custom TGP.
- Higher values send `{ customTgp: 1, ppab: value - 1 }` for custom TGP plus PPAB/Dynamic Boost levels.

`PPT_GPUC0` and `PPT_GPUC2` are kept on the separate `Tpptdp (0x41)` path, so Dynamic Boost and GPU temperature target writes do not stomp the GPU power preset command.

Victus capability records explicitly set `SupportsGpuPowerBoost = false`, matching field reports that those BIOSes can expose WMI fan/status paths without supporting custom TGP/PPAB writes.
