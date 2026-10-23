// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemMarker : BouquetItem, IBouquetItemMarker
    {
        #region "ICloneable"
        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return CloneWithoutSubscribers();
        }
        #endregion

        /// <summary>
        ///     Initializes new marker
        /// </summary>
        /// <param name="description">Without leading #DESCRIPTION tag</param>
        /// <param name="markerNumber">Marker sequential number trough entire settings</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if description is null</exception>
        public BouquetItemMarker(string description, string markerNumber)
        {
            ArgumentNullException.ThrowIfNull(description);

            markerNumber ??= "0";

            Description = description.Trim();
            MarkerNumber = markerNumber;
        }

        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetItemType.Marker</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType => Enums.BouquetItemType.Marker;

        /// <summary>
        ///     Marker text as seen in settings
        /// </summary>
        /// <value></value>
        /// <returns>If empty returns "-----------------------------"</returns>
        /// <remarks></remarks>
        [DataMember]
        public string Description
        {
            get;
            set
            {
                if (value == field)
                    return;

                if (string.IsNullOrEmpty(value))
                    value = "-------------------------";

                field = value.Trim();
                OnPropertyChanged(nameof(Description));
            }
        } = string.Empty;

        /// <summary>
        ///     Sequential number of marker in total markers count through all bouquets
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Default value is 0</remarks>
        [DataMember]
        public string MarkerNumber
        {
            get;
            set
            {
                if (value == field)
                    return;

                field = value ?? "0";
                OnPropertyChanged(nameof(MarkerNumber));
            }
        } = "0";

        /// <summary>
        ///     Return line in format ready to be written to bouquet
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Join("\t", string.Join(":", FavoritesTypeFlag, LineSpecifierFlag, MarkerNumber, "0", "0", "0", "0", "0", "0", "0", ""), string.Join(" ", "#DESCRIPTION", Description));
        }
    }
}