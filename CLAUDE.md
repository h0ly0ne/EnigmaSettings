# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

EnigmaSettings is an open-source C# **library** (no executable) for reading, manipulating, and writing
Enigma1 / Enigma2 set-top-box channel settings (lamedb / services / bouquets / `satellites.xml`).
It supports Enigma1, Enigma2 ver3, and Enigma2 ver4 formats and automatic conversion between them.
Root namespace for all projects is `Krkadoni.EnigmaSettings`. Licensed MIT.

## Project layout — one source set, three target frameworks

All three projects compile **the same `.cs` files** that physically live in `EnigmaSettings/`. There is no
per-target source fork; changes to a file in `EnigmaSettings/` affect every target. Edit source there.

| Project | Target | Style | How it includes source |
|---|---|---|---|
| `EnigmaSettings/EnigmaSettings.csproj` | .NET Framework 4.0 | classic MSBuild | explicit `<Compile Include>` list |
| `EnigmaSettings.NetStandard/` | netstandard2.0 | SDK-style (primary modern target) | glob `..\EnigmaSettings\**\*.cs` |
| `EnigmaSettingsPCL/` | PCL Profile111 | classic MSBuild | linked `<Compile Include="..\EnigmaSettings\...">` |

Consequences to keep in mind when adding/removing source files:
- **New `.cs` file in `EnigmaSettings/`** → it is auto-picked-up by the NetStandard glob, but you must
  **manually add `<Compile Include>` entries** to both `EnigmaSettings.csproj` (plain) and
  `EnigmaSettingsPCL.csproj` (with a `<Link>`), or those targets won't see it.
- `Resources.resx` is embedded; the NetStandard project sets `<LogicalName>Krkadoni.EnigmaSettings.Resources.resources</LogicalName>` so `Resources.Designer.cs` can resolve it. Don't rename without updating this.
- The net40 project does **not** include `Interfaces/IPathProvider.cs`; PCL/NetStandard do. PCL also supplies
  its own `Interfaces/ICloneable.cs` because PCL lacks `System.ICloneable`.

## Build

There are **no tests** and **no CI** in this repo (`.github/workflows/` is empty). "Verifying" a change means
it compiles on the target(s) you touched.

- **netstandard2.0 (preferred, cross-platform):**
  `dotnet build EnigmaSettings.NetStandard/EnigmaSettings.NetStandard.csproj`
- **net40 and PCL Profile111:** these legacy targets need full **MSBuild / Visual Studio** (not `dotnet build`).
  Use the `vs-build` skill, or `msbuild EnigmaSettings.sln`. PCL Profile111 also requires the matching
  PCL targeting pack installed.
- Build everything via the solution with MSBuild: `msbuild EnigmaSettings.sln /p:Configuration=Release`.

Use the `vs-build` skill for building — it selects MSBuild vs `dotnet` correctly per target.

## Architecture

The library is fully **interface-driven** (every domain type has an `I*` interface in `Interfaces/`) so any
part can be replaced. Three pillars:

1. **`SettingsIO` (`ISettingsIO`)** — the load/save engine. `Load(file)` parses a settings file directory into
   a `Settings` graph; `Save(folder, settings)` writes it back. Sync + `*Async` variants exist. It owns all the
   file-format constants (`eDVB services /`, `bouquets`, `userbouquets.tv.epl`, `services.locked`, default
   Enigma1 path `/var/tuxbox/config/enigma/`, etc.). `XmlSatellitesIO` handles `satellites.xml` separately.

2. **`Settings` (`ISettings`)** — the in-memory aggregate root holding `Bouquets`, `Services`, `Transponders`,
   `Satellites`. It exposes the high-level domain operations the library exists for, e.g.
   `ChangeSatellitePosition`, `RemoveStreams`, `RemoveEmptyBouquets`, `RemoveSatellite`,
   `MatchServicesWithTransponders`, `AddMissingXmlSatellites/Transponders`, `RenumberMarkers`. Mutations here
   keep the cross-references (services↔transponders↔satellites↔bouquets) consistent.

3. **`InstanceFactory` (`IInstanceFactory`)** — IoC seam. Every concrete object (services, transponders,
   bouquet items, etc.) is created through this factory rather than `new`, so consumers can inject custom
   implementations. When adding a new domain type, add an `InitNew*` method here and to `IInstanceFactory`.

### Domain model
- **Transponders**: base `Transponder` + `TransponderDVBS` / `TransponderDVBT` / `TransponderDVBC`
  (satellite / terrestrial / cable). `XmlTransponder` / `XmlSatellite` are the `satellites.xml` representations.
- **Bouquets**: `IBouquet` with `FileBouquet` (Enigma2 userbouquet files) and `BouquetsBouquet` (Enigma1).
  A bouquet contains `BouquetItem`s — polymorphic: `BouquetItemService`, `BouquetItemMarker`,
  `BouquetItemStream`, `BouquetItemFileBouquet`, `BouquetItemBouquetsBouquet`.
- **Service** + `Flag` model the channel and its raw C/F/P flags.
- All enums (service/transponder types, DVB parameters, `SettingsVersion`, flag types) live in one place:
  `Interfaces/Enums.cs`.

### GUI-binding & logging conventions (followed throughout)
- Domain types implement `INotifyPropertyChanged` **and** `IEditableObject` (`BeginEdit`/`EndEdit`/`CancelEdit`
  snapshot-and-rollback). Preserve this pattern when adding properties — back them with fields, raise
  `OnPropertyChanged`, and snapshot them in `BeginEdit`.
- Logging goes through `ILog`; default is `NullLogger` (no-op) with `DebugLogger` available. Code accepts an
  `ILog` rather than logging directly to a sink.

## Source control

This is a **git** repo (unlike most sibling projects under `C:\zeba\` which are TFVC). Normal git workflow applies.
