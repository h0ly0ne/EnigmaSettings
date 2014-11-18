// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponder : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     Namespace of the satellite
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match transponder to satellite</remarks>
        string NameSpc { get; set; }

        /// <summary>
        ///     Transponder ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Should be unique for satellite, but often not</remarks>
        string TSID { get; set; }

        /// <summary>
        ///     Original network ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string NID { get; set; }

        /// <summary>
        ///     Frequency in Hertz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Frequency { get; set; }

        /// <summary>
        ///     Transponder type (Satellite, Cable, Terrestrial)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Enums.TransponderType TransponderType { get; }

        /// <summary>
        ///     0xNamespace:0xTSID:0xNID
        /// </summary>
        /// <value></value>
        /// <returns>Returns 1st line of transponder info for services file</returns>
        /// <remarks>Used to match transponder to service</remarks>
        string TransponderId { get; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();

    }
}