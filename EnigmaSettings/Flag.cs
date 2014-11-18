// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.ComponentModel;
using System.Globalization;
using Krkadoni.EnigmaSettings.Interfaces;
using Krkadoni.EnigmaSettings.Properties;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class Flag : IFlag
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
        private string _mFlagString;
        
        public void BeginEdit()
        {
            if (_isEditing) return;
            _mFlagString = _flagString;
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            if (_flagString != _mFlagString)
            {
                _flagString = _mFlagString;
                OnPropertyChanged("FlagInt");
                OnPropertyChanged("FlagValue");
                OnPropertyChanged("FlagString");
                OnPropertyChanged("C_FlagType");
                OnPropertyChanged("F_FlagType");
            }
            _isEditing = false;
        }

        #endregion

        #region "ICloneable"

        /// <summary>
        /// Performs MemberwiseClone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        private string _flagString = string.Empty;

        /// <summary>
        ///     Initializes new Flag object and fills all the properties
        /// </summary>
        /// <param name="flagString">Flag in format X:Y where X is flag type and Y is hex value of the flag</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if FlagString is null or empty value</exception>
        public Flag(string flagString)
        {
            if (string.IsNullOrEmpty(flagString))
                throw new ArgumentNullException(Resources.Flag_New_Flag_string_cannot_be_empty_);
            _flagString = flagString;
        }

        /// <summary>
        ///     Type of F flag
        /// </summary>
        /// <value></value>
        /// <returns>F Flag type from enum, if it's not supported returns 'Unknown'</returns>
        /// <remarks>If current flag is not F, type will be 'None'</remarks>
        public Enums.FFlagType FFlagType
        {
            get
            {
                if (!FlagString.ToLower().StartsWith("f:"))
                    return Enums.FFlagType.None;
                if (FlagValue.TrimStart('0').Length == 0)
                    return Enums.FFlagType.Empty;
                switch (FlagInt)
                {
                    case 1:
                        return Enums.FFlagType.NoSdt;
                    case 2:
                        return Enums.FFlagType.Dontshow;
                    case 4:
                        return Enums.FFlagType.NoDVB;
                    case 8:
                        return Enums.FFlagType.HoldName;
                    case 64:
                        return Enums.FFlagType.NewFound;
                    case 3:
                        return Enums.FFlagType.NoSdtDontshow;
                    case 5:
                        return Enums.FFlagType.NoSdtNoDVB;
                    case 6:
                        return Enums.FFlagType.DontshowNoDVB;
                    case 7:
                        return Enums.FFlagType.NoSdtDontshowNoDVB;
                    case 9:
                        return Enums.FFlagType.NoSdtHoldName;
                    case 10:
                        return Enums.FFlagType.DontshowHoldName;
                    case 11:
                        return Enums.FFlagType.NoSdtDontshowHoldName;
                    case 12:
                        return Enums.FFlagType.NoDVBHoldName;
                    case 13:
                        return Enums.FFlagType.NoSdtNoDVBHoldName;
                    case 14:
                        return Enums.FFlagType.DontshowNoDVBHoldName;
                    case 15:
                        return Enums.FFlagType.NoSdtDontshowNoDVBHoldName;
                    case 65:
                        return Enums.FFlagType.NoSdtNewFound;
                    case 66:
                        return Enums.FFlagType.DontshowNewFound;
                    case 67:
                        return Enums.FFlagType.NoSdtDontshowNewFound;
                    case 68:
                        return Enums.FFlagType.NoDVBNewFound;
                    case 69:
                        return Enums.FFlagType.NoSdtNoDVBNewFound;
                    case 70:
                        return Enums.FFlagType.DontshowNoDVBNewFound;
                    case 71:
                        return Enums.FFlagType.NoSdtDontshowNoDVBNewFound;
                    case 72:
                        return Enums.FFlagType.HoldNameNewFound;
                    case 73:
                        return Enums.FFlagType.NoSdtHoldNameNewFound;
                    case 74:
                        return Enums.FFlagType.DontshowHoldNameNewFound;
                    case 75:
                        return Enums.FFlagType.NoSdtDontshowHoldNameNewFound;
                    case 76:
                        return Enums.FFlagType.NoDVBHoldNameNewFound;
                    case 77:
                        return Enums.FFlagType.NoSdtNoDVBHoldNameNewFound;
                    case 78:
                        return Enums.FFlagType.DontshowNoDVBHoldNameNewFound;
                    case 79:
                        return Enums.FFlagType.NoSdtDontshowNoDVBHoldNameNewFound;
                    default:
                        return Enums.FFlagType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Type of C flag
        /// </summary>
        /// <value></value>
        /// <returns>C Flag type from enum, if it's not supported returns 'Unknown'</returns>
        /// <remarks>If current flag is not C, type will be 'None'</remarks>
        public Enums.CFlagType CFlagType
        {
            get
            {
                if (!FlagString.ToLower().StartsWith("c:"))
                    return Enums.CFlagType.None;
                if (FlagValue.Trim('0').Length == 0)
                    return Enums.CFlagType.Empty;
                if (FlagValue.Length < 2)
                    return Enums.CFlagType.Unknown;
                switch (FlagValue.Substring(0, 2))
                {
                    case "00":
                        return Enums.CFlagType.Video;
                    case "01":
                        return Enums.CFlagType.Audio;
                    case "02":
                        return Enums.CFlagType.Teletext;
                    case "03":
                        return Enums.CFlagType.Pcr;
                    case "04":
                        return Enums.CFlagType.Ac3Audio;
                    case "05":
                        return Enums.CFlagType.VideoType;
                    case "06":
                        return Enums.CFlagType.AudioChannel;
                    case "07":
                        return Enums.CFlagType.Ac3Delay;
                    case "08":
                        return Enums.CFlagType.PcmDelay;
                    case "09":
                        return Enums.CFlagType.Subtitle;
                    default:
                        return Enums.CFlagType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Type of the flag from 3rd line of service data in lamedb
        /// </summary>
        /// <value></value>
        /// <returns>C, F, P or Unknown</returns>
        /// <remarks>Defaults to 'Unknown'</remarks>
        public Enums.FlagType FlagType
        {
            get
            {
                if (FlagString.Length == 0)
                    return Enums.FlagType.Unknown;
                switch (FlagString.Split(':')[0].ToLower())
                {
                    case "c":
                        return Enums.FlagType.C;
                    case "f":
                        return Enums.FlagType.F;
                    case "p":
                        return Enums.FlagType.P;
                    default:
                        return Enums.FlagType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Value of the current flag without flag type
        /// </summary>
        /// <value></value>
        /// <returns>Hex value if flag is numeric</returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">Throws argument exception if set before speficifing flag type</exception>
        public string FlagValue
        {
            get { return FlagString.Length > 0 ? FlagString.Split(':')[1] : string.Empty; }
            set
            {
                if (FlagString.Length == 0)
                    throw new ArgumentException(Resources.Flag_FlagValue_Cannot_change_flag_value_before_specifing_flag_);
                if (value == null)
                    value = string.Empty;
                if (value != FlagString.Split(':')[0] + ":" + value)
                {
                    FlagString = FlagString.Split(':')[0] + ":" + value;
                }
            }
        }

        /// <summary>
        ///     Flag value as integer if flag is hexadecimal
        /// </summary>
        /// <value></value>
        /// <returns>0 if flag is not numeric</returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">Throws argument exception if set before speficifing flag type</exception>
        public int FlagInt
        {
            get
            {
                if (FlagValue.Length > 0 && FlagType != Enums.FlagType.Unknown)
                {
                    try
                    {
                        return Int32.Parse(FlagValue, NumberStyles.HexNumber);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                return 0;
            }
            set
            {
                if (FlagString.Length == 0)
                    throw new ArgumentException(Resources.Flag_FlagValue_Cannot_change_flag_value_before_specifing_flag_);
                switch (FlagType)
                {
                    case Enums.FlagType.C:
                        value = Convert.ToInt32(value.ToString("X").PadLeft(6, '0'));
                        break;
                    case Enums.FlagType.F:
                        value = Convert.ToInt32(value.ToString("X"));
                        break;
                }
                FlagString = FlagString.Split(':')[0] + ":" + value;
            }
        }

        /// <summary>
        ///     Flag in format X:Y where X is flag type and Y is hex value of the flag
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if setting null or empty value</exception>
        /// <exception cref="ArgumentException">Throws argument exception if setting flag in invalid format</exception>
        public string FlagString
        {
            get { return _flagString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(Resources.Flag_New_Flag_string_cannot_be_empty_);
                if (value.IndexOf(":", StringComparison.InvariantCulture) == -1)
                    throw new ArgumentException(Resources.Flag_New_Invalid_flag);
                if (value == _flagString) return;
                _flagString = value.Trim();
                OnPropertyChanged("FlagInt");
                OnPropertyChanged("FlagValue");
                OnPropertyChanged("FlagString");
                OnPropertyChanged("C_FlagType");
                OnPropertyChanged("F_FlagType");
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