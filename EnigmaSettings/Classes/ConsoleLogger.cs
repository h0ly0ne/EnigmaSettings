// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    public class ConsoleLogger : ILog
    {
        public void Debug(string message)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }

        public void Warn(string message)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }

        public void Warn(string message, Exception ex)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
                if (ex != null)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Info(string message)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Error(string message)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }

        public void Error(string message, Exception ex)
        {
            try
            {
                if (message != null)
                {
                    Console.WriteLine(message);
                }
                if (ex != null)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }     
        }
    }
}
