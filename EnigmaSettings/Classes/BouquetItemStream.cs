// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemStream : BouquetItem, IBouquetItemStream
    {
        private string _description = string.Empty;
        private string _extraFlag1 = "0";
        private string _extraFlag2 = "0";
        private string _serviceId = "0";
        private string _streamFlag = "1";
        private string _url = string.Empty;

        #region "IEditable"
        private bool _isEditing;
        private string _mDescription;
        private string _mExtraFlag1;
        private string _mExtraFlag2;
        private string _mServiceId;
        private string _mStreamFlag;
        private string _mUrl;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing)
                return;

            _mDescription = _description;
            _mExtraFlag1 = _extraFlag1;
            _mExtraFlag2 = _extraFlag2;
            _mServiceId = _serviceId;
            _mStreamFlag = _streamFlag;
            _mUrl = _url;
            _isEditing = true;
        }

        public override void EndEdit()
        {
            base.EndEdit();
            _isEditing = false;
        }

        public override void CancelEdit()
        {
            base.CancelEdit();
            if (!_isEditing)
                return;

            Description = _mDescription;
            ExtraFlag1 = _mExtraFlag1;
            ExtraFlag2 = _mExtraFlag2;
            ServiceID = _mServiceId;
            StreamFlag = _mStreamFlag;
            URL = _mUrl;


            _isEditing = false;
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs Memberwise Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            return MemberwiseClone();
        }
        #endregion

        /// <summary>
        ///     Initializes new stream
        /// </summary>
        /// <param name="description">Without leading #DESCRIPTION tag</param>
        /// <param name="url">URL of the stream. Will be automatically URL encoded</param>
        /// <param name="streamFlag">Stream Flag (3rd field in line) in Hex format</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if description is null, URL or streamFlag is
        ///     null or empty
        /// </exception>
        public BouquetItemStream(string description, string url, string streamFlag)
        {
            if (description == null || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(streamFlag))
                throw new ArgumentNullException();

            _description = description.Trim();
            _url = url;
            _streamFlag = streamFlag;
            _lineSpecifierFlag = "0";
        }

        /// <summary>
        ///     Initializes new stream
        /// </summary>
        /// <param name="bouquetLine"></param>
        /// <param name="description"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">
        ///     Throws argument null exception if description is null or bouquet line is null or
        ///     empty
        /// </exception>
        public BouquetItemStream(string bouquetLine, string description)
        {
            if (string.IsNullOrEmpty(bouquetLine) || description == null)
                throw new ArgumentNullException();

            if (bouquetLine.ToUpper().StartsWith("#SERVICE"))
                bouquetLine = bouquetLine.Substring(9).Trim();

            string[] sData = bouquetLine.Trim().Split(':');
            FavoritesTypeFlag = sData[0];
            LineSpecifierFlag = sData[1];
            StreamFlag = sData[2];
            ServiceID = sData[3];
            ExtraFlag1 = sData[4];
            ExtraFlag2 = sData[5];
            URL = sData[10];
            Description = description;
        }

        /// <summary>
        ///     URL of the stream
        /// </summary>
        /// <value></value>
        /// <returns>URL decoded string</returns>
        /// <remarks>Will automatically encode string</remarks>
        [DataMember]
        public string URL
        {
            get => Uri.UnescapeDataString(_url);
            set
            {
                if (value == _url)
                    return;

                value ??= string.Empty;
                _url = Uri.EscapeDataString(value);
                OnPropertyChanged(nameof(URL));
            }
        }

        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetItemType.Stream</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType => Enums.BouquetItemType.Stream;

        /// <summary>
        ///     Text as seen in settings
        /// </summary>
        /// <value></value>
        /// <returns>'Stream' if empty</returns>
        /// <remarks></remarks>
        [DataMember]
        public string Description
        {
            get => _description;
            set
            {
                if (value == _description)
                    return;

                if (string.IsNullOrEmpty(value))
                    value = "Stream";

                _description = value.Trim();
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        ///     Value of the current flag without flag type
        /// </summary>
        /// <value></value>
        /// <returns>Hex value</returns>
        /// <remarks></remarks>
        [DataMember]
        public string StreamFlag
        {
            get => _streamFlag;
            set
            {
                if (value == _streamFlag)
                    return;

                if (string.IsNullOrEmpty(value))
                    value = "1";

                _streamFlag = value;
                OnPropertyChanged(nameof(StreamFlag));
                OnPropertyChanged(nameof(StreamFlagInt));
            }
        }

        /// <summary>
        ///     StreamFlag value as integer
        /// </summary>
        /// <value></value>
        /// <returns>0 if flag is not valid hex</returns>
        /// <remarks></remarks>
        [DataMember]
        public int StreamFlagInt
        {
            get
            {
                try
                {
                    return int.Parse(StreamFlag, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                if (value == StreamFlagInt)
                    return;

                _streamFlag = value.ToString("X");
                OnPropertyChanged(nameof(StreamFlag));
                OnPropertyChanged(nameof(StreamFlagInt));
            }
        }

        /// <summary>
        ///     Extra flag as 5th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        [DataMember]
        public string ExtraFlag1
        {
            get => _extraFlag1;
            set
            {
                if (value == _extraFlag1)
                    return;

                if (string.IsNullOrEmpty(value))
                    return;

                _extraFlag1 = value;
                OnPropertyChanged(nameof(ExtraFlag1));
                OnPropertyChanged(nameof(ExtraFlag1Int));
            }
        }

        /// <summary>
        ///     Extra flag as 5th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        [DataMember]
        public int ExtraFlag1Int
        {
            get
            {
                try
                {
                    return int.Parse(_extraFlag1, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                if (value == ExtraFlag1Int)
                    return;

                _extraFlag1 = value.ToString("X");
                OnPropertyChanged(nameof(ExtraFlag1));
                OnPropertyChanged(nameof(ExtraFlag1Int));
            }
        }

        /// <summary>
        ///     Extra flag as 6th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        [DataMember]
        public string ExtraFlag2
        {
            get => _extraFlag2;
            set
            {
                if (value == _extraFlag2)
                    return;

                if (string.IsNullOrEmpty(value))
                    return;

                _extraFlag2 = value;
                OnPropertyChanged(nameof(ExtraFlag2));
                OnPropertyChanged(nameof(ExtraFlag2Int));
            }
        }

        /// <summary>
        ///     Extra flag as 6th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        [DataMember]
        public int ExtraFlag2Int
        {
            get
            {
                try
                {
                    return Int32.Parse(_extraFlag2, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                if (value == ExtraFlag2Int)
                    return;

                _extraFlag2 = value.ToString("X");
                OnPropertyChanged(nameof(ExtraFlag2));
                OnPropertyChanged(nameof(ExtraFlag2Int));
            }
        }

        /// <summary>
        ///     Extra flag as 4th item in service line
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>In hex format</remarks>
        [DataMember]
        public string ServiceID
        {
            get => _serviceId;
            set
            {
                if (value == _serviceId)
                    return;

                if (string.IsNullOrEmpty(value))
                    return;

                _serviceId = value;
                OnPropertyChanged(nameof(ServiceID));
                OnPropertyChanged(nameof(ServiceIDInt));
            }
        }

        /// <summary>
        ///     Extra flag as 4th item in service line
        /// </summary>
        /// <value></value>
        /// <returns>Integer value of flag</returns>
        /// <remarks></remarks>
        [DataMember]
        public int ServiceIDInt
        {
            get
            {
                try
                {
                    return int.Parse(_serviceId, NumberStyles.HexNumber);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                if (value == ExtraFlag2Int)
                    return;

                _serviceId = value.ToString("X");
                OnPropertyChanged(nameof(ServiceID));
                OnPropertyChanged(nameof(ServiceIDInt));
            }
        }

        /// <summary>
        ///     Return line in format ready to be written to bouquet
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Join("\t", string.Join(":", FavoritesType.ToString(), LineSpecifierFlag, StreamFlag, "0", ExtraFlag1, ExtraFlag2, "0", "0", "0", "0", _url), string.Join(" ", "#DESCRIPTION", Description));
        }
    }
}