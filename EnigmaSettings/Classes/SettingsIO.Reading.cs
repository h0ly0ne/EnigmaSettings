// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    // Reading of lamedb / services, bouquets, whitelist / blacklist and services.locked
    public partial class SettingsIO
    {
        #region "Reading"
        /// <summary>
        ///     Reads file content from the disk, parses content, sets version number, initializes new lists of transponders and
        ///     services
        /// </summary>
        /// <param name="fileName">Full path to settings file on the disk</param>
        /// <remarks>Does not match transponders and services</remarks>
        protected virtual ISettings ReadSettingsFile(string fileName)
        {
            try
            {
                ISettings settings = Factory.InitNewSettings();
                settings.Log = Log;
                string[] settingsFileLines = Read(fileName);
                CheckIfSettingsValid(settingsFileLines, fileName);
                ReadSettingsVersion(settings, settingsFileLines, fileName);

                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma2Ver5)
                {
                    ReadTransponders(settings, settingsFileLines, fileName, out var transponderOpenIndex, out var transponderEndIndex);
                    ReadServices(settings, settingsFileLines, fileName, transponderOpenIndex, transponderEndIndex);
                }
                else
                    ReadLameDb5(settings, settingsFileLines, fileName);

                if (Log.IsDebugEnabled) Log.Debug($"Settings loaded with {settings.Services.Count} services and {settings.Transponders.Count} transponders");
                return settings;
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_There_was_an_error_while_reading_services_0__1_, Environment.NewLine, ex.Message), ex);
            }
        }

        /// <summary>
        ///     Checks if settings file appears to be valid Enigma settings file
        /// </summary>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">
        ///     Throws settings exception if settings file does not appear to be valid enigma
        ///     settings file
        /// </exception>
        protected virtual void CheckIfSettingsValid(string[] settingsFileLines, string fileName)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Checking {fileName} to make sure we have valid settings");
            var fName = Path.GetFileName(fileName);

            if (settingsFileLines == null || settingsFileLines.Length == 0)
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_File__0__is_empty_, fName));

            if (!settingsFileLines[0].StartsWith(SettingsFirstLine, StringComparison.OrdinalIgnoreCase) || !settingsFileLines[0].Trim().EndsWith('/'))
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fName));

            if (Log.IsDebugEnabled) Log.Debug($"File {fName} looks like a valid settings file, checking for version");
        }

        /// <summary>
        ///     Reads settings version and sets SettingsVersion property for specified settings instance
        /// </summary>
        /// <param name="settings">Instance of ISettings we're setting settings version to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if settings version can't be determined</exception>
        protected virtual void ReadSettingsVersion(ISettings settings, string[] settingsFileLines, string fileName)
        {
            Log.Debug("Looking for settings version");
            var fName = Path.GetFileName(fileName);
            var sVersion = settingsFileLines[0].ToLowerInvariant().Trim().Replace(SettingsFirstLine.ToLowerInvariant(), "").TrimEnd('/');

            if (!int.TryParse(sVersion, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fName));

            settings.SettingsVersion = GetSettingsVersion(sVersion);

            if (settings.SettingsVersion == Enums.SettingsVersion.Unknown)
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_Unsuported_settings_version__0_, sVersion));
            
            if (Log.IsDebugEnabled) Log.Debug($"Found settings version {settings.SettingsVersion}");
        }

        /// <summary>
        ///     Reads transponders from specified lines and adds them to specified instance of ISettings
        /// </summary>
        /// <param name="settings">Instance of ISettings we're adding transponders to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <param name="transponderOpenIndex">Returns line number of transponder open tag</param>
        /// <param name="transponderEndIndex">Returns line number of transponder end tag</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if there's no transponder open tag</exception>
        protected virtual void ReadTransponders(ISettings settings, string[] settingsFileLines, string fileName, out int transponderOpenIndex, out int transponderEndIndex)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Reading transponders from {fileName}");

            //search for the beginning of transponder section
            transponderOpenIndex = -1;
            transponderEndIndex = -1;
            var sCount = settingsFileLines.Length;

            for (var i = 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsTransponderOpenTag) continue;
                transponderOpenIndex = i;
                break;
            }

            if (transponderOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_No__transponder__tag_was_found_in__0_, Path.GetFileName(fileName)));

            if (Log.IsDebugEnabled) Log.Debug($"Transponder section found at line {transponderOpenIndex + 1}");

            // the section must be closed by "end" before the services section starts (structural error otherwise)
            int endTag = Array.FindIndex(settingsFileLines, transponderOpenIndex + 1, x => x.Trim() == SettingsClosingTag);
            int servicesTag = Array.FindIndex(settingsFileLines, transponderOpenIndex + 1, x => x.Trim() == SettingsServicesOpenTag);
            if (endTag == -1 || (servicesTag != -1 && servicesTag < endTag))
                throw new SettingsException($"Transponder section in {Path.GetFileName(fileName)} has no end tag");

            //reading transponders, each entry spans 3 lines (id, data, "/"); a broken entry is skipped like Enigma2 does
            for (var i = transponderOpenIndex + 1; i < endTag; i += 3)
            {
                string firstLine = settingsFileLines[i];
                string secondLine = i + 1 < endTag ? settingsFileLines[i + 1] : string.Empty;

                if (secondLine.Trim().Length == 0)
                {
                    Log.Warn($"Incomplete transponder entry at line {i + 1} skipped: {firstLine}");
                    continue;
                }

                try
                {
                    AddTransponder(settings, firstLine, secondLine);
                }
                catch (Exception ex)
                {
                    Log.Warn($"Invalid transponder entry at line {i + 1} skipped: {firstLine} {secondLine.Trim()}", ex);
                }
            }

            if (Log.IsDebugEnabled) Log.Debug($"Transponder section ends at line {endTag + 1}");
            transponderEndIndex = endTag;
        }

        /// <summary>
        ///     Initializes the transponder of one lamedb entry (type from the data line, Enigma1 cable / terrestrial
        ///     from the namespace) and adds it to the settings
        /// </summary>
        protected virtual void AddTransponder(ISettings settings, string firstLine, string secondLine)
        {
            switch (char.ToLowerInvariant(secondLine.Trim()[0]))
            {
                case 's':
                {
                    // DVBT & DVBC transponders in Enigma1 are saved as satellite transponders
                    if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                    {
                        if (firstLine.StartsWith("ffff", StringComparison.OrdinalIgnoreCase))
                            AddTransponderDVBC(settings, firstLine, secondLine);
                        else if (firstLine.StartsWith("eeee", StringComparison.OrdinalIgnoreCase))
                            AddTransponderDVBT(settings, firstLine, secondLine);
                        else if (firstLine.StartsWith("0000", StringComparison.OrdinalIgnoreCase))
                            AddTransponderIPTV(settings, firstLine, secondLine);
                        else
                            AddTransponderDVBS(settings, firstLine, secondLine);
                    }
                    else
                    {
                        if (firstLine.StartsWith("0000", StringComparison.OrdinalIgnoreCase))
                            AddTransponderIPTV(settings, firstLine, secondLine);
                        else
                            AddTransponderDVBS(settings, firstLine, secondLine);
                    }

                    break;
                }
                case 't':
                {
                    AddTransponderDVBT(settings, firstLine, secondLine);

                    break;
                }
                case 'c':
                {
                    AddTransponderDVBC(settings, firstLine, secondLine);

                    break;
                }
                default:
                {
                    Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadSettingsFile_Unknown_transponder_type_for_transponder__0___1___adding_it_as_satellite_transponder, firstLine, secondLine));
                    AddTransponderDVBS(settings, firstLine, secondLine);

                    break;
                }
            }
        }

        /// <summary>
        ///     Reads services from specified lines and adds them to specified instance of ISettings
        /// </summary>
        /// <param name="settings">Instance of ISettings we're adding transponders to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <param name="transponderOpenIndex">Line number of transponder open tag</param>
        /// <param name="transponderEndIndex">Line number of transponder end tag</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if there's no services open tag</exception>
        protected virtual void ReadServices(ISettings settings, string[] settingsFileLines, string fileName, int transponderOpenIndex, int transponderEndIndex)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Reading services from {fileName}");

            //search for the beginning of services section after transponders section
            var servicesOpenIndex = -1;
            var sCount = settingsFileLines.Length;

            for (var i = transponderEndIndex + 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsServicesOpenTag)
                    continue;

                servicesOpenIndex = i;
                break;
            }

            if (servicesOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadServicesFile_No__services__tag_was_found_in_file__0_, Path.GetFileName(fileName)));

            if (Log.IsDebugEnabled) Log.Debug($"Services section found at line {servicesOpenIndex + 1}");

            //reading services, each entry spans 3 lines (id, name, flags)
            var debug = Log.IsDebugEnabled;
            for (int i = servicesOpenIndex + 1; i + 2 < sCount; i += 3)
            {
                if (settingsFileLines[i].Trim() == SettingsClosingTag)
                    break;

                var line1 = settingsFileLines[i];
                var line2 = settingsFileLines[i + 1];
                var line3 = settingsFileLines[i + 2];
                if (debug)
                    Log.Debug($"Initializing new service {line1}    {line2}    {line3}");

                // a broken entry is skipped like Enigma2 does, the 3 line grid stays intact
                try
                {
                    settings.Services.Add(Factory.InitNewService(line1, line2, line3));
                }
                catch (Exception ex)
                {
                    Log.Warn($"Invalid service entry at line {i + 1} skipped: {line1} / {line2}", ex);
                }
            }

            Log.Debug("Services read sucessfully");
        }

        /// <summary>
        ///     Initializes new DVBT transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBT(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new terrestrial transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBT(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new DVBC transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBC(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new cable transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBC(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new DVBS transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBS(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new satellite transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBS(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new IPTV transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderIPTV(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new iptv transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderIPTV(firstLine, secondLine));
        }

        /// <summary>
        ///     Reads bouquet from disk and parses content. Goes recursively if bouquet has other bouquets as items.
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>Nothing if bouquet type is unknown. Type is recognized from filename</returns>
        /// <remarks>Adds bouquet to Bouquets list if not already in it, otherwise updates existing</remarks>
        protected virtual IFileBouquet ReadFileBouquet(string fileName, ref ISettings settings)
        {
            bool ownsContext = _bouquetReadContext == null;
            if (ownsContext)
                _bouquetReadContext = new BouquetReadContext();

            string fullPath = Path.GetFullPath(fileName);
            try
            {
                // every bouquet file is read once per load: a second reference gets the same instance
                // and a reference back to a bouquet that is still being read (a cycle) is not followed
                if (_bouquetReadContext.Read.TryGetValue(fullPath, out var alreadyRead))
                {
                    if (!_bouquetReadContext.InProgress.Contains(fullPath))
                        return alreadyRead;

                    Log.Warn($"Bouquet {Path.GetFileName(fileName)} references itself (directly or through sub bouquets), reference ignored");
                    return null;
                }

                IFileBouquet bqt = Factory.InitNewFileBouquet();
                string fName = Path.GetFileName(fileName);
                bqt.FileName = fName;

                //do not read bouquets that have their own methods for reading
                if (bqt.BouquetType == Enums.BouquetType.E1Bouquets || string.Equals(fName, WhiteListFile, StringComparison.OrdinalIgnoreCase) || string.Equals(fName, BlackListFile, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fName, ServicesLockedFile, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Warn($"File {fName} is not a bouquet file, not reading it as bouquet");
                    return null;
                }

                //do not read unknown bouquet types
                if (bqt.BouquetType == Enums.BouquetType.Unknown)
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquets_File__0__is_unknown_bouquet_type, fName));
                    return null;
                }

                if (!_fileHelper.Exists(fileName))
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_LoadSettings_File__0__does_not_exist_, fName));
                    return null;
                }
                IFileBouquet existing =
                    settings.Bouquets.OfType<IFileBouquet>()
                        .FirstOrDefault(x => String.Equals(x.FileName, fName, StringComparison.Ordinal));
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    if (bqt.BouquetType != Enums.BouquetType.Unknown
                        && !string.Equals(fName, BouquetsTvFile, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, BouquetsRadioFile, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, UserBouquetTvEpl, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, UserBouquetRadioEpl, StringComparison.OrdinalIgnoreCase)
                        && !fName.StartsWith("alternatives.", StringComparison.OrdinalIgnoreCase))
                    {
                        settings.Bouquets.Add(bqt);
                    }
                }

                _bouquetReadContext.Read[fullPath] = bqt;
                _bouquetReadContext.InProgress.Add(fullPath);

                if (Log.IsDebugEnabled) Log.Debug($"Reading bouquet {bqt.FileName}");
                string[] bouquetLines = Read(fileName, true);

                if (bouquetLines == null || bouquetLines.Length == 0)
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_Bouquet__0__is_empty, bqt.FileName));
                }
                else
                {
                    string dir = Path.GetDirectoryName(fileName);
                    var cnt = bouquetLines.Length;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        string line = bouquetLines[i];
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            DetectAndReadReference(dir, ref bqt, bouquetLines, i, settings);
                        }
                    }
                    if (Log.IsDebugEnabled) Log.Debug($"Bouquet {bqt.FileName} successfully parsed with {cnt} items");
                }
                return bqt;
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(string.Format(Resources.Resources.Settings_ReadBouquet_Failed_to_read_bouquet__0_, Path.GetFileName(fileName)), ex);
            }
            finally
            {
                _bouquetReadContext?.InProgress.Remove(fullPath);
                if (ownsContext)
                    _bouquetReadContext = null;
            }
        }

        /// <summary>
        ///     Bouquet files read during the current load (per thread, so parallel loads don't interfere)
        /// </summary>
        private sealed class BouquetReadContext
        {
            public readonly Dictionary<string, IFileBouquet> Read = new(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> InProgress = new(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> Listed = new(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     True for the files that list the top level bouquets (bouquets.tv/.radio, userbouquets.*.epl)
        /// </summary>
        protected static bool IsBouquetList(string fileName)
        {
            string name = Path.GetFileName(fileName);
            return string.Equals(name, BouquetsTvFile, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, BouquetsRadioFile, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, UserBouquetTvEpl, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, UserBouquetRadioEpl, StringComparison.OrdinalIgnoreCase);
        }

        [ThreadStatic]
        private static BouquetReadContext _bouquetReadContext;

        /// <summary>
        ///     Determines type of line and initializes corresponding bouquet item type
        /// </summary>
        /// <param name="folder">Directory with all the settings files</param>
        /// <param name="bouquet">Parent bouquet that this item belongs to</param>
        /// <param name="bouquetLines">Bouquet file content as string array</param>
        /// <param name="index">Index of the current bouquet line</param>
        /// <param name="settings">Reference to existing settings instance</param>
        /// <remarks></remarks>
        protected virtual void DetectAndReadReference(string folder, ref IFileBouquet bouquet, string[] bouquetLines, int index, ISettings settings)
        {
            string line = bouquetLines[index];
            string description = string.Empty;
            int bCount = bouquetLines.Length;
            string nextLine = index < (bCount - 1) ? bouquetLines[index + 1] : string.Empty;
            if (nextLine.StartsWith("#DESCRIPTION", StringComparison.OrdinalIgnoreCase))
                description = bouquetLines[index + 1];

            if (line.StartsWith("#NAME", StringComparison.Ordinal))
            {
                bouquet.Name = line.Substring(line.Length >= 6 ? 6 : 5);
                if (Log.IsDebugEnabled) Log.Debug($"Found bouquet NAME: {line}");
            }
            else if (line.StartsWith("#SERVICE", StringComparison.Ordinal))
            {
                string serviceData = line.Length > 9 ? line.Substring(9).Trim() : string.Empty;
                string[] sData = serviceData.Split(':');

                if (sData.Length < 2 || !int.TryParse(sData[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var lineSpecifier))
                {
                    Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, Path.Combine(folder, bouquet.Name ?? string.Empty)));
                    return;
                }

                string favoritesType = sData[0];

                // the line specifier is a bit mask (Enigma2 eServiceReference flags): isDirectory=1, mustDescent=2,
                // canDescent=4, isMarker=64, isGroup=128, isNumberedMarker=256, isInvisible=512
                bool isMarker = (lineSpecifier & MarkerFlag) != 0;
                bool isGroup = !isMarker && (lineSpecifier & GroupFlag) != 0;
                bool isDirectory = !isMarker && !isGroup && ((lineSpecifier & DirectoryFlags) != 0 || favoritesType == FavoritesTypeFile);

                if (lineSpecifier < 0 || favoritesType == FavoritesTypeUnknown)
                    Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, Path.Combine(folder, bouquet.Name ?? string.Empty)));
                else if ((isDirectory || isGroup) && sData.Length < 11)
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_Unsupported_line___1___in_bouquet__0_, Path.GetFileName(bouquet.FileName), line));
                else if (isMarker)
                {
                    // plain, numbered (320), invisible (576) markers and spacers (832); the flag is written back as read
                    IBouquetItemMarker bqim = TryReadEntry(() => ReadMarkerReference(line, description), out var markerError);
                    if (bqim != null)
                    {
                        bouquet.BouquetItems.Add(bqim);
                        if (Log.IsDebugEnabled) Log.Debug($"Marker {bqim.Description} ({sData[1]}) added to bouquet {bouquet.Name}");
                    }
                    else
                        Log.Warn($"Invalid marker {line} not added to bouquet {bouquet.Name}", markerError);
                }
                else if (isDirectory)
                {
                    if (Log.IsDebugEnabled) Log.Debug($"Found (sub)directory item {sData[10]}");
                    var tmpName = sData[10];

                    //workaround for FROM BOUQUET "userbouquet.dbe01.radio" ORDER BY bouquet
                    if (tmpName.IndexOf('"') > -1)
                    {
                        Log.Debug("Non standard bouquet name, try to workaround it");
                        tmpName = tmpName.Substring(tmpName.IndexOf('"') + 1);
                        tmpName = tmpName.Substring(0, tmpName.IndexOf('"'));
                        tmpName = tmpName.Trim();
                        sData[10] = tmpName;
                        if (Log.IsDebugEnabled) Log.Debug($"Workaround completed with result {tmpName}");
                    }
                    string fName = Path.GetFileName(sData[10]);

                    IFileBouquet subBqt = ReadFileBouquet(Path.Combine(folder, fName), ref settings);

                    if (subBqt != null)
                    {
                        if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                            subBqt.FileName = sData[10];

                        if ((lineSpecifier & InvisibleFlag) != 0)
                            subBqt.Hidden = true;

                        // listed in bouquets.tv / .radio (or E1 userbouquets.*.epl) wins over being referenced
                        // from another bouquet, whichever is read first
                        string subPath = Path.GetFullPath(Path.Combine(folder, fName));
                        if (IsBouquetList(bouquet.FileName))
                        {
                            subBqt.IsSubBouquet = false;
                            _bouquetReadContext?.Listed.Add(subPath);
                        }
                        else if (_bouquetReadContext == null || !_bouquetReadContext.Listed.Contains(subPath))
                        {
                            subBqt.IsSubBouquet = true;
                        }

                        IBouquetItemFileBouquet bib = Factory.InitNewBouquetItemFileBouquet(subBqt);
                        bib.FavoritesTypeFlag = sData[0];
                        bib.LineSpecifierFlag = sData[1];
                        bouquet.BouquetItems.Add(bib);
                        if (Log.IsDebugEnabled) Log.Debug($"Bouquet {Path.GetFileName(sData[10])} added as bouquet item to bouquet {bouquet.Name} ");
                    }
                    else
                    {
                        Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_Bouquet__0__in_bouquet__1__is_of_unknown_type, Path.GetFileName(sData[10]),
                            bouquet.Name));
                    }
                }
                else if (isGroup)
                {
                    var altTmp = sData[10];

                    if (altTmp.IndexOf('"') > -1)
                    {
                        altTmp = altTmp.Substring(altTmp.IndexOf('"') + 1);
                        altTmp = altTmp.Substring(0, altTmp.IndexOf('"')).Trim();
                    }

                    string altFile = Path.GetFileName(altTmp);

                    IFileBouquet altBqt = ReadFileBouquet(Path.Combine(folder, altFile), ref settings);

                    if (altBqt != null)
                    {
                        IBouquetItemAlternative bia = Factory.InitNewBouquetItemAlternative(altBqt);
                        bia.FavoritesTypeFlag = sData[0];
                        bia.LineSpecifierFlag = sData[1];
                        bouquet.BouquetItems.Add(bia);
                        if (Log.IsDebugEnabled) Log.Debug($"Alternative {altFile} added to bouquet {bouquet.Name}");
                    }
                    else
                        Log.Warn($"Alternative {altFile} in bouquet {bouquet.Name} could not be read");
                }
                else if (_streamFavoritesTypes.Contains(favoritesType) || (sData.Length > 10 && sData[10].Contains("//", StringComparison.Ordinal)) || sData.Length == 12)
                {
                    //it is a stream (any remaining flags, e.g. invisible, are kept)
                    IBouquetItemStream bqis = TryReadEntry(() => ReadStreamReference(line, description), out var streamError);
                    if (bqis == null)
                    {
                        Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadFileBouquet_Invalid_stream_reference__0__not_added_to_bouquet__1_, line, bouquet.Name), streamError);
                    }
                    else
                    {
                        bouquet.BouquetItems.Add(bqis);
                        if (description.Length == 0)
                            Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_Stream_service__1__added_to_bouquet__0__without_description, bouquet.Name, bqis.Description));
                        else if (Log.IsDebugEnabled)
                            Log.Debug($"Stream service {bqis.Description} added to bouquet {bouquet.Name}");
                    }
                }
                else
                {
                    //it is a regular DVB service (any remaining flags, e.g. invisible, are kept)
                    IBouquetItemService bqis = TryReadEntry(() => ReadServiceReference(line), out var serviceError);
                    if (bqis != null)
                    {
                        bqis.Description = StripDescriptionTag(description);
                        bouquet.BouquetItems.Add(bqis);
                        if (Log.IsDebugEnabled) Log.Debug($"DVB service {serviceData} added to bouquet {bouquet.Name}");
                    }
                    else
                        Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadFileBouquet_Invalid_service_reference__0__not_added_to_bouquet__1_, line, bouquet.Name), serviceError);
                }
            }
            else if (line.StartsWith("#TYPE", StringComparison.Ordinal))
            {
            }
            else if (line.StartsWith("#DESCRIPTION", StringComparison.Ordinal))
            {
            }
            else if (line.StartsWith('/'))
            {
            }
            else
                Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_Unsupported_line___1___in_bouquet__0_, Path.GetFileName(bouquet.FileName), line));
        }

        /// <summary>
        ///     Reads one entry of a list file; a broken entry returns null (with the error) instead of failing the
        ///     whole load, like Enigma2 skips entries it cannot parse
        /// </summary>
        private static T TryReadEntry<T>(Func<T> read, out Exception error) where T : class
        {
            try
            {
                error = null;
                return read();
            }
            catch (Exception ex)
            {
                error = ex;
                return null;
            }
        }

        /// <summary>
        ///     Text of a #DESCRIPTION line without the tag (with or without colon)
        /// </summary>
        protected static string StripDescriptionTag(string descriptionLine)
        {
            if (string.IsNullOrEmpty(descriptionLine))
                return string.Empty;

            if (descriptionLine.StartsWith("#DESCRIPTION:", StringComparison.OrdinalIgnoreCase))
                return descriptionLine.Substring(13).Trim();

            return descriptionLine.StartsWith("#DESCRIPTION", StringComparison.OrdinalIgnoreCase) ? descriptionLine.Substring(12).Trim() : descriptionLine.Trim();
        }

        /// <summary>
        ///     Initializes new IBouquetItemService object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemService ReadServiceReference(string bouquetLine)
        {
            try
            {
                if (bouquetLine.StartsWith("#SERVICE", StringComparison.Ordinal))
                    bouquetLine = bouquetLine.Substring(9).Trim();

                return Factory.InitNewBouquetItemService(bouquetLine);
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(string.Format(Resources.Resources.SettingsIO_ReadServiceReference_Failed_to_initialize_new_service_from_line__0_, bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemMarker object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <param name="description">Text of the marker visible in settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemMarker ReadMarkerReference(string bouquetLine, string description)
        {
            try
            {
                if (bouquetLine.StartsWith("#SERVICE", StringComparison.OrdinalIgnoreCase))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                description = StripDescriptionTag(description);
                string[] sData = bouquetLine.Split(':');
                IBouquetItemMarker bqim = Factory.InitNewBouquetItemMarker(description, sData[2]);
                bqim.FavoritesTypeFlag = sData[0];
                bqim.LineSpecifierFlag = sData[1];
                return bqim;
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(string.Format(Resources.Resources.SettingsIO_ReadMarkerReference_Failed_to_initialize_new_marker_from_line__0_,
                    bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemStream object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <param name="description">Text of the stream service visible in settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemStream ReadStreamReference(string bouquetLine, string description)
        {
            try
            {
                if (bouquetLine.StartsWith("#SERVICE", StringComparison.OrdinalIgnoreCase))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                description = StripDescriptionTag(description);
                string[] sData = bouquetLine.Split(':');
                if (description.Length == 0 && sData.Length > 11)
                {
                    description = Uri.UnescapeDataString(sData[11]);
                }
                IBouquetItemStream bqis = Factory.InitNewBouquetItemStream(bouquetLine, description);
                bqis.FavoritesTypeFlag = sData[0];
                bqis.LineSpecifierFlag = sData[1];
                return bqis;
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(string.Format(
                    Resources.Resources.SettingsIO_ReadStreamReference_Failed_to_initialize_new_stream_reference_from_line__0_, bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Reads bouquets file from Enigma1
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>bouquets Bouquet with other bouquets as bouquet items</returns>
        /// <remarks></remarks>
        protected virtual IFileBouquet ReadE1Bouquets(string fileName, ref ISettings settings)
        {
            Log.Debug("Reading Enigma1 bouquets file");

            try
            {
                IFileBouquet bqt = Factory.InitNewFileBouquet();
                bqt.FileName = BouquetsFile;
                bqt.Name = BouquetsFile;

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    settings.Bouquets.Add(bqt);
                }

                string[] fileLines = Read(fileName);

                //search for the beginning of bouquets (first line is the header)
                int openIndex = Array.FindIndex(fileLines, 1, x => x.Trim() == BouquetsOpenTag);
                if (openIndex == -1)
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, BouquetsOpenTag, bqt.FileName));
                    return bqt;
                }

                int endIndex = Array.FindIndex(fileLines, openIndex + 1, x => x.Trim() == SettingsClosingTag);
                if (endIndex == -1)
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, SettingsClosingTag, bqt.FileName));
                    endIndex = fileLines.Length;
                }

                //every bouquet is: order number, name, services..., "/"
                var block = new List<string>();
                for (int i = openIndex + 1; i < endIndex; i++)
                {
                    string l = fileLines[i].Trim();
                    if (l != "/")
                    {
                        if (l.Length > 0)
                            block.Add(l);
                        continue;
                    }

                    AddE1Bouquet(bqt, block, fileName);
                    block.Clear();
                }

                return bqt;
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(Resources.Resources.SettingsIO_ReadE1Bouquets_Failed_to_read_Enigma1_bouquets_file_, ex);
            }
        }

        /// <summary>
        ///     Adds one Enigma1 bouquet (order number, name, service lines) to the bouquets bouquet
        /// </summary>
        private void AddE1Bouquet(IFileBouquet bouquets, List<string> block, string fileName)
        {
            if (block.Count < 2)
                return;

            string orderNumber = block[0];
            string name = block[1];

            if (!int.TryParse(orderNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order)
                && !int.TryParse(orderNumber, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out order))
            {
                Log.Warn($"Invalid order number {orderNumber} for Enigma1 bouquet {name} in {fileName}, using 0");
                order = 0;
            }

            IBouquetsBouquet bq = Factory.InitNewBouquetsBouquet();
            bq.Name = name;
            IBouquetItemBouquetsBouquet bib = Factory.InitNewBouquetItemBouquetsBouquet(bq);
            bib.BouquetOrderNumberInt = order;
            bouquets.BouquetItems.Add(bib);
            if (Log.IsDebugEnabled) Log.Debug($"Found Enigma1 bouquet {name} in {fileName}");

            for (int i = 2; i < block.Count; i++)
            {
                if (block[i].Split(':').Length >= 5)
                {
                    var i1 = i;
                    var item = TryReadEntry(() => Factory.InitNewBouquetItemService(block[i1]), out var error);
                    if (item == null)
                    {
                        Log.Warn($"Invalid service line {block[i]} in Enigma1 bouquet {name} skipped", error);
                        continue;
                    }

                    bq.BouquetItems.Add(item);
                    if (Log.IsDebugEnabled)
                        Log.Debug($"Added service {block[i]} to Enigma1 bouquet {name}");
                }
                else
                {
                    Log.Warn(string.Format(Resources.Resources.Settings_ReadE1Bouquets_Ignoring_line__0__in_Enigma1_bouquet__1___doesn_t_look_like_regular_service, block[i], name));
                }
            }
        }

        /// <summary>
        ///     Reads Enigma2 whitelist and blacklist file and marks corresponding services as black/white listed
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        /// <remarks>Should be called last when loading settings </remarks>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if file info is null</exception>
        /// <exception cref="SettingsException">Throws settingsException</exception>
        protected virtual void ReadBlackWhiteList(string fileName, ref ISettings settings)
        {
            var sLocked = new List<IBouquetItemService>();

            if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
            {
                return;
            }
            ArgumentNullException.ThrowIfNull(fileName);

            if (!_fileHelper.Exists(fileName))
            {
                return;
            }

            var fName = Path.GetFileName(fileName);
            bool blackListed = string.Equals(fName, BlackListFile, StringComparison.OrdinalIgnoreCase);

            string[] bouquetFileLines = Read(fileName).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            // whole bouquets: 1:7:1:0:0:0:0:0:0:0:FROM BOUQUET "userbouquet.x.tv" ORDER BY bouquet
            foreach (string line in bouquetFileLines.Where(x => x.Contains("FROM BOUQUET", StringComparison.OrdinalIgnoreCase)))
            {
                int start = line.IndexOf('"');
                int end = start > -1 ? line.IndexOf('"', start + 1) : -1;
                string bouquetFile = end > start ? Path.GetFileName(line.Substring(start + 1, end - start - 1).Trim()) : string.Empty;
                IFileBouquet bouquet = settings.Bouquets.OfType<IFileBouquet>()
                    .FirstOrDefault(x => string.Equals(Path.GetFileName(x.FileName), bouquetFile, StringComparison.OrdinalIgnoreCase));

                if (bouquet == null)
                {
                    Log.Warn($"Bouquet {bouquetFile} listed in {fName} was not found");
                    continue;
                }

                bouquet.ServiceSecurity = blackListed ? Enums.ServiceSecurity.BlackListed : Enums.ServiceSecurity.WhiteListed;
            }

            sLocked.AddRange(bouquetFileLines.Where(x => !x.Contains("FROM BOUQUET", StringComparison.OrdinalIgnoreCase)).Select(x =>
                {
                    var item = TryReadEntry(() => ReadServiceReference(x), out var error);
                    if (item == null)
                        Log.Warn($"Invalid line in {fName} skipped: {x}", error);
                    return item;
                })
                .Where(bqis => bqis != null));

            if (sLocked.Count == 0) return;

            //matching  services
            try
            {
                Log.Debug(!blackListed ? "Matching whitelisted services." : "Matching blacklisted services.");

                var query = settings.Services.Join(sLocked, srv => srv.ServiceId, bi => bi.ServiceId, (srv, x) => new
                {
                    Service = srv,
                    LockedBouquetItem = x
                }, StringComparer.OrdinalIgnoreCase);

                foreach (var match in query.ToList())
                {
                    if (blackListed)
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                        if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as BLACKLISTED");
                    }
                    else
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.WhiteListed;
                        if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as WHITELISTED");
                    }
                    sLocked.Remove(match.LockedBouquetItem);
                }
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(Resources.Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //logging items that are not matched
            foreach (IBouquetItemService bi in sLocked)
            {
                Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Locked_bouquet_item__0__has_no_matching_service, bi.ServiceId));
            }
        }

        /// <summary>
        ///     Reads Enigma1 services.locked file and marks bouquets and services as locked
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings">Settings with already loaded services and bouquets</param>
        /// <remarks>Should be last function to call when loading Settings</remarks>
        protected virtual void ReadServicesLocked(string fileName, ref ISettings settings)
        {
            Log.Debug("Reading services.locked file");
            var sLocked = new List<IBouquetItem>();
            try
            {
                string[] lockedLines = Read(fileName);
                int lineNumber = 0;
                foreach (string line in lockedLines)
                {
                    lineNumber += 1;
                    if (lineNumber <= 1) continue;
                    if (string.IsNullOrEmpty(line) || line.Split(':').Length < 10)
                    {
                        Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Invalid_line__0__found_in__1_, line, fileName));
                    }
                    else
                    {
                        string[] sData = line.Split(':');
                        IBouquetItemService srv = TryReadEntry(() => Factory.InitNewBouquetItemService(line), out var error);

                        if (srv == null)
                        {
                            Log.Warn($"Invalid line {line} in {fileName} skipped", error);
                            continue;
                        }

                        if (srv.LineSpecifierType == Enums.LineSpecifier.Unknown)
                        {
                            Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, fileName));
                        }
                        else if (srv.LineSpecifierType.ToString().StartsWith("isdirectory", StringComparison.OrdinalIgnoreCase))
                        {
                            if (Log.IsDebugEnabled) Log.Debug($"Found reference to locked bouquet {line} in {fileName}");
                            try
                            {
                                int orderNumber = Int32.Parse(sData[4], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                                sLocked.Add(Factory.InitNewBouquetItemBouquetsBouquet(orderNumber));
                            }
                            catch (Exception ex)
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources
                                            .Resources.SettingsIO_ReadServicesLocked_Unable_to_parse_bouquet_order_number__0__to_integer_for_bouquet__1__on_line__2_,
                                        sData[4], fileName, line));
                                Log.Warn(ex.ToString());
                            }
                        }
                        else if (srv.LineSpecifierType == Enums.LineSpecifier.NormalService)
                        {
                            sLocked.Add(srv);
                            if (Log.IsDebugEnabled) Log.Debug($"Found reference to locked service {line} in {fileName}");
                        }
                        else
                        {
                            Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Unsupported_type__0__found_in_line__1__in_file__2_,
                                srv.LineSpecifierType, line, fileName));
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(Resources.Resources.SettingsIO_ReadServicesLocked_Failed_to_read_services_locked_file, ex);
            }

            Log.Debug("All items from services.locked loaded.");
            if (sLocked.Count == 0)
                return;

            IFileBouquet bouquets = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => x.BouquetType == Enums.BouquetType.E1Bouquets);

            //matching locked bouquets by order number
            try
            {
                if (bouquets != null)
                {
                    Log.Debug("Matching locked bouquets.");
                    IList<IBouquetItemBouquetsBouquet> bis = bouquets.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList();
                    var query = bis.Join(sLocked.OfType<IBouquetItemBouquetsBouquet>().ToList(), bs => bs.BouquetOrderNumber,
                        x => x.BouquetOrderNumber, (bs, x) => new
                        {
                            BouquetsBouquetItem = bs,
                            LockedBouquetItem = x
                        }, StringComparer.OrdinalIgnoreCase);

                    foreach (var match in query.ToList())
                    {
                        if (match.BouquetsBouquetItem.Bouquet != null)
                        {
                            match.BouquetsBouquetItem.Bouquet.Locked = true;
                            if (Log.IsDebugEnabled) Log.Debug($"Bouquet {match.BouquetsBouquetItem.Bouquet.Name} has been marked as LOCKED");
                        }
                        else
                        {
                            Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Bouquet_item__0__has_no_matching_Enigma1_bouquet_,
                                match.LockedBouquetItem.BouquetOrderNumber));
                        }
                        sLocked.Remove(match.LockedBouquetItem);
                    }
                }
                else
                {
                    Log.Debug("Enigma1 bouquets bouquet was not found, no locked bouquets to match.");
                }
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(Resources.Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //matching locked services
            try
            {
                Log.Debug("Matching locked services.");
                var query = settings.Services.Join(sLocked.OfType<IBouquetItemService>().ToList(), srv => srv.ServiceId,
                    bi => bi.ServiceId, (srv, x) => new
                    {
                        Service = srv,
                        LockedBouquetItem = x
                    }, StringComparer.OrdinalIgnoreCase);

                foreach (var match in query.ToList())
                {
                    match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                    if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as LOCKED");
                    sLocked.Remove(match.LockedBouquetItem);
                }
            }
            catch (Exception ex) when (ex is not SettingsException)
            {
                throw new SettingsException(Resources.Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //logging items that are not matched
            foreach (IBouquetItem bi in sLocked)
            {
                if (bi is IBouquetItemService service)
                {
                    Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Locked_bouquet_item__0__has_no_matching_service, service.ServiceId));
                }
                else
                {
                    if (bi is IBouquetItemBouquetsBouquet bouquet)
                    {
                        Log.Warn(string.Format(Resources.Resources.SettingsIO_ReadServicesLocked_Bouquet_item__0__has_no_matching_Enigma1_bouquet_,
                            bouquet.BouquetOrderNumber));
                    }
                }
            }
        }
        #endregion
    }
}
