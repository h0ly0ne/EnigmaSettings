// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    /// <summary>
    ///     Error handling of the public entry points (SettingsIO.Load / Save and the public ISettings methods):
    ///     every exception leaves as a SettingsException and is logged exactly once, by the outermost entry point
    ///     of the current call (public methods calling each other or Load calling the match methods log once).
    ///     Argument checks of the public methods run before the boundary, so ArgumentExceptions of the caller
    ///     are not wrapped.
    /// </summary>
    internal static class ErrorBoundary
    {
        [ThreadStatic]
        private static int _depth;

        public static void Run(ILog log, string message, Action action)
        {
            Run(log, message, () =>
            {
                action();
                return 0;
            });
        }

        public static T Run<T>(ILog log, string message, Func<T> func)
        {
            _depth++;
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                var settingsException = ex as SettingsException ?? new SettingsException(message, ex);

                if (_depth == 1)
                    log.Error(settingsException.Message, ex);

                if (ReferenceEquals(settingsException, ex))
                    throw;

                throw settingsException;
            }
            finally
            {
                _depth--;
            }
        }
    }
}
