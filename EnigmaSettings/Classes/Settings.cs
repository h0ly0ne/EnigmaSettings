// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class Settings : ISettings
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "IEditable"
        private bool _isEditing;
        private IList<IBouquet> _mBouquets;
        private IList<IXmlSatellite> _mSatellites;
        private IList<IXmlCable> _mCables;
        private IList<IService> _mServices;
        private IList<ITransponder> _mTransponders;
        private string _mSettingsFileName;
        private Enums.SettingsVersion _mSettingsVersion;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mBouquets = new List<IBouquet>(_bouquets);
            _mServices = new List<IService>(_services);
            _mTransponders = new List<ITransponder>(_transponders);
            _mSatellites = new List<IXmlSatellite>(_satellites);
            _mCables = new List<IXmlCable>(_cables);
            _mSettingsFileName = _settingsFileName;
            _mSettingsVersion = _settingsVersion;
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;

            Bouquets.Clear();
            foreach (var bouquet in _mBouquets)
            {
                Bouquets.Add(bouquet);
            }
            Services.Clear();
            foreach (var service in _mServices)
            {
                Services.Add(service);
            }
            Transponders.Clear();
            foreach (var transponder in _mTransponders)
            {
                Transponders.Add(transponder);
            }
            Satellites.Clear();
            foreach (var satellite in _mSatellites)
            {
                Satellites.Add(satellite);
            }
            Cables.Clear();
            foreach (var cable in _mCables)
            {
                Cables.Add(cable);
            }
            SettingsFileName = _mSettingsFileName;
            SettingsVersion = _mSettingsVersion;
            _isEditing = false;
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var settings = (ISettings)MemberwiseClone();
            settings.Bouquets = new List<IBouquet>();
            settings.Satellites = new List<IXmlSatellite>();
            settings.Cables = new List<IXmlCable>();
            settings.Services = new List<IService>();
            settings.Transponders = new List<ITransponder>();
            settings.Log = this.Log;

            foreach (var bouquet in Bouquets)
            {
                if (bouquet != null)
                    settings.Bouquets.Add((IBouquet)bouquet.Clone());
            }

            foreach (var satellite in Satellites)
            {
                if (satellite != null)
                    settings.Satellites.Add((IXmlSatellite)satellite.Clone());
            }

            foreach (var cable in Cables)
            {
                if (cable != null)
                    settings.Cables.Add((IXmlCable)cable.Clone());
            }

            foreach (var service in Services)
            {
                if (service != null)
                    settings.Services.Add((IService)service.Clone());
            }

            foreach (var transponder in Transponders)
            {
                if (transponder != null)
                    settings.Transponders.Add((ITransponder)transponder.Clone());
            }

            return settings;
        }
        #endregion

        #region "Properties"
        private IList<IBouquet> _bouquets = new List<IBouquet>();
        private IList<IXmlSatellite> _satellites = new List<IXmlSatellite>();
        private IList<IXmlCable> _cables = new List<IXmlCable>();
        private IList<IService> _services = new List<IService>();
        private IList<ITransponder> _transponders = new List<ITransponder>();

        private ILog _log;
        private static readonly ILog NullLogger = new NullLogger();

        private string _settingsFileName = string.Empty;
        private Enums.SettingsVersion _settingsVersion = Enums.SettingsVersion.Unknown;

        /// <summary>
        ///     Instance used for logging
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ILog Log
        {
            get => _log ?? NullLogger;
            set
            {
                if (value == null) 
                    return;

                if (value == _log) 
                    return;

                _log = value;
                OnPropertyChanged(nameof(Log));
            }
        }

        /// <summary>
        ///     Full path to settings file location on disk
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SettingsFileName
        {
            get => _settingsFileName;
            set
            {
                if (value == _settingsFileName) 
                    return;

                _settingsFileName = value;
                OnPropertyChanged(nameof(SettingsFileName));
            }
        }

        /// <summary>
        ///     Directory with all the files for the settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SettingsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(SettingsFileName))
                    return string.Empty;

                try
                {
                    return Path.GetDirectoryName(SettingsFileName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///     Version of the settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Enums.SettingsVersion SettingsVersion
        {
            get => _settingsVersion;
            set
            {
                if (value == _settingsVersion) 
                    return;

                _settingsVersion = value;
                OnPropertyChanged(nameof(SettingsVersion));
            }
        }

        /// <summary>
        ///     List of satellites from satellites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Each satellite has corresponding xml transponders from satellites.xml file</remarks>
        [DataMember]
        public IList<IXmlSatellite> Satellites
        {
            get => _satellites;
            set
            {
                value ??= new List<IXmlSatellite>();
                _satellites = value;
                OnPropertyChanged(nameof(Satellites));
            }
        }

        /// <summary>
        ///     List of cables from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Each cable has corresponding xml transponders from cables.xml file</remarks>
        [DataMember]
        public IList<IXmlCable> Cables
        {
            get => _cables;
            set
            {
                value ??= new List<IXmlCable>();

                _cables = value;
                OnPropertyChanged(nameof(Cables));
            }
        }

        /// <summary>
        ///     All transponders (ATSC, DVBC, DVBS, DVBT, IPTV) from settings/lamedb file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>These are not transponders from atsc.xml/cables.xml/satellites.xml/terrestrial.xml file</remarks>
        [DataMember]
        public IList<ITransponder> Transponders
        {
            get => _transponders;
            set
            {
                value ??= new List<ITransponder>();
                _transponders = value;
                OnPropertyChanged(nameof(Transponders));
            }
        }

        /// <summary>
        ///     All services from settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Each service should have corresponding transponder from services file</remarks>
        [DataMember]
        public IList<IService> Services
        {
            get => _services;
            set
            {
                value ??= new List<IService>();
                _services = value;
                OnPropertyChanged(nameof(Services));
            }
        }

        /// <summary>
        ///     All bouquets except the ones stored in 'bouquets' file from Enigma 1
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IList<IBouquet> Bouquets
        {
            get => _bouquets;
            set
            {
                value ??= new List<IBouquet>();
                _bouquets = value;
                OnPropertyChanged(nameof(Bouquets));
            }
        }
        #endregion

        #region "Public methods"
        /// <summary>
        ///     Returns list of satellite transponders from list with all transponders
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<ITransponderDVBS> FindSatelliteTransponders()
        {
            return
                new List<ITransponderDVBS>(
                    Transponders.OfType<ITransponderDVBS>().OrderBy(x => x.OrbitalPositionInt).ThenBy(x => x.Frequency).ToList());
        }

        /// <summary>
        ///     Returns list of cable transponders from list with all transponders
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<ITransponderDVBC> FindCableTransponders()
        {
            return
                new List<ITransponderDVBC>(
                    Transponders.OfType<ITransponderDVBC>().OrderBy(x => x.Frequency).ToList());
        }

        /// <summary>
        ///     Returns list of transponders for selected satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<ITransponderDVBS> FindTranspondersForSatellite(IXmlSatellite satellite)
        {
            return
                new List<ITransponderDVBS>(
                    FindSatelliteTransponders().Where(x => x.OrbitalPositionInt == Convert.ToInt32(satellite.Position)).ToList());
        }

        /// <summary>
        ///     Returns list of services for selected satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<IService> FindServicesForSatellite(IXmlSatellite satellite)
        {
            return
                new List<IService>(
                    Services.Where(
                        x =>
                            (x.Transponder) is ITransponderDVBS dvbs &&
                            dvbs.OrbitalPositionInt == Convert.ToInt32(satellite.Position)).ToList());
        }

        /// <summary>
        ///     Returns list of satellite transponders without corresponding satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<ITransponderDVBS> FindTranspondersWithoutSatellite()
        {
            return new List<ITransponderDVBS>(FindSatelliteTransponders().Where(x => x.Satellite == null).ToList());
        }

        /// <summary>
        ///     Returns list of services without corresponding transponder
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<IService> FindServicesWithoutTransponder()
        {
            return new List<IService>(Services.Where(x => x.Transponder == null).ToList());
        }

        /// <summary>
        ///     Returns list of services for transponder
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<IService> FindServicesForTransponder(ITransponder transponder)
        {
            return new List<IService>(Services.Where(x => x.TransponderId == transponder.TransponderId).ToList());
        }

        /// <summary>
        ///     Returns list of locked services
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        public IList<IService> FindLockedServices()
        {
            return new List<IService>(Services.Where(x => x.ServiceSecurity == Enums.ServiceSecurity.BlackListed).ToList());
        }

        /// <summary>
        ///     Returns list of bouquets that have duplicate names
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public IList<IFileBouquet> FindBouquetsWithDuplicateFilename()
        {
            try
            {
                Log.Debug("Searching  for bouquets with duplicate filenames");
                var fileNames = Bouquets.OfType<IFileBouquet>().GroupBy(x => x.FileName).Where(g => g.Count() > 1).Select(grp => grp.Key).ToList();
                foreach (var fileName in fileNames)
                {
                    Log.Debug($"Bouquet filename {fileName} is not unique.");
                }
                return Bouquets.OfType<IFileBouquet>().GroupBy(x => x.FileName).Where(grp => grp.Count() > 1).SelectMany(grp => grp).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException("There was an error while searching for bouquets with duplicate filenames", ex);
            }
        }

        /// <summary>
        ///     Adds new xmlSatellites and xmlTransponders for all transponders in settings file
        ///     that don't have corresponding xmlSatellite.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="frequencyToleration">Toleration in Hz when searching for existing transponder</param>
        /// <remarks>Only transponders for new xmlSatellites will be added if not found.</remarks>
        /// <exception cref="SettingsException"></exception>
        public void AddMissingXmlSatellites(IInstanceFactory factory, int frequencyToleration = 30000)
        {
            var trans = FindTranspondersWithoutSatellite().ToList();

            if (trans.Count == 0)
            {
                Log.Debug("All satellite transponders have corresponding satellite");
                return;
            }
            Log.Warn($"Found {trans.Count} transponders without corresponding satellite");


            Log.Debug("Adding missing xmlSatellites for transponders in settings file");

            try
            {
                var missingPositions = trans.Select(x => x.OrbitalPositionInt).Distinct().OrderBy(x => x).ToList();
                var newSatellites = new List<IXmlSatellite>();

                //adding non existing satellites
                foreach (var position in missingPositions)
                {
                    if (position == 0) continue;
                    var mPosition = position;
                    if (Satellites.Any(x => x.Position == mPosition.ToString(CultureInfo.CurrentCulture)))
                    {
                        var xmlSat = factory.InitNewXmlSatellite();
                        xmlSat.Position = position.ToString(CultureInfo.CurrentCulture);
                        xmlSat.Flags = "0";
                        xmlSat.Name = "Sat " + xmlSat.PositionString;
                        Satellites.Add(xmlSat);
                        newSatellites.Add(xmlSat);
                        IList<ITransponderDVBS> found = FindTranspondersWithoutSatellite()
                            .Where(x => x.OrbitalPositionInt == Convert.ToInt32(xmlSat.Position))
                            .ToList();
                        foreach (var transponder in found)
                        {
                            transponder.Satellite = xmlSat;
                        }
                        Log.Warn(string.Format(Resources.Settings_AddMissingXMLSatellites_New_satellite_for_position__0__added_to_satellites, position));
                    }
                    else
                    {
                        Log.Warn(
                            string.Format(
                                Resources
                                    .Settings_AddMissingXMLSatellites_WARNING__Some_transponders_are_market_as_satellite_but_have_invalid_position_0_degrees_));
                    }
                }

                foreach (var newsat in newSatellites)
                {
                    var transList = FindTranspondersForSatellite(newsat);

                    foreach (var tran in transList)
                    {
                        var mTran = tran;
                        IList<IXmlTransponder> query =
                            newsat.Transponders.Where(
                                t =>
                                    ((Convert.ToInt32(t.Frequency) < Convert.ToInt32(mTran.Frequency) + frequencyToleration) &&
                                     (Convert.ToInt32(t.Frequency) > Convert.ToInt32(mTran.Frequency) - frequencyToleration)) &&
                                    t.Polarization == mTran.Polarization && t.SymbolRate == mTran.SymbolRate).ToList();

                        if (query.Count != 0) continue;
                        var xmlTran = factory.InitNewXmlTransponder(tran);
                        tran.Satellite.Transponders.Add(xmlTran);
                        Log.Warn(string.Format(Resources.Settings_AddMissingXMLSatellites_Added_new_transponder__0__for_previously_missing_satellite__1_,
                            tran.TransponderId, newsat.PositionString));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_AddMissingXMLSatellites_Failed_to_update_missing_XML_satellites, ex);
            }
        }

        /// <summary>
        ///     Adds new xmlTransponders for all transponders in settings file
        ///     that don't have corresponding xmlTransponder in xmlSatellites
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="frequencyToleration">Toleration in Hz when searching for existing transponder</param>
        /// <remarks>Transponders are added to all satellites without matching transponder</remarks>
        /// <exception cref="SettingsException"></exception>
        public void AddMissingXmlTranspondersSatellites(IInstanceFactory factory, int frequencyToleration = 30000)
        {
            try
            {
                AddMissingXmlSatellites(factory, frequencyToleration);

                Log.Debug("Adding missing xmlTransponders for transponders in settings file");

                IList<ITransponderDVBS> tranS = FindSatelliteTransponders().OrderBy(x => x.OrbitalPositionInt).ToList();

                foreach (var tran in tranS)
                {
                    var mTran = tran;
                    IList<IXmlTransponder> query =
                        tran.Satellite.Transponders.Where(
                            t =>
                                ((Convert.ToInt32(t.Frequency) < Convert.ToInt32(mTran.Frequency) + frequencyToleration) &&
                                 (Convert.ToInt32(t.Frequency) > Convert.ToInt32(mTran.Frequency) - frequencyToleration)) &&
                                t.Polarization == mTran.Polarization && t.SymbolRate == mTran.SymbolRate).ToList();

                    if (query.Any()) continue;
                    //there is no corresponding transponder in xml, go ahead and add it
                    var xmlTran = factory.InitNewXmlTransponder(tran);
                    tran.Satellite.Transponders.Add(xmlTran);
                    Log.Warn($"Transponder ID {tran.TransponderId} added to xmlTransponders");
                }
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_AddMissingXMLTransponders_Failed_to_update_missing_XML_transponders, ex);
            }
        }

        /// <summary>
        ///     Matches all services to corresponding transponders and warns missing transponders
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void MatchServicesWithTransponders()
        {
            Log.Debug("Matching services with transponders");
            try
            {
                var query = Services.Join(Transponders, sr => sr.TransponderId, tr => tr.TransponderId, (sr, tr) => new
                {
                    Transponder = tr,
                    Service = sr
                });

                foreach (var match in query.ToList())
                {
                    Log.Debug(string.Format("Service ID {0} {1} matched with transponder ID {2}", match.Service.SID, match.Service.Name,
                        match.Transponder.TransponderId));
                    match.Service.Transponder = match.Transponder;
                }

                IList<IService> servc = FindServicesWithoutTransponder();
                if (servc.Count == 0)
                {
                    Log.Debug("All services have corresponding transponder");
                }
                else
                {
                    foreach (IService srv in servc)
                    {
                        Log.Warn(string.Format(Resources.Settings_MatchServicesWithTransponders_No_transponder_has_been_found_for_service__0_____1_,
                            srv.ServiceId, srv.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    Resources.Settings_MatchServicesWithTransponders_There_was_an_error_while_trying_to_match_services_with_transponders, ex);
            }
        }

        /// <summary>
        ///     Matches transponders from settings file without corresponding satellite to satellites from xml file
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void MatchSatellitesWithTransponders()
        {
            if (Satellites.Count == 0)
            {
                Log.Debug("There is no Satellites from satellites.xml file, skipping matching satellites with transponders");
                return;
            }
            Log.Debug("Matching satellites with transponders");

            try
            {
                IList<ITransponderDVBS> transDVBS = FindSatelliteTransponders();
                var query = transDVBS.Join(Satellites, tr => Convert.ToString(tr.OrbitalPositionInt), sat => sat.Position, (tr, sat) => new
                {
                    Transponder = tr,
                    Satellite = sat
                });

                foreach (var match in query.ToList())
                {
                    Log.Debug($"Transponder {match.Transponder.TransponderId} matched to satellite {match.Satellite.Name}");
                    match.Transponder.Satellite = match.Satellite;
                }

                foreach (ITransponderDVBS tr in transDVBS.Where(x => x.Satellite == null).ToList())
                {
                    Log.Warn(string.Format(Resources.Settings_MatchSatellitesWithTransponders_No_satellite_has_been_found_for_transponder__0_,
                        tr.TransponderId));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    Resources.Settings_MatchSatellitesWithTransponders_There_was_an_error_while_matching_transponders_with_satellites_, ex);
            }
        }

        /// <summary>
        ///     Matches services from file bouquets to services from settings
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void MatchFileBouquetsServices()
        {
            if (Bouquets.Count == 0)
            {
                Log.Debug("There is no bouquets, skip matching file bouquets services to services from settings");
                return;
            }
            Log.Debug("Matching file bouquets services to services from settings");

            try
            {
                IList<IBouquetItemService> bis = Bouquets.SelectMany(x => x.BouquetItems).OfType<IBouquetItemService>().ToList();
                var query = bis.Join(Services, bs => bs.ServiceId.ToLower(), srv => srv.ServiceId.ToLower(), (bs, srv) => new
                {
                    BouquetItem = bs,
                    Service = srv
                });

                foreach (var match in query.ToList())
                {
                    match.BouquetItem.Service = match.Service;
                }

                foreach (IBouquet bq in Bouquets)
                {
                    foreach (IBouquetItemService bqis in bq.BouquetItems.OfType<IBouquetItemService>().ToList().Where(bqis => bqis.Service != null))
                    {
                        Log.Debug($"Bouquet item {bqis.ServiceId} in file bouquet {bq.Name} matched to service {bqis.Service}");
                    }
                }

                IList<IBouquetItemService> notMatched = bis.Where(x => x.Service == null).ToList();
                if (notMatched.Count == 0)
                {
                    Log.Debug("All file bouquets services have corresponding service from settings");
                }
                else
                {
                    foreach (IBouquetItemService nm in notMatched)
                    {
                        Log.Warn(string.Format(Resources.Settings_MatchBouquetServices_Bouquet_service__0__not_found_in_settings, nm.ServiceId));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }
        }

        /// <summary>
        ///     Matches services from bouquets inside 'bouquets' file to services from settings
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void MatchBouquetsBouquetsServices()
        {
            Log.Debug("Matching services from Enigma1 bouquets file to services from settings");

            try
            {
                IBouquet bouquetsBouquet = Bouquets.SingleOrDefault(x => x.BouquetType == Enums.BouquetType.E1Bouquets);

                if (bouquetsBouquet == null)
                {
                    Log.Debug("'bouquets' bouquet not found in the list of bouquets, skipping matching...");
                    return;
                }

                var a = bouquetsBouquet.BouquetItems.OfType<IBouquetItemBouquetsBouquet>();
                var b = a.SelectMany(x => x.Bouquet.BouquetItems);
                IList<IBouquetItemService> bis = b.OfType<IBouquetItemService>().ToList();

                if (bis.Count == 0)
                {
                    Log.Debug("'bouquets' bouquet has no services, skipping matching...");
                    return;
                }

                var query = bis.Join(Services, bs => bs.ServiceId.ToLower(), srv => srv.ServiceId.ToLower(), (bs, srv) => new
                {
                    BouquetItem = bs,
                    Service = srv
                });

                foreach (var match in query.ToList())
                {
                    match.BouquetItem.Service = match.Service;
                }

                foreach (IBouquetItemBouquetsBouquet bq in bouquetsBouquet.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList())
                {
                    foreach (IBouquetItemService bqis in bq.Bouquet.BouquetItems.OfType<IBouquetItemService>().ToList().Where(bqis => bqis.Service != null)
                        )
                    {
                        Log.Debug(string.Format("Bouquet item {0} in Enigma1 bouquet {1} matched to service {2}", bqis.ServiceId, bq.Bouquet.Name,
                            bqis.Service));
                    }
                }

                IList<IBouquetItemService> notMatched = bis.Where(x => x.Service == null).ToList();
                if (notMatched.Count == 0)
                {
                    Log.Debug("All services from Enigma1 bouquets file have corresponding service from settings");
                }
                else
                {
                    foreach (IBouquetItemService nm in notMatched)
                    {
                        Log.Warn(string.Format(Resources.Settings_MatchBouquetServices_Bouquet_service__0__not_found_in_settings, nm.ServiceId));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }
        }

        /// <summary>
        ///     Renumbers markers in all bouquets
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RenumberMarkers()
        {
            try
            {
                Log.Debug("Renumbering marker numbers");
                IList<IBouquetItemMarker> bim = Bouquets.SelectMany(x => x.BouquetItems).OfType<IBouquetItemMarker>().ToList();
                for (int i = 0; i <= bim.Count - 1; i++)
                {
                    bim[i].MarkerNumber = i.ToString("X");
                    Log.Debug($"Marker {bim[i].Description} now has number {i:X} (integer value {i}) ");
                }
                Log.Debug("Finished renumbering markers");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RenumberMarkers_There_was_an_error_while_renumbering_markers_, ex);
            }
        }

        /// <summary>
        ///     Renumbers file names for all file bouquets that have filename starting with 'userbouquet.dbe'
        /// </summary>
        /// <remarks>Preserves path in filename if any is set</remarks>
        /// <exception cref="SettingsException"></exception>
        public void RenumberBouquetFileNames()
        {
            try
            {
                Log.Debug("Renumbering bouquets filenames");
                IList<IFileBouquet> bqsTv = Bouquets.OfType<IFileBouquet>().Where(x =>
                {
                    string s = Path.GetFileName(x.FileName);
                    return s != null && (x.BouquetType == Enums.BouquetType.UserBouquetTv && s.StartsWith("userbouquet.dbe"));
                }).OrderBy(x => x.FileName).ToList();
                int index = 0;
                foreach (IFileBouquet bouquet in bqsTv)
                {
                    string name = Path.GetFileName(bouquet.FileName);
                    if (name != null && !name.StartsWith("userbouquet.dbe")) continue;
                    string fileName = string.Empty;
                    if (bouquet.FileName.IndexOf("/", StringComparison.CurrentCulture) > -1)
                    {
                        fileName = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf("/", StringComparison.CurrentCulture) + 1);
                    }
                    fileName = fileName + "userbouquet.dbe" + index.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0') + ".tv";
                    if (fileName != bouquet.FileName)
                    {
                        Log.Debug(string.Format("Changed filename for bouquet {0} from {1} to {2}", bouquet.Name, Path.GetFileName(bouquet.FileName),
                            Path.GetFileName(fileName)));
                        bouquet.FileName = fileName;
                    }
                    index += 1;
                }
                index = 0;
                IList<IFileBouquet> bqsRadio = Bouquets.OfType<IFileBouquet>().Where(x =>
                {
                    string name = Path.GetFileName(x.FileName);
                    return name != null && (x.BouquetType == Enums.BouquetType.UserBouquetRadio && name.StartsWith("userbouquet.dbe"));
                }).OrderBy(x => x.FileName).ToList();
                foreach (IFileBouquet bouquet in bqsRadio)
                {
                    string fileName = string.Empty;
                    if (bouquet.FileName.IndexOf("/", StringComparison.CurrentCulture) > -1)
                    {
                        fileName = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf("/", StringComparison.CurrentCulture) + 1);
                    }
                    fileName = fileName + "userbouquet.dbe" + index.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0') + ".radio";
                    if (fileName != bouquet.FileName)
                    {
                        Log.Debug(string.Format("Changed filename for bouquet {0} from {1} to {2}", bouquet.Name, Path.GetFileName(bouquet.FileName),
                            Path.GetFileName(fileName)));
                        bouquet.FileName = fileName;
                    }
                    index += 1;
                }
                Log.Debug("Finished renumbering file names");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RenumberBouquetFileNames_There_was_an_error_while_trying_to_renumber_bouquet_file_names_, ex);
            }
        }

        /// <summary>
        ///     Removes services without transponder from the list of services and all bouquets
        /// </summary>
        /// <remarks></remarks>
        public void RemoveServicesWithoutTransponder()
        {
            Log.Debug("Removing services without transponder");
            IList<IService> srvcs = Services.Where(x => x.Transponder == null).ToList();
            RemoveServices(srvcs);
            Log.Debug($"Removed {srvcs.Count} services without transponder");
        }

        /// <summary>
        ///     Removes markers that are followed by another marker, or not followed with any items
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveEmptyMarkers()
        {
            RemoveEmptyMarkers(null);
        }

        /// <summary>
        /// Removes markers that are followed by another marker, or not followed with any items
        /// </summary>
        /// <param name="preserveCondition">Markers that match specified condition will always be preserved</param>
        /// <exception cref="SettingsException"></exception>
        public void RemoveEmptyMarkers(Func<IBouquetItemMarker, bool> preserveCondition)
        {
            try
            {
                Log.Debug("Removing empty markers");

                List<IBouquetItemMarker> preserveList = null;
                if (preserveCondition != null)
                {
                    preserveList = Bouquets.SelectMany(x => x.BouquetItems).OfType<IBouquetItemMarker>().Where(preserveCondition).ToList();
                }

                foreach (IBouquet bouquet in Bouquets.Where(bouquet => bouquet.BouquetItems.Count > 0))
                {

                    for (int i = bouquet.BouquetItems.Count - 1; i >= 0; i += -1)
                    {
                        var bouquetItemMarker = bouquet.BouquetItems.ElementAt(i) as IBouquetItemMarker;
                        //we're only interested in markers
                        if (bouquetItemMarker == null) continue;
                        if (i == bouquet.BouquetItems.Count - 1)
                        {
                            if (preserveList is { Count: > 0 } && preserveList.Contains(bouquetItemMarker))
                            {
                                Log.Debug(String.Format("Marker {0} in bouquet {1} is the last item, " +
                                                        "but not deleting it because it's matching preserve condition",
                                                        bouquetItemMarker.Description, bouquet.Name));
                            }
                            else
                            {
                                //if marker is last item in the list remove it
                                bouquet.BouquetItems.RemoveAt(i);
                                Log.Debug(String.Format("Empty marker {0} removed from bouquet {1} because it's the last item in the bouquet",
                                    bouquetItemMarker.Description, bouquet.Name));
                            }
                        }
                        else if (bouquet.BouquetItems.ElementAt(i + 1) is IBouquetItemMarker)
                        {
                            if (preserveList is { Count: > 0 } && preserveList.Contains(bouquetItemMarker))
                            {
                                Log.Debug(String.Format("Marker {0} in bouquet {1} is suceeded by another marker, " +
                                                        "but not deleting it because it's matching preserve condition",
                                                        bouquetItemMarker.Description, bouquet.Name));
                            }
                            else
                            {
                                //if next item in the bouquet is also marker remove this one
                                bouquet.BouquetItems.RemoveAt(i);
                                Log.Debug(String.Format("Empty marker {0} removed from bouquet {1} because it's suceeded by another marker",
                                    bouquetItemMarker.Description, bouquet.Name));
                            }
                        }
                    }
                }
                Log.Debug("Finished removing empty markers");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveDoubleMarkers_There_was_an_error_while_removing_double_markers, ex);
            }
        }

        /// <summary>
        ///     Removes bouquets without any items
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveEmptyBouquets()
        {
            try
            {
                Log.Debug("Removing empty bouquets");
                IList<IBouquet> bqs = Bouquets.Where(x => x.BouquetItems.Count == 0).ToList();
                foreach (var bouquet in bqs)
                {
                    Bouquets.Remove(bouquet);
                    Log.Debug($"Removed empty bouquet {bouquet.Name} from the list of bouquets");
                }
                Log.Debug("Finished removing empty bouquets");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveEmptyBouquets_There_was_an_error_while_removing_empty_bouquets, ex);
            }
        }

        /// <summary>
        ///     Removes all stream references from all bouquets
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveStreams()
        {
            try
            {
                Log.Debug("Removing streams from bouquets");
                foreach (var bouquet in Bouquets)
                {
                    IList<IBouquetItemStream> streams = bouquet.BouquetItems.OfType<IBouquetItemStream>().ToList();
                    foreach (var stream in streams)
                    {
                        bouquet.BouquetItems.Remove(stream);
                        Log.Debug($"Stream {stream.Description} removed from bouquet {bouquet.Name}");
                    }
                }
                Log.Debug("Finished removing streams from bouquets");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveStreams_There_was_an_error_while_removing_streams, ex);
            }
        }

        /// <summary>
        ///     Removes service from list of services and all bouquets
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveService(IService service)
        {
            try
            {
                if (service == null)
                    throw new ArgumentNullException();

                Log.Debug($"Removing service {service.Name} from settings");

                foreach (var bouquet in Bouquets)
                {
                    IList<IBouquetItemService> biServices = bouquet.BouquetItems.OfType<IBouquetItemService>().Where(x => x.ServiceId == service.ServiceId).ToList();

                    foreach (var bis in biServices)
                    {
                        bouquet.BouquetItems.Remove(bis);
                        Log.Debug($"Service {service.Name} removed from bouquet {bouquet.Name}");
                    }
                }
                if (Services != null && Services.Contains(service))
                    Services.Remove(service);

                Log.Debug($"Service {service.Name} removed from settings");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                if (service != null)
                    throw new SettingsException(string.Format(Resources.Settings_RemoveService_There_was_an_error_while_removing_service__0_, service.Name), ex);
                
                throw new SettingsException(string.Format(Resources.Settings_RemoveService_There_was_an_error_while_removing_service__0_, String.Empty), ex);
            }
        }

        /// <summary>
        ///     Removes services from list of services and all bouquets
        /// </summary>
        /// <param name="srv">List of services to be removed</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveServices(IList<IService> srv)
        {
            try
            {
                if (srv.Count == 1)
                    RemoveService(srv.First());
                else if (srv.Count > 0)
                {
                    Log.Debug($"Removing {srv.Count} services from settings");

                    foreach (var bouquet in Bouquets)
                    {
                        IList<IBouquetItemService> bouquetServices = bouquet.BouquetItems.OfType<IBouquetItemService>().Where(x => (x.Service != null)).ToList();
                        var query = bouquetServices.Join(srv, bs => bs.ServiceId.ToLower(), sr => sr.ServiceId.ToLower(), (bs, sr) => new
                        {
                            BouquetItem = bs,
                            Service = sr
                        });

                        foreach (var match in query.ToList())
                        {
                            bouquet.BouquetItems.Remove(match.BouquetItem);
                            Log.Debug($"Service {match.Service.Name} removed from bouquet {bouquet.Name}");
                        }
                    }

                    foreach (var service in srv.Where(service => Services.Contains(service)))
                    {
                        Services.Remove(service);
                        Log.Debug($"Service {service.Name} removed from services");
                    }

                    Log.Debug("Services removed");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveServices_There_was_an_error_while_deleting_services, ex);
            }
        }

        /// <summary>
        ///     Removes all services from list of services and all bouquets
        /// </summary>
        public void RemoveAllServices()
        {
            RemoveServices(Services.ToList());
        }

        /// <summary>
        ///     Removes transponder from list of transponders
        ///     and all services on that transponder from all bouquets and from list of services
        /// </summary>
        /// <param name="transponder"></param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveTransponder(ITransponder transponder)
        {
            try
            {
                if (transponder == null)
                    throw new ArgumentNullException();

                Log.Debug($"Removing transponder {transponder.TransponderId} from settings");

                foreach (var bouquet in Bouquets)
                {
                    IList<IBouquetItemService> biServices = bouquet.BouquetItems.OfType<IBouquetItemService>().Where(x => x.Service != null && Equals(x.Service.Transponder.TransponderId, transponder.TransponderId)).ToList();
                    
                    foreach (var bis in biServices)
                    {
                        bouquet.BouquetItems.Remove(bis);
                        Log.Debug($"Service {bis.Service.Name} removed from bouquet {bouquet.Name}");
                    }
                }

                foreach (var service in Services.Where(x => Equals(x.Transponder.TransponderId, transponder.TransponderId)).ToList())
                {
                    Services.Remove(service);
                    Log.Debug($"Service {service.Name} removed from services");
                }

                if (Transponders != null && Transponders.Contains(transponder))
                    Transponders.Remove(transponder);

                Log.Debug($"Transponder {transponder.TransponderId} removed from settings");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);

                if (transponder != null)
                    throw new SettingsException(string.Format(Resources.Settings_RemoveTransponder_There_was_an_error_while_removing_transponder__0_, transponder.TransponderId), ex);

                throw new SettingsException(
                    string.Format(Resources.Settings_RemoveTransponder_There_was_an_error_while_removing_transponder__0_, String.Empty), ex);
            }
        }

        /// <summary>
        ///     Removes transponders from list of transponders
        ///     and all services on specified transponders from all bouquets and from list of services
        /// </summary>
        /// <param name="trans"></param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveTransponders(IList<ITransponder> trans)
        {
            try
            {
                if (trans.Count == 1)
                    RemoveTransponder(trans.First());
                else if (trans.Count > 0)
                {
                    Log.Debug($"Removing {trans.Count} transponders from settings");

                    foreach (var bouquet in Bouquets)
                    {
                        IList<IBouquetItemService> bouquetServices = bouquet.BouquetItems.OfType<IBouquetItemService>().Where(x => (x.Service != null)).ToList();

                        var query = bouquetServices.Join(trans, bs => bs.Service.Transponder.TransponderId, tr => tr.TransponderId, (bs, tr) => new
                        {
                            BouquetItem = bs,
                            Transponder = tr
                        });
                        var list1 = query.ToList();

                        foreach (var match in list1)
                        {
                            bouquet.BouquetItems.Remove(match.BouquetItem);
                            Log.Debug($"Service {match.BouquetItem.Service.Name} removed from bouquet {bouquet.Name}");
                        }
                    }

                    var query2 = Services.Join(trans, sr => sr.Transponder.TransponderId, tr => tr.TransponderId, (sr, tr) => new
                    {
                        Service = sr,
                        Transponder = tr
                    });
                    var list2 = query2.ToList();

                    foreach (var match in list2)
                    {
                        Services.Remove(match.Service);
                        Log.Debug($"Service {match.Service.Name} removed from services");
                    }

                    foreach (var tran in trans)
                    {
                        if (!Transponders.Contains(tran))
                            continue;

                        Transponders.Remove(tran);
                        Log.Debug($"Transponder {tran.TransponderId} removed from transponders");
                    }
                    Log.Debug("Transponders removed");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveTransponders_There_was_an_error_while_removing_transponders, ex);
            }
        }

        /// <summary>
        ///     Removes all transponders from list of transponders
        ///     and all services on specified transponders from all bouquets and from list of services
        /// </summary>
        public void RemoveAllTransponders()
        {
            RemoveTransponders(Transponders.ToList());
        }

        /// <summary>
        ///     Remove satellite from list of satellites,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that satellite from all bouquets and from list of services
        /// </summary>
        /// <param name="satellite"></param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveSatellite(IXmlSatellite satellite)
        {
            try
            {
                if (satellite == null)
                    throw new ArgumentNullException();

                Log.Debug($"Removing satellite {satellite.Name}");
                IList<ITransponder> trans = Transponders.OfType<ITransponderDVBS>().Where(x => Equals(x.Satellite, satellite)).Select(x => (ITransponder)x).ToList();
                RemoveTransponders(trans);

                if (Satellites.Contains(satellite))
                {
                    Satellites.Remove(satellite);
                    Log.Debug($"Satellite {satellite.Name} removed from satellites");
                }

                Log.Debug($"Satellite {satellite.Name} removed");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveSatellite_There_was_an_error_while_removing_satellite, ex);
            }
        }

        /// <summary>
        ///     Remove satellite from list of satellites,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that satellite from all bouquets and from list of services
        /// </summary>
        /// <param name="orbitalPosition">Orbital position as integer, as seen in satellites.xml file - IE. Astra 19.2 = 192</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveSatellite(int orbitalPosition)
        {
            try
            {
                Log.Debug($"Removing satellite position {orbitalPosition}");
                IList<ITransponder> trans = Transponders.OfType<ITransponderDVBS>().Where(x => x.OrbitalPositionInt == orbitalPosition).Select(x => (ITransponder)x).ToList();
                RemoveTransponders(trans);
                IXmlSatellite satellite = Satellites.FirstOrDefault(x => x.Position == orbitalPosition.ToString(CultureInfo.CurrentCulture));

                if (satellite != null)
                {
                    if (Satellites != null && Satellites.Contains(satellite))
                    {
                        Satellites.Remove(satellite);
                        Log.Debug($"Satellite {satellite.Name} removed from satellites");
                    }
                }

                Log.Debug($"Satellite position {orbitalPosition} removed");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveSatellite_There_was_an_error_while_removing_satellite, ex);
            }
        }

        /// <summary>
        ///     Remove all satellites from list of satellites,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that satellites from all bouquets and from list of services
        /// </summary>
        public void RemoveAllSatellites()
        {
            foreach (var ixsCurrentSatellite in Satellites.ToList())
            {
                RemoveSatellite(ixsCurrentSatellite);
            }
        }

        /// <summary>
        ///     Remove cable from list of cables,
        /// </summary>
        /// <param name="cable"></param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void RemoveCable(IXmlCable cable)
        {
            try
            {
                if (cable == null)
                    throw new ArgumentNullException();

                Log.Debug($"Removing cable {cable.Name}");

                if (Cables != null && Cables.Contains(cable))
                {
                    Cables.Remove(cable);
                    Log.Debug($"Cable {cable.Name} removed from cables");
                }

                Log.Debug($"Cable {cable.Name} removed");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveCable_There_was_an_error_while_removing_cable, ex);
            }
        }

        /// <summary>
        ///     Remove all cables from list of cables,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that cables from all bouquets and from list of services
        /// </summary>
        public void RemoveAllCables()
        {
            foreach (var ixcCurrentCable in Cables.ToList())
            {
                RemoveCable(ixcCurrentCable);
            }
        }

        /// <summary>
        ///     Removes IBouquetItemService, IBouquetItemFileBouquet and IBouquetItemBouquetsBouquet bouquet items without valid
        ///     reference to service or another bouquet from all bouquets
        /// </summary>
        /// <remarks>IBouquetsBouquet, IBouquetItemMarker and IBouquetItemStream items are preserved</remarks>
        public void RemoveInvalidBouquetItems()
        {
            try
            {
                Log.Debug("Removing bouquet items without valid reference to service or another bouquet");

                foreach (var bouquet in Bouquets)
                {
                    IList<IBouquetItemService> biServices = bouquet.BouquetItems.OfType<IBouquetItemService>().Where(x => x.Service == null).ToList();
                    foreach (var bis in biServices)
                    {
                        bouquet.BouquetItems.Remove(bis);
                        Log.Debug($"Removed invalid service reference {bis.ServiceId} from bouquet {bouquet}");
                    }

                    IList<IBouquetItemFileBouquet> biFileBouquets =
                        bouquet.BouquetItems.OfType<IBouquetItemFileBouquet>().Where(x => x.Bouquet == null).ToList();
                    foreach (var bifb in biFileBouquets)
                    {
                        bouquet.BouquetItems.Remove(bifb);
                        Log.Debug($"Removed invalid reference to file bouquet {bifb.FileName} from bouquet {bouquet}");
                    }

                    IList<IBouquetItemBouquetsBouquet> biBouquetsBouquets =
                        bouquet.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().Where(x => x.Bouquet == null).ToList();
                    foreach (var bibb in biBouquetsBouquets)
                    {
                        bouquet.BouquetItems.Remove(bibb);
                        Log.Debug($"Removed invalid reference to bouquets bouquet number {bibb.BouquetOrderNumberInt} from bouquet {bouquet}");
                    }
                }

                Log.Debug("Finished removing invalid bouquet items");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_RemoveInvalidBouquetItems_There_was_an_error_while_removing_invalid_bouquet_items, ex);
            }
        }

        /// <summary>
        ///     Updates namespaces for all satellite transponders with calculated namespace
        /// </summary>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        public void UpdateSatelliteTranspondersNameSpaces()
        {
            try
            {
                Log.Debug("Updating namespaces for satellite transponders");
                IList<ITransponderDVBS> trans = Transponders.OfType<ITransponderDVBS>().Where(x => x.NameSpc != x.CalculatedNameSpace).ToList();
                foreach (var transponder in trans)
                {
                    var oldNameSpc = transponder.NameSpc;
                    transponder.NameSpc = transponder.CalculatedNameSpace;
                    Log.Debug($"Updated namespace for transponder {string.Join(":", transponder.Frequency, transponder.SymbolRate.PadRight(8, '0'), transponder.Polarization, transponder.FEC)} on satellite position {transponder.OrbitalPositionInt} from {oldNameSpc} to {transponder.NameSpc}");
                }
                Log.Debug("Finished updating namespaces for satellite transponders");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    Resources.Settings_UpdateSatelliteTranspondersNameSpaces_There_was_an_error_while_updating_namespaces_for_satellite_transponders_, ex);
            }
        }

        /// <summary>
        ///     Changes satellite position to new position for satellite and belonging transponders
        /// </summary>
        /// <param name="satellite">Instance of IXmlSatellite</param>
        /// <param name="newPosition">New orbital position as integer as seen in satellites.xml file, e.g. Astra 19.2 = 192</param>
        /// <exception cref="SettingsException">Throws SettingsException if another satellite exists on new position</exception>
        /// <remarks>Updates transponder namespace with calculated one, if different</remarks>
        public void ChangeSatellitePosition(IXmlSatellite satellite, int newPosition)
        {
            try
            {
                if (satellite == null)
                    throw new ArgumentNullException();

                if (satellite.Position == newPosition.ToString(CultureInfo.CurrentCulture))
                    return;

                Log.Debug($"Changing position for satellite {satellite.Name} from {satellite.Position} to {newPosition}");

                if (Satellites.Any(x => x.Position == newPosition.ToString(CultureInfo.CurrentCulture)))
                {
                    throw new SettingsException(
                        string.Format(
                            Resources
                                .Settings_ChangeSatellitePosition_Cannot_change_position_for_satellite__1__to__2___0_There_is_already_satellite_on_this_position,
                            Environment.NewLine, satellite.Name, newPosition));
                }

                var trans = FindTranspondersForSatellite(satellite);
                foreach (var transponder in trans)
                {
                    var oldNameSpc = transponder.NameSpc;
                    transponder.OrbitalPositionInt = newPosition;
                    transponder.NameSpc = transponder.CalculatedNameSpace;
                    Log.Debug(transponder.NameSpc != oldNameSpc ? $"Updated namespace & position for transponder {string.Join(":", transponder.Frequency, transponder.SymbolRate.PadRight(8, '0'), transponder.Polarization, transponder.FEC)} from {oldNameSpc} to {transponder.NameSpc}" : $"Updated position for transponder {string.Join(":", transponder.Frequency, transponder.SymbolRate.PadRight(8, '0'), transponder.Polarization, transponder.FEC)}");
                }

                satellite.Position = newPosition.ToString(CultureInfo.CurrentCulture);
                Log.Debug($"Finished changing satellite position for satellite {satellite.Name}");
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MoveSatellite_There_was_an_error_while_moving_satellite_to_a_new_position, ex);
            }
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}