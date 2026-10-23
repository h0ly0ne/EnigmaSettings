// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ILog
    {
        /// <summary>
        ///     False when debug messages are discarded, so callers can skip building them
        /// </summary>
        bool IsDebugEnabled => true;

        void Debug(string message);
        void Warn(string message);
        void Warn(string message, Exception ex);
        void Info(string message);
        void Error(string message);
        void Error(string message, Exception ex);
    }
}