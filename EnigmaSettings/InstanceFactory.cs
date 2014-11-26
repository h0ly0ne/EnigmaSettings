// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public class InstanceFactory : IInstanceFactory
    {
        public IFileBouquet InitNewFileBouquet()
        {
            return new FileBouquet();
        }

        public IBouquetItemMarker InitNewBouquetItemMarker(string description, string markerNumber)
        {
            return new BouquetItemMarker(description, markerNumber);
        }

        public IBouquetItemService InitNewBouquetItemService(string bouquetLine)
        {
            return new BouquetItemService(bouquetLine);
        }

        public IBouquetItemStream InitNewBouquetItemStream(string description, string url, string streamFlag)
        {
            return new BouquetItemStream(description, url, streamFlag);
        }

        public IService InitNewService(string serviceData, string name, string flags)
        {
            return new Service<Flag>(serviceData, name, flags);
        }

        public ITransponderDVBC InitNewTransponderDVBC(string transponderData, string transponderFrequency)
        {
            return new TransponderDVBC(transponderData, transponderFrequency);
        }

        public ITransponderDVBS InitNewTransponderDVBS(string transponderData, string transponderFrequency)
        {
            return new TransponderDVBS(transponderData, transponderFrequency);
        }

        public ITransponderDVBT InitNewTransponderDVBT(string transponderData, string transponderFrequency)
        {
            return new TransponderDVBT(transponderData, transponderFrequency);
        }

        public IXmlSatellite InitNewxmlSatellite()
        {
            return new XmlSatellite();
        }

        public IXmlTransponder InitNewxmlTransponder()
        {
            return new XmlTransponder();
        }

        public IXmlTransponder InitNewxmlTransponder(ITransponderDVBS transponder)
        {
            return new XmlTransponder
            {
                FECInner = transponder.FEC,
                Frequency = transponder.Frequency,
                Inversion = transponder.Inversion,
                Modulation = transponder.Modulation,
                Pilot = transponder.Pilot,
                Polarization = transponder.Polarization,
                RollOff = transponder.RollOff,
                SymbolRate = transponder.SymbolRate,
                System = transponder.System
            };
        }

        public ISettings InitNewSettings()
        {
            return new Settings();
        }

        public IXmlSatellitesIO InitNewXmlSatelliteIO(IFileHelper fileProvider)
        {
            return new XmlSatellitesIO(this, fileProvider);
        }

        public IBouquetsBouquet InitNewBouquetsBouquet()
        {
            return new BouquetsBouquet();
        }

        public IBouquetItemBouquetsBouquet InitNewBouquetItemBouquetsBouquet(int bouquetOrderNumber)
        {
            return new BouquetItemBouquetsBouquet(bouquetOrderNumber);
        }

        public IBouquetItemBouquetsBouquet InitNewBouquetItemBouquetsBouquet(IBouquetsBouquet bouquet)
        {
            return new BouquetItemBouquetsBouquet(bouquet);
        }

        public IBouquetItemFileBouquet InitNewBouquetItemFileBouquet(string fileName)
        {
            return new BouquetItemFileBouquet(fileName);
        }

        public IBouquetItemFileBouquet InitNewBouquetItemFileBouquet(IFileBouquet bouquet)
        {
            return new BouquetItemFileBouquet(bouquet);
        }

        public IBouquetItemService InitNewBouquetItemService(IService service)
        {
            return new BouquetItemService(service);
        }

        public IBouquetItemStream InitNewBouquetItemStream(string bouquetLine, string description)
        {
            return new BouquetItemStream(bouquetLine, description);
        }
    }
}