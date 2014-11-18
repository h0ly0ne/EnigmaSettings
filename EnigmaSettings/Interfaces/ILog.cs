// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ILog
    {
        void Debug(string message);
        void Warn(string message);
        void Warn(string message, Exception ex);
        void Info(string message);
        void Error(string message);
        void Error(string message, Exception ex);
    }
}
