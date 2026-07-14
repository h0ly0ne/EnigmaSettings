// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;

namespace Krkadoni.EnigmaSettings
{
    public sealed class FileSavedEventArgs(string file, bool success) : EventArgs
    {
        /// <summary>
        ///     File we tried to save
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string File => file;

        /// <summary>
        ///     Success of the save operation
        /// </summary>
        /// <value></value>
        /// <returns>True if save succeeded, false if it fails</returns>
        /// <remarks></remarks>
        public bool Success => success;
    }
}