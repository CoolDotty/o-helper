# O-Helper

O-Helper is a lightweight Windows Forms tray application for supported HP OMEN systems. It provides hardware controls through HP WMI BIOS communication via `hpqBIntM` / `hpqBDataIn` in `root\wmi`, with features enabled according to detected model capabilities.

<img width="518" height="648" alt="image" src="https://github.com/user-attachments/assets/a7ed4210-842c-4eaa-8eba-3f910a5f5979" />

## Current Scope

- Performance mode switching for HP OMEN firmware modes.
- Fan, power, GPU, display, keyboard lighting, overlay, and peripheral controls where confirmed HP paths exist.
- Graceful fallback or hidden UI for unsupported hardware features.
- Configuration in `%APPDATA%\OHelper\config.json`.

## Build

```powershell
dotnet build app/OHelper.sln
```

Release publish:

```powershell
dotnet publish app/OHelper.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true --no-self-contained
```

## Runtime Requirements

O-Helper is Windows-only and requires administrator privileges for HP WMI BIOS operations. Without elevation, calls to `root\wmi` with privileges enabled can fail with access denied.

## Feature Status

See [OMEN_FEATURE_AUDIT.md](OMEN_FEATURE_AUDIT.md) for the current GPU, Advanced tab, ASUS-dormant path, and unsupported WMI stub audit.

## Notes

Runtime UI paths should use confirmed HP implementations or stay hidden/guarded until a supported implementation exists for the detected hardware.
