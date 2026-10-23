// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class XmlCable : IXmlCable
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (XmlCable)MemberwiseClone();
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

        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var cab = (IXmlCable)CloneWithoutSubscribers();
            cab.Transponders = new List<IXmlTransponder>();

            foreach (var xmlTransponder in Transponders)
            {
                if (xmlTransponder != null)
                    cab.Transponders.Add((IXmlTransponder)xmlTransponder.Clone());
            }

            return cab;
        }

        private IList<IXmlTransponder> _transponders = new List<IXmlTransponder>();
        private string _flags;
        private string _name = string.Empty;
        private string _satfeed = string.Empty;
        private string _countrycode = string.Empty;

        /// <summary>
        ///     Cable flags from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Flags
        {
            get => _flags;
            set
            {
                if (value == _flags)
                    return;

                _flags = value;
                OnPropertyChanged(nameof(Flags));
            }
        }

        /// <summary>
        ///     Cable name from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                value ??= string.Empty;

                if (value == _name)
                    return;

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        ///     Cable satfeed from cables.xml file
        /// </summary>
        /// <value>true/false</value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SatFeed
        {
            get => _satfeed;
            set
            {
                value ??= string.Empty;

                if (value == _satfeed)
                    return;

                _satfeed = value;
                OnPropertyChanged(nameof(SatFeed));
            }
        }

        /// <summary>
        ///     Cable countrycode from cables.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string CountryCode
        {
            get => _countrycode;
            set
            {
                value ??= string.Empty;

                if (value == _countrycode)
                    return;

                _countrycode = value;
                OnPropertyChanged(nameof(CountryCode));
            }
        }

        /// <summary>
        ///     List of transponders from xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IList<IXmlTransponder> Transponders
        {
            get => _transponders;
            set
            {
                value ??= new List<IXmlTransponder>();
                _transponders = value;
                OnPropertyChanged(nameof(Transponders));
            }
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