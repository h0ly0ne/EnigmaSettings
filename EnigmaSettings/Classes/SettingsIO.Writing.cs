// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Krkadoni.EnigmaSettings.Interfaces;
using static Krkadoni.EnigmaSettings.Enums;

namespace Krkadoni.EnigmaSettings
{
    // Writing of lamedb / services, bouquets, whitelist / blacklist and services.locked
    public partial class SettingsIO
    {
        #region "Writing"
        protected virtual void WriteSettingsFile(string directory, ISettings settings, string strLocalFilenameOverride)
        {
            Log.Debug("Writing settings file to disk");

            string fileName = settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2 ? "services" : "lamedb";

            if (!string.IsNullOrEmpty(strLocalFilenameOverride))
                fileName = strLocalFilenameOverride;

            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver5)
            {
                WriteLameDb5(directory, settings, strLocalFilenameOverride);
                return;
            }

            var sContent = new StringBuilder();
            var fi = Path.Combine(directory, fileName);
            OnFileSaving(this, fi);

            try
            {
                sContent.Append(SettingsFirstLine + GetSettingsVersion(settings.SettingsVersion) + "/" + "\n");
                sContent.Append(TranspondersToString(settings));
                sContent.Append(ServicesToString(settings));
                sContent.Append(EditorName + "\n");

                _fileHelper.WriteAllText(Path.Combine(directory, fileName), sContent.ToString());
                if (Log.IsDebugEnabled) Log.Debug($"Sucessfully written {fileName} to {Path.Combine(directory, fileName)}");
                OnFileSaved(this, fi, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fi, false);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_Save_Failed_to_save_settings_to__0_, Path.Combine(directory, fileName)), ex);
            }
        }

        protected virtual string TranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsTransponderOpenTag + "\n");

            switch (settings.SettingsVersion)
            {
                case Enums.SettingsVersion.Enigma1Ver1:
                case Enums.SettingsVersion.Enigma1Ver2:
                    sContent.Append(DVBSTranspondersToString(settings, 1));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
                    sContent.Append(IPTVTranspondersToString(settings));
                    sContent.Append(DVBSTranspondersToString(settings, -1));
                    break;
                case Enums.SettingsVersion.Enigma2Ver3:
                case Enums.SettingsVersion.Enigma2Ver4:
                case Enums.SettingsVersion.Enigma2Ver5:
                    sContent.Append(DVBSTranspondersToString(settings, 0));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
                    sContent.Append(IPTVTranspondersToString(settings));
                    break;
            }

            sContent.Append(SettingsClosingTag + "\n");
            return sContent.ToString();
        }

        protected virtual string DVBSTranspondersToString(ISettings settings, int positionOrder)
        {
            var sContent = new StringBuilder();
            foreach (
                ITransponderDVBS tran in
                    settings.Transponders.OfType<ITransponderDVBS>()
                        .Where(x => (positionOrder > 0 && x.OrbitalPositionInt >= 0) || (positionOrder < 0 && x.OrbitalPositionInt < 0) || positionOrder == 0)
                        .OrderByDescending(x => x.OrbitalPositionInt)
                        .ToList())
            {
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                    {
                        if (tran.SystemType == Enums.DVBSSystemType.DVBS)
                        {
                            sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                            sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Polarization, tran.FEC, tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture), tran.Inversion));
                            sContent.Append("\n" + "/" + "\n");
                        }
                        else
                            if (Log.IsDebugEnabled) Log.Debug($"Transponder {tran} is not DVBS transponder, not supported in Enigma1, skipping");

                        break;
                    }
                    case Enums.SettingsVersion.Enigma2Ver5:
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver3:
                    {
                        sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                        sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Polarization, tran.FEC, tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture), tran.Inversion));
                        string extraData = string.Empty;

                        if (IsVer4OrLater(settings.SettingsVersion))
                        {
                            if (tran.Flags != null)
                                extraData = ":" + tran.Flags;
                            else
                                extraData = ":0";
                        }

                        if (tran.SystemType == Enums.DVBSSystemType.DVBS2)
                        {
                            if (tran.System != null)
                                extraData += ":" + tran.System;
                            else
                                extraData += ":1";

                            if (tran.Modulation != null)
                                extraData += ":" + tran.Modulation;
                            else
                                extraData += ":0";

                            if (tran.RollOff != null)
                                extraData += ":" + tran.RollOff;
                            else
                                extraData += ":0";

                            if (tran.Pilot != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.Pilot;

                            if (tran.IsId != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.IsId;

                            if (tran.PlsCode != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.PlsCode;

                            if (tran.PlsMode != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.PlsMode;

                            if (tran.T2miPlpId != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.T2miPlpId;

                            if (tran.T2miPid != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.T2miPid;
                        }

                        sContent.Append(extraData + "\n" + "/" + "\n");
                    }
                    break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string DVBCTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderDVBC tran in settings.Transponders.OfType<ITransponderDVBC>())
            {
                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                    {
                        string sRate = tran.SymbolRate ?? "0";
                        string frequency = tran.Frequency ?? "0";

                        if (frequency.Length < 8)
                            frequency = frequency + "00";
                        
                        sRate = sRate.Trim('0').Length > 0 ? sRate.PadRight(8, '0') : "0000";
                        sContent.Append("\t" + "s " + string.Join(":", frequency, sRate, "15", tran.FEC, "0", tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");

                        break;
                    }
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "c " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Inversion, tran.Modulation, tran.FEC));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver5:
                        sContent.Append("\t" + "c " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Inversion, tran.Modulation, tran.FEC, tran.Flags, tran.System));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string DVBTTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderDVBT tran in settings.Transponders.OfType<ITransponderDVBT>())
            {
                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                        sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, "0000", "0", "0", "0", tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "t " + string.Join(":", tran.Frequency, tran.Bandwidth, tran.FECHigh, tran.FECLow, tran.Modulation, tran.Transmission, tran.GuardInterval, tran.Hierarchy, tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver5:
                        sContent.Append("\t" + "t " + string.Join(":", tran.Frequency, tran.Bandwidth, tran.FECHigh, tran.FECLow, tran.Modulation, tran.Transmission, tran.GuardInterval, tran.Hierarchy, tran.Inversion, tran.Flags, tran.System, tran.PlpId));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string IPTVTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderIPTV tran in settings.Transponders.OfType<ITransponderIPTV>())
            {
                // optional fields are positional, so keep every field up to the last one that is set
                string[] optional = [tran.Inversion, tran.Modulation, tran.FEC, tran.Flags, tran.System];
                int last = Array.FindLastIndex(optional, x => x != null);
                var fields = new List<string> { tran.Frequency ?? "0", tran.SymbolRate ?? "0" };
                fields.AddRange(optional.Take(last + 1).Select(x => x ?? "0"));

                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID)).Append('\n');
                sContent.Append("\ts ").Append(string.Join(":", fields)).Append("\n/\n");
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Sort order of services in the settings file, matching the order transponders are written in
        /// </summary>
        private static int ServiceWriteGroup(IService service, bool enigma1)
        {
            switch (service.Transponder)
            {
                case null:
                    return 5;
                case ITransponderDVBS dvbs:
                    return enigma1 && dvbs.OrbitalPositionInt < 0 ? 4 : 0;
                case ITransponderDVBC:
                    return 1;
                case ITransponderDVBT:
                    return 2;
                default:
                    return 3;
            }
        }

        protected virtual string ServicesToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsServicesOpenTag + "\n");

            bool enigma1 = settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2;

            // OrderBy is stable, so the original order is kept inside each group
            foreach (IService service in settings.Services.OrderBy(x => ServiceWriteGroup(x, enigma1)))
                sContent.Append(ServiceToString(service, settings, service.Transponder == null));

            sContent.Append(SettingsClosingTag + "\n");
            return sContent.ToString();
        }

        protected virtual string ServiceToString(IService service, ISettings settings, bool bLocalTransponderIsNull = false)
        {
            var sContent = new StringBuilder();
            if (service.Transponder == null && bLocalTransponderIsNull != true)
            {
                Log.Warn(string.Format(Resources.SettingsIO_ServicesToString_Service__0__has_no_transponder_set__not_writing_it_to_settings_,
                    service.ToString().Replace("\t", "  ")));
            }
            else
            {
                bool serviceSupported = true;
                string flags = string.Empty;

                if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                {
                    if (service.Transponder is ITransponderDVBS dvbs)
                    {
                        ITransponderDVBS tran = dvbs;
                        if (tran.SystemType != Enums.DVBSSystemType.DVBS)
                            serviceSupported = false;
                    }

                    //make sure provider name is not empty for Enigma1 settings
                    IList<IFlag> flagList = service.FlagList;
                    IFlag pFlag = flagList.SingleOrDefault(x => x.FlagType == Enums.FlagType.P);
                    if (pFlag != null && pFlag.FlagValue.Length == 0)
                    {
                        pFlag.FlagValue = "unknown";
                        flags = string.Join(",", flagList.Select(x => x.FlagString).ToArray());
                    }
                    else if (pFlag == null)
                    {
                        flags = "p:unknown," + service.Flags;
                    }
                }

                if (serviceSupported)
                {
                    var sData = bLocalTransponderIsNull
                        ? string.Join(":", service.SID.PadLeft(4, '0'), service.TransponderId, service.Type, service.ProgNumber)
                        : string.Join(":", service.SID.PadLeft(4, '0'), service.Transponder.NameSpc.PadLeft(8, '0'), service.Transponder.TSID.PadLeft(4, '0'), service.Transponder.NID.PadLeft(4, '0'), service.Type, service.ProgNumber);

                    // the optional 7th field (source id) only exists in Enigma2 lamedb
                    if (!string.IsNullOrEmpty(service.SourceID) && settings.SettingsVersion is not (Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2))
                        sData += ":" + service.SourceID;

                    sContent.Append(sData + "\n");
                    sContent.Append(service.Name + "\n");

                    if (flags.Length > 0)
                        sContent.Append(flags + "\n");
                    else
                        sContent.Append(service.Flags + "\n");
                }
                else
                    if (Log.IsDebugEnabled) Log.Debug($"Service {service.ToString().Replace("\t", "  ")} is not supported in Enigma1. Not writing it to settings.");
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Writes bouquets to corresponding files
        /// </summary>
        /// <param name="directory">Directory info of the directory all bouquet files are written to</param>
        /// <param name="settings">Settings instance to be written></param>
        /// <remarks></remarks>
        protected virtual void WriteBouquets(string directory, ISettings settings)
        {
            if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
            {
                WriteTvBouquetsE1(directory, settings);
                WriteRadioBouquetsE1(directory, settings);
                WriteEnigma1Bouquets(directory, settings);
                WriteServicesLocked(directory, settings);
            }
            else if (settings.SettingsVersion is Enums.SettingsVersion.Enigma2Ver3 or Enums.SettingsVersion.Enigma2Ver4 or Enums.SettingsVersion.Enigma2Ver5)
            {
                WriteTvBouquetsE2(directory, settings);
                WriteRadioBouquetsE2(directory, settings);
                WriteAlternativeBouquets(directory, settings);
                WriteWhiteList(directory, settings);
                WriteBlackList(directory, settings);
            }
        }

        /// <summary>
        ///     Writes text to specified fileName
        /// </summary>
        /// <param name="fileName">Full path to file location od disk</param>
        /// <param name="content">File content as text</param>
        /// <param name="bouquetName">Name of the bouquet we're saving (for log purpose)</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if write fails</exception>
        protected virtual void WriteBouquet(string fileName, string content, string bouquetName)
        {
            try
            {
                OnFileSaving(this, fileName);
                _fileHelper.WriteAllText(fileName, content);
                if (Log.IsDebugEnabled) Log.Debug($"Sucessfully written bouquet {bouquetName} to {fileName}");
                OnFileSaved(this, fileName, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fileName, false);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteTvBouquetsE2_Failed_to_write_bouquet__0__to__1_, bouquetName, fileName), ex);
            }
        }

        /// <summary>
        ///     Creates text representation of the bouquet ready to be written to disk in Enigma1 format
        /// </summary>
        /// <param name="bouquet"></param>
        /// <returns>Returns text ready to be written to disk as a bouquet</returns>
        protected virtual string E1BouquetToString(IFileBouquet bouquet)
        {
            return BouquetToString(bouquet, true);
        }

        /// <summary>
        ///     Creates text representation of the bouquet ready to be written to disk in Enigma2 format
        /// </summary>
        /// <param name="bouquet"></param>
        /// <returns>Returns text ready to be written to disk as a bouquet</returns>
        protected virtual string E2BouquetToString(IFileBouquet bouquet)
        {
            return BouquetToString(bouquet, false);
        }

        /// <summary>
        ///     Creates text representation of the bouquet; Enigma1 uses "#SERVICE: " / "#DESCRIPTION: " and
        ///     #TYPE lines for sub bouquets, does not know alternatives and only supports DVB-S services
        /// </summary>
        protected virtual string BouquetToString(IFileBouquet bouquet, bool enigma1)
        {
            string serviceTag = enigma1 ? "#SERVICE: " : "#SERVICE ";
            string descriptionTag = enigma1 ? "#DESCRIPTION: " : "#DESCRIPTION ";
            bool debug = Log.IsDebugEnabled;

            var sContent = new StringBuilder();
            sContent.Append("#NAME ").Append(bouquet.Name).Append('\n');

            foreach (IBouquetItem bqItem in bouquet.BouquetItems)
            {
                switch (bqItem)
                {
                    case IBouquetItemBouquetsBouquet bib:
                        if (debug) Log.Debug($"Found bouquets bouquet {bib.Bouquet?.Name} in a file bouquet, skipping write to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemFileBouquet bif when enigma1:
                        string fName = Path.GetFileName(bif.FileName) ?? throw new SettingsException($"File bouquet reference in bouquet {bouquet.Name} has no file name");
                        if (fName.StartsWith("userbouquet.", StringComparison.OrdinalIgnoreCase))
                            fName = fName.Substring(12);
                        sContent.Append(serviceTag).Append(string.Join(":", bif.FavoritesTypeFlag, bif.LineSpecifierFlag, "0", fName.Split('.')[0], "0", "0", "0", "0", "0", "0", bif.FileName)).Append('\n');
                        sContent.Append("#TYPE 16385\n");
                        sContent.Append(bif.FileName.ToLowerInvariant()).Append('\n');
                        if (debug) Log.Debug($"File bouquet reference {bif.FileName} added to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemFileBouquet bif:
                        sContent.Append(serviceTag).Append(string.Join(":", bif.FavoritesTypeFlag, bif.LineSpecifierFlag, "1", "0", "0", "0", "0", "0", "0", "0", "FROM BOUQUET \"" + bif.FileName + "\" ORDER BY bouquet")).Append('\n');
                        if (debug) Log.Debug($"File bouquet reference {bif.FileName} added to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemAlternative bia when enigma1:
                        Log.Warn($"Alternatives are not supported in Enigma1, alternative {bia.FileName} not written to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemAlternative bia:
                        sContent.Append(serviceTag).Append(string.Join(":", bia.FavoritesTypeFlag, bia.LineSpecifierFlag, "1", "0", "0", "0", "0", "0", "0", "0", "FROM BOUQUET \"" + bia.FileName + "\" ORDER BY bouquet")).Append('\n');
                        if (debug) Log.Debug($"Alternative reference {bia.FileName} added to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemMarker bim:
                        sContent.Append(serviceTag).Append(string.Join(":", bim.FavoritesTypeFlag, bim.LineSpecifierFlag, bim.MarkerNumber, "0", "0", "0", "0", "0", "0", "0", "")).Append('\n');
                        sContent.Append(descriptionTag).Append(bim.Description).Append('\n');
                        if (debug) Log.Debug($"Marker {bim.Description} added to bouquet {bouquet.Name}");
                        break;

                    case IBouquetItemService bis:
                        AppendServiceReference(sContent, bis, bouquet, enigma1, serviceTag, descriptionTag);
                        break;

                    case IBouquetItemStream biss:
                        sContent.Append("#SERVICE ").Append(string.Join(":", biss.FavoritesTypeFlag, biss.LineSpecifierFlag, biss.StreamFlag, biss.ServiceID, biss.ExtraFlag1, biss.ExtraFlag2, "0", "0", "0", "0", BouquetItemStream.EncodeUrl(biss.URL), Uri.EscapeDataString(biss.Description).Replace("'", "%27"))).Append('\n');
                        sContent.Append("#DESCRIPTION ").Append(biss.Description).Append('\n');
                        if (debug) Log.Debug($"Stream {biss.Description}\t{biss.URL} added to bouquet {bouquet.Name}");
                        break;

                    default:
                        Log.Warn(string.Format(Resources.SettingsIO_WriteBouquets_Skipping_unknown_bouquet_item_type_in_bouquet__0_, bouquet.Name));
                        break;
                }
            }

            return sContent.ToString();
        }

        /// <summary>
        ///     Writes a service reference. References without matching service (or without transponder) are kept
        ///     as they were read, like Enigma does; use ISettings.RemoveInvalidBouquetItems to drop them.
        /// </summary>
        private void AppendServiceReference(StringBuilder sContent, IBouquetItemService bis, IFileBouquet bouquet, bool enigma1, string serviceTag, string descriptionTag)
        {
            if (bis.Service == null)
            {
                Log.Warn($"Service reference {bis.ServiceId} in bouquet {bouquet.Name} has no matching service, written unchanged");
            }
            else if (bis.Service.Transponder == null)
            {
                Log.Warn($"Service {bis.Service.Name} in bouquet {bouquet.Name} has no matching transponder, written unchanged");
            }
            else if (enigma1 && bis.Service.Transponder is ITransponderDVBS { SystemType: not Enums.DVBSSystemType.DVBS })
            {
                Log.Warn(string.Format(Resources.SettingsIO_E1BouquetToString_Service__0__in_bouquet__1__is_not_supported_in_Enigma1__Not_writing_it_, bis.Service.Name, bouquet.Name));
                return;
            }

            if (string.IsNullOrEmpty(bis.ServiceId))
                return;

            sContent.Append(serviceTag).Append(string.Join(":", bis.FavoritesTypeFlag, bis.LineSpecifierFlag, bis.ServiceId, "0", "0", "0", "")).Append('\n');

            if (!string.IsNullOrEmpty(bis.Description))
                sContent.Append(descriptionTag).Append(bis.Description).Append('\n');

            if (Log.IsDebugEnabled) Log.Debug($"Service {bis.ServiceId} added to bouquet {bouquet.Name}");
        }

        /// <summary>
        ///     Writes 'userbouquets.tv.epl' file and all TV bouquets to disk in Enigma1 format
        /// </summary>
        /// <param name="directory">Directory where all TV bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteTvBouquetsE1(string directory, ISettings settings)
        {
            WriteBouquetListE1(directory, settings, Enums.BouquetType.UserBouquetTv, UserBouquetTvEpl, "#NAME User - bouquets (TV)", ".tv");
        }

        /// <summary>
        ///     Writes 'bouquets.tv' file and all TV bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all TV bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        protected virtual void WriteTvBouquetsE2(string directory, ISettings settings)
        {
            WriteBouquetListE2(directory, settings, Enums.BouquetType.UserBouquetTv, BouquetsTvFile, "#NAME User - Bouquets (TV)", "1");
        }

        /// <summary>
        ///     Writes 'userbouquets.radio.epl' file and all radio bouquets to disk in Enigma1 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteRadioBouquetsE1(string directory, ISettings settings)
        {
            WriteBouquetListE1(directory, settings, Enums.BouquetType.UserBouquetRadio, UserBouquetRadioEpl, "#NAME User - bouquets (Radio)", ".radio");
        }

        /// <summary>
        ///     Writes 'bouquets.radio' file and all radio bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        protected virtual void WriteRadioBouquetsE2(string directory, ISettings settings)
        {
            WriteBouquetListE2(directory, settings, Enums.BouquetType.UserBouquetRadio, BouquetsRadioFile, "#NAME User - Bouquets (Radio)", "2");
        }

        /// <summary>
        ///     Writes the Enigma1 list file (userbouquets.*.epl) and all bouquets of the given type; bouquets are
        ///     renamed to userbouquet.dbeNN in list order
        /// </summary>
        private void WriteBouquetListE1(string directory, ISettings settings, Enums.BouquetType type, string listFile, string listName, string extension)
        {
            var list = new StringBuilder(listName).Append('\n');
            int bouquetIndex = 0;

            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == type).ToList())
            {
                string dbeIndex = "dbe" + bouquetIndex.ToString("00", CultureInfo.InvariantCulture);
                int lastSlash = bouquet.FileName.LastIndexOf('/');
                string enigmaPath = lastSlash > -1 ? bouquet.FileName.Substring(0, lastSlash + 1) : DefaultEnigma1Path;
                bouquet.FileName = enigmaPath + "userbouquet." + dbeIndex + extension;

                list.Append("#SERVICE: 4097:7:0:").Append(dbeIndex).Append(":0:0:0:0:0:0:").Append(bouquet.FileName).Append('\n');
                list.Append("#TYPE 16385\n");
                list.Append(bouquet.FileName).Append('\n');
                if (Log.IsDebugEnabled) Log.Debug($"Bouquet {bouquet.FileName} added to {listFile}");

                WriteBouquet(Path.Combine(directory, Path.GetFileName(bouquet.FileName)), E1BouquetToString(bouquet), bouquet.Name);
                bouquetIndex += 1;
            }

            WriteBouquet(Path.Combine(directory, listFile), list.ToString(), listFile);
        }

        /// <summary>
        ///     Writes the Enigma2 list file (bouquets.tv / bouquets.radio) and all bouquets of the given type;
        ///     sub bouquets are written as files but only referenced by their parent bouquet
        /// </summary>
        private void WriteBouquetListE2(string directory, ISettings settings, Enums.BouquetType type, string listFile, string listName, string serviceType)
        {
            var list = new StringBuilder(listName).Append('\n');

            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == type).ToList())
            {
                string fName = Path.GetFileName(bouquet.FileName);

                if (!bouquet.IsSubBouquet)
                {
                    list.Append("#SERVICE 1:").Append(bouquet.Hidden ? "519" : "7").Append(':').Append(serviceType)
                        .Append(":0:0:0:0:0:0:0:FROM BOUQUET \"").Append(fName).Append("\" ORDER BY bouquet\n");
                    if (Log.IsDebugEnabled) Log.Debug($"Bouquet {fName} added to {listFile}");
                }

                if (!string.IsNullOrEmpty(fName))
                    WriteBouquet(Path.Combine(directory, fName), E2BouquetToString(bouquet), bouquet.Name);
            }

            WriteBouquet(Path.Combine(directory, listFile), list.ToString(), listFile);
        }

        /// <summary>
        ///     Writes alternatives files referenced inside user bouquets (alternatives.*.tv)
        /// </summary>
        /// <param name="directory">Directory where alternatives files will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're writing</param>
        protected virtual void WriteAlternativeBouquets(string directory, ISettings settings)
        {
            // All user bouquets that can hold alternatives are registered at the top level by the
            // loader, so a single-level scan of settings.Bouquets covers every alternatives file.
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().ToList())
            {
                foreach (IBouquetItemAlternative alt in bouquet.BouquetItems.OfType<IBouquetItemAlternative>().ToList())
                {
                    if (alt.Bouquet == null)
                        continue;

                    string fName = Path.GetFileName(alt.FileName);
                    
                    if (fName == null)
                        continue;

                    string content = E2BouquetToString(alt.Bouquet);
                    WriteBouquet(Path.Combine(directory, fName), content, alt.Bouquet.Name);
                    if (Log.IsDebugEnabled) Log.Debug($"Alternatives file {fName} written");
                }
            }
        }

        /// <summary>
        ///     Writes Enigma2 locked services in blacklist file
        /// </summary>
        /// <param name="directory">Directory we're saving blacklist file to</param>
        /// <param name="settings">Instance of ISettings with locked services</param>
        /// <remarks></remarks>
        protected virtual void WriteBlackList(string directory, ISettings settings)
        {
            var fContent = new StringBuilder();
            IList<IService> sLocked = settings.Services.Where(x => x.ServiceSecurity == Enums.ServiceSecurity.BlackListed).ToList();
            if (sLocked.Any())
            {
                foreach (IService srv in sLocked)
                {
                    if (srv.Transponder == null)
                    {
                        Log.Warn(string.Format(
                            Resources.SettingsIO_WriteBlackList_Service__0__has_no_matching_transponder__not_adding_it_to_blacklist_file, srv));
                    }
                    else
                    {
                        fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to blacklist file");
                    }
                }
            }
            else
            {
                Log.Debug("There is no locked services to put inside blacklist file");
            }

            WriteFile(Path.Combine(directory, BlackListFile), fContent.ToString(), Utf8NoBom);
        }

        /// <summary>
        ///     Writes Enigma2 whitelisted services in whitelist file
        /// </summary>
        /// <param name="directory">Directory we're saving whitelist file to</param>
        /// <param name="settings">Instance of ISettings with whitelisted services</param>
        /// <remarks></remarks>
        protected virtual void WriteWhiteList(string directory, ISettings settings)
        {
            var fContent = new StringBuilder();
            IList<IService> sLocked = settings.Services.Where(x => x.ServiceSecurity == Enums.ServiceSecurity.WhiteListed).ToList();
            if (sLocked.Any())
            {
                foreach (IService srv in sLocked)
                {
                    if (srv.Transponder == null)
                    {
                        Log.Warn(string.Format(
                            Resources.SettingsIO_WriteWhiteList_Service__0__has_no_matching_transponder__not_adding_it_to_whitelist_file, srv));
                    }
                    else
                    {
                        fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to whitelist file");
                    }
                }
            }
            else
            {
                Log.Debug("There is no services to put inside whitelist file");
            }
            WriteFile(Path.Combine(directory, WhiteListFile), fContent.ToString(), Utf8NoBom);
        }

        /// <summary>
        ///     Writes locked services and bouquets to Enigma1 services.locked file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successful</exception>
        protected virtual void WriteServicesLocked(string directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver2)
                {
                    Log.Debug("Skipping write to services.locked file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory, ServicesLockedFile);
                var fContent = new StringBuilder();
                fContent.Append("Parentallocked Services" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        IList<IBouquetItemBouquetsBouquet> lockedBouquets =
                            existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().Where(x => x.Bouquet == null || x.Bouquet.Locked).ToList();
                        if (lockedBouquets.Any())
                        {
                            foreach (IBouquetItemBouquetsBouquet lbq in lockedBouquets)
                            {
                                if (lbq.Bouquet != null)
                                {
                                    fContent.Append(string.Join(":", "1:15:fffffffd:12", lbq.BouquetOrderNumber, "ffffffff:0:0:0:0:") + "\n");
                                    if (Log.IsDebugEnabled) Log.Debug($"Enigma1 bouquet {lbq.Bouquet.Name} added to services.locked");
                                }
                                else
                                {
                                    Log.Warn(
                                        string.Format(
                                            Resources
                                                .SettingsIO_WriteServicesLocked_Reference_to_Enigma1_bouquet__0__has_no_matching_bouquet_set__not_adding_it_to_services_locked,
                                            lbq.BouquetOrderNumber));
                                }
                            }
                        }
                        else
                        {
                            Log.Debug("There was no locked bouquets found in the list of Enigma1 Bouquets");
                        }
                    }
                    else
                    {
                        Log.Debug("There is no Enigma1 bouquets, no locked bouquets then.");
                    }
                }
                else
                {
                    Log.Debug("Enigma1 Bouquets bouquet not found in the list of bouquets, assuming no locked bouquets exists");
                }

                //adding locked services
                IList<IService> sLocked = settings.FindLockedServices().ToList();
                if (sLocked.Any())
                {
                    foreach (IService srv in sLocked)
                    {
                        if (srv.Transponder == null)
                        {
                            Log.Warn(
                                string.Format(Resources.SettingsIO_WriteServicesLocked_Service__0__has_no_transponder__not_adding_it_to_services_locked,
                                    srv));
                        }
                        else
                        {
                            fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                            if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to services.locked");
                        }
                    }
                }
                else
                {
                    Log.Debug("There is no locked services to put inside services.locked file");
                }

                WriteFile(fName, fContent.ToString(), Utf8NoBom);
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteServicesLocked_Failed_to_write_services_locked_file_to__0_, directory), ex);
            }
        }

        /// <summary>
        ///     Writes Enigma1 bouquets to bouquets file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successful</exception>
        protected virtual void WriteEnigma1Bouquets(string directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver2)
                {
                    Log.Debug("Skipping write to bouquets file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory, BouquetsFile);
                var fContent = new StringBuilder();
                fContent.Append("eDVB bouquets /2/" + "\n" + "bouquets" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        int bqOrderNumber = existing.BouquetItems.Count * -1;
                        foreach (IBouquetItemBouquetsBouquet bbq in existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList())
                        {
                            if (bbq.Bouquet != null)
                            {
                                //making sure all bouquets have correct order numbers 
                                bbq.BouquetOrderNumberInt = bqOrderNumber;

                                fContent.Append(string.Join("\n", bbq.BouquetOrderNumberInt.ToString(CultureInfo.InvariantCulture), bbq.Bouquet.Name, ""));
                                foreach (IBouquetItemService srv in bbq.Bouquet.BouquetItems.OfType<IBouquetItemService>())
                                {
                                    if (srv.Service != null)
                                    {
                                        if (srv.Service.Transponder != null)
                                        {
                                            fContent.Append(string.Join(":", srv.Service.SID.PadLeft(4, '0'), srv.Service.Transponder.TransponderId.ToLowerInvariant(), Convert.ToInt32(srv.Service.ServiceType, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)) + "\n");
                                            if (Log.IsDebugEnabled) Log.Debug($"Service {srv} in bouquet {bbq.Bouquet.Name} added to Enigma1 bouquets file");
                                        }
                                        else
                                        {
                                            Log.Warn(
                                                string.Format(
                                                    Resources
                                                        .SettingsIO_WriteEnigma1Bouquets_Service__0__in_Enigma1_bouqet__0__has_no_matching_transponder__not_adding_it_to_bouquets_file_,
                                                    srv.Service, bbq.Bouquet.Name));
                                        }
                                    }
                                    else
                                    {
                                        Log.Warn(
                                            string.Format(
                                                Resources
                                                    .SettingsIO_WriteEnigma1Bouquets_Service_reference__0__has_no_matching_service__not_adding_it_to_bouquet__1_,
                                                srv.ServiceId, bbq.Bouquet.Name));
                                    }
                                }
                                fContent.Append("/" + "\n");
                                bqOrderNumber += 1;
                                if (Log.IsDebugEnabled) Log.Debug($"Enigma1 bouquet {bbq.Bouquet.Name} added to Enigma1 bouquets file");
                            }
                            else
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources
                                            .SettingsIO_WriteEnigma1Bouquets_Reference_to_Enigma1_bouquet__0__has_no_matching_bouquet_set__not_adding_it_to_bouquets_file,
                                        bbq.BouquetOrderNumber));
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("Enigma1 bouquets bouquet has no items.");
                    }
                }
                else
                {
                    Log.Debug("Enigma1 Bouquets bouquet not found in the list of bouquets, assuming no Enigma1 bouquets");
                }

                fContent.Append("end" + "\n" + EditorName + "\n");

                WriteFile(fName, fContent.ToString(), Utf8NoBom);
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    $"Failed to write Enigma1 bouquets file to {directory}", ex);
            }
        }
        #endregion
    }
}
