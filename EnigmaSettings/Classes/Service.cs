// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
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

        /// <summary>
        ///     MemberwiseClone without the PropertyChanged subscribers of this instance,
        ///     so changes on the copy are not reported to listeners of the original
        /// </summary>
        protected object CloneWithoutSubscribers()
        {
            var clone = (Service<TFlag>)MemberwiseClone();
            clone.PropertyChanged = null;
            clone._editSnapshot = null;
            return clone;
        }

        protected void OnPropertyChanged(String propertyName = "")
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
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var s = (IService)CloneWithoutSubscribers();
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
                throw new ArgumentNullException(nameof(serviceData), Resources.Service_ServiceData_Service_data_cannot_be_empty_);

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
        public Enums.ServiceType ServiceType =>
            int.TryParse(Type, NumberStyles.Integer, CultureInfo.InvariantCulture, out var code) && code >= 0 && Enum.IsDefined((Enums.ServiceType)code)
                ? (Enums.ServiceType)code
                : Enums.ServiceType.Unknown;

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
                string type = int.TryParse(Type, NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeInt)
                    ? typeInt.ToString("X", CultureInfo.InvariantCulture)
                    : "0";

                return Transponder == null
                    ? string.Join(":", type, HexField(SID), HexField(TSID), HexField(ONID), HexField(NameSpace)).ToLowerInvariant()
                    : string.Join(":", type, HexField(SID), HexField(Transponder.TSID), HexField(Transponder.NID), HexField(Transponder.NameSpc)).ToLowerInvariant();
            }
        }

        /// <summary>
        ///     Hex field of a service reference without leading zeros ("0" when empty)
        /// </summary>
        private static string HexField(string value)
        {
            string trimmed = value?.TrimStart('0') ?? string.Empty;
            return trimmed.Length == 0 ? "0" : trimmed;
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
        public string TransponderId => Transponder == null ? field.ToLowerInvariant() : Transponder.TransponderId;

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
                var mFlagList = new List<IFlag>();

                if (string.IsNullOrEmpty(_flags))
                    return new ReadOnlyCollection<IFlag>(mFlagList);

                foreach (string fString in _flags.Split(','))
                {
                    if (fString.Contains(':') && FlagFactory(fString) is { } f)
                        mFlagList.Add(f);
                }

                return new ReadOnlyCollection<IFlag>(mFlagList);
            }
        }

        // TFlag(string) constructor, looked up once per flag type instead of Activator.CreateInstance per flag
        private static readonly Func<string, TFlag> FlagFactory = CreateFlagFactory();

        private static Func<string, TFlag> CreateFlagFactory()
        {
            var ctor = typeof(TFlag).GetConstructor([typeof(string)]);
            if (ctor == null)
                return _ => null;

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(string), "flagString");
            return System.Linq.Expressions.Expression.Lambda<Func<string, TFlag>>(System.Linq.Expressions.Expression.New(ctor, parameter), parameter).Compile();
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
            _flags = flagsList == null ? string.Empty : string.Join(",", flagsList.Where(x => x != null).Select(x => x.FlagString));
            OnPropertyChanged(nameof(Flags));
            OnPropertyChanged(nameof(FlagList));
        }

        public override string ToString()
        {
            var sData = Transponder == null ? string.Join(":", SID, "00000000", "0000", "0000", Type, ProgNumber) : string.Join(":", SID.PadLeft(4, '0'), Transponder.NameSpc.PadLeft(8, '0'), Transponder.TSID.PadLeft(4, '0'), Transponder.NID.PadLeft(4, '0'), Type, ProgNumber);

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
            return CloneWithoutSubscribers();
        }
    }
}