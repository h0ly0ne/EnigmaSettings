// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IXmlSatellite : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     List of transponders from xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IList<IXmlTransponder> Transponders { get; set; }

        /// <summary>
        ///     Satellite name from satellites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Name { get; set; }

        /// <summary>
        ///     Satellite flags from satellites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; set; }

        /// <summary>
        ///     Satellite position
        /// </summary>
        /// <value>Position as integer number with length 3 (ie. 19.2E = 192)</value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Position { get; set; }

        /// <summary>
        ///     Parses position to position string
        /// </summary>
        /// <value></value>
        /// <returns>IE. for position value '192' returns '19.2° E'</returns>
        /// <remarks></remarks>
        string PositionString { get; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}