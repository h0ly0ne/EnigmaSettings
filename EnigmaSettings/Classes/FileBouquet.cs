// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class FileBouquet : IFileBouquet
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

        private IList<IBouquetItem> _bouquetItems = new List<IBouquetItem>();
        private string _fileName = string.Empty;
        private string _name = string.Empty;

        #region "IEditable"

        private bool _isEditing;
        private string _mName;
        private string _mFileName;
        private IList<IBouquetItem> _mBouquetItems;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mName = _name;
            _mFileName = _fileName;
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
            FileName = _mFileName;
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
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var fb = (IFileBouquet)MemberwiseClone();
            fb.BouquetItems = new List<IBouquetItem>();
            foreach (var bouquetItem in BouquetItems)
            {
                if (bouquetItem != null)
                    fb.BouquetItems.Add((IBouquetItem)bouquetItem.Clone());
            }
            return fb;
        }

        #endregion

        /// <summary>
        ///     Filename of the bouquet without leading directory
        /// </summary>
        /// <value></value>
        /// <returns>Name of the file to be read or to be saved to</returns>
        /// <remarks></remarks>
        [DataMember]
        public string FileName
        {
            get => _fileName;
            set
            {
                if (value == _fileName)
                    return;

                value ??= string.Empty;
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        /// <summary>
        ///     Type of the bouquet. Determined from FileName.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Enums.BouquetType BouquetType => GetTypeFromFileName(_fileName);

        /// <summary>
        ///     List of bouquet items
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IList<IBouquetItem> BouquetItems
        {
            get => _bouquetItems;
            set
            {
                value ??= new List<IBouquetItem>();
                _bouquetItems = value;
                OnPropertyChanged(nameof(BouquetItems));
            }
        }

        /// <summary>
        ///     Name of the bouquet as displayed in the list.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Makes sense only with non system bouquets</remarks>
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) 
                    return;

                value ??= string.Empty;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        ///     Determines type of bouquet by its filename
        /// </summary>
        /// <param name="fName">Filename without path</param>
        /// <returns>BouquetType enum for known filenames</returns>
        /// <remarks></remarks>
        protected Enums.BouquetType GetTypeFromFileName(string fName)
        {
            if (string.IsNullOrEmpty(fName))
                return Enums.BouquetType.Unknown;

            if (fName.ToLower() == "bouquets")
                return Enums.BouquetType.E1Bouquets;
            
            if (fName.ToLower().EndsWith(".tv") || fName.ToLower().EndsWith(".tv.epl"))
                return Enums.BouquetType.UserBouquetTv;
            
            if (fName.ToLower().EndsWith(".radio") || fName.ToLower().EndsWith(".radio.epl"))
                return Enums.BouquetType.UserBouquetRadio;
            
            return Enums.BouquetType.Unknown;
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