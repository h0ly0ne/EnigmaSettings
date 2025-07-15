// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IXmlCable : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     List of transponders from xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IList<IXmlTransponder> Transponders { get; set; }

        /// <summary>
        ///     Cable name from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Name { get; set; }

        /// <summary>
        ///     Cable flags from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; set; }

        /// <summary>
        ///     Cables satfeed from cables.xml file
        /// </summary>
        /// <value>true/false</value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SatFeed { get; set; }

        /// <summary>
        ///     Cables countrycode from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string CountryCode { get; set; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}