// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsSavedEventArgs(string folder, bool success, ISettings settings) : EventArgs
    {
        /// <summary>
        ///     Settings directory on disk
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Folder => folder;

        /// <summary>
        ///     Instance of settings we tried to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Settings => settings;

        /// <summary>
        ///     Success of the save operation
        /// </summary>
        /// <value></value>
        /// <returns>True if save succeeded, false if it fails</returns>
        /// <remarks></remarks>
        public bool Success => success;
    }
}