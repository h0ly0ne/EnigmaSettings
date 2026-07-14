// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsSavingEventArgs(string folder, ISettings settings) : EventArgs
    {
        /// <summary>
        ///     Directory we're saving settings to
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Folder => folder;

        /// <summary>
        ///     Instance of settings we're trying to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Settings => settings;
    }
}