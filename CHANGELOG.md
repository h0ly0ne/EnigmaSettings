# Changelog

## Unreleased

### Fixed

- **Enigma1 `bouquets` file could never be read.** The parser cut the content with an end
  offset computed before an earlier `Substring`, so every E1 list with a `bouquets` file failed
  with `ArgumentOutOfRangeException`. It is now parsed line by line (LF and CRLF).
- **Enigma1 load no longer fails when `bouquets` or `services.locked` is missing.**
- **`IService.SourceID` was dropped on save** for every service that has a transponder. It is now
  written for Enigma2 (v3/v4/v5) and never for Enigma1.
- **IPTV transponders and their services were lost on save.** `ITransponderIPTV` entries are now
  written (v3/v4/v5 and Enigma1) and lamedb5 reads them back as IPTV instead of DVB-S.
- **Culture-dependent file output.** Numbers were formatted/parsed with the current culture, so on
  e.g. sv-SE / nb-NO a western orbital position was written as `−300` (U+2212) and the file could
  not be loaded again. All file parsing/formatting now uses the invariant culture and string
  comparisons are ordinal.
- **`LoadAsync` / `SaveAsync` always threw `PlatformNotSupportedException`** (`Delegate.BeginInvoke`
  does not exist on .NET Core/.NET 5+). They now run on the thread pool; the `IAsyncResult` passed
  to a legacy callback is the `Task` / `Task<ISettings>`.
- **Blank lines in `whitelist` / `blacklist`** aborted the whole load; they are skipped now.
- **`whitelist`, `blacklist`, `services.locked` and the E1 `bouquets` file were written with a
  UTF-8 BOM**, which corrupts the first entry on the receiver. They are written without BOM.
- **`satellites.xml` / `cables.xml` encoding.** Files were always read as ISO-8859-1 (ignoring the
  declaration, so UTF-8 names became e.g. `TÃ¼rksat`) and written as UTF-8 bytes under an
  `iso-8859-1` declaration. The reader now honours BOM/declaration; the writer emits real
  ISO-8859-1 (characters outside Latin-1 as character references).
- `Save(..., xmlSatellitesIO, xmlCablesIO, ...)` ignored the passed writers and always used the
  factory defaults.
- `#SERVICE` lines with fewer than 11 fields or a non-numeric line specifier no longer abort the load.
- `SettingsLoaded` / `SettingsSaved` were raised twice (failure and then success/failure).
- Null reference when a sub bouquet of an Enigma1 list could not be read, and when writing
  `services.locked` with a bouquet reference that has no bouquet.
- Service lists: entries after the `end` tag are no longer read; a transponder without data line no
  longer throws.
- Enigma1 save dropped DVB-S transponders/services at orbital position 0.
- Namespaces shorter than 8 digits were right-padded (changing the value); they are left-padded.
- `RemoveTransponder(s)` threw for services without a transponder.
- `ArgumentNullException`s now carry the parameter name instead of the message as parameter name.
- **DVB-T convenience enums did not match the Enigma2 coding** (`lib/dvb/frontendparms.h`).
  `BandwidthType`, `FECHighType`, `FECLowType`, `ModulationType`, `TransmissionType`,
  `GuardIntervalType`, `HierarchyType` and `InversionType` returned wrong members (e.g. modulation
  `2` = QAM64 was reported as `Qam16`, bandwidth `0` = 8 MHz as `Auto`). The enums now use the
  Enigma2 values and gained the missing members (`Qam256`, 5/10/1.712 MHz, 1k/4k/16k/32k,
  1/128, 19/128, 19/256, FEC 6/7, 8/9, 3/5, 4/5). `DVBTModulationType` no longer has the
  `Auto`/`QPSK` value collision. The corrections announced in the format modernization entry
  below are only now actually in the code.
- **DVB-S `RollOffType` / `PilotType`** now follow the Enigma2 coding (roll-off `2` = 0.20,
  `3` = auto; pilot `0` = off, `1` = on, `2` = auto). Previously roll-off `2` was `Unknown` and
  pilot `0`/`2` were swapped.
- **`AddMissingXmlSatellites` never added a missing satellite** (inverted condition) and logged a
  misleading "position 0 degrees" warning instead. Missing satellites are created again and linked
  to their transponders.
- `AddMissingXmlTranspondersSatellites` threw for transponders without satellite (position 0) and
  for non-numeric frequencies.
- **Sub bouquets were promoted to the main list on save.** The loader now records in the new
  `IFileBouquet.IsSubBouquet` whether a bouquet is only referenced from another bouquet; such
  bouquets are written as files but not listed in `bouquets.tv` / `bouquets.radio`. A bouquet
  that is listed there and also referenced from another bouquet stays listed. Set
  `IsSubBouquet = true` for sub bouquets created through the API.
- **Stream URLs set through the API broke the bouquet line.** `IBouquetItemStream.URL` is now
  always the decoded URL and is encoded like Enigma2 does when written (`:` / `%` / control
  characters as `%xx`). Previously `URL` returned the encoded form after loading, and a plain URL
  set by the caller was written with raw `:`. Stream lines with fewer than 11 fields no longer
  throw.
- `RemoveEmptyBouquets` left references to the removed sub bouquets in their parents; they are
  removed too (repeated until no empty bouquet remains).
- `CalculatedNameSpace` / ONID-TSID validation treated orbital position 0 as 360.0°.
- `RenumberBouquetFileNames` numbered by file name text (`dbe10` before `dbe2`) instead of the
  bouquet order.
- **`satellites.xml` / `cables.xml` used the wrong FEC attribute.** Enigma reads and writes
  `fec_inner`; the library used `fec`, so the FEC was never read and written under a name Enigma
  ignores. `fec_inner` is used now (`fec` from files written by older versions is still read).
  `tsid` / `onid` of satellite transponders and `inversion` / `system` of cable transponders are
  kept (new `IXmlTransponder.Tsid` / `Onid`).
- **A bouquet that references itself (directly or through sub bouquets) crashed the process** with
  an uncatchable `StackOverflowException`. The reference is now ignored with a warning, and a
  bouquet referenced several times is read once per load and shared.
- **Custom service names in bouquets were lost.** A `#DESCRIPTION` line after a normal service
  (renamed channel) is kept in the new `IBouquetItemService.Description` and written back.
- **`Settings.Clone()` produced an inconsistent copy.** Every copied service had its own transponder
  copy instead of the one in `Transponders`, copied transponders had their own satellite copies,
  and bouquet items still pointed to the original services and sub bouquets. Saving a modified
  copy (e.g. after `ChangeSatellitePosition`) wrote services with the old namespace. All references
  of the copy now point to the copied objects.
- **Copies raised `PropertyChanged` on the subscribers of the original** (`MemberwiseClone` copied
  the event). `Clone()` / `ShallowCopy()` of all model types start without subscribers.
- **`IService.UpdateFlags(list)` ignored the list** and rebuilt the flags from the current ones.
- **`IFlag.FlagInt` corrupted `c:` flags** (hex value converted to decimal, type byte lost, values
  with A-F threw). `c:` is written as 6 hex digits, `f:` as hex.
- `IFlag.FlagValue` cut values containing `:` (e.g. provider `Sky:DE`).
- `IService.ServiceId` of a service without transponder had empty fields for zero values
  (`1:::1:c00000`).
- **Enigma1 cable transponders were not read** (Enigma1 stores them as `s` lines with an `ffff`
  namespace, only `c` lines were parsed): frequency was `0`, symbol rate `null`, and saving the list
  failed with a `NullReferenceException`.
- **Bouquet entries without matching service were dropped on save.** References to services that
  are not in lamedb (or have no transponder) are written back unchanged, including their
  `#DESCRIPTION`, like Enigma does. Use `ISettings.RemoveInvalidBouquetItems()` to remove them.
- `InstanceFactory.InitNewXmlTransponder(ITransponderDVBC)` copies inversion and system.
- **Enigma1 → Enigma2 conversion wrote wrong tuning data.** Enigma1 terrestrial transponders only
  store frequency and inversion; the missing fields were written as `0`, which Enigma2 reads as
  8 MHz / FEC 1/2 / QPSK / 2k / 1/32 / no hierarchy. They are now set to "auto" (and flags,
  system, PLP to 0). Enigma1 cable transponders are converted to Enigma2 units (frequency / 100,
  symbol rate / 10 — the Enigma1 writer multiplies again) with auto modulation, so they are no
  longer written as `c 11300000:69000000:2::0::`.
- `FindSatelliteTransponders` / `FindCableTransponders` sorted the frequency as text
  (`10700000` before `9750000`).
- `FindTranspondersForSatellite` / `FindServicesForSatellite` threw a `FormatException` for a
  satellite with a non-numeric position; they return an empty list now.
- `FindBouquetsWithDuplicateFilename` compares the file name without directory and ignoring case,
  so two bouquets that end up in the same file on Windows (`userbouquet.Fav.tv` /
  `userbouquet.fav.tv`) are reported and `Save` refuses them instead of silently overwriting one.

### Changed

- New `Task` based overloads `LoadAsync(..., CancellationToken)` and `SaveAsync(..., CancellationToken)`
  on `SettingsIO` and (as default interface members) on `ISettingsIO`.
- `ILog.IsDebugEnabled` (default `true`, `NullLogger` returns `false`); debug messages are only
  built when it is enabled, which saves most string allocations while loading/saving large lists.
- `RemoveServices` / `RemoveTransponders` / `RemoveService` / `RemoveTransponder` run in linear
  time (hash sets, single pass per list) instead of O(n·m).
- `IFileHelper.OpenRead` (default interface member); the XML readers go through `IFileHelper`
  and the file helper is no longer a static field shared by all `XmlSatellitesIO`/`XmlCablesIO`
  instances. `SerializerSatellites`/`SerializerCables` got `LoadFromFile`/`SaveToFile` overloads
  taking an `IFileHelper`.
- `FileHelper` writes atomically (temporary file, then replace) and plain `WriteAllText` uses UTF-8
  without BOM explicitly.
- Library is no longer tied to `win-x64` (no `RuntimeIdentifier`), publish profiles are not
  self-contained, Debug builds emit portable PDBs again.
- `*.user` files are no longer tracked.
- `SettingsIO(IInstanceFactory, IFileHelper)` constructor and `SettingsIO.FileProvider` property;
  all settings, bouquet and xml files go through the given `IFileHelper`.
- Service / bouquet matching joins use `StringComparer.OrdinalIgnoreCase` instead of lower-casing
  every key.
- `IFileBouquet.BouquetType` is cached per file name instead of being recomputed on every access.
- `RemoveStreams`, `RemoveInvalidBouquetItems` and `RemoveEmptyBouquets` remove in one pass per list.
- Settings files are no longer deleted before writing (the atomic write replaces them).
- `IService.FlagList` creates flags through a compiled constructor delegate instead of
  `Activator.CreateInstance` and returns an empty list when no flags are set (was a
  `NullReferenceException`).
- `ISettings.SettingsDirectory` raises `PropertyChanged`.
- `IXmlSatellite.PositionString` always shows one decimal (`13.0° E`, `0.0° E` instead of `13° E` / `° E`).
- `Settings.Clone()` copies flat and relinks instead of deep-cloning every reference
  (48 000 services / 120 bouquets: 808 ms and 2.4 GB allocations before, 36 ms and 23 MB now).
- `Clone()` of bouquet items and transponders is `virtual` / `override` instead of `new`, so the
  same (deep) copy is made whether it is called through the class or an interface.
- `ITransponder.TransponderId` is cached until `NameSpc`, `TSID` or `NID` change.
- `IService.ServiceType` maps the numeric type directly instead of a 35-case switch.
- Enigma1 and Enigma2 bouquet writers share one implementation (`BouquetToString`,
  one list writer per format for TV and radio); `#DESCRIPTION` parsing shared by markers, streams
  and services.
- `SettingsIO` split into `SettingsIO.cs`, `SettingsIO.Reading.cs` and `SettingsIO.Writing.cs`.
- `SettingsIO.ReadText`, `DeleteFileIfExists` and `GetBouquetNameFromFileName` are no longer used
  and marked `[Obsolete]`.
- Prefix / suffix checks use `StartsWith` / `EndsWith` with `OrdinalIgnoreCase` instead of
  upper-/lower-casing a copy of every line (reading 48 000 bouquet entries: 129 ms / 68 MB →
  112 ms / 59 MB).
- `SerializerSatellites` / `SerializerCables` derive from the new `XmlFileRoot<T>` that holds the
  shared serializer, `Serialize`, `Deserialize`, `LoadFromFile` and `SaveToFile`.
- Removed 7 unused resource strings, commented-out code and unused `using static` directives.

### Breaking

- `SettingsIO._fileHelper` is `protected` (was a public field); use `SettingsIO.FileProvider`.
- Values of the DVB-T enums, `DVBSRollOffType.X20` and `DVBSPilotType` changed to the Enigma2
  coding (see Fixed). Code that persisted or compared the numeric enum values must be adjusted.
- `IBouquetItemStream.URL` returns the decoded URL (see Fixed).
- New interface members `IXmlTransponder.Tsid` / `Onid`, `IBouquetItemService.Description` and
  `IFileBouquet.IsSubBouquet`;
  own implementations of these interfaces must add them.

### Removed (breaking)

- `ObjectCopier` — it relied on `BinaryFormatter` (removed in .NET 9) and no library type is
  `[Serializable]`, so it threw for every input. Use `ICloneable.Clone()` on the model types.

## 1.1.0.1

### Fixed

- **lamedb v4 service lines with a 7th `source id` field** now load. Newer enigma2
  images (and tools such as DemonEdit) write the service reference as
  `sid:namespace:tsid:onid:type:number:sourceid` — enigma2 itself reads it with
  `sscanf "%x:%x:%x:%x:%d:%d:%x"`. The reader previously required exactly six fields
  and threw *"Invalid service data"* on the first service, aborting the whole load.
  Six or seven fields are now accepted, the value is exposed as `IService.SourceID`,
  and it is written back on Enigma2 saves so the file round-trips faithfully
  (classic six-field lists are unchanged).

## Unreleased — Enigma2 format modernization

Brings the Enigma2 support up to date: lamedb version 5, more IPTV/stream service
types, service alternatives, SPACE/HIDDEN bouquet entries, and a completed DVB
service-type / DVB-T modulation enum set.

### Added (non-breaking)

- **lamedb5 (`eDVB services /5/`)** read and write. It is version-driven: set
  `settings.SettingsVersion = Enums.SettingsVersion.Enigma2Ver5` and `Save(...)`
  writes a file named `lamedb5`; `Load(...)` of a `/5/` file is auto-detected.
  v4↔v5 conversion is lossless (load one version, set the other, save).
- `SettingsVersion.Enigma2Ver5`.
- **Service alternatives** (`#SERVICE 1:134:…:FROM BOUQUET "alternatives.*"`): new
  `IBouquetItemAlternative` / `BouquetItemAlternative`, `BouquetItemType.Alternative`,
  and `IInstanceFactory.InitNewBouquetItemAlternative(...)` overloads. The referenced
  `alternatives.<name>.tv` file is read and written, and its inner services are matched.
- **IPTV/stream detection** for service-reference types `5001`, `5002`, `8193`, `8739`
  (in addition to `4097`); the actual type is preserved on round-trip. New
  `FavoritesType` members `Stream5001`, `Stream5002`, `Stream8193`, `Stream8739`.
- **DVB service types 28–32** added to `ServiceType`: `AcStereoHdtv` (28),
  `AcStereoHdNvodTimeShifted` (29), `AcStereoHdNvodReference` (30), `Hevc` (31),
  `HevcUhd` (32). (Unknown/unmapped types were already preserved via the raw `Type`
  string; this only completes the convenience enum.)
- `LineSpecifier` members `Alternative` (134), `Udp` (256), `Hidden` (519), `Space` (832).
- `IFileBouquet.Hidden` — set when a userbouquet is referenced as hidden (`519`) in
  `bouquets.tv`/`bouquets.radio`; re-emitted as `519` on save.
- **SPACE (832)** spacer markers are preserved (modeled as markers).

### Behavior changes (review when upgrading the consumer)

- **`DVBTModulationType` corrected** to the DVB-T constellation coding
  `QPSK=0, Qam16=1, Qam64=2, Auto=3, Qam256=4` (it previously had a `Auto=1`/`QPSK=1`
  value collision and lacked `0`/`Qam256`). This only affects the convenience enum
  lookup; the raw `Modulation` string was always stored and written unchanged.
- **`DVBSRollOffType` corrected** to the DVB-S2 roll-off coding `0.35=0, 0.25=1,
  0.20=2, Auto=3` (it previously had `X20=3` and no `Auto`). Convenience-enum only;
  the raw roll-off value is stored and written unchanged.
- **`E2BouquetToString` now writes nested file-bouquet references** that were
  previously built but never appended (silent drop). Affects only file-bouquet items
  nested inside a userbouquet body (alternatives and nested sub-bouquets); normal
  top-level `bouquets.tv` references were unaffected.

### Fixed

- **Cleanup/remove utilities now reach inside service alternatives.** `RemoveStreams`,
  `RemoveService`, `RemoveServices`, `RemoveTransponder`, `RemoveTransponders`,
  `RemoveSatellite` (via transponders) and `RemoveInvalidBouquetItems` previously walked only
  the top-level bouquets and skipped an alternative's inner bouquet (which is intentionally
  kept out of `Settings.Bouquets`). A service/stream/transponder referenced only inside an
  alternative is now removed consistently, so it can no longer be silently re-introduced on
  save. `RemoveInvalidBouquetItems` additionally drops dangling alternative references (an
  `IBouquetItemAlternative` whose `Bouquet` is null).

### Notes / known limitations

- A SPACE (832) entry that has no `#DESCRIPTION` is read as a marker whose description
  defaults to a placeholder, so it is not byte-identical on round-trip (the line
  specifier and value are preserved).
- The legacy **PCL Profile111** project (`EnigmaSettingsPCL`) does not build — a
  pre-existing issue unrelated to this work: `XmlSatellitesIO` uses
  `System.IO.FileStream`, which is unavailable in Profile111. The netstandard2.0 and
  .NET Framework 4.0 builds are unaffected. (The PCL `<Compile>` includes for new
  files are kept current so the target is whole if that pre-existing break is fixed.)
- `satellites.xml` reads/writes the full DVB-S2/S2X/T2MI attribute set
  (`system`, `modulation`, `pls_mode`, `pls_code`, `is_id`, `t2mi_plp_id`, `t2mi_pid`)
  for **lamedb5 (v5) saves** as well as v3/v4 — v4↔v5 round-trips are lossless
  including `satellites.xml` S2X/T2MI data. A regression test now covers the v5
  save path.
