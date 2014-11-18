// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.ComponentModel;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public abstract class BouquetItem : IBouquetItem
    {
        #region "INotifyPropertyChanged"

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region "IEditable"

        private bool _isEditing;
        private string _mFavoritesTypeFlag;
        private string _mLineSpecifierFlag;

        public virtual void BeginEdit()
        {
            if (_isEditing) return;
            _mFavoritesTypeFlag = _favoritesTypeFlag;
            _mLineSpecifierFlag = _lineSpecifierFlag;
            _isEditing = true;
        }

        public virtual void EndEdit()
        {
            _isEditing = false;
        }

        public virtual void CancelEdit()
        {
            if (!_isEditing) return;
            FavoritesTypeFlag = _mFavoritesTypeFlag;
            LineSpecifierFlag = _mLineSpecifierFlag;
            _isEditing = false;
        }

        #endregion

        #region "ICloneable"

        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return ShallowCopy();
        }

        #endregion

        // ReSharper disable InconsistentNaming
        protected string _favoritesTypeFlag = "1";
        protected string _lineSpecifierFlag = "0";
        // ReSharper restore InconsistentNaming

        /// <summary>
        ///     Type of bouquet item. Each subclass defines it's own type
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract Enums.BouquetItemType BouquetItemType { get; }

        /// <summary>
        ///     First number in SERVICE line inside one of the bouquets files
        /// </summary>
        public Enums.FavoritesType FavoritesType
        {
            get
            {
                switch (FavoritesTypeFlag)
                {
                    case null:
                        return Enums.FavoritesType.Unknown;
                    case "-1":
                        return Enums.FavoritesType.InvalidId;
                    case "0":
                        return Enums.FavoritesType.StructureId;
                    case "1":
                        return Enums.FavoritesType.DVBService;
                    case "2":
                        return Enums.FavoritesType.File;
                    default:
                        return Enums.FavoritesType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Second number in SERVICE line inside one of the bouquets files
        /// </summary>
        public Enums.LineSpecifier LineSpecifierType
        {
            get
            {
                switch (LineSpecifierFlag)
                {
                    case null:
                        return Enums.LineSpecifier.Unknown;
                    case "0":
                        return Enums.LineSpecifier.NormalService;
                    case "1":
                        return Enums.LineSpecifier.IsDirectory;
                    case "2":
                        return Enums.LineSpecifier.MustChangeDirectory;
                    case "3":
                        return Enums.LineSpecifier.IsDirectoryMustChangeDirectory;
                    case "4":
                        return Enums.LineSpecifier.MayChangeDirectory;
                    case "5":
                        return Enums.LineSpecifier.IsDirectoryMayChangeDirectory;
                    case "6":
                        return Enums.LineSpecifier.MustChangeDirectoryMayChangeDirectory;
                    case "7":
                        return Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectory;
                    case "8":
                        return Enums.LineSpecifier.AutomaticallySorted;
                    case "9":
                        return Enums.LineSpecifier.IsDirectoryAutomaticallySorted;
                    case "10":
                        return Enums.LineSpecifier.MustChangeDirectoryAutomaticallySorted;
                    case "11":
                        return Enums.LineSpecifier.IsDirectoryMustChangeDirectoryAutomaticallySorted;
                    case "12":
                        return Enums.LineSpecifier.MayChangeDirectoryAutomaticallySorted;
                    case "13":
                        return Enums.LineSpecifier.IsDirectoryMayChangeDirectoryAutomaticallySorted;
                    case "14":
                        return Enums.LineSpecifier.MustChangeDirectoryMayChangeDirectoryAutomaticallySorted;
                    case "15":
                        return Enums.LineSpecifier.IsDirectoryMustChangeDirectoryMayChangeDirectoryAutomaticallySorted;
                    case "16":
                        return Enums.LineSpecifier.InternalSortKey;
                    case "32":
                        return Enums.LineSpecifier.SortKeyIs1;
                    case "64":
                        return Enums.LineSpecifier.Marker;
                    case "128":
                        return Enums.LineSpecifier.ServiceNotPlayable;
                    default:
                        return Enums.LineSpecifier.Unknown;
                }
            }
        }

        /// <summary>
        ///     First number in SERVICE line inside one of the bouquets files
        /// </summary>
        public string FavoritesTypeFlag
        {
            get { return _favoritesTypeFlag; }
            set
            {
                if (value == null)
                    return;
                if (value == _favoritesTypeFlag) return;
                _favoritesTypeFlag = value;
                OnPropertyChanged("FavoritesTypeFlag");
                OnPropertyChanged("FavoritesType");
            }
        }

        /// <summary>
        ///     Second number in SERVICE line inside one of the bouquets files
        /// </summary>
        public string LineSpecifierFlag
        {
            get { return _lineSpecifierFlag; }
            set
            {
                if (value == null)
                    return;
                if (value == _lineSpecifierFlag) return;
                _lineSpecifierFlag = value;
                OnPropertyChanged("LineSpecifierFlag");
                OnPropertyChanged("LineSpecifierType");
            }
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

    }
}