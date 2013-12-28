// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;

namespace Krkadoni.EnigmaSettings
{
    public sealed class SettingsLoadingEventArgs : EventArgs
    {

        private readonly FileInfo _file;

        public SettingsLoadingEventArgs(FileInfo file)
        {
            _file = file;
        }

        /// <summary>
        ///     Settings file we're trying to load
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public FileInfo File
        {
            get { return _file; }
        }

    }
}