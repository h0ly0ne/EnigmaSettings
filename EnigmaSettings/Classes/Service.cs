// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class Service<TFlag> : IService where TFlag : class, IFlag
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "IEditable"
        private bool _isEditing;
        private string _mFlags;
        private string _mTSID;
        private string _mONID;
        private string _mNameSpace;
        private bool _mLocked;
        private string _mName;
        private string _mProgNumber;
        private string _mSourceId;
        private Enums.ServiceSecurity _mServiceSecurity;
        private string _mSid;
        private ITransponder _mTransponder;
        private string _mType;

        public void BeginEdit()
        {
            if (_isEditing)
                return;

            _mFlags = _flags;
            _mTSID = _TSID;
            _mONID = _ONID;
            _mNameSpace = _nameSpace;
            _mLocked = _locked;
            _mName = _name;
            _mProgNumber = _progNumber;
            _mSourceId = _sourceId;
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
            if (!_isEditing)
                return;

            Locked = _mLocked;
            Name = _mName;
            TSID = _mTSID;
            ONID = _mONID;
            NameSpace = _mNameSpace;
            ProgNumber = _mProgNumber;
            SourceID = _mSourceId;
            ServiceSecurity = _mServiceSecurity;
            SID = _mSid;
            Transponder = _mTransponder;
            Type = _mType;
            
            if (_flags != _mFlags)
            {
                _flags = _mFlags;
                OnPropertyChanged(nameof(Flags));
                OnPropertyChanged(nameof(FlagList));
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
            var s = (IService)MemberwiseClone();
            s.Transponder = (ITransponder)Transponder?.Clone();
            return s;
        }
        #endregion

        private string _flags;
        private string _TSID;
        private string _ONID;
        private string _nameSpace;
        private bool _locked;
        private string _name = string.Empty;
        private string _progNumber = "0";
        private string _sourceId = string.Empty;
        private Enums.ServiceSecurity _serviceSecurity = Enums.ServiceSecurity.None;
        private string _sid = "0";
        private ITransponder _transponder;
        private string _type = "0";

        /// <summary>
        ///     Initializes service properties from 1st line of service in lamedb
        /// </summary>
        /// <param name="serviceData">In format 0xSID:0xNAMESPACE:0xTSID:0xNID:type:prognumber[:0xsourceid]</param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <remarks>
        ///     Throws argument null exception and argument exception if ServiceData is null, empty or invalid.
        ///     Newer enigma2 lamedb files (still tagged "eDVB services /4/", e.g. produced by DemonEdit)
        ///     add a 7th field, the source id, to the reference line:
        ///     sid:namespace:tsid:onid:type:number:sourceid. enigma2 reads it with
        ///     sscanf(line, "%x:%x:%x:%x:%d:%d:%x", ...) - the 7th value defaults to 0 when absent - so we
        ///     accept six or seven fields and preserve the source id verbatim for a faithful save round-trip.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if serviceData or name is null/empty</exception>
        /// <exception cref="ArgumentException">Throws argument exception if serviceData is invalid</exception>
        public Service(string serviceData, string name, string flags)
        {
            if (string.IsNullOrEmpty(serviceData))
                throw new ArgumentNullException(Resources.Service_ServiceData_Service_data_cannot_be_empty_);

            if (string.IsNullOrEmpty(name))
                name = string.Empty;

            var sData = serviceData.Split(':');

            if (sData.Length is 6 or 7)
            {
                SID = sData[0].Trim();
                Type = sData[4].Trim();
                ProgNumber = sData[5].Trim();

                if (sData.Length >= 7)
                    SourceID = sData[6].Trim();

                TransponderId = string.Join(":", sData[1], sData[2], sData[3]);
                _TSID = sData[2];
                _ONID = sData[3];
                _nameSpace = sData[1];
                Name = name;
                _flags = flags;
            }
            else
                throw new ArgumentException(Resources.Service_ServiceData_Invalid_service_data_);
        }

        /// <summary>
        ///     Service ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string SID
        {
            get => _sid;
            set
            {
                value ??= "0";

                if (value == _sid) 
                    return;

                _sid = value;

                OnPropertyChanged(nameof(SID));
                OnPropertyChanged(nameof(ServiceId));
            }
        }

        /// <summary>
        ///     Defines type of service (TV,Radio,Data, etc.)
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        [DataMember]
        public string Type
        {
            get => _type;
            set
            {
                value ??= "0";

                if (value.Trim() == _type)
                    return;

                _type = value.Trim();
                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(ServiceType));
                OnPropertyChanged(nameof(ServiceId));
            }
        }

        /// <summary>
        ///     Program number
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string ProgNumber
        {
            get => _progNumber;
            set
            {
                value ??= "0";

                if (value == _progNumber)
                    return;

                _progNumber = value;
                OnPropertyChanged(nameof(ProgNumber));
            }
        }

        /// <summary>
        ///     Source id - optional 7th field of the service reference line in newer enigma2 lamedb files
        ///     (sid:namespace:tsid:onid:type:number:sourceid). Empty when the line carried only the classic
        ///     six fields, in which case it is not written back out.
        /// </summary>
        /// <value></value>
        /// <returns>Hex string exactly as stored in the lamedb, or empty when not present</returns>
        /// <remarks>enigma2 defaults this to 0 when absent</remarks>
        [DataMember]
        public string SourceID
        {
            get => _sourceId;
            set
            {
                value ??= string.Empty;

                if (value == _sourceId)
                    return;

                _sourceId = value;
                OnPropertyChanged(nameof(SourceID));
            }
        }

        /// <summary>
        ///     1 -TV, 2-radio, 3-data
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd693747(v=vs.85).aspx">Service types</see>
        [DataMember]
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
                    case "28":
                        return Enums.ServiceType.AcStereoHdtv;
                    case "29":
                        return Enums.ServiceType.AcStereoHdNvodTimeShifted;
                    case "30":
                        return Enums.ServiceType.AcStereoHdNvodReference;
                    case "31":
                        return Enums.ServiceType.Hevc;
                    case "32":
                        return Enums.ServiceType.HevcUhd;
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
        ///     Transport Stream ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string TSID
        {
            get => _TSID;
            set
            {
                if (value == _TSID)
                    return;

                value ??= string.Empty;
                _TSID = value;
                OnPropertyChanged(nameof(TSID));
            }
        }

        /// <summary>
        ///     Originator Network ID
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string ONID
        {
            get => _ONID;
            set
            {
                if (value == _ONID)
                    return;

                value ??= string.Empty;
                _ONID = value;
                OnPropertyChanged(nameof(ONID));
            }
        }

        /// <summary>
        ///     Transponder NameSpace
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string NameSpace
        {
            get => _nameSpace;
            set
            {
                if (value == _nameSpace)
                    return;

                value ??= string.Empty;
                _nameSpace = value;
                OnPropertyChanged(nameof(NameSpace));
            }
        }

        /// <summary>
        ///     Flags for Audio, Video, Subtitles, Provider etc..
        /// </summary>
        /// <value>Returns 'p:' if empty</value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
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
        [DataMember]
        public string ServiceId
        {
            get
            {
                if (Transponder == null)
                {
                    string mSid = SID.TrimStart('0');
                    
                    if (mSid.Length == 0)
                        mSid = "0";

                    return string.Join(":", Convert.ToInt32(Type).ToString("X"), mSid.TrimStart('0'), !string.IsNullOrEmpty(TSID) ? TSID.TrimStart('0') : "0", !string.IsNullOrEmpty(ONID) ? ONID.TrimStart('0') : "0", !string.IsNullOrEmpty(NameSpace) ? NameSpace.TrimStart('0') : "0").ToLower();
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

                    return string.Join(":", Convert.ToInt32(Type).ToString("X"), mSid, mTSID, mNid, mNameSpc).ToLower();
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
        [DataMember]
        public string TransponderId => Transponder == null ? field.ToLower() : Transponder.TransponderId;

        [DataMember]
        public ITransponder Transponder
        {
            get => _transponder;
            set
            {
                if (value == _transponder)
                    return;

                _transponder = value;
                OnPropertyChanged(nameof(Transponder));
                OnPropertyChanged(nameof(TransponderId));
                OnPropertyChanged(nameof(ServiceId));
            }
        }

        /// <summary>
        ///     Determines if service is in Whitelist or Blacklist file
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        ///     List of flag objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Use UpdateFlags method to update flags</remarks>
        [DataMember]
        public ReadOnlyCollection<IFlag> FlagList
        {
            get
            {
                string[] fStrings = _flags.Split(',');
                var mFlagList = new List<IFlag>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (string fString  in fStrings)
                {
                    if (fString.IndexOf(":", StringComparison.CurrentCulture) > -1 && fString.Split(':').Length >= 2)
                    {
                        IFlag f = Activator.CreateInstance(typeof (TFlag), fString) as TFlag;

                        if (f == null)
                            continue;

                        mFlagList.Add(f);
                    }
                }
                return new ReadOnlyCollection<IFlag>(mFlagList);
            }
        }

        /// <summary>
        ///     Determines if service is locked for updates
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public bool Locked
        {
            get => _locked;
            set
            {
                if (value == Locked)
                    return;

                _locked = value;
                OnPropertyChanged(nameof(Locked));
            }
        }

        /// <summary>
        ///     Syncs Flags string from the list of flags
        /// </summary>
        /// <remarks></remarks>
        public void UpdateFlags(IList<IFlag> flagsList)
        {
            _flags = string.Join(",", FlagList.Select(x => x.FlagString).ToArray());
            OnPropertyChanged(nameof(Flags));
            OnPropertyChanged(nameof(FlagList));
        }

        public override string ToString()
        {
            var sData = Transponder == null ? string.Join(":", SID, "00000000", "0000", "0000", Type, ProgNumber) : string.Join(":", SID.PadLeft(4, '0'), Transponder.NameSpc.PadRight(8, '0'), Transponder.TSID.PadLeft(4, '0'), Transponder.NID.PadLeft(4, '0'), Type, ProgNumber);

            if (_sourceId.Length > 0)
                sData = sData + ":" + _sourceId;

            return string.Join("\t", sData, Name, Flags);
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