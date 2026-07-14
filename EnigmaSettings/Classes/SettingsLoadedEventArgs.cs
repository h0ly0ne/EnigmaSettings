// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsLoadedEventArgs(string file, bool success, ISettings settings) : EventArgs
    {
        /// <summary>
        ///     Settings file we tried to load
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string File => file;

        /// <summary>
        ///     Success of the load operation
        /// </summary>
        /// <value></value>
        /// <returns>True if load succeeded, false if it fails</returns>
        /// <remarks></remarks>
        public bool Success => success;

        /// <summary>
        ///     Loaded settings instance
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Settings => settings;
    }
}