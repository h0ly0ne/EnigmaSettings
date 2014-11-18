// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsSavingEventArgs : EventArgs
    {

        private readonly string _folder;
        private readonly ISettings _settings;

        public SettingsSavingEventArgs(string folder, ISettings settings)
        {
            _folder = folder;
            _settings = settings;
        }

        /// <summary>
        ///     Directory we're saving settings to
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Folder
        {
            get { return _folder; }
        }

        /// <summary>
        ///     Instance of settings we're trying to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Settings
        {
            get { return _settings; }
        }
    }
}