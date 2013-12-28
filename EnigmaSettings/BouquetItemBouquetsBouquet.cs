// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Globalization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
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

        /// <summary>
        ///     Initializes new instance from Bouquet instance and sets corresponding properties
        /// </summary>
        /// <param name="bouquet"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if bouquet is null</exception>
        public BouquetItemBouquetsBouquet(IBouquetsBouquet bouquet)
        {
            if (bouquet == null)
                throw new ArgumentNullException();
            Bouquet = bouquet;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.InvariantCulture);
            _lineSpecifierFlag =
                Convert.ToInt16(Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted)
                    .ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Initializes new instance and sets BouquetOrderNumber property
        /// </summary>
        /// <param name="bouquetOrderNumber"></param>
        /// <remarks></remarks>
        public BouquetItemBouquetsBouquet(int bouquetOrderNumber)
        {
            _bouquetOrderNumberInt = bouquetOrderNumber;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.InvariantCulture);
            _lineSpecifierFlag =
                Convert.ToInt16(Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted)
                    .ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetType.E1BouquetsBouquet</returns>
        /// <remarks></remarks>
        public override Enums.BouquetItemType BouquetItemType
        {
            get { return Enums.BouquetItemType.BouquetsBouquet; }
        }

        /// <summary>
        ///     Reference to matcing bouquet instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IBouquetsBouquet Bouquet
        {
            get { return _bouquet; }
            set
            {
                if (value == _bouquet) return;
                _bouquet = value;
                OnPropertyChanged("Bouquet");
            }
        }

        /// <summary>
        ///     Negative number in hex format as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        public string BouquetOrderNumber
        {
            get { return BouquetOrderNumberInt.ToString("X").ToLower(); }
        }

        /// <summary>
        ///     Negative number as integer as found in bouquets file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match locked bouquets</remarks>
        public int BouquetOrderNumberInt
        {
            get { return _bouquetOrderNumberInt; }
            set
            {
                if (value != _bouquetOrderNumberInt)
                {
                    _bouquetOrderNumberInt = value;
                    OnPropertyChanged("BouquetOrderNumberInt");
                    OnPropertyChanged("BouquetOrderNumber");
                }
            }
        }
    }
}