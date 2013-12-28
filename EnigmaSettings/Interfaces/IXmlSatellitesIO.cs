// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System.Collections.Generic;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    /// <summary>
    ///     Used to load or save satellites from/to satellites.xml file
    /// </summary>
    /// <remarks></remarks>
    public interface IXmlSatellitesIO
    {
        /// <summary>
        ///     Implementation of instance factory used to instatiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IInstanceFactory Factory { get; }

        /// <summary>
        ///     Loads satellites and transponders from satellites.xml file
        /// </summary>
        /// <param name="fileName">Full path to satellites.xml file on disk</param>
        /// <returns>List of IXMLSatellite objects and corresponding transponders for each satellite</returns>
        /// <remarks></remarks>
        IList<IXmlSatellite> LoadSatellitesFromFile(string fileName);

        /// <summary>
        ///     Saves satellites and transponders to satellites.xml file
        /// </summary>
        /// <param name="fileName">Full path to satellites.xml file on disk</param>
        /// <param name="settings">
        ///     Settings instance with the list of IXMLSatellite objects and corresponding transponders for each
        ///     satellite
        /// </param>
        /// <remarks></remarks>
        void SaveSatellitesToFile(string fileName, ISettings settings);
    }
}