// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class XmlSatellite : IXmlSatellite
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (XmlSatellite)MemberwiseClone();
            clone.PropertyChanged = null;
            return clone;
        }

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
        private string _mFlags;
        private string _mName;
        private string _mPosition;
        private IList<IXmlTransponder> _mTransponders;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mFlags = _flags;
            _mName = _name;
            _mPosition = _position;
            _mTransponders = new List<IXmlTransponder>(_transponders);
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            Flags = _mFlags;
            Name = _mName;
            Position = _mPosition;
            Transponders.Clear();
            foreach (IXmlTransponder transponder in _mTransponders)
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
            var sat = (IXmlSatellite)CloneWithoutSubscribers();
            sat.Transponders = new List<IXmlTransponder>();

            foreach (var xmlTransponder in Transponders)
            {
                if (xmlTransponder != null)
                    sat.Transponders.Add((IXmlTransponder)xmlTransponder.Clone());
            }

            return sat;
        }

        private IList<IXmlTransponder> _transponders = new List<IXmlTransponder>();
        private string _flags;
        private string _name = string.Empty;
        private string _position = string.Empty;

        /// <summary>
        ///     Satellite flags from satellites.xml file
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
        ///     Satellite name from satellites.xml file
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
        ///     Satellite position
        /// </summary>
        /// <value>Position as integer number with length 3 (e.g. 19.2E = 192)</value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string Position
        {
            get => _position;
            set
            {
                value ??= string.Empty;

                if (value == _position) 
                    return;

                _position = value;
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionString));
            }
        }

        /// <summary>
        ///     Parses position to position string
        /// </summary>
        /// <value></value>
        /// <returns>IE. for position value '192' returns '19.2° E'</returns>
        /// <remarks></remarks>
        [DataMember]
        public string PositionString
        {
            get
            {
                if (Position == null || !int.TryParse(Position, NumberStyles.Integer, CultureInfo.InvariantCulture, out var position))
                    return string.Empty;

                // positions are stored in tenths of a degree, west negative (e.g. 192 = 19.2° E, -300 = 30.0° W)
                string pos = (Math.Abs(position) / 10m).ToString("0.0", CultureInfo.InvariantCulture);
                return position < 0 ? pos + "° W" : pos + "° E";
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