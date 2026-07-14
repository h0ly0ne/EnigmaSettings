// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsLoadingEventArgs(string file) : EventArgs
    {
        /// <summary>
        ///     Settings file we're trying to load
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string File => file;
    }
}