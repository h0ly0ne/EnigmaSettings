// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemFileBouquet : BouquetItem, IBouquetItemFileBouquet
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

        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var bifb = (IBouquetItemFileBouquet)MemberwiseClone();
            bifb.Bouquet = Bouquet != null ? (IFileBouquet)Bouquet.Clone() : null;
            return bifb;
        }

        #endregion

        /// <summary>
        ///     Initializes new instance and sets filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if fileName is empty or null</exception>
        public BouquetItemFileBouquet(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException();
            _fileName = fileName;
            _favoritesTypeFlag = Convert.ToInt16(Enums.FavoritesType.DVBService).ToString(CultureInfo.CurrentCulture);
            _lineSpecifierFlag =
                Convert.ToInt16(Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted)
                    .ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Initializes new instance and sets matching bouquet
        /// </summary>
        /// <param name="bouquet"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if bouquet is null</exception>
        public BouquetItemFileBouquet(IFileBouquet bouquet)
        {
            if (bouquet == null)
                throw new ArgumentNullException();
            Bouquet = bouquet;
        }

        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetItemType.FileBouquet</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType
        {
            get { return Enums.BouquetItemType.FileBouquet; }
        }

        /// <summary>
        ///     Reference to matching bouquet instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        ///     Filename of bouquet as found in bouquet line
        /// </summary>
        /// <value></value>
        /// <returns>Local value if matching bouquet is empty, otherwise returns Bouquet.FileName</returns>
        /// <remarks></remarks>
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