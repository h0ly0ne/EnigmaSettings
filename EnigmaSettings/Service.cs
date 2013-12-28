// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Krkadoni.EnigmaSettings.Properties;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class Service<TFlag> : IService where TFlag : class, IFlag
    {
        #region "INotifyPropertyChanged"

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName = "")
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
        private bool _mLocked;
        private string _mName;
        private string _mProgNumber;
        private Enums.ServiceSecurity _mServiceSecurity;
        private string _mSid;
        private ITransponder _mTransponder;
        private string _mType;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mFlags = _flags;
            _mLocked = _locked;
            _mName = _name;
            _mProgNumber = _progNumber;
            _mServiceSecurity = _serviceSecurity;
            _mSid = _sid;
            _mTransponder = _transponder;
            _mType = _type;
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            Locked = _mLocked;
            Name = _mName;
            ProgNumber = _mProgNumber;
            ServiceSecurity = _mServiceSecurity;
            SID = _mSid;
            Transponder = _mTransponder;
            Type = _mType;
            if (_flags != _mFlags)
            {
                _flags = _mFlags;
                OnPropertyChanged("Flags");
                OnPropertyChanged("FlagList");
            }
            _isEditing = false;
        }

        #endregion

        private readonly string _transponderId = string.Empty;
        private string _flags = "p:";
        private bool _locked;
        private string _name = string.Empty;
        private string _progNumber = "0";
        private Enums.ServiceSecurity _serviceSecurity = Enums.ServiceSecurity.None;
        private string _sid = "0";
        private ITransponder _transponder;
        private string _type = "0";

        /// <summary>
        ///     Initializes service properties from 1st line of service in lamedb
        /// </summary>
        /// <param name="serviceData">In format 0xSID:0xNAMESPACE:0xTSID:0xNID:type:prognumber</param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <remarks>Throws argument null exception and argument exception if ServiceData is null, empty or invalid</remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if serviceData or name is null/empty</exception>
        /// <exception cref="ArgumentException">Throws argument exception if serviceData is invalid</exception>
        public Service(string serviceData, string name, string flags)
        {
            if (string.IsNullOrEmpty(serviceData))
                throw new ArgumentNullException(Resources.Service_ServiceData_Service_data_cannot_be_empty_);
            if (string.IsNullOrEmpty(name))
                //throw new ArgumentNullException(Resources.Service_New_Service_name_cannot_be_empty);
                name = string.Empty;
            string[] sData = serviceData.Split(':');
            if (sData.Length != 6)
                throw new ArgumentException(Resources.Service_ServiceData_Invalid_service_data_);
            SID = sData[0].Trim();
            Type = sData[4].Trim();
            ProgNumber = sData[5].Trim();
            _transponderId = string.Join(":", new[]
            {
                sData[1],
                sData[2],
                sData[3]
            });
            Name = name;
            _flags = flags;
        }

        /// <summary>
        ///     Service ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string SID
        {
            get { return _sid; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _sid) return;
                _sid = value;
                OnPropertyChanged("SID");
                OnPropertyChanged("ServiceId");
            }
        }

        /// <summary>
        ///     Defines type of service (TV,Radio,Data, etc..)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        public string Type
        {
            get { return _type; }
            set
            {
                if (value == null)
                    value = "0";
                if (value.Trim() == _type) return;
                _type = value.Trim();
                OnPropertyChanged("Type");
                OnPropertyChanged("ServiceType");
                OnPropertyChanged("ServiceId");
            }
        }

        /// <summary>
        ///     Program number
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ProgNumber
        {
            get { return _progNumber; }
            set
            {
                if (value == null)
                    value = "0";
                if (value == _progNumber) return;
                _progNumber = value;
                OnPropertyChanged("ProgNumber");
            }
        }

        /// <summary>
        ///     1 -TV, 2-radio, 3-data
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        public Enums.ServiceType ServiceType
        {
            get
            {
                switch (Type)
                {
                    case "1":
                        return Enums.ServiceType.Tv;
                    case "2":
                        return Enums.ServiceType.Radio;
                    case "3":
                        return Enums.ServiceType.Data;
                    case "0":
                        return Enums.ServiceType.Reserved0;
                    case "4":
                        return Enums.ServiceType.NvodMpeg2Sd;
                    case "5":
                        return Enums.ServiceType.NvodTimeShifted;
                    case "6":
                        return Enums.ServiceType.Mosaic;
                    case "7":
                        return Enums.ServiceType.Reserved7;
                    case "8":
                        return Enums.ServiceType.Reserved8;
                    case "9":
                        return Enums.ServiceType.Reserved9;
                    case "10":
                        return Enums.ServiceType.AcRadio;
                    case "11":
                        return Enums.ServiceType.AcMosaic;
                    case "12":
                        return Enums.ServiceType.DataBroadcastService;
                    case "13":
                        return Enums.ServiceType.ReservedCi;
                    case "14":
                        return Enums.ServiceType.RcsMap;
                    case "15":
                        return Enums.ServiceType.RcsFls;
                    case "16":
                        return Enums.ServiceType.DVBMhp;
                    case "17":
                        return Enums.ServiceType.Mpeg2Hd;
                    case "18":
                        return Enums.ServiceType.Reserved18;
                    case "19":
                        return Enums.ServiceType.Reserved19;
                    case "20":
                        return Enums.ServiceType.Reserved20;
                    case "21":
                        return Enums.ServiceType.Reserved21;
                    case "22":
                        return Enums.ServiceType.AcSdtv;
                    case "23":
                        return Enums.ServiceType.AcSdNvodTimeShifted;
                    case "24":
                        return Enums.ServiceType.AcSdNvodReference;
                    case "25":
                        return Enums.ServiceType.AcHdtv;
                    case "26":
                        return Enums.ServiceType.AcHdNvodTimeShifted;
                    case "27":
                        return Enums.ServiceType.AcHdNvodReference;
                    case "134":
                        return Enums.ServiceType.UserDefined134Bskyb;
                    case "135":
                        return Enums.ServiceType.UserDefined135Bskyb;
                    default:
                        return Enums.ServiceType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Service name (2nd line in settings file)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                if (value == null)
                    value = string.Empty;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        ///     Flags for Audio, Video, Subtitles, Provider etc..
        /// </summary>
        /// <value>Returns 'p:' if empty</value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Flags
        {
            get
            {
                if (string.IsNullOrEmpty(_flags))
                    return "p:";
                return _flags;
            }
        }

        /// <summary>
        ///     Service information in format Type:0xSID:0xTSID:0xNID,0xNAMESPACE
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Used to match service from bouquets</remarks>
        public string ServiceId
        {
            get
            {
                if (Transponder == null)
                {
                    string mSid = SID.TrimStart('0');
                    if (mSid.Length == 0)
                        mSid = "0";
                    return string.Join(":", new[] {"0", mSid, "0", "0", "0"}).ToLower();
                }
                else
                {
                    string mSid = SID.TrimStart('0');
                    if (mSid.Length == 0)
                        mSid = "0";
                    string mTSID = Transponder.TSID.TrimStart('0');
                    if (mTSID.Length == 0)
                        mTSID = "0";
                    string mNid = Transponder.NID.TrimStart('0');
                    if (mNid.Length == 0)
                        mNid = "0";
                    string mNameSpc = Transponder.NameSpc.TrimStart('0');
                    if (mNameSpc.Length == 0)
                        mNameSpc = "0";
                    return string.Join(":", new[] {Convert.ToInt32(Type).ToString("X"), mSid, mTSID, mNid, mNameSpc}).ToLower();
                }
            }
        }

        /// <summary>
        ///     0xNAMESPACE:0xTSID:0xNID
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     If Transponder is set returns its TransponderID, otherwise returns value from service data it has been
        ///     initialized with
        /// </returns>
        /// <remarks></remarks>
        public string TransponderId
        {
            get
            {
                if (Transponder == null)
                {
                    return _transponderId.ToLower();
                }
                return Transponder.TransponderId;
            }
        }

        public ITransponder Transponder
        {
            get { return _transponder; }
            set
            {
                if (value != _transponder)
                {
                    _transponder = value;
                    OnPropertyChanged("Transponder");
                    OnPropertyChanged("TransponderId");
                    OnPropertyChanged("ServiceId");
                }
            }
        }

        /// <summary>
        ///     Determines if service is in Whitelist or Blacklist file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public Enums.ServiceSecurity ServiceSecurity
        {
            get { return _serviceSecurity; }
            set
            {
                if (value == _serviceSecurity) return;
                _serviceSecurity = value;
                OnPropertyChanged("ServiceSecurity");
            }
        }

        /// <summary>
        ///     List of flag objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Use UpdateFlags method to update flags</remarks>
        public ReadOnlyCollection<IFlag> FlagList
        {
            get
            {
                string[] fStrings = _flags.Split(',');
                var mFlagList = new BindingList<IFlag>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (string fString  in fStrings)
                {
                    if (fString.IndexOf(":", StringComparison.InvariantCulture) > -1 && fString.Split(':').Length >= 2)
                    {
                        IFlag f = Activator.CreateInstance(typeof (TFlag), new object[] {fString}) as TFlag;
                        if (f == null) continue;
                        mFlagList.Add(f);
                    }
                }
                return new ReadOnlyCollection<IFlag>(mFlagList);
            }
        }

        /// <summary>
        ///     Syncs Flags string from the list of flags
        /// </summary>
        /// <remarks></remarks>
        public void UpdateFlags(IList<IFlag> flagsList)
        {
            _flags = string.Join(",", FlagList.Select(x => x.FlagString).ToArray());
            OnPropertyChanged("Flags");
            OnPropertyChanged("FlagList");
        }

        /// <summary>
        ///     Determines if service is locked for updates
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (value != Locked)
                {
                    _locked = value;
                    OnPropertyChanged("Locked");
                }
            }
        }

        public override string ToString()
        {
            string sData;
            if (Transponder == null)
            {
                sData = string.Join(":", new[] {SID, "00000000", "0000", "0000", Type, ProgNumber});
            }
            else
            {
                sData = string.Join(":", new[]
                {
                    SID.PadLeft(4, '0'),
                    Transponder.NameSpc.PadRight(8, '0'),
                    Transponder.TSID.PadLeft(4, '0'),
                    Transponder.NID.PadLeft(4, '0'),
                    Type,
                    ProgNumber
                });
            }
            return string.Join("\t", new[]
            {
                sData,
                Name,
                Flags
            });
        }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        public IService ShallowCopy()
        {
            return (IService)MemberwiseClone();
        }
    }
}