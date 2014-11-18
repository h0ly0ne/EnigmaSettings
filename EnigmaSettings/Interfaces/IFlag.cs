// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IFlag : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     Type of the flag from 3rd line of service data in lamedb
        /// </summary>
        /// <value></value>
        /// <returns>C, F, P or Unknown</returns>
        /// <remarks>Defaults to 'Unknown'</remarks>
        Enums.FlagType FlagType { get; }

        /// <summary>
        ///     Type of F flag
        /// </summary>
        /// <value></value>
        /// <returns>F Flag type from enum, if it's not supported returns 'Unknown'</returns>
        /// <remarks>If current flag is not F, type will be 'None'</remarks>
        Enums.FFlagType FFlagType { get; }

        /// <summary>
        ///     Type of C flag
        /// </summary>
        /// <value></value>
        /// <returns>C Flag type from enum, if it's not supported returns 'Unknown'</returns>
        /// <remarks>If current flag is not C, type will be 'None'</remarks>
        Enums.CFlagType CFlagType { get; }

        /// <summary>
        ///     Flag in format X:Y where X is flag type and Y is hex value of the flag
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string FlagString { get; set; }

        /// <summary>
        ///     Value of the current flag without flag type
        /// </summary>
        /// <value></value>
        /// <returns>Hex value if flag is numeric</returns>
        /// <remarks></remarks>
        string FlagValue { get; set; }

        /// <summary>
        ///     Flag value as integer if flag is hexadecimal
        /// </summary>
        /// <value></value>
        /// <returns>0 if flag is not numeric</returns>
        /// <remarks></remarks>
        int FlagInt { get; set; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}