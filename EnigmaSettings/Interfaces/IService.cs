// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IService : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     Service ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SID { get; set; }

        /// <summary>
        ///     Defines type of service (TV,Radio,Data, etc..)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        string Type { get; set; }

        /// <summary>
        ///     Program number
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string ProgNumber { get; set; }

        /// <summary>
        ///     1 -TV, 2-radio, 3-data
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        Enums.ServiceType ServiceType { get; }

        /// <summary>
        ///     Service name (2nd line in settings file)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Name { get; set; }

        /// <summary>
        ///     Flags for Audio, Video, Subtitles, Provider etc..
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; }

        /// <summary>
        ///     Service information in format Type:0xSID:0xTSID:0xNID,0xNAMESPACE
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match service from bouquets</remarks>
        string ServiceId { get; }

        /// <summary>
        ///     0xNAMESPACE:0xTSID:0xNID
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     If Transponder is set returns its TransponderID, otherwise returns value from service data it has been
        ///     initialized with
        /// </returns>
        /// <remarks></remarks>
        string TransponderId { get; }

        /// <summary>
        ///     Transponder instance service belongs to
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        ITransponder Transponder { get; set; }

        /// <summary>
        ///     Determines if service is in Whitelist or Blacklist file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Enums.ServiceSecurity ServiceSecurity { get; set; }

        /// <summary>
        ///     List of flag objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        ReadOnlyCollection<IFlag> FlagList { get; }

        /// <summary>
        ///     Determines if service is locked for updates
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        bool Locked { get; set; }

        /// <summary>
        ///     Updates Flags property from items in list
        /// </summary>
        /// <param name="flagsList"></param>
        /// <remarks>Should be called when Flags are updated in list</remarks>
        void UpdateFlags(IList<IFlag> flagsList);

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}