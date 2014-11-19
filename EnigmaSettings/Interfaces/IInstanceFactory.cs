// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    /// <summary>
    ///     Initializes objects for specified interfaces.
    ///     Used as primitive DI instead of full blown DI for speed
    /// </summary>
    /// <remarks></remarks>
    public interface IInstanceFactory
    {
        IXmlSatellite InitNewxmlSatellite();

        IXmlTransponder InitNewxmlTransponder();

        IXmlTransponder InitNewxmlTransponder(ITransponderDVBS transponder);

        IBouquetItemFileBouquet InitNewBouquetItemFileBouquet(string fSileName);

        IBouquetItemFileBouquet InitNewBouquetItemFileBouquet(IFileBouquet bouquet);

        IBouquetItemMarker InitNewBouquetItemMarker(string description, string markerNumber);

        IBouquetItemService InitNewBouquetItemService(string bouquetLine);

        IBouquetItemService InitNewBouquetItemService(IService service);

        IBouquetItemStream InitNewBouquetItemStream(string description, string url, string streamFlag);

        IBouquetItemStream InitNewBouquetItemStream(string bouquetLine, string description);

        IFileBouquet InitNewFileBouquet();

        IBouquetsBouquet InitNewBouquetsBouquet();

        ITransponderDVBC InitNewTransponderDVBC(string transponderData, string transponderFrequency);

        ITransponderDVBS InitNewTransponderDVBS(string transponderData, string transponderFrequency);

        ITransponderDVBT InitNewTransponderDVBT(string transponderData, string transponderFrequency);

        IService InitNewService(string serviceData, string name, string flags);

        ISettings InitNewSettings(IPathProvider pathProvider);

        IXmlSatellitesIO InitNewXmlSatelliteIO(IFileProvider fileProvider);

        IBouquetItemBouquetsBouquet InitNewBouquetItemBouquetsBouquet(int bouquetOrderNumber);

        IBouquetItemBouquetsBouquet InitNewBouquetItemBouquetsBouquet(IBouquetsBouquet bouquet);
    }
}