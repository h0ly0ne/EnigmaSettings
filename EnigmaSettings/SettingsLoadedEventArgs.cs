// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsLoadedEventArgs : EventArgs
    {

        private readonly string _file;
        private readonly bool _success;
        private readonly ISettings _settings;

        public SettingsLoadedEventArgs(string file, bool success, ISettings settings)
        {
            _file = file;
            _success = success;
            _settings = settings;
        }

        /// <summary>
        ///     Settings file we tried to load
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string File
        {
            get { return _file; }
        }

        /// <summary>
        ///     Success of the load operation
        /// </summary>
        /// <value></value>
        /// <returns>True if load suceeded, false if it fails</returns>
        /// <remarks></remarks>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        ///     Loaded settings instance
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