// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class XmlSatellite : IXmlSatellite
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

        private readonly IList<IXmlTransponder> _transponders = new BindingList<IXmlTransponder>();
        private string _flags;
        private string _name = string.Empty;
        private string _position = string.Empty;

        /// <summary>
        ///     Satellite flags from satellites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Flags
        {
            get { return _flags; }
            set
            {
                if (value == _flags) return;
                _flags = value;
                OnPropertyChanged("Flags");
            }
        }

        /// <summary>
        ///     Satellite name from satellites.xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///     Satellite position
        /// </summary>
        /// <value>Position as integer number with length 3 (ie. 19.2E = 192)</value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Position
        {
            get { return _position; }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (value == _position) return;
                _position = value;
                OnPropertyChanged("Position");
                OnPropertyChanged("PositionString");
            }
        }

        /// <summary>
        ///     Parses position to position string
        /// </summary>
        /// <value></value>
        /// <returns>IE. for position value '192' returns '19.2° E'</returns>
        /// <remarks></remarks>
        public string PositionString
        {
            get
            {
                int i;
                if (Position == null || !Int32.TryParse(Position, out i))
                    return string.Empty;
                string pos = Math.Abs(Convert.ToInt32(Position)).ToString(CultureInfo.InvariantCulture);
                if (pos.EndsWith("0"))
                {
                    pos = pos.Substring(0, pos.Length - 1);
                }
                else
                {
                    pos = pos.Substring(0, pos.Length - 1) + "." + pos.Substring(pos.Length - 1);
                    if (pos.StartsWith("."))
                        pos = "0" + pos;
                }
                if (Convert.ToInt32(Position) < 0)
                {
                    return pos + "° W";
                }
                return pos + "° E";
            }
        }

        /// <summary>
        ///     List of transponders from xml file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IList<IXmlTransponder> Transponders
        {
            get { return _transponders; }
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public IXmlSatellite ShallowCopy()
        {
            return (IXmlSatellite)MemberwiseClone();
        }
    }
}