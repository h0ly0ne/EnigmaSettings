// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public class DebugLogger : ILog
    {
        public void Debug(string message)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            } 
        }

        public void Warn(string message)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            } 
        }

        public void Warn(string message, Exception ex)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
                if (ex != null)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void Info(string message)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void Error(string message)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            } 
        }

        public void Error(string message, Exception ex)
        {
            try
            {
                if (message != null)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
                if (ex != null)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }     
        }
    }
}
