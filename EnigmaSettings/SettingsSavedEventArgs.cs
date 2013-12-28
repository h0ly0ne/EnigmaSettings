// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsSavedEventArgs : EventArgs
    {

        private readonly DirectoryInfo _folder;
        private readonly bool _success;
        private readonly ISettings _settings;

        public SettingsSavedEventArgs(DirectoryInfo folder, bool success, ISettings settings)
        {
            _folder = folder;
            _success = success;
            _settings = settings;
        }

        /// <summary>
        ///     Settings directory on disk
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DirectoryInfo Folder
        {
            get { return _folder; }
        }

        /// <summary>
        ///     Instance of settings we tried to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Settings
        {
            get {return _settings; }
        }

        /// <summary>
        ///     Success of the save operation
        /// </summary>
        /// <value></value>
        /// <returns>True if save suceeded, false if it fails</returns>
        /// <remarks></remarks>
        public bool Success
        {
            get { return _success; }
        }
    }
}