// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemAlternative : BouquetItem, IBouquetItemAlternative
    {
        private readonly string _fileName = string.Empty;
        private IFileBouquet _bouquet;

        #region "IEditable"

        private bool _isEditing;
        private IFileBouquet _mBouquet;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mBouquet = _bouquet;
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
            _isEditing = false;
        }

        #endregion

        #region "ICloneable"

        public new object Clone()
        {
            var clone = (IBouquetItemAlternative)MemberwiseClone();
            clone.Bouquet = Bouquet != null ? (IFileBouquet)Bouquet.Clone() : null;
            return clone;
        }

        #endregion

        /// <summary>Initializes a new instance and sets the alternatives filename.</summary>
        /// <exception cref="ArgumentNullException">Thrown if fileName is null or empty.</exception>
        public BouquetItemAlternative(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException();
            _fileName = fileName;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.CurrentCulture);
            _lineSpecifierFlag = Convert.ToInt16(Enums.LineSpecifier.Alternative).ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>Initializes a new instance and sets the alternatives bouquet.</summary>
        /// <exception cref="ArgumentNullException">Thrown if bouquet is null.</exception>
        public BouquetItemAlternative(IFileBouquet bouquet)
        {
            if (bouquet == null)
                throw new ArgumentNullException();
            Bouquet = bouquet;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.CurrentCulture);
            _lineSpecifierFlag = Convert.ToInt16(Enums.LineSpecifier.Alternative).ToString(CultureInfo.CurrentCulture);
        }

        [DataMember]
        public override Enums.BouquetItemType BouquetItemType
        {
            get { return Enums.BouquetItemType.Alternative; }
        }

        [DataMember]
        public IFileBouquet Bouquet
        {
            get { return _bouquet; }
            set
            {
                if (value == _bouquet) return;
                _bouquet = value;
                OnPropertyChanged("Bouquet");
            }
        }

        [DataMember]
        public string FileName
        {
            get
            {
                if (Bouquet != null)
                    return Bouquet.FileName;
                return _fileName;
            }
        }
    }
}
