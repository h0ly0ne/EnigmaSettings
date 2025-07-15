// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;

namespace Krkadoni.EnigmaSettings
{
    public sealed class FileSavingEventArgs : EventArgs
    {
        private readonly string _file;

        public FileSavingEventArgs (string file)
        {
            _file = file;
        }

        /// <summary>
        ///     File we're trying to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string File
        {
            get { return _file; }
        }
    }
}