<div align="center">
  <h1>Rexon Menu</h1>
  <p><strong>A reconstructed, buildable C# Gorilla Tag Unity mod menu.</strong></p>

  <p>
    <img alt=".NET Framework 4.7.2" src="https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4?logo=dotnet&logoColor=white">
    <img alt="C# 14" src="https://img.shields.io/badge/C%23-14.0-239120?logo=csharp&logoColor=white">
    <img alt="Platform: Windows x64" src="https://img.shields.io/badge/platform-Windows_x64-0078D4?logo=windows&logoColor=white">
    <img alt="Build configuration: CompileOnly" src="https://img.shields.io/badge/config-CompileOnly-2ea44f">
  </p>

  <p>
    <a href="#overview">Overview</a> •
    <a href="#project-layout">Project layout</a> •
    <a href="#building">Building</a> •
    <a href="#build-output">Output</a> •
    <a href="#troubleshooting">Troubleshooting</a>
  </p>
</div>

> [!IMPORTANT]
> Join the discord: https://discord.gg/wyvern also all of this is written by deepseek, I can not be asked to write anything about this mod menu. Also the entrypoint is Loading Loader Load so install SharpMonoInjector to load it. Run it by downloading SharpMonoInjector console version and then doing: smi.exe inject -p "Gorilla Tag" -a TheDLLName.dll -n Loading -c Loader -m Load

## Overview

Rexon Menu is organized as a modular Unity menu with separate runtime bridges for game-state access and shader/material handling. The main assembly contains the menu interface, bootstrap logic, input handling, configuration, utilities, patches, and the recovered module catalog.

The project targets **.NET Framework 4.7.2**, **C# 14**, and **Windows x64**. It compiles against the Gorilla Tag managed assemblies and BepInEx/Harmony dependencies from a local game installation.

### Highlights

- Modular component architecture grouped by movement, visuals, room utilities, settings, and other feature areas.
- Input abstraction for both XR controllers and desktop controls.
- Separate material/game-state and shader bridges.
- Embedded UI bundles and audio assets.
- Companion DLLs embedded as named manifest resources and loaded in memory with `Assembly.Load(byte[])`.
- Resource assembly-name validation before startup continues.
- A single main runtime artifact; the two companion projects do not need to be deployed beside it.

## Project layout

```text
RexonMenu/
├── Rexon_FullSource.sln
├── Rexon_Menu/                         Main menu and runtime assembly
│   ├── Core/                           Bootstrap, modules, patches, and utilities
│   ├── Interface/                      Menu UI and bundle management
│   ├── Loading/                        Public entry point and in-memory DLL loader
│   ├── Resources/                      Embedded UI/audio assets
│   └── references/compile-only-publicized/
├── Rexon_Menu_Mat/                     Game-state and material bridge
└── Rexon_Shader/                       Shader bridge and Harmony patch
```

| Project | Purpose | Build output |
| --- | --- | --- |
| `Rexon_Menu` | Main menu, loader, interface, modules, and resources | `Rexon_Menu.dll` |
| `Rexon_Menu_Mat` | Photon/game-state bridge used by the main assembly | `Rexon-Menu-Mat.dll` |
| `Rexon_Shader` | Shader lookup, material helpers, and primitive patch | `Rexon-Shader.dll` |

During a full build, the two companion outputs are embedded into `Rexon_Menu.dll` under stable manifest-resource names. The startup loader reads those resources into memory and loads them before any code that depends on the companion assemblies runs.

## Requirements

- Windows x64.
- A local Steam installation of Gorilla Tag.
- BepInEx installed in the Gorilla Tag directory, including `BepInEx.dll` and `0Harmony.dll` under `BepInEx/core`.
- Visual Studio/MSBuild 18 or newer with C# 14 support.
- The .NET Framework 4.7.2 Developer Pack/reference assemblies.

The repository already contains its compile-only publicized references under `Rexon_Menu/references/compile-only-publicized`. Other Unity and game assemblies are resolved from the installed game.

By default, every project expects Gorilla Tag at:

```text
C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag
```

## Building

### Visual Studio

1. Open `Rexon_FullSource.sln`.
2. Select the `CompileOnly` configuration.
3. Select the `x64` platform.
4. Choose **Build → Build Solution**.

### Developer PowerShell

Open a Visual Studio Developer PowerShell in the repository root and run:

```powershell
msbuild .\Rexon_FullSource.sln /restore /t:Build /p:Configuration=CompileOnly /p:Platform=x64 /m
```

For Gorilla Tag installed in a different Steam library, override `GorillaTagRoot` for the entire solution:

```powershell
msbuild .\Rexon_FullSource.sln /restore /t:Build /p:Configuration=CompileOnly /p:Platform=x64 "/p:GorillaTagRoot=D:\SteamLibrary\steamapps\common\Gorilla Tag" /m
```

The build performs these steps automatically:

1. Compiles `Rexon-Menu-Mat.dll` and `Rexon-Shader.dll`.
2. Embeds both outputs as named resources in the main project.
3. Compiles the resources into `Rexon_Menu.dll`.

> [!NOTE]
> Build the full solution after changing either companion project. Project references ensure the bridge DLLs are rebuilt before the main assembly embeds them.

## Build output

Successful builds are written to the following directories:

```text
Rexon_Menu/artifacts/compile-only/Rexon_Menu.dll
Rexon_Menu_Mat/artifacts/compile-only/Rexon-Menu-Mat.dll
Rexon_Shader/artifacts/compile-only/Rexon-Shader.dll
```

`Rexon_Menu.dll` is the main runtime artifact. The material and shader assemblies are also emitted as normal project outputs, but they are already embedded in the main DLL and loaded before code that depends on them runs.

## Startup flow

```text
Loading.Loader.Load()
        │
        ├── loads Rexon-Menu-Mat from its embedded resource
        ├── loads Rexon-Shader from its embedded resource
        ├── initializes MatBridge
        └── starts the main Bootstrapper
                ├── applies Harmony patches
                ├── creates the persistent menu runtime
                └── creates the notification runtime
```

## Troubleshooting

| Problem | Resolution |
| --- | --- |
| `MSB3644` or missing .NET Framework reference assemblies | Install the .NET Framework 4.7.2 Developer Pack. |
| `CS1617` reports that C# 14 is unsupported | Build with Visual Studio/MSBuild 18+ or another compiler with C# 14 support. |
| Unity, Gorilla Tag, BepInEx, or Harmony references are missing | Verify `GorillaTagRoot` and confirm BepInEx is installed under the selected game directory. |
| A companion DLL resource is missing | Build `Rexon_FullSource.sln`, not an isolated `CoreCompile` target. Project references must finish before the main assembly is compiled. |
| The build reports `CS0618` warnings | The current source uses several game/Unity APIs now marked obsolete. These warnings do not prevent a successful build. |

## Responsible use

This source includes modules that can affect gameplay and networking. Use it only in environments and on accounts where you have permission, and follow the game platform's rules. This project is not affiliated with or endorsed by Another Axiom.
