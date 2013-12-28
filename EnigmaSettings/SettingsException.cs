// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;

namespace Krkadoni.EnigmaSettings
{
    [Serializable]
    public class SettingsException : Exception
    {
        public SettingsException(string message) : base(message)
        {
        }

        public SettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}