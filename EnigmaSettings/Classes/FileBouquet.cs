// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
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

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (FileBouquet)MemberwiseClone();
            clone.PropertyChanged = null;
            clone._editSnapshot = null;
            return clone;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private IList<IBouquetItem> _bouquetItems = new List<IBouquetItem>();
        private string _fileName = string.Empty;
        private string _name = string.Empty;
        private bool _hidden;

        #region "IEditable"
        // field values captured by BeginEdit (see EditSnapshot), null while not editing
        private object[] _editSnapshot;

        public void BeginEdit()
        {
            _editSnapshot ??= EditSnapshot.Capture(this);
        }

        public void EndEdit()
        {
            _editSnapshot = null;
        }

        public void CancelEdit()
        {
            if (_editSnapshot == null)
                return;

            EditSnapshot.Restore(this, _editSnapshot);
            _editSnapshot = null;
            OnPropertyChanged(string.Empty);
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var fb = (IFileBouquet)CloneWithoutSubscribers();
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
                _bouquetType = null;
                OnPropertyChanged(nameof(FileName));
                OnPropertyChanged(nameof(BouquetType));
            }
        }

        /// <summary>True when the bouquet is referenced as hidden (LineSpecifier 519) in bouquets.tv/radio.</summary>
        [DataMember]
        public bool Hidden
        {
            get => _hidden;
            set
            {
                if (value == _hidden)
                    return;

                _hidden = value;
                OnPropertyChanged(nameof(Hidden));
            }
        }

        /// <summary>
        ///     True when the bouquet is only referenced from another bouquet (sub bouquet); it is then written
        ///     as a file but not listed in bouquets.tv / bouquets.radio. False for bouquets listed there.
        /// </summary>
        [DataMember]
        public bool IsSubBouquet
        {
            get => _isSubBouquet;
            set
            {
                if (value == _isSubBouquet)
                    return;

                _isSubBouquet = value;
                OnPropertyChanged(nameof(IsSubBouquet));
            }
        }

        private bool _isSubBouquet;

        /// <summary>
        ///     Parental control of the whole bouquet: BlackListed when listed in the Enigma2 blacklist,
        ///     WhiteListed when listed in the whitelist
        /// </summary>
        [DataMember]
        public Enums.ServiceSecurity ServiceSecurity
        {
            get => _serviceSecurity;
            set
            {
                if (value == _serviceSecurity)
                    return;

                _serviceSecurity = value;
                OnPropertyChanged(nameof(ServiceSecurity));
            }
        }

        private Enums.ServiceSecurity _serviceSecurity = Enums.ServiceSecurity.None;

        /// <summary>
        ///     Type of the bouquet. Determined from FileName.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Enums.BouquetType BouquetType => _bouquetType ??= GetTypeFromFileName(_fileName);

        // cached, BouquetType is read in nearly every bouquet filter
        private Enums.BouquetType? _bouquetType;

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
        protected static Enums.BouquetType GetTypeFromFileName(string fName)
        {
            if (string.IsNullOrEmpty(fName))
                return Enums.BouquetType.Unknown;

            if (string.Equals(fName, "bouquets", StringComparison.OrdinalIgnoreCase))
                return Enums.BouquetType.E1Bouquets;
            
            if (fName.EndsWith(".tv", StringComparison.OrdinalIgnoreCase) || fName.EndsWith(".tv.epl", StringComparison.OrdinalIgnoreCase))
                return Enums.BouquetType.UserBouquetTv;

            if (fName.EndsWith(".radio", StringComparison.OrdinalIgnoreCase) || fName.EndsWith(".radio.epl", StringComparison.OrdinalIgnoreCase))
                return Enums.BouquetType.UserBouquetRadio;
            
            return Enums.BouquetType.Unknown;
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return CloneWithoutSubscribers();
        }
    }
}