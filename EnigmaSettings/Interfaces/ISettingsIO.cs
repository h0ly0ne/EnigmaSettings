// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.ComponentModel;

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
        ///     Implementation of instance factory used to instantiate objects
        /// </summary>
        /// <value></value>
        IInstanceFactory Factory { get; }

        /// <summary>
        ///     Logger to be used for log output
        /// </summary>
        /// <value></value>
        ILog Log { get; set; }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO and XmlCablesIO from Factory
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        ISettings Load(string settingsFile);

        /// <summary>
        ///     Loads up and links all the settings data
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO">Implementation of reading/writing satellites file</param>
        /// <param name="xmlCablesIO">Implementation of reading/writing cables file</param>
        ISettings Load(string settingsFile, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO);

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory asynchronously
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        void LoadAsync(string settingsFile, AsyncCallback callback);

        /// <summary>
        ///     Loads up and links all the settings data asynchronously
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO"></param>
        /// <param name="xmlCablesIO"></param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        void LoadAsync(string settingsFile, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, AsyncCallback callback);

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        void Save(string folder, ISettings settings, string strLocalFilenameOverride, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles);

        /// <summary>
        ///     Saves settings to disk
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="xmlSatellitesIO">Instance of satellites.xml writer implementation</param>
        /// <param name="xmlCablesIO">Instance of cables.xml writer implementation</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        void Save(string folder, ISettings settings, string strLocalFilenameOverride, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles);

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer asynchronously
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        void SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, AsyncCallback callback);

        /// <summary>
        ///     Saves settings to disk asynchronously
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="xmlSatellitesIO">Instance of satellites.xml writer implementation</param>
        /// <param name="xmlCablesIO">Instance of cables.xml writer implementation</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        void SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, AsyncCallback callback);
    }
}