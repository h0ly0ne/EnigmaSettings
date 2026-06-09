# Changelog

## Unreleased — Enigma2 format modernization

Brings the Enigma2 support up to date: lamedb version 5, more IPTV/stream service
types, service alternatives, SPACE/HIDDEN bouquet entries, and a completed DVB
service-type / DVB-T modulation enum set. A new xUnit test project
(`EnigmaSettings.Tests`) covers the format with round-trip fixtures.

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
- **`RemoveEmptyMarkers` preserves SPACE (832) spacers.** SPACE entries are intentional layout
  and are never removed; a regular marker followed only by a SPACE is no longer treated as
  empty. Ordinary empty/duplicate markers are still removed as before.

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
