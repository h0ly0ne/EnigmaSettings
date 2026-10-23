// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
   public class NullLogger : ILog
    {
       public bool IsDebugEnabled => false;

       public void Debug(string message)
       {
           
       }

       public void Warn(string message)
       {
           
       }

       public void Warn(string message, Exception ex)
       {
          
       }

       public void Info(string message)
       {
          
       }

       public void Error(string message)
       {
           
       }

       public void Error(string message, Exception ex)
       {
          
       }
    }
}