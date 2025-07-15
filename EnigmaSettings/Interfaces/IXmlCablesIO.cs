// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System.Collections.Generic;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    /// <summary>
    ///     Used to load or save cables from/to cables.xml file
    /// </summary>
    /// <remarks></remarks>
    public interface IXmlCablesIO
    {
        /// <summary>
        ///     Implementation of instance factory used to instantiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IInstanceFactory Factory { get; }

        /// <summary>
        ///     Loads cables and transponders from satellites.xml file
        /// </summary>
        /// <param name="fileName">Full path to cables.xml file on disk</param>
        /// <returns>List of IXmlCable objects and corresponding transponders for each cable</returns>
        /// <remarks></remarks>
        IList<IXmlCable> LoadCablesFromFile(string fileName);

        /// <summary>
        ///     Saves cables and transponders to cables.xml file
        /// </summary>
        /// <param name="fileName">Full path to cables.xml file on disk</param>
        /// <param name="settings">
        ///     Settings instance with the list of IXmlCable objects and corresponding transponders for each
        ///     cable
        /// </param>
        /// <remarks></remarks>
        void SaveCablesToFile(string fileName, ISettings settings);
    }
}