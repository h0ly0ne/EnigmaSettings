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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "IEditable"
        private bool _isEditing;
        private string _mFlags;
        private string _mName;
        private string _mSatFeed;
        private string _mCountryCode;
        private IList<IXmlTransponder> _mTransponders;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mFlags = _flags;
            _mName = _name;
            _mSatFeed = _satfeed;
            _mCountryCode = _countrycode;
            _mTransponders = new List<IXmlTransponder>(_transponders);
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing)
                return;

            Flags = _mFlags;
            Name = _mName;
            SatFeed = _mSatFeed;
            CountryCode = _mCountryCode;
            Transponders.Clear();

            foreach (var transponder in _mTransponders)
            {
                Transponders.Add(transponder);
            }

            _isEditing = false;
        }
        #endregion

        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var cab = (IXmlCable)MemberwiseClone();
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
                OnPropertyChanged($"Flags");
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
                OnPropertyChanged($"Name");
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
                OnPropertyChanged($"SatFeed");
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
                OnPropertyChanged($"CountryCode");
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
                OnPropertyChanged($"Transponders");
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