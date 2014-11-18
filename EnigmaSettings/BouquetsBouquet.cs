// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class BouquetsBouquet : IBouquetsBouquet
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
        private bool _mLocked;
        private string _mName;
        private IList<IBouquetItem> _mBouquetItems;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mLocked = _locked;
            _mName = _name;
            _mBouquetItems = new List<IBouquetItem>(_bouquetItems);
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            Name = _mName;
            Locked = _mLocked;
            BouquetItems.Clear();
            foreach (IBouquetItem bouquetItem in _mBouquetItems)
            {
                BouquetItems.Add(bouquetItem);
            }
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
            var bb = new BouquetsBouquet {Name = Name, Locked = Locked};
            bb.BouquetItems = new List<IBouquetItem>();
            foreach (var bouquetItem in BouquetItems)
            {
                bb.BouquetItems.Add((IBouquetItem)bouquetItem.Clone());
            }
            return bb;
        }

        #endregion

        private IList<IBouquetItem> _bouquetItems = new BindingList<IBouquetItem>();
        private bool _locked;
        private string _name = string.Empty;

        /// <summary>
        ///     List of bouquet items
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IBouquetItem> BouquetItems
        {
            get { return _bouquetItems; }
            set {                 
                if (value == null)
                    value = new List<IBouquetItem>();
                _bouquetItems = value;
                OnPropertyChanged("BouquetItems");
            }
        }

        /// <summary>
        ///     Type of settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Enums.BouquetType BouquetType
        {
            get { return Enums.BouquetType.E1BouquetsBouquet; }
        }

        /// <summary>
        ///     Bouquet name
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">Throws argument exception if name is null or empty</exception>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException();
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Determines if bouquet is locked.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Locked bouquets are stored inside services.locked file</remarks>
        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (value == _locked) return;
                _locked = value;
                OnPropertyChanged("Locked");
            }
        }
    }
}