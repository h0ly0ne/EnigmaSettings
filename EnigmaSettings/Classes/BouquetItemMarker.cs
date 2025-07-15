// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemMarker : BouquetItem, IBouquetItemMarker
    {
        private string _description = string.Empty;

        private string _markerNumber = "0";

        #region "IEditable"
        private bool _isEditing;
        private string _mDescription;
        private string _mMarkerNumber = "0";

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) 
                return;

            _mDescription = _description;
            _mMarkerNumber = _markerNumber;
            _isEditing = true;
        }

        public override void EndEdit()
        {
            base.EndEdit();
            _isEditing = false;
        }

        public override void CancelEdit()
        {
            base.CancelEdit();
            if (!_isEditing) 
                return;

            Description = _mDescription;
            MarkerNumber = _mMarkerNumber;
            _isEditing = false;
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            return MemberwiseClone();
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
            if (description == null)
                throw new ArgumentNullException(nameof(description));

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
            get => _description;
            set
            {
                if (value == _description) return;
                if (string.IsNullOrEmpty(value))
                {
                    value = "-------------------------";
                }
                _description = value.Trim();
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        ///     Sequential number of marker in total markers count through all bouquets
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Default value is 0</remarks>
        [DataMember]
        public string MarkerNumber
        {
            get => _markerNumber;
            set
            {
                if (value == _markerNumber) 
                    return;

                _markerNumber = value ?? "0";
                OnPropertyChanged(nameof(MarkerNumber));
            }
        }

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