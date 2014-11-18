// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;

namespace Krkadoni.EnigmaSettings
{
    public sealed class FileLoadingEventArgs : EventArgs
    {
        private readonly string _file;

        public FileLoadingEventArgs(string file)
        {
            _file = file;
        }

        /// <summary>
        ///     File we're trying to load
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