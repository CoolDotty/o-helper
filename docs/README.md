# O-Helper

O-Helper is a lightweight Windows Forms tray application for HP OMEN laptops. It is being refactored from an ASUS-focused upstream codebase into an HP OMEN controller that uses HP WMI BIOS communication through `hpqBIntM` / `hpqBDataIn` in `root\wmi`.

<img width="518" height="648" alt="image" src="https://github.com/user-attachments/assets/a7ed4210-842c-4eaa-8eba-3f910a5f5979" />

## Current Scope

- Performance mode switching for HP OMEN firmware modes.
- Fan, power, GPU, display, keyboard lighting, overlay, and peripheral controls where confirmed HP paths exist.
- Graceful fallback or hidden UI for ASUS-only inherited features.
- Configuration in `%APPDATA%\OHelper\config.json`.

## Build

```powershell
dotnet build app/OHelper.sln
```

For local elevated launch:

```powershell
dev.bat
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

The repository still contains inherited ASUS peripheral and compatibility code. Runtime UI paths for HP OMEN should either use confirmed HP implementations or stay hidden/guarded until an HP equivalent exists.
