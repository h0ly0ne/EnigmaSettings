// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetsBouquet : IBouquetsBouquet
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (BouquetsBouquet)MemberwiseClone();
            clone.PropertyChanged = null;
            clone._editSnapshot = null;
            return clone;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

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
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var bb = new BouquetsBouquet { Name = Name, Locked = Locked, BouquetItems = new List<IBouquetItem>() };

            foreach (var bouquetItem in BouquetItems)
            {
                bb.BouquetItems.Add((IBouquetItem)bouquetItem.Clone());
            }

            return bb;
        }
        #endregion

        /// <summary>
        ///     List of bouquet items
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IList<IBouquetItem> BouquetItems
        {
            get;
            set
            {
                value ??= new List<IBouquetItem>();
                field = value;
                OnPropertyChanged(nameof(BouquetItems));
            }
        } = new List<IBouquetItem>();

        /// <summary>
        ///     Type of settings file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public Enums.BouquetType BouquetType => Enums.BouquetType.E1BouquetsBouquet;

        /// <summary>
        ///     Bouquet name
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">Throws argument exception if name is null or empty</exception>
        [DataMember]
        public string Name
        {
            get;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException(Resources.Resources.BouquetsBouquet_Name_Bouquet_name_cannot_be_empty, nameof(value));

                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Name));
            }
        } = string.Empty;

        /// <summary>
        ///     Determines if bouquet is locked.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Locked bouquets are stored inside services.locked file</remarks>
        [DataMember]
        public bool Locked
        {
            get;
            set
            {
                if (value == field)
                    return;

                field = value;
                OnPropertyChanged(nameof(Locked));
            }
        }

        /// <summary>
        /// Performs MemberwiseClone
        /// </summary>
        /// <returns></returns>
        public object ShallowCopy()
        {
            return CloneWithoutSubscribers();
        }
    }
}