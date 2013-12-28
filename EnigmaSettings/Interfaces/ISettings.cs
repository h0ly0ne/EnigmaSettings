// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Collections.Generic;
using System.ComponentModel;
using log4net;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ISettings : INotifyPropertyChanged, IEditableObject
    {
        /// <summary>
        ///     Log4Net instance used for logging
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        ILog Log { get; set; }

        /// <summary>
        ///     Full path to settings file location on disk
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SettingsFileName { get; set; }

        /// <summary>
        ///     Directory with all the files for the settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SettingsDirectory { get; }

        /// <summary>
        ///     Version of the settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Enums.SettingsVersion SettingsVersion { get; set; }

        /// <summary>
        ///     List of satellites from satelites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Each satelite has coresponding xml transponders from satellites.xml file</remarks>
        IList<IXmlSatellite> Satellites { get; }

        /// <summary>
        ///     All transponders (DVBS, DVBT, DVBC) from settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>These are not transponders from satellites.xml file</remarks>
        IList<ITransponder> Transponders { get; }

        /// <summary>
        ///     All services from settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Each service should have corresponding transponder from services file</remarks>
        IList<IService> Services { get; }

        /// <summary>
        ///     All bouquets except the ones stored in 'bouquets' file from Enigma 1
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IList<IBouquet> Bouquets { get; }

        /// <summary>
        ///     Returns list of satellite transponders from list with all transponders
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<ITransponderDVBS> FindSatelliteTransponders();

        /// <summary>
        ///     Returns list of transponders for selected satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<ITransponderDVBS> FindTranspondersForSatellite(IXmlSatellite satellite);

        /// <summary>
        ///     Returns list of transponders for selected satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<IService> FindServicesForSatellite(IXmlSatellite satellite);

        /// <summary>
        ///     Returns list of satellite transponders without corresponding satellite
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<ITransponderDVBS> FindTranspondersWithoutSatellite();

        /// <summary>
        ///     Returns list of services without corresponding transponder
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<IService> FindServicesWithoutTransponder();

        /// <summary>
        ///     Returns list of services for transponder
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<IService> FindServicesForTransponder(ITransponder transponder);

        /// <summary>
        ///     Returns list of locked services
        /// </summary>
        /// <returns>Will return new list each time as a result of LINQ query</returns>
        /// <remarks></remarks>
        IList<IService> FindLockedServices();

        /// <summary>
        ///     Returns list of bouquets that have duplicate names
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        IList<IFileBouquet> FindBouquetsWithDuplicateFilename();

        /// <summary>
        ///     Removes services without transponder from the list of services
        /// </summary>
        /// <remarks></remarks>
        void RemoveServicesWithoutTransponder();

        /// <summary>
        ///     Adds new xmlSatellites and xmlTransponders for all transponders in settings file
        ///     that don't have corresponding xmlSatellite.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="frequencyToleration">Toleration in Hz when searching for existing transponder</param>
        /// <remarks>Only transponders for new xmlSatellites will be added if not found.</remarks>
        void AddMissingXmlSatellites(IInstanceFactory factory, int frequencyToleration = 30000);

        /// <summary>
        ///     Adds new xmlTransponders for all transponders in settings file
        ///     that don't have corresponding xmlTransponder in xmlsatellites.xml
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="frequencyToleration">Toleration in Hz when searching for existing transponder</param>
        /// <remarks>Transponders are added to all satellites without matching transponder</remarks>
        void AddMissingXmlTransponders(IInstanceFactory factory, int frequencyToleration = 30000);

        /// <summary>
        ///     Matches services without transponder with corresponding transponders
        /// </summary>
        /// <remarks></remarks>
        void MatchServicesWithTransponders();

        /// <summary>
        ///     Matches transponders from settings file without corresponding satellite to satellites from xml file
        /// </summary>
        /// <remarks></remarks>
        void MatchSatellitesWithTransponders();

        /// <summary>
        ///     Matches services from file bouquets to services from settings
        /// </summary>
        /// <remarks></remarks>
        void MatchFileBouquetsServices();

        /// <summary>
        ///     Matches services from bouquets inside 'bouquets' to services from settings
        /// </summary>
        /// <remarks></remarks>
        void MatchBouquetsBouquetsServices();

        /// <summary>
        ///     Renumbers markers in all bouquets
        /// </summary>
        /// <remarks></remarks>
        void RenumberMarkers();

        /// <summary>
        ///     Renumbers file names for all file bouquets that have filename starting with 'userbouquet.dbe'
        /// </summary>
        /// <remarks>Preserves path in filename if any is set</remarks>
        void RenumberBouquetFileNames();

        /// <summary>
        ///     Removes markers that are followed by another marker, or not followed with any items
        /// </summary>
        /// <remarks></remarks>
        void RemoveEmptyMarkers();

        /// <summary>
        /// Removes markers that are followed by another marker, or not followed with any items
        /// </summary>
        /// <param name="preserveCondition">Markers that match specified condition will always be preserved</param>
        /// <exception cref="SettingsException"></exception>
        void RemoveEmptyMarkers(Func<IBouquetItemMarker, bool> preserveCondition);

        /// <summary>
        ///     Removes bouquets without any items
        /// </summary>
        /// <remarks></remarks>
        void RemoveEmptyBouquets();

        /// <summary>
        ///     Removes all stream references from all bouquets
        /// </summary>
        /// <remarks></remarks>
        void RemoveStreams();

        /// <summary>
        ///     Removes service from list of services and all bouquets
        /// </summary>
        /// <remarks></remarks>
        void RemoveService(IService service);

        /// <summary>
        ///     Removes services from list of services and all bouquets
        /// </summary>
        /// <param name="srv">List of services to be removed</param>
        /// <remarks></remarks>
        void RemoveServices(IList<IService> srv);

        /// <summary>
        ///     Removes transponder from list of transponders
        ///     and all services on that transponder from all bouquets and from list of services
        /// </summary>
        /// <param name="transponder"></param>
        /// <remarks></remarks>
        void RemoveTransponder(ITransponder transponder);

        /// <summary>
        ///     Removes transponders from list of transponders
        ///     and all services on specified transponders from all bouquets and from list of services
        /// </summary>
        /// <param name="trans"></param>
        /// <remarks></remarks>
        void RemoveTransponders(IList<ITransponder> trans);

        /// <summary>
        ///     Remove satellite from list of satellites,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that satellite from all bouquets and from list of services
        /// </summary>
        /// <param name="satellite"></param>
        /// <remarks></remarks>
        void RemoveSatellite(IXmlSatellite satellite);

        /// <summary>
        ///     Remove satellite from list of satellites,
        ///     removes all corresponding transponders from list of transponders
        ///     removes services on that satellite from all bouquets and from list of services
        /// </summary>
        /// <param name="orbitalPosition">Orbital position as integer, as seen in satellites.xml file - IE. Astra 19.2 = 192</param>
        /// <remarks></remarks>
        void RemoveSatellite(int orbitalPosition);

        /// <summary>
        ///     Removes IBouquetItemService, IBouquetItemFileBouquet and IBouquetItemBouquetsBouquet bouquet items without valid
        ///     reference to service or another bouquet from all bouquets
        /// </summary>
        /// <remarks>IBouquetsBouquet, IBouquetItemMarker and IBouquetItemStream items are preserved</remarks>
        void RemoveInvalidBouquetItems();

        /// <summary>
        ///     Updates namespaces for all satellite transponders with calculated namespace
        /// </summary>
        /// <remarks></remarks>
        void UpdateSatelliteTranspondersNameSpaces();

        /// <summary>
        ///     Changes satellite position to new position for satellite and belonging transponders
        /// </summary>
        /// <param name="satellite">Instance of IXmlSatellite</param>
        /// <param name="newPosition">New orbital position as integer as seen in satellites.xml file, IE. Astra 19.2 = 192</param>
        /// <remarks></remarks>
        void ChangeSatellitePosition(IXmlSatellite satellite, int newPosition);
    }
}