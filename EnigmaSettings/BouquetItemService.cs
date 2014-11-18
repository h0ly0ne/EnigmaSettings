// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Runtime.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    [DataContract]
    public class BouquetItemService : BouquetItem, IBouquetItemService
    {
        private readonly string _serviceId = string.Empty;

        private IService _service;

        #region "IEditable"

        private bool _isEditing;
        private IService _mService;

        public override void BeginEdit()
        {
            base.BeginEdit();
            if (_isEditing) return;
            _mService = _service;
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
            if (!_isEditing) return;
            Service = _mService;
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
            var bis = (IBouquetItemService)MemberwiseClone();
            bis.Service = Service != null ? (IService)Service.Clone() : null;
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
                throw new ArgumentNullException();

            if (bouquetLine.ToUpper().StartsWith("#SERVICE"))
            {
                bouquetLine = bouquetLine.Substring(9).Trim();
            }

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
                mType = Convert.ToInt32(sData[4]).ToString("X");
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

            _serviceId = string.Join(":", new[]
            {
                mType,
                mSid,
                mTSID,
                mNid,
                mNameSpc
            }).ToLower();
        }

        /// <summary>
        ///     Initializes new bouquet item with reference to service
        /// </summary>
        /// <param name="service"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if service is null</exception>
        public BouquetItemService(IService service)
        {
            if (service == null)
                throw new ArgumentNullException();
            Service = service;
        }

        /// <summary>
        ///     Bouquet item type
        /// </summary>
        /// <value></value>
        /// <returns>Enums.BouquetItemType.Service</returns>
        /// <remarks></remarks>
        [DataMember]
        public override Enums.BouquetItemType BouquetItemType
        {
            get { return Enums.BouquetItemType.Service; }
        }

        /// <summary>
        ///     Used for matching with service from settings
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public string ServiceId
        {
            get
            {
                if (Service != null)
                {
                    return Service.ServiceId;
                }
                return _serviceId;
            }
        }

        /// <summary>
        ///     Service from settings file that this line in bouquet represents
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [DataMember]
        public IService Service
        {
            get { return _service; }
            set
            {
                if (value == _service) return;
                _service = value;
                OnPropertyChanged("Service");
            }
        }

        /// <summary>
        ///     Return line in format ready to be written to bouquet
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            return string.Join(":", new[]
            {
                FavoritesTypeFlag,
                LineSpecifierFlag,
                Service.ServiceId,
                "0",
                "0",
                "0"
            });
        }
    }
}