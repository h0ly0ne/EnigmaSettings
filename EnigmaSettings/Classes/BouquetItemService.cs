// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemService : BouquetItem, IBouquetItemService
    {
        private IService _service;

        #region "IEditable"
        private bool _isEditing;
        private IService _mService;
        private string _mDescription;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) 
                return;

            _mService = _service;
            _mDescription = _description;
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

            Service = _mService;
            Description = _mDescription;
            _isEditing = false;
        }
        #endregion

        #region "ICloneable"
        /// <summary>
        /// Performs deep Clone on the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var bis = (IBouquetItemService)CloneWithoutSubscribers();
            bis.Service = (IService)Service?.Clone();
            return bis;
        }
        #endregion

        /// <summary>
        ///     Initializes new bouquet item that is service
        /// </summary>
        /// <param name="bouquetLine">Bouquet line as it's written in the bouquet</param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if bouquetLine is null or empty</exception>
        public BouquetItemService(string bouquetLine)
        {
            if (string.IsNullOrEmpty(bouquetLine))
                throw new ArgumentNullException(nameof(bouquetLine));

            if (bouquetLine.ToUpperInvariant().StartsWith("#SERVICE", StringComparison.Ordinal))
                bouquetLine = bouquetLine.Substring(9).Trim();

            string[] sData = bouquetLine.Trim().Split(':');
            _favoritesTypeFlag = sData[0];
            _lineSpecifierFlag = sData[1];

            string mType;
            string mSid;
            string mNid;
            string mNameSpc;
            string mTSID;

            if (sData.Length >= 10)
            {
                mType = sData[2];
                mSid = sData[3].TrimStart('0');
                mTSID = sData[4].TrimStart('0');
                mNid = sData[5].TrimStart('0');
                mNameSpc = sData[6].TrimStart('0');
            }
            else
            {
                mType = Convert.ToInt32(sData[4], CultureInfo.InvariantCulture).ToString("X", CultureInfo.InvariantCulture);
                mSid = sData[0].TrimStart('0');
                mTSID = sData[2].TrimStart('0');
                mNid = sData[3].TrimStart('0');
                mNameSpc = sData[1].TrimStart('0');
            }

            if (mType.Length == 0)
                mType = "1";
            if (mSid.Length == 0)
                mSid = "0";
            if (mTSID.Length == 0)
                mTSID = "0";
            if (mNid.Length == 0)
                mNid = "0";
            if (mNameSpc.Length == 0)
                mNameSpc = "0";

            ServiceId = string.Join(":", mType, mSid, mTSID, mNid, mNameSpc).ToLowerInvariant();
        }

        /// <summary>
        ///     Initializes new bouquet item with reference to service
        /// </summary>
        /// <param name="service"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if service is null</exception>
        public BouquetItemService(IService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        ///     Bouquet item type
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetItemType.Service</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType => Enums.BouquetItemType.Service;

        /// <summary>
        ///     Used for matching with service from settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string ServiceId
        {
            get => Service != null ? Service.ServiceId : field;
        } = string.Empty;

        /// <summary>
        ///     Service from settings file that this line in bouquet represents
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IService Service
        {
            get => _service;
            set
            {
                if (value == _service)
                    return;

                _service = value;
                OnPropertyChanged(nameof(Service));
            }
        }

        /// <summary>
        ///     Name shown for this service in the bouquet (#DESCRIPTION line), empty when the service name is used
        /// </summary>
        [DataMember]
        public string Description
        {
            get => _description;
            set
            {
                value ??= string.Empty;

                if (value == _description)
                    return;

                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _description = string.Empty;

        /// <summary>
        ///     Return line in format ready to be written to bouquet
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Join(":", FavoritesTypeFlag, LineSpecifierFlag, Service.ServiceId, "0", "0", "0");
        }
    }
}