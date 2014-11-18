// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.ComponentModel;
using System.IO;
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ISettingsIO : INotifyPropertyChanged, IEditableObject
    {
        #region Events

        /// <summary>
        ///     Raised when starting to load settings from disk
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<SettingsLoadingEventArgs> SettingsLoading;

        /// <summary>
        ///     Raised when settings loading finishes
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<SettingsLoadedEventArgs> SettingsLoaded;

        /// <summary>
        ///     Raised when starting to save settings to disk
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<SettingsSavingEventArgs> SettingsSaving;

        /// <summary>
        ///     Raised when settings save finishes
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        /// <summary>
        ///     Raised when starting to load file from disk
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<FileLoadingEventArgs> FileLoading;

        /// <summary>
        ///     Raised when file load finishes
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<FileLoadedEventArgs> FileLoaded;

        /// <summary>
        ///     Raised when starting to save file to disk
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<FileSavingEventArgs> FileSaving;

        /// <summary>
        ///     Raised when file save finishes
        /// </summary>
        /// <remarks></remarks>
        event EventHandler<FileSavedEventArgs> FileSaved;

        #endregion

        /// <summary>
        ///     Implementation of instance factory used to instatiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IInstanceFactory Factory { get; }

        /// <summary>
        ///     Log4Net logger to be used for log output
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        ILog Log { get; set; }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <returns></returns>
        /// <remarks></remarks>
        ISettings Load(string settingsFile);

        /// <summary>
        ///     Loads up and links all the settings data
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO">Implementation of reading/writing satellites file</param>
        /// <returns></returns>
        /// <remarks></remarks>
        ISettings Load(string settingsFile, IXmlSatellitesIO xmlSatellitesIO);

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory asynchronusly
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <returns></returns>
        /// <remarks></remarks>
        void LoadAsync(string settingsFile, AsyncCallback callback);


        /// <summary>
        ///     Loads up and links all the settings data asynchronusly
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO"></param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <returns></returns>
        /// <remarks></remarks>
        void LoadAsync(string settingsFile, IXmlSatellitesIO xmlSatellitesIO, AsyncCallback callback);


        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <remarks></remarks>
        void Save(string folder, ISettings settings);

        /// <summary>
        ///     Saves settings to disk
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="xmlSatellitesIO">Instance of satellite.xml writer implementation</param>
        /// <remarks></remarks>
        void Save(string folder, ISettings settings, IXmlSatellitesIO xmlSatellitesIO);


        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer asynchronusly
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        /// <remarks></remarks>
        void SaveAsync(string folder, ISettings settings, AsyncCallback callback);

        /// <summary>
        ///     Saves settings to disk asynchronusly
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="xmlSatellitesIO">Instance of satellite.xml writer implementation</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        /// <remarks></remarks>
        void SaveAsync(string folder, ISettings settings, IXmlSatellitesIO xmlSatellitesIO, AsyncCallback callback);

    }
}