// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemBouquetsBouquet : BouquetItem, IBouquetItemBouquetsBouquet
    {
        private IBouquetsBouquet _bouquet;
        private int _bouquetOrderNumberInt;

        #region "IEditable"

        private bool _isEditing;
        private IBouquetsBouquet _mBouquet;
        private int _mBouquetOrderNumberInt;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mBouquet = _bouquet;
            _mBouquetOrderNumberInt = _bouquetOrderNumberInt;
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
            if (!_isEditing) return;
            Bouquet = _mBouquet;
            BouquetOrderNumberInt = _mBouquetOrderNumberInt;
            _isEditing = false;
        }

        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var bibb = (IBouquetItemBouquetsBouquet)MemberwiseClone();
            bibb.Bouquet = (IBouquetsBouquet)Bouquet?.Clone();
            return bibb;
        }
        #endregion

        /// <summary>
        ///     Initializes new instance from Bouquet instance and sets corresponding properties
        /// </summary>
        /// <param name="bouquet"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if bouquet is null</exception>
        public BouquetItemBouquetsBouquet(IBouquetsBouquet bouquet)
        {
            Bouquet = bouquet ?? throw new ArgumentNullException();
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.CurrentCulture);
            _lineSpecifierFlag = Convert.ToInt16(Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted).ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Initializes new instance and sets BouquetOrderNumber property
        /// </summary>
        /// <param name="bouquetOrderNumber"></param>
        /// <remarks></remarks>
        public BouquetItemBouquetsBouquet(int bouquetOrderNumber)
        {
            _bouquetOrderNumberInt = bouquetOrderNumber;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.CurrentCulture);
            _lineSpecifierFlag = Convert.ToInt16(Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted).ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetType.E1BouquetsBouquet</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType => Enums.BouquetItemType.BouquetsBouquet;

        /// <summary>
        ///     Reference to matching bouquet instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IBouquetsBouquet Bouquet
        {
            get => _bouquet;
            set
            {
                if (value == _bouquet) 
                    return;

                _bouquet = value;
                OnPropertyChanged(nameof(Bouquet));
            }
        }

        /// <summary>
        ///     Negative number in hex format as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        [DataMember]
        public string BouquetOrderNumber => BouquetOrderNumberInt.ToString("X").ToLower();

        /// <summary>
        ///     Negative number as integer as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        [DataMember]
        public int BouquetOrderNumberInt
        {
            get => _bouquetOrderNumberInt;
            set
            {
                if (value != _bouquetOrderNumberInt)
                {
                    _bouquetOrderNumberInt = value;
                    OnPropertyChanged(nameof(BouquetOrderNumberInt));
                    OnPropertyChanged(nameof(BouquetOrderNumber));
                }
            }
        }
    }
}