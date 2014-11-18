using System;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
   public class NullLogger : ILog
    {
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
