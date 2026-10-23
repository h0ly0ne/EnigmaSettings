// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Krkadoni.EnigmaSettings.Interfaces;
using static Krkadoni.EnigmaSettings.Enums;

namespace Krkadoni.EnigmaSettings
{
    public partial class SettingsIO : ISettingsIO
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region "IEditable"
        private bool _isEditing;
        private string _mEditorName;
        private ILog _mLog;

        public void BeginEdit()
        {
            if (_isEditing) return;
            _mEditorName = _editorName;
            _mLog = _log;
            _isEditing = true;
        }

        public void EndEdit()
        {
            _isEditing = false;
        }

        public void CancelEdit()
        {
            if (!_isEditing) return;
            EditorName = _mEditorName;
            Log = _mLog;
            _isEditing = false;
        }
        #endregion

        protected const string SettingsFirstLine = "eDVB services /";
        protected const string SettingsTransponderOpenTag = "transponders";
        protected const string SettingsClosingTag = "end";
        protected const string SettingsServicesOpenTag = "services";
        protected const string BouquetsOpenTag = "bouquets";
        protected const string SatelliteXmlFileName = "satellites.xml";
        protected const string CablesXmlFileName = "cables.xml";
        protected const string UserBouquetTvEpl = "userbouquets.tv.epl";
        protected const string UserBouquetRadioEpl = "userbouquets.radio.epl";
        protected const string BouquetsFile = "bouquets";
        protected const string ServicesLockedFile = "services.locked";
        protected const string BouquetsTvFile = "bouquets.tv";
        protected const string BouquetsRadioFile = "bouquets.radio";
        protected const string WhiteListFile = "whitelist";
        protected const string BlackListFile = "blacklist";
        protected const string DefaultEnigma1Path = "/var/tuxbox/config/enigma/";

        private static readonly HashSet<string> _streamFavoritesTypes = ["4097", "5001", "5002", "8193", "8739"];
        // Enigma reads these files line by line, a BOM would corrupt the first entry
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
        private static readonly HashSet<int> DirectoryLineSpecifiers = [1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 519];
        private static readonly string FavoritesTypeFile = ((int)Enums.FavoritesType.File).ToString(CultureInfo.InvariantCulture);
        private static readonly string FavoritesTypeUnknown = ((int)Enums.FavoritesType.Unknown).ToString(CultureInfo.InvariantCulture);

        #region "Events"
        /// <summary>
        ///     Raised when file load finishes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<FileLoadedEventArgs> FileLoaded;

        /// <summary>
        ///     Raised when starting to load file from disk
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<FileLoadingEventArgs> FileLoading;

        /// <summary>
        ///     Raised when file save finishes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<FileSavedEventArgs> FileSaved;

        /// <summary>
        ///     Raised when starting to save file to disk
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<FileSavingEventArgs> FileSaving;

        /// <summary>
        ///     Raised when settings loading finishes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<SettingsLoadedEventArgs> SettingsLoaded;

        /// <summary>
        ///     Raised when starting to load settings from disk
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<SettingsLoadingEventArgs> SettingsLoading;

        /// <summary>
        ///     Raised when settings save finishes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        /// <summary>
        ///     Raised when starting to save settings to disk
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<SettingsSavingEventArgs> SettingsSaving;

        /// <summary>
        ///     Raises FileLoaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="success"></param>
        /// <remarks></remarks>
        protected void OnFileLoaded(object sender, string file, bool success)
        {
            var e = new FileLoadedEventArgs(file, success);
            var handler = FileLoaded;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises FileLoading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnFileLoading(object sender, string file)
        {
            var e = new FileLoadingEventArgs(file);
            var handler = FileLoading;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises  FileSaved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="success"></param>
        /// <remarks></remarks>
        protected void OnFileSaved(object sender, string file, bool success)
        {
            var e = new FileSavedEventArgs(file, success);
            var handler = FileSaved;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises FileSaving event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnFileSaving(object sender, string file)
        {
            var e = new FileSavingEventArgs(file);
            var handler = FileSaving;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises SettingsLoaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsLoaded(object sender, string file, bool success, ISettings settings)
        {
            var e = new SettingsLoadedEventArgs(file, success, settings);
            var handler = SettingsLoaded;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises SettingsLoading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnSettingsLoading(object sender, string file)
        {
            var e = new SettingsLoadingEventArgs(file);
            var handler = SettingsLoading;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises SettingsSaved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="folder"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsSaved(object sender, string folder, bool success, ISettings settings)
        {
            var e = new SettingsSavedEventArgs(folder, success, settings);
            var handler = SettingsSaved;
            handler?.Invoke(sender, e);
        }

        /// <summary>
        ///     Raises SettingsSaving event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="folder"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsSaving(object sender, string folder, ISettings settings)
        {
            var e = new SettingsSavingEventArgs(folder, settings);
            var handler = SettingsSaving;
            handler?.Invoke(sender, e);
        }
        #endregion

        #region "Properties"
        private static readonly ILog NullLogger = new NullLogger();
        protected readonly IFileHelper _fileHelper;

        private string _editorName = "Generated by EnigmaSettings - http://www.krkadoni.com";
        private ILog _log;

        public string EditorName
        {
            get => _editorName;
            set
            {
                value ??= string.Empty;

                if (value == _editorName) 
                    return;

                _editorName = value;
                OnPropertyChanged(nameof(EditorName));
            }
        }

        /// <summary>
        ///     Implementation of instance factory used to instantiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IInstanceFactory Factory { get; }

        /// <summary>
        ///     Logger to be used for log output
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ILog Log
        {
            get => _log ?? NullLogger;
            set
            {
                if (value == null)
                    return;

                if (value == _log)
                    return;

                _log = value;
                OnPropertyChanged(nameof(Log));
            }
        }
        #endregion

        #region "Public Methods"
        /// <summary>
        ///     Uses custom instance factory implementation to instantiate objects
        /// </summary>
        /// <param name="factory"></param>
        public SettingsIO(IInstanceFactory factory) : this(factory, new FileHelper())
        {
        }

        /// <summary>
        ///     Uses custom instance factory and file access implementation (e.g. for in-memory or remote file systems)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="fileHelper">File access used for all settings, bouquet and xml files</param>
        public SettingsIO(IInstanceFactory factory, IFileHelper fileHelper)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory), Resources.SettingsIO_New_Invalid_instance_factory_);
            _fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper), Resources.XmlSatellitesIO_XmlSatellitesIO_Invalid_file_provider);
        }

        /// <summary>
        ///     Uses default InstanceFactory to instantiate objects
        /// </summary>
        public SettingsIO() : this(new InstanceFactory(), new FileHelper())
        {
        }

        /// <summary>
        ///     File access implementation used for reading and writing
        /// </summary>
        public IFileHelper FileProvider => _fileHelper;

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO, XmlCableIO from Factory
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        public ISettings Load(string settingsFile)
        {
            IXmlSatellitesIO satellitesIO = Factory.InitNewXmlSatelliteIO(_fileHelper);
            IXmlCablesIO cablesIO = Factory.InitNewXmlCableIO(_fileHelper);
            return Load(settingsFile, satellitesIO, cablesIO);
        }

        /// <summary>
        ///     Loads up and links all the settings data
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="satellitesIO">Implementation of reading/writing satellites file</param>
        /// <param name="cablesIO">Implementation of reading/writing cables file</param>
        public ISettings Load(string settingsFile, IXmlSatellitesIO satellitesIO, IXmlCablesIO cablesIO)
        {
            try
            {
                if (settingsFile == null)
                {
                    Log.Error(Resources.Settings_LoadSettings_Invalid_filename_);
                    throw new SettingsException(Resources.Settings_LoadSettings_Invalid_filename_);
                }

                if (!_fileHelper.Exists(settingsFile))
                {
                    Log.Info(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, settingsFile));
                    throw new SettingsException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, settingsFile));
                }

                OnSettingsLoading(this, settingsFile);

                Log.Info(string.Format(Resources.Settings_Load_Loading_settings_from__0_, settingsFile));

                var st = new Stopwatch();
                st.Start();

                ISettings settings = ReadSettingsFile(settingsFile);
                settings.SettingsFileName = settingsFile;
                settings.MatchServicesWithTransponders();


                if (_fileHelper.Exists(Path.Combine(settings.SettingsDirectory, SatelliteXmlFileName)))
                {
                    IList<IXmlSatellite> sats;
                    try
                    {
                        sats = satellitesIO.LoadSatellitesFromFile(Path.Combine(settings.SettingsDirectory, SatelliteXmlFileName)).ToList();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"There was an error while reading {SatelliteXmlFileName} file.", ex);
                        sats = new List<IXmlSatellite>();
                    }

                    foreach (var sat in sats)
                    {
                        settings.Satellites.Add(sat);
                    }

                    settings.MatchSatellitesWithTransponders();
                    settings.AddMissingXmlSatellites(Factory);
                }

                if (_fileHelper.Exists(Path.Combine(settings.SettingsDirectory, CablesXmlFileName)))
                {
                    IList<IXmlCable> cabs;
                    try
                    {
                        cabs = cablesIO.LoadCablesFromFile(Path.Combine(settings.SettingsDirectory, CablesXmlFileName)).ToList();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"There was an error while reading {CablesXmlFileName} file.", ex);
                        cabs = new List<IXmlCable>();
                    }

                    foreach (var cab in cabs)
                    {
                        settings.Cables.Add(cab);
                    }
                }

                _bouquetReadContext = new BouquetReadContext();

                if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                {
                    ReadFileBouquet(Path.Combine(settings.SettingsDirectory, UserBouquetTvEpl), ref settings);
                    ReadFileBouquet(Path.Combine(settings.SettingsDirectory, UserBouquetRadioEpl), ref settings);
                    if (_fileHelper.Exists(Path.Combine(settings.SettingsDirectory, BouquetsFile)))
                        ReadE1Bouquets(Path.Combine(settings.SettingsDirectory, BouquetsFile), ref settings);
                    settings.MatchBouquetsBouquetsServices();
                    settings.MatchFileBouquetsServices();
                    if (_fileHelper.Exists(Path.Combine(settings.SettingsDirectory, ServicesLockedFile)))
                        ReadServicesLocked(Path.Combine(settings.SettingsDirectory, ServicesLockedFile), ref settings);
                }
                else if (settings.SettingsVersion is Enums.SettingsVersion.Enigma2Ver3 or Enums.SettingsVersion.Enigma2Ver4 or Enums.SettingsVersion.Enigma2Ver5)
                {
                    ReadFileBouquet(Path.Combine(settings.SettingsDirectory, BouquetsTvFile), ref settings);
                    ReadFileBouquet(Path.Combine(settings.SettingsDirectory, BouquetsRadioFile), ref settings);
                    settings.MatchFileBouquetsServices();
                    ReadBlackWhiteList(Path.Combine(settings.SettingsDirectory, WhiteListFile), ref settings);
                    ReadBlackWhiteList(Path.Combine(settings.SettingsDirectory, BlackListFile), ref settings);
                }

                st.Stop();
                Log.Info($"Settings loaded in {st.ElapsedMilliseconds} ms");
                OnSettingsLoaded(this, settingsFile, true, settings);
                return settings;
            }
            catch (SettingsException ex)
            {
                OnSettingsLoaded(this, settingsFile, false, null);
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                OnSettingsLoaded(this, settingsFile, false, null);
                Log.Error(string.Format(Resources.Settings_Load_Failed_to_load_settings_from__0_, settingsFile), ex);
                throw new SettingsException(string.Format(Resources.Settings_Load_Failed_to_load_settings_from__0_, settingsFile), ex);
            }
            finally
            {
                _bouquetReadContext = null;
            }
        }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory asynchronously
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <remarks>The <see cref="IAsyncResult"/> passed to the callback is a <see cref="Task{ISettings}"/>.</remarks>
        public void LoadAsync(string settingsFile, AsyncCallback callback)
        {
            WithCallback(LoadAsync(settingsFile, CancellationToken.None), callback);
        }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO, XmlCableIO from Factory on a background thread
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="cancellationToken">Cancels the load before it starts</param>
        public Task<ISettings> LoadAsync(string settingsFile, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Load(settingsFile), cancellationToken);
        }

        /// <summary>
        ///     Loads up and links all the settings data on a background thread
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO">Implementation of reading/writing satellites file</param>
        /// <param name="xmlCablesIO">Implementation of reading/writing cables file</param>
        /// <param name="cancellationToken">Cancels the load before it starts</param>
        public Task<ISettings> LoadAsync(string settingsFile, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Load(settingsFile, xmlSatellitesIO, xmlCablesIO), cancellationToken);
        }

        private static void WithCallback(Task task, AsyncCallback callback)
        {
            if (callback != null)
                task.ContinueWith(t => callback(t), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        /// <summary>
        ///     Loads up and links all the settings data asynchronously
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO"></param>
        /// <param name="xmlCablesIO"></param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <remarks>The <see cref="IAsyncResult"/> passed to the callback is a <see cref="Task{ISettings}"/>.</remarks>
        public void LoadAsync(string settingsFile, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, AsyncCallback callback)
        {
            WithCallback(LoadAsync(settingsFile, xmlSatellitesIO, xmlCablesIO, CancellationToken.None), callback);
        }

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml and cables.xml writer
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <remarks></remarks>
        public void Save(string folder, ISettings settings, string strLocalFilenameOverride, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles)
        {
            IXmlSatellitesIO satellitesIO = Factory.InitNewXmlSatelliteIO(_fileHelper);
            IXmlCablesIO cablesIO = Factory.InitNewXmlCableIO(_fileHelper);
            Save(folder, settings, strLocalFilenameOverride, satellitesIO, cablesIO, bLocalEnableDatabaseFile, bLocalEnableSatellites, bLocalEnableCables, bLocalEnableBouquetFiles);
        }

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml and cables.xml writer and is controlled by specific file save parameters
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="xmlSatellitesIO">Instance of satellites.xml writer implementation</param>
        /// <param name="xmlCablesIO">Instance of cables.xml writer implementation</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file saving</param>
        /// <param name="bLocalEnableSatellitesFile">Indicates whether to enable satellites file saving</param>
        /// <param name="bLocalEnableCablesFile">Indicates whether to enable cables file saving</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <remarks></remarks>
        public void Save(string folder, ISettings settings, string strLocalFilenameOverride, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellitesFile, bool bLocalEnableCablesFile, bool bLocalEnableBouquetFiles)
        {
            ArgumentNullException.ThrowIfNull(folder);
            ArgumentNullException.ThrowIfNull(settings);
            xmlSatellitesIO ??= Factory.InitNewXmlSatelliteIO(_fileHelper);
            xmlCablesIO ??= Factory.InitNewXmlCableIO(_fileHelper);

            Log.Info($"Saving settings version {settings.SettingsVersion} to {folder}");
            OnSettingsSaving(this, folder, settings);

            if (!_fileHelper.FolderExists(folder))
            {
                try
                {
                    _fileHelper.CreateFolder(folder);
                    if (Log.IsDebugEnabled) Log.Debug($"Folder {folder} successfully created");
                }
                catch (Exception ex)
                {
                    OnSettingsSaved(this, folder, false, settings);
                    Log.Error(string.Format(Resources.SettingsIO_Save_There_was_an_error_creating_folder__0_, folder), ex);
                    throw new SettingsException(string.Format(Resources.SettingsIO_Save_There_was_an_error_creating_folder__0_, folder), ex);
                }
            }
            else
            {
                if (Log.IsDebugEnabled) Log.Debug($"Folder {folder} exists");
            }

            var st = new Stopwatch();
            st.Start();

            try
            {
                if (settings.FindBouquetsWithDuplicateFilename().Any())
                    throw new SettingsException(Resources.SettingsIO_Save_Bouquet_filenames_are_not_unique_);

                if (bLocalEnableDatabaseFile)
                    WriteSettingsFile(folder, settings, strLocalFilenameOverride);

                if (bLocalEnableBouquetFiles)
                    WriteBouquets(folder, settings);

                if (bLocalEnableSatellitesFile)
                    xmlSatellitesIO.SaveSatellitesToFile(Path.Combine(folder, SatelliteXmlFileName), settings);

                if (bLocalEnableCablesFile)
                    xmlCablesIO.SaveCablesToFile(Path.Combine(folder, CablesXmlFileName), settings);

                st.Stop();
                Log.Info($"Settings saved successfully in {st.ElapsedMilliseconds} ms.");
                OnSettingsSaved(this, folder, true, settings);
            }
            catch (SettingsException ex)
            {
                OnSettingsSaved(this, folder, false, settings);
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                OnSettingsSaved(this, folder, false, settings);
                Log.Error(string.Format(Resources.SettingsIO_Save_Failed_to_save_settings_to__0_, folder), ex);
                throw new SettingsException(string.Format(Resources.SettingsIO_Save_Failed_to_save_settings_to__0_, folder), ex);
            }
        }

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer and
        ///     initializes default cables.xml writer and asynchronously
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        /// <remarks></remarks>
        /// <remarks>The <see cref="IAsyncResult"/> passed to the callback is a <see cref="Task"/>.</remarks>
        public void SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, AsyncCallback callback)
        {
            WithCallback(SaveAsync(folder, settings, strLocalFilenameOverride, bLocalEnableDatabaseFile, bLocalEnableSatellites, bLocalEnableCables, bLocalEnableBouquetFiles, CancellationToken.None), callback);
        }

        /// <summary>
        ///     Saves settings to disk on a background thread, initializes default satellites.xml and cables.xml writer
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="strLocalFilenameOverride">Override for the local filename</param>
        /// <param name="bLocalEnableDatabaseFile">Indicates whether to enable database file</param>
        /// <param name="bLocalEnableSatellites">Indicates whether to enable satellites</param>
        /// <param name="bLocalEnableCables">Indicates whether to enable cables</param>
        /// <param name="bLocalEnableBouquetFiles">Indicates whether to enable bouquet file saving</param>
        /// <param name="cancellationToken">Cancels the save before it starts</param>
        public Task SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Save(folder, settings, strLocalFilenameOverride, bLocalEnableDatabaseFile, bLocalEnableSatellites, bLocalEnableCables, bLocalEnableBouquetFiles), cancellationToken);
        }

        /// <summary>
        ///     Saves settings to disk on a background thread
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
        /// <param name="cancellationToken">Cancels the save before it starts</param>
        public Task SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Save(folder, settings, strLocalFilenameOverride, xmlSatellitesIO, xmlCablesIO, bLocalEnableDatabaseFile, bLocalEnableSatellites, bLocalEnableCables, bLocalEnableBouquetFiles), cancellationToken);
        }

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
        /// <remarks></remarks>
        public void SaveAsync(string folder, ISettings settings, string strLocalFilenameOverride, IXmlSatellitesIO xmlSatellitesIO, IXmlCablesIO xmlCablesIO, bool bLocalEnableDatabaseFile, bool bLocalEnableSatellites, bool bLocalEnableCables, bool bLocalEnableBouquetFiles, AsyncCallback callback)
        {
            WithCallback(SaveAsync(folder, settings, strLocalFilenameOverride, xmlSatellitesIO, xmlCablesIO, bLocalEnableDatabaseFile, bLocalEnableSatellites, bLocalEnableCables, bLocalEnableBouquetFiles, CancellationToken.None), callback);
        }
        #endregion

        #region "Protected methods"
        #region "Utils"
        /// <summary>
        ///     Reads file content into string array
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="warnEmpty"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if fileName is null </exception>
        /// <exception cref="ArgumentException">Throws ArgumentException if file does not exist</exception>
        protected virtual string[] Read(string fileName, bool warnEmpty = false)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName), Resources.Settings_Read_Filename_cannot_be_empty_);
    
            if (!_fileHelper.Exists(fileName))
               throw new ArgumentException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fileName));

            OnFileLoading(this, fileName);
            if (Log.IsDebugEnabled) Log.Debug(string.Format(Resources.Settings_Read_Reading_file__0_, fileName));

            var st = new Stopwatch();
            string[] fileLines;
            st.Start();

            try
            {
                fileLines = _fileHelper.ReadAllLines(fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileLoaded(this, fileName, false);
                throw new SettingsException(string.Format(Resources.Settings_Read_Failed_to_read_file__0__1__2_, fileName, "", ""), ex);
            }

            st.Stop();

            if (Log.IsDebugEnabled) Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName, st.ElapsedMilliseconds));
           // Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName, "?"));

            if (fileLines.Length == 0)
            {
                if (warnEmpty)
                    Log.Warn(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fileName));
            }

            OnFileLoaded(this, fileName, true);
            return fileLines;
        }

        /// <summary>
        ///     Reads file content into string array
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="warnEmpty"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if fileName is null </exception>
        /// <exception cref="ArgumentException">Throws ArgumentException if file does not exist</exception>
        protected virtual string ReadText(string fileName, bool warnEmpty = true)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName), Resources.Settings_Read_Filename_cannot_be_empty_);

            if (!_fileHelper.Exists(fileName))
                throw new ArgumentException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fileName));
            OnFileLoading(this, fileName);
            if (Log.IsDebugEnabled) Log.Debug(string.Format(Resources.Settings_Read_Reading_file__0_, fileName));

            var st = new Stopwatch();
            string fileText;
            st.Start();

            try
            {
                fileText = _fileHelper.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileLoaded(this, fileName, false);
                throw new SettingsException(string.Format(Resources.Settings_Read_Failed_to_read_file__0__1__2_, fileName, "", ""), ex);
            }

            st.Stop();

            if (Log.IsDebugEnabled) Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName, st.ElapsedMilliseconds));
            //Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName, "?"));

            if (fileText.Length == 0)
            {
                if (warnEmpty)
                    Log.Warn(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fileName));
            }
            OnFileLoaded(this, fileName, true);
            return fileText;
        }

        /// <summary>
        ///     Checks if file exists on disk, if it does tries to delete it
        /// </summary>
        /// <param name="fileName"></param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if unable to delete existing file</exception>
        protected virtual void DeleteFileIfExists(string fileName)
        {
            if (_fileHelper.Exists(fileName))
            {
                try
                {
                    _fileHelper.Delete(fileName);
                    if (Log.IsDebugEnabled) Log.Debug($"Existing file {fileName} successfully deleted");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                    throw new SettingsException(
                        string.Format(Resources.SettingsIO_DeleteFileIfExists_File__0__already_exists_and_could_not_be_deleted_, fileName), ex);
                }
            }
        }

        /// <summary>
        ///     Writes text to specified file with specified encoding
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="fContent">Content of the file</param>
        /// <param name="encoding">Encoding used to write file to disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws settings exception if it fails</exception>
        protected virtual void WriteFile(string fileName, string fContent, Encoding encoding)
        {
            OnFileSaving(this, fileName);
            if (Log.IsDebugEnabled) Log.Debug($"Writing file {fileName} to disk");
            try
            {
                _fileHelper.WriteAllText(fileName, fContent, encoding);
                if (Log.IsDebugEnabled) Log.Debug($"File {fileName} successfully written.");
                OnFileSaved(this, fileName, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fileName, false);
                throw new SettingsException($"There was an error while writing file {fileName} to disk", ex);
            }
        }

        /// <summary>
        ///     Returns enum type for specified version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual Enums.SettingsVersion GetSettingsVersion(string version)
        {
            switch (version)
            {
                case "1":
                    return Enums.SettingsVersion.Enigma1Ver1;
                case "2":
                    return Enums.SettingsVersion.Enigma1Ver2;
                case "3":
                    return Enums.SettingsVersion.Enigma2Ver3;
                case "4":
                    return Enums.SettingsVersion.Enigma2Ver4;
                case "5":
                    return Enums.SettingsVersion.Enigma2Ver5;
                default:
                    return Enums.SettingsVersion.Unknown;
            }
        }

        /// <summary>True for lamedb formats that carry the extended DVB-S2/T2 fields (v4 and v5).</summary>
        protected static bool IsVer4OrLater(Enums.SettingsVersion version)
        {
            return version is Enums.SettingsVersion.Enigma2Ver4 or Enums.SettingsVersion.Enigma2Ver5;
        }

        /// <summary>
        ///     Returns version number as seen in settings file for specified enum
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual int GetSettingsVersion(Enums.SettingsVersion version)
        {
            switch (version)
            {
                case Enums.SettingsVersion.Enigma1Ver1:
                    return 1;
                case Enums.SettingsVersion.Enigma1Ver2:
                    return 2;
                case Enums.SettingsVersion.Enigma2Ver3:
                    return 3;
                case Enums.SettingsVersion.Enigma2Ver4:
                    return 4;
                case Enums.SettingsVersion.Enigma2Ver5:
                    return 5;
                default:
                    return Convert.ToInt32(version, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        ///     Returns bouquet name for known bouquet types
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual string GetBouquetNameFromFileName(string fileName)
        {
            switch (fileName.ToLowerInvariant())
            {
                case BlackListFile:
                    return "blacklist";
                case WhiteListFile:
                    return "whitelist";
                case BouquetsRadioFile:
                    return "Bouquets (Radio)";
                case BouquetsTvFile:
                    return "Bouquets (TV)";
                case BouquetsFile:
                    return "bouquets";
                case UserBouquetRadioEpl:
                    return "User - bouquets (Radio)";
                case UserBouquetTvEpl:
                    return "User - bouquets (TV)";
                default:
                    if (fileName.ToLowerInvariant().EndsWith(".tv", StringComparison.Ordinal))
                        return "TV Bouquet";
                    
                    return fileName.ToLowerInvariant().EndsWith(".radio", StringComparison.Ordinal) ? "Radio Bouquet" : string.Empty;
            }
        }
        #endregion
        #region "Reading"
        /// <summary>
        ///     Reads file content from the disk, parses content, sets version number, initializes new lists of transponders and
        ///     services
        /// </summary>
        /// <param name="fileName">Full path to settings file on the disk</param>
        /// <remarks>Does not match transponders and services</remarks>
        protected virtual ISettings ReadSettingsFile(string fileName)
        {
            try
            {
                ISettings settings = Factory.InitNewSettings();
                settings.Log = Log;
                string[] settingsFileLines = Read(fileName);
                CheckIfSettingsValid(settingsFileLines, fileName);
                ReadSettingsVersion(settings, settingsFileLines, fileName);

                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma2Ver5)
                {
                    ReadTransponders(settings, settingsFileLines, fileName, out var transponderOpenIndex, out var transponderEndIndex);
                    ReadServices(settings, settingsFileLines, fileName, transponderOpenIndex, transponderEndIndex);
                }
                else
                    ReadLameDb5(settings, settingsFileLines, fileName);

                if (Log.IsDebugEnabled) Log.Debug($"Settings loaded with {settings.Services.Count} services and {settings.Transponders.Count} transponders");
                return settings;
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_There_was_an_error_while_reading_services_0__1_, Environment.NewLine, ex.Message), ex);
            }
        }

        /// <summary>
        ///     Checks if settings file appears to be valid Enigma settings file
        /// </summary>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">
        ///     Throws settings exception if settings file does not appear to be valid enigma
        ///     settings file
        /// </exception>
        protected virtual void CheckIfSettingsValid(string[] settingsFileLines, string fileName)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Checking {fileName} to make sure we have valid settings");
            var fName = Path.GetFileName(fileName);

            if (settingsFileLines == null || settingsFileLines.Length == 0)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fName));

            if (!settingsFileLines[0].StartsWith(SettingsFirstLine, StringComparison.OrdinalIgnoreCase) || !settingsFileLines[0].Trim().EndsWith('/'))
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fName));

            if (Log.IsDebugEnabled) Log.Debug($"File {fName} looks like a valid settings file, checking for version");
        }

        /// <summary>
        ///     Reads settings version and sets SettingsVersion property for specified settings instance
        /// </summary>
        /// <param name="settings">Instance of ISettings we're setting settings version to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if settings version can't be determined</exception>
        protected virtual void ReadSettingsVersion(ISettings settings, string[] settingsFileLines, string fileName)
        {
            Log.Debug("Looking for settings version");
            var fName = Path.GetFileName(fileName);
            var sVersion = settingsFileLines[0].ToLowerInvariant().Trim().Replace(SettingsFirstLine.ToLowerInvariant(), "").TrimEnd('/');

            if (!int.TryParse(sVersion, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fName));

            settings.SettingsVersion = GetSettingsVersion(sVersion);

            if (settings.SettingsVersion == Enums.SettingsVersion.Unknown)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_Unsuported_settings_version__0_, sVersion));
            
            if (Log.IsDebugEnabled) Log.Debug($"Found settings version {settings.SettingsVersion}");
        }

        /// <summary>
        ///     Reads transponders from specified lines and adds them to specified instance of ISettings
        /// </summary>
        /// <param name="settings">Instance of ISettings we're adding transponders to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <param name="transponderOpenIndex">Returns line number of transponder open tag</param>
        /// <param name="transponderEndIndex">Returns line number of transponder end tag</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if there's no transponder open tag</exception>
        protected virtual void ReadTransponders(ISettings settings, string[] settingsFileLines, string fileName, out int transponderOpenIndex, out int transponderEndIndex)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Reading transponders from {fileName}");

            //search for the beginning of transponder section
            transponderOpenIndex = -1;
            transponderEndIndex = -1;
            var sCount = settingsFileLines.Length;

            for (var i = 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsTransponderOpenTag) continue;
                transponderOpenIndex = i;
                break;
            }

            if (transponderOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_No__transponder__tag_was_found_in__0_, Path.GetFileName(fileName)));

            if (Log.IsDebugEnabled) Log.Debug($"Transponder section found at line {transponderOpenIndex + 1}");

            //reading transponders, each entry spans 3 lines (id, data, "/")
            for (var i = transponderOpenIndex + 1; i < sCount; i += 3)
            {
                if (settingsFileLines[i].Trim() != SettingsClosingTag)
                {
                    if (i + 1 >= sCount || settingsFileLines[i + 1].Trim().Length == 0)
                    {
                        Log.Warn($"Incomplete transponder entry at line {i + 1} ignored");
                        break;
                    }

                    switch (char.ToLowerInvariant(settingsFileLines[i + 1].Trim()[0]))
                    {
                        case 's':
                        {
                            // DVBT & DVBC transponders in Enigma1 are saved as satellite transponders
                            if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                            {
                                if (settingsFileLines[i].ToLowerInvariant().StartsWith("ffff", StringComparison.Ordinal))
                                    AddTransponderDVBC(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                else if (settingsFileLines[i].ToLowerInvariant().StartsWith("eeee", StringComparison.Ordinal))
                                    AddTransponderDVBT(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                else if (settingsFileLines[i].ToLowerInvariant().StartsWith("0000", StringComparison.Ordinal))
                                    AddTransponderIPTV(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                else
                                    AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            }
                            else
                            {
                                if (settingsFileLines[i].ToLowerInvariant().StartsWith("0000", StringComparison.Ordinal))
                                    AddTransponderIPTV(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                else
                                    AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            }

                            break;
                        }
                        case 't':
                        {
                            AddTransponderDVBT(settings, settingsFileLines[i], settingsFileLines[i + 1]);

                            break;
                        }
                        case 'c':
                        {
                            AddTransponderDVBC(settings, settingsFileLines[i], settingsFileLines[i + 1]);

                            break;
                        }
                        default:
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadSettingsFile_Unknown_transponder_type_for_transponder__0___1___adding_it_as_satellite_transponder, settingsFileLines[i], settingsFileLines[i + 1]));
                            AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);

                            break;
                        }
                    }
                }
                else
                {
                    //we have reached the end of transponders part
                    if (Log.IsDebugEnabled) Log.Debug($"Transponder section ends at line {i + 1}");
                    transponderEndIndex = i;
                    return;
                }
            }

            Log.Debug("Transponder section has no end tag");
        }

        /// <summary>
        ///     Reads services from specified lines and adds them to specified instance of ISettings
        /// </summary>
        /// <param name="settings">Instance of ISettings we're adding transponders to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <param name="transponderOpenIndex">Line number of transponder open tag</param>
        /// <param name="transponderEndIndex">Line number of transponder end tag</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if there's no services open tag</exception>
        protected virtual void ReadServices(ISettings settings, string[] settingsFileLines, string fileName, int transponderOpenIndex, int transponderEndIndex)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Reading services from {fileName}");

            //search for the beginning of services section after transponders section
            var servicesOpenIndex = -1;
            var sCount = settingsFileLines.Length;

            for (var i = transponderEndIndex + 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsServicesOpenTag)
                    continue;

                servicesOpenIndex = i;
                break;
            }

            if (servicesOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_No__services__tag_was_found_in_file__0_, Path.GetFileName(fileName)));

            if (Log.IsDebugEnabled) Log.Debug($"Services section found at line {servicesOpenIndex + 1}");

            //reading services, each entry spans 3 lines (id, name, flags)
            var debug = Log.IsDebugEnabled;
            for (int i = servicesOpenIndex + 1; i + 2 < sCount; i += 3)
            {
                if (settingsFileLines[i].Trim() == SettingsClosingTag)
                    break;

                var line1 = settingsFileLines[i];
                var line2 = settingsFileLines[i + 1];
                var line3 = settingsFileLines[i + 2];
                if (debug)
                    Log.Debug($"Initializing new service {line1}    {line2}    {line3}");
                settings.Services.Add(Factory.InitNewService(line1, line2, line3));
            }

            Log.Debug("Services read sucessfully");
        }

        /// <summary>
        ///     Initializes new DVBT transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBT(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new terrestrial transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBT(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new DVBC transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBC(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new cable transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBC(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new DVBS transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderDVBS(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new satellite transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderDVBS(firstLine, secondLine));
        }

        /// <summary>
        ///     Initializes new IPTV transponder and adds it to specified ISettings instance
        /// </summary>
        /// <param name="settings">ISettings instance we're adding transponder to</param>
        /// <param name="firstLine">First line of transponder data from settings file</param>
        /// <param name="secondLine">Second line of transponder data from settings file</param>
        /// <remarks></remarks>
        protected virtual void AddTransponderIPTV(ISettings settings, string firstLine, string secondLine)
        {
            if (Log.IsDebugEnabled) Log.Debug($"Initializing new iptv transponder {firstLine} {secondLine}");
            settings.Transponders.Add(Factory.InitNewTransponderIPTV(firstLine, secondLine));
        }

        /// <summary>
        ///     Reads bouquet from disk and parses content. Goes recursively if bouquet has other bouquets as items.
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>Nothing if bouquet type is unknown. Type is recognized from filename</returns>
        /// <remarks>Adds bouquet to Bouquets list if not already in it, otherwise updates existing</remarks>
        protected virtual IFileBouquet ReadFileBouquet(string fileName, ref ISettings settings)
        {
            bool ownsContext = _bouquetReadContext == null;
            if (ownsContext)
                _bouquetReadContext = new BouquetReadContext();

            string fullPath = Path.GetFullPath(fileName);
            try
            {
                // every bouquet file is read once per load: a second reference gets the same instance
                // and a reference back to a bouquet that is still being read (a cycle) is not followed
                if (_bouquetReadContext.Read.TryGetValue(fullPath, out var alreadyRead))
                {
                    if (!_bouquetReadContext.InProgress.Contains(fullPath))
                        return alreadyRead;

                    Log.Warn($"Bouquet {Path.GetFileName(fileName)} references itself (directly or through sub bouquets), reference ignored");
                    return null;
                }

                IFileBouquet bqt = Factory.InitNewFileBouquet();
                string fName = Path.GetFileName(fileName);
                bqt.FileName = fName;

                //do not read bouquets that have their own methods for reading
                if (bqt.BouquetType == Enums.BouquetType.E1Bouquets || string.Equals(fName, WhiteListFile, StringComparison.OrdinalIgnoreCase) || string.Equals(fName, BlackListFile, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fName, ServicesLockedFile, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Warn($"File {fName} is not a bouquet file, not reading it as bouquet");
                    return null;
                }

                //do not read unknown bouquet types
                if (bqt.BouquetType == Enums.BouquetType.Unknown)
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquets_File__0__is_unknown_bouquet_type, fName));
                    return null;
                }

                if (!_fileHelper.Exists(fileName))
                {
                    Log.Warn(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fName));
                    return null;
                }
                IFileBouquet existing =
                    settings.Bouquets.OfType<IFileBouquet>()
                        .FirstOrDefault(x => String.Equals(x.FileName, fName, StringComparison.Ordinal));
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    if (bqt.BouquetType != Enums.BouquetType.Unknown
                        && !string.Equals(fName, BouquetsTvFile, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, BouquetsRadioFile, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, UserBouquetTvEpl, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(fName, UserBouquetRadioEpl, StringComparison.OrdinalIgnoreCase)
                        && !fName.ToLowerInvariant().StartsWith("alternatives.", StringComparison.Ordinal))
                    {
                        settings.Bouquets.Add(bqt);
                    }
                }

                _bouquetReadContext.Read[fullPath] = bqt;
                _bouquetReadContext.InProgress.Add(fullPath);

                if (Log.IsDebugEnabled) Log.Debug($"Reading bouquet {bqt.FileName}");
                string[] bouquetLines = Read(fileName, true);

                if (bouquetLines == null || !bouquetLines.Any())
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_Bouquet__0__is_empty, bqt.FileName));
                }
                else
                {
                    string dir = Path.GetDirectoryName(fileName);
                    var cnt = bouquetLines.Length;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        string line = bouquetLines[i];
                        if (line.Trim().Length > 0)
                        {
                            DetectAndReadReference(dir, ref bqt, bouquetLines, i, settings);
                        }
                    }
                    if (Log.IsDebugEnabled) Log.Debug($"Bouquet {bqt.FileName} successfully parsed with {cnt} items");
                }
                return bqt;
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(string.Format(Resources.Settings_ReadBouquet_Failed_to_read_bouquet__0_, Path.GetFileName(fileName)), ex);
            }
            finally
            {
                _bouquetReadContext?.InProgress.Remove(fullPath);
                if (ownsContext)
                    _bouquetReadContext = null;
            }
        }

        /// <summary>
        ///     Bouquet files read during the current load (per thread, so parallel loads don't interfere)
        /// </summary>
        private sealed class BouquetReadContext
        {
            public readonly Dictionary<string, IFileBouquet> Read = new(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> InProgress = new(StringComparer.OrdinalIgnoreCase);
            public readonly HashSet<string> Listed = new(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     True for the files that list the top level bouquets (bouquets.tv/.radio, userbouquets.*.epl)
        /// </summary>
        protected static bool IsBouquetList(string fileName)
        {
            string name = Path.GetFileName(fileName);
            return string.Equals(name, BouquetsTvFile, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, BouquetsRadioFile, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, UserBouquetTvEpl, StringComparison.OrdinalIgnoreCase)
                   || string.Equals(name, UserBouquetRadioEpl, StringComparison.OrdinalIgnoreCase);
        }

        [ThreadStatic]
        private static BouquetReadContext _bouquetReadContext;

        /// <summary>
        ///     Determines type of line and initializes corresponding bouquet item type
        /// </summary>
        /// <param name="folder">Directory with all the settings files</param>
        /// <param name="bouquet">Parent bouquet that this item belongs to</param>
        /// <param name="bouquetLines">Bouquet file content as string array</param>
        /// <param name="index">Index of the current bouquet line</param>
        /// <param name="settings">Reference to existing settings instance</param>
        /// <remarks></remarks>
        protected virtual void DetectAndReadReference(string folder, ref IFileBouquet bouquet, string[] bouquetLines, int index, ISettings settings)
        {
            string line = bouquetLines[index];
            string description = string.Empty;
            int bCount = bouquetLines.Length;
            string nextLine = index < (bCount - 1) ? bouquetLines[index + 1] : string.Empty;
            if (nextLine.ToUpperInvariant().StartsWith("#DESCRIPTION", StringComparison.Ordinal))
                description = bouquetLines[index + 1];

            if (line.StartsWith("#NAME", StringComparison.Ordinal))
            {
                bouquet.Name = line.Substring(line.Length >= 6 ? 6 : 5);
                if (Log.IsDebugEnabled) Log.Debug($"Found bouquet NAME: {line}");
            }
            else if (line.StartsWith("#SERVICE", StringComparison.Ordinal))
            {
                string serviceData = line.Length > 9 ? line.Substring(9).Trim() : string.Empty;
                string[] sData = serviceData.Split(':');

                if (sData.Length < 2 || !int.TryParse(sData[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var lineSpecifier))
                {
                    Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, Path.Combine(folder, bouquet.Name ?? string.Empty)));
                    return;
                }

                string favoritesType = sData[0];
                var lineType = (Enums.LineSpecifier)lineSpecifier;
                bool isDirectory = DirectoryLineSpecifiers.Contains(lineSpecifier) || favoritesType == FavoritesTypeFile;

                if (lineType == Enums.LineSpecifier.Unknown || favoritesType == FavoritesTypeUnknown)
                    Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, Path.Combine(folder, bouquet.Name ?? string.Empty)));
                else if ((isDirectory || lineType == Enums.LineSpecifier.Alternative) && sData.Length < 11)
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_Unsupported_line___1___in_bouquet__0_, Path.GetFileName(bouquet.FileName), line));
                else if (isDirectory)
                {
                    if (Log.IsDebugEnabled) Log.Debug($"Found (sub)directory item {sData[10]}");
                    var tmpName = sData[10];

                    //workaround for FROM BOUQUET "userbouquet.dbe01.radio" ORDER BY bouquet
                    if (tmpName.IndexOf('"') > -1)
                    {
                        Log.Debug("Non standard bouquet name, try to workaround it");
                        tmpName = tmpName.Substring(tmpName.IndexOf('"') + 1);
                        tmpName = tmpName.Substring(0, tmpName.IndexOf('"'));
                        tmpName = tmpName.Trim();
                        sData[10] = tmpName;
                        if (Log.IsDebugEnabled) Log.Debug($"Workaround completed with result {tmpName}");
                    }
                    string fName = Path.GetFileName(sData[10]);

                    IFileBouquet subBqt = ReadFileBouquet(Path.Combine(folder, fName), ref settings);

                    if (subBqt != null)
                    {
                        if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                            subBqt.FileName = sData[10];

                        if (lineType == Enums.LineSpecifier.Hidden)
                            subBqt.Hidden = true;

                        // listed in bouquets.tv / .radio (or E1 userbouquets.*.epl) wins over being referenced
                        // from another bouquet, whichever is read first
                        string subPath = Path.GetFullPath(Path.Combine(folder, fName));
                        if (IsBouquetList(bouquet.FileName))
                        {
                            subBqt.IsSubBouquet = false;
                            _bouquetReadContext?.Listed.Add(subPath);
                        }
                        else if (_bouquetReadContext == null || !_bouquetReadContext.Listed.Contains(subPath))
                        {
                            subBqt.IsSubBouquet = true;
                        }

                        IBouquetItemFileBouquet bib = Factory.InitNewBouquetItemFileBouquet(subBqt);
                        bib.FavoritesTypeFlag = sData[0];
                        bib.LineSpecifierFlag = sData[1];
                        bouquet.BouquetItems.Add(bib);
                        if (Log.IsDebugEnabled) Log.Debug($"Bouquet {Path.GetFileName(sData[10])} added as bouquet item to bouquet {bouquet.Name} ");
                    }
                    else
                    {
                        Log.Warn(string.Format(Resources.Settings_ReadBouquet_Bouquet__0__in_bouquet__1__is_of_unknown_type, Path.GetFileName(sData[10]),
                            bouquet.Name));
                    }
                }
                else
                    switch (lineType)
                    {
                        case Enums.LineSpecifier.Marker:
                            {
                                IBouquetItemMarker bqim = ReadMarkerReference(line, description);

                                if (bqim != null)
                                {
                                    bouquet.BouquetItems.Add(bqim);
                                    if (Log.IsDebugEnabled) Log.Debug($"Marker {bqim.Description} added to bouquet {bouquet.Name}");
                                }
                                else
                                    Log.Warn($"Invalid marker {line} not added to bouquet {bouquet.Name}");
                            }
                            break;
                        case Enums.LineSpecifier.Space:
                            {
                                IBouquetItemMarker bqim = ReadMarkerReference(line, description);

                                if (bqim != null)
                                {
                                    bouquet.BouquetItems.Add(bqim);
                                    if (Log.IsDebugEnabled) Log.Debug($"Spacer entry preserved in bouquet {bouquet.Name}");
                                }
                            }
                            break;
                        case Enums.LineSpecifier.Alternative:
                            {
                                var altTmp = sData[10];

                                if (altTmp.IndexOf('"') > -1)
                                {
                                    altTmp = altTmp.Substring(altTmp.IndexOf('"') + 1);
                                    altTmp = altTmp.Substring(0, altTmp.IndexOf('"')).Trim();
                                }

                                string altFile = Path.GetFileName(altTmp);

                                IFileBouquet altBqt = ReadFileBouquet(Path.Combine(folder, altFile), ref settings);

                                if (altBqt != null)
                                {
                                    IBouquetItemAlternative bia = Factory.InitNewBouquetItemAlternative(altBqt);
                                    bia.FavoritesTypeFlag = sData[0];
                                    bia.LineSpecifierFlag = sData[1];
                                    bouquet.BouquetItems.Add(bia);
                                    if (Log.IsDebugEnabled) Log.Debug($"Alternative {altFile} added to bouquet {bouquet.Name}");
                                }
                                else
                                    Log.Warn($"Alternative {altFile} in bouquet {bouquet.Name} could not be read");
                            }
                            break;
                        case Enums.LineSpecifier.NormalService:
                            if (_streamFavoritesTypes.Contains(favoritesType) || (sData.Length > 10 && sData[10].Contains("//", StringComparison.Ordinal)) || sData.Length == 12)
                            {
                                //it is a stream
                                IBouquetItemStream bqis = ReadStreamReference(line, description);
                                if (bqis != null)
                                {
                                    bouquet.BouquetItems.Add(bqis);
                                    if (description.Length == 0)
                                       Log.Warn(string.Format(Resources.Settings_ReadBouquet_Stream_service__1__added_to_bouquet__0__without_description, bouquet.Name, bqis.Description));
                                    else
                                        if (Log.IsDebugEnabled) Log.Debug($"Stream service {bqis.Description} added to bouquet {bouquet.Name}");
                                }
                                else
                                    Log.Warn(string.Format(Resources.SettingsIO_ReadFileBouquet_Invalid_stream_reference__0__not_added_to_bouquet__1_, line, bouquet.Name));
                            }
                            else
                            {
                                //it is a regular DVB service
                                IBouquetItemService bqis = ReadServiceReference(line);
                                if (bqis != null)
                                {
                                    bqis.Description = StripDescriptionTag(description);
                                    bouquet.BouquetItems.Add(bqis);
                                    if (Log.IsDebugEnabled) Log.Debug($"DVB service {serviceData} added to bouquet {bouquet.Name}");
                                }
                                else
                                    Log.Warn(string.Format(Resources.SettingsIO_ReadFileBouquet_Invalid_service_reference__0__not_added_to_bouquet__1_, line, bouquet.Name));
                            }
                            break;
                    }
            }
            else if (line.StartsWith("#TYPE", StringComparison.Ordinal))
            {
            }
            else if (line.StartsWith("#DESCRIPTION", StringComparison.Ordinal))
            {
            }
            else if (line.StartsWith('/'))
            {
            }
            else
                Log.Warn(string.Format(Resources.Settings_ReadBouquet_Unsupported_line___1___in_bouquet__0_, Path.GetFileName(bouquet.FileName), line));
        }

        /// <summary>
        ///     Text of a #DESCRIPTION line without the tag (with or without colon)
        /// </summary>
        protected static string StripDescriptionTag(string descriptionLine)
        {
            if (string.IsNullOrEmpty(descriptionLine))
                return string.Empty;

            if (descriptionLine.StartsWith("#DESCRIPTION:", StringComparison.OrdinalIgnoreCase))
                return descriptionLine.Substring(13).Trim();

            return descriptionLine.StartsWith("#DESCRIPTION", StringComparison.OrdinalIgnoreCase) ? descriptionLine.Substring(12).Trim() : descriptionLine.Trim();
        }

        /// <summary>
        ///     Initializes new IBouquetItemService object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemService ReadServiceReference(string bouquetLine)
        {
            try
            {
                if (bouquetLine.StartsWith("#SERVICE", StringComparison.Ordinal))
                    bouquetLine = bouquetLine.Substring(9).Trim();

                return Factory.InitNewBouquetItemService(bouquetLine);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(string.Format(Resources.SettingsIO_ReadServiceReference_Failed_to_initialize_new_service_from_line__0_, bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemMarker object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <param name="description">Text of the marker visible in settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemMarker ReadMarkerReference(string bouquetLine, string description)
        {
            try
            {
                if (bouquetLine.ToUpperInvariant().StartsWith("#SERVICE", StringComparison.Ordinal))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                if (description == null)
                {
                    description = string.Empty;
                }
                else if (description.ToUpperInvariant().StartsWith("#DESCRIPTION:", StringComparison.Ordinal))
                {
                    description = description.Substring(13);
                }
                else if (description.ToUpperInvariant().StartsWith("#DESCRIPTION", StringComparison.Ordinal))
                {
                    description = description.Substring(12);
                }
                string[] sData = bouquetLine.Split(':');
                IBouquetItemMarker bqim = Factory.InitNewBouquetItemMarker(description, sData[2]);
                bqim.FavoritesTypeFlag = sData[0];
                bqim.LineSpecifierFlag = sData[1];
                return bqim;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(string.Format(Resources.SettingsIO_ReadMarkerReference_Failed_to_initialize_new_marker_from_line__0_,
                    bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemStream object from bouquet-line
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <param name="description">Text of the stream service visible in settings</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemStream ReadStreamReference(string bouquetLine, string description)
        {
            try
            {
                if (bouquetLine.ToUpperInvariant().StartsWith("#SERVICE", StringComparison.Ordinal))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                if (description == null)
                {
                    description = string.Empty;
                }
                else if (description.ToUpperInvariant().StartsWith("#DESCRIPTION:", StringComparison.Ordinal))
                {
                    description = description.Substring(13);
                }
                else if (description.ToUpperInvariant().StartsWith("#DESCRIPTION", StringComparison.Ordinal))
                {
                    description = description.Substring(12).Trim();
                }
                string[] sData = bouquetLine.Split(':');
                if (description.Length == 0 && sData.Length > 11)
                {
                    description = Uri.UnescapeDataString(sData[11]);
                }
                IBouquetItemStream bqis = Factory.InitNewBouquetItemStream(bouquetLine, description);
                bqis.FavoritesTypeFlag = sData[0];
                bqis.LineSpecifierFlag = sData[1];
                return bqis;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(string.Format(
                    Resources.SettingsIO_ReadStreamReference_Failed_to_initialize_new_stream_reference_from_line__0_, bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Reads bouquets file from Enigma1
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>bouquets Bouquet with other bouquets as bouquet items</returns>
        /// <remarks></remarks>
        protected virtual IFileBouquet ReadE1Bouquets(string fileName, ref ISettings settings)
        {
            Log.Debug("Reading Enigma1 bouquets file");

            try
            {
                IFileBouquet bqt = Factory.InitNewFileBouquet();
                bqt.FileName = BouquetsFile;
                bqt.Name = BouquetsFile;

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    settings.Bouquets.Add(bqt);
                }

                string[] fileLines = Read(fileName);

                //search for the beginning of bouquets (first line is the header)
                int openIndex = Array.FindIndex(fileLines, 1, x => x.Trim() == BouquetsOpenTag);
                if (openIndex == -1)
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, BouquetsOpenTag, bqt.FileName));
                    return bqt;
                }

                int endIndex = Array.FindIndex(fileLines, openIndex + 1, x => x.Trim() == SettingsClosingTag);
                if (endIndex == -1)
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, SettingsClosingTag, bqt.FileName));
                    endIndex = fileLines.Length;
                }

                //every bouquet is: order number, name, services..., "/"
                var block = new List<string>();
                for (int i = openIndex + 1; i < endIndex; i++)
                {
                    string l = fileLines[i].Trim();
                    if (l != "/")
                    {
                        if (l.Length > 0)
                            block.Add(l);
                        continue;
                    }

                    AddE1Bouquet(bqt, block, fileName);
                    block.Clear();
                }

                return bqt;
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.SettingsIO_ReadE1Bouquets_Failed_to_read_Enigma1_bouquets_file_, ex);
            }
        }

        /// <summary>
        ///     Adds one Enigma1 bouquet (order number, name, service lines) to the bouquets bouquet
        /// </summary>
        private void AddE1Bouquet(IFileBouquet bouquets, List<string> block, string fileName)
        {
            if (block.Count < 2)
                return;

            string orderNumber = block[0];
            string name = block[1];

            if (!int.TryParse(orderNumber, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order)
                && !int.TryParse(orderNumber, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out order))
            {
                Log.Warn($"Invalid order number {orderNumber} for Enigma1 bouquet {name} in {fileName}, using 0");
                order = 0;
            }

            IBouquetsBouquet bq = Factory.InitNewBouquetsBouquet();
            bq.Name = name;
            IBouquetItemBouquetsBouquet bib = Factory.InitNewBouquetItemBouquetsBouquet(bq);
            bib.BouquetOrderNumberInt = order;
            bouquets.BouquetItems.Add(bib);
            if (Log.IsDebugEnabled) Log.Debug($"Found Enigma1 bouquet {name} in {fileName}");

            for (int i = 2; i < block.Count; i++)
            {
                if (block[i].Split(':').Length >= 5)
                {
                    bq.BouquetItems.Add(Factory.InitNewBouquetItemService(block[i]));
                    if (Log.IsDebugEnabled)
                        Log.Debug($"Added service {block[i]} to Enigma1 bouquet {name}");
                }
                else
                {
                    Log.Warn(string.Format(Resources.Settings_ReadE1Bouquets_Ignoring_line__0__in_Enigma1_bouquet__1___doesn_t_look_like_regular_service, block[i], name));
                }
            }
        }

        /// <summary>
        ///     Reads Enigma2 whitelist and blacklist file and marks corresponding services as black/white listed
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        /// <remarks>Should be called last when loading settings </remarks>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if file info is null</exception>
        /// <exception cref="SettingsException">Throws settingsException</exception>
        protected virtual void ReadBlackWhiteList(string fileName, ref ISettings settings)
        {
            var sLocked = new List<IBouquetItemService>();

            if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
            {
                return;
            }
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!_fileHelper.Exists(fileName))
            {
                return;
            }

            var fName = Path.GetFileName(fileName);

            string[] bouquetFileLines = Read(fileName);
            sLocked.AddRange(bouquetFileLines.Where(x => x.Trim().Length > 0).Select(ReadServiceReference).Where(bqis => bqis != null));

            if (!sLocked.Any()) return;
            bool blackListed = (string.Equals(fName, BlackListFile, StringComparison.OrdinalIgnoreCase));

            //matching  services
            try
            {
                Log.Debug(!blackListed ? "Matching whitelisted services." : "Matching blacklisted services.");

                var query = settings.Services.Join(sLocked, srv => srv.ServiceId, bi => bi.ServiceId, (srv, x) => new
                {
                    Service = srv,
                    LockedBouquetItem = x
                }, StringComparer.OrdinalIgnoreCase);

                foreach (var match in query.ToList())
                {
                    if (blackListed)
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                        if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as BLACKLISTED");
                    }
                    else
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.WhiteListed;
                        if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as WHITELISTED");
                    }
                    sLocked.Remove(match.LockedBouquetItem);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //logging items that are not matched
            foreach (IBouquetItemService bi in sLocked)
            {
                Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Locked_bouquet_item__0__has_no_matching_service, bi.ServiceId));
            }
        }

        /// <summary>
        ///     Reads Enigma1 services.locked file and marks bouquets and services as locked
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings">Settings with already loaded services and bouquets</param>
        /// <remarks>Should be last function to call when loading Settings</remarks>
        protected virtual void ReadServicesLocked(string fileName, ref ISettings settings)
        {
            Log.Debug("Reading services.locked file");
            var sLocked = new List<IBouquetItem>();
            try
            {
                string[] lockedLines = Read(fileName);
                int lineNumber = 0;
                foreach (string line in lockedLines)
                {
                    lineNumber += 1;
                    if (lineNumber <= 1) continue;
                    if (string.IsNullOrEmpty(line) || line.Split(':').Length < 10)
                    {
                        Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Invalid_line__0__found_in__1_, line, fileName));
                    }
                    else
                    {
                        string[] sData = line.Split(':');
                        IBouquetItemService srv = Factory.InitNewBouquetItemService(line);

                        if (srv.LineSpecifierType == Enums.LineSpecifier.Unknown)
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, fileName));
                        }
                        else if (srv.LineSpecifierType.ToString().ToLowerInvariant().StartsWith("isdirectory", StringComparison.Ordinal))
                        {
                            if (Log.IsDebugEnabled) Log.Debug($"Found reference to locked bouquet {line} in {fileName}");
                            try
                            {
                                int orderNumber = Int32.Parse(sData[4], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                                sLocked.Add(Factory.InitNewBouquetItemBouquetsBouquet(orderNumber));
                            }
                            catch (Exception ex)
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources
                                            .SettingsIO_ReadServicesLocked_Unable_to_parse_bouquet_order_number__0__to_integer_for_bouquet__1__on_line__2_,
                                        sData[4], fileName, line));
                                Log.Warn(ex.ToString());
                            }
                        }
                        else if (srv.LineSpecifierType == Enums.LineSpecifier.NormalService)
                        {
                            sLocked.Add(srv);
                            if (Log.IsDebugEnabled) Log.Debug($"Found reference to locked service {line} in {fileName}");
                        }
                        else
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Unsupported_type__0__found_in_line__1__in_file__2_,
                                srv.LineSpecifierType, line, fileName));
                        }
                    }
                }
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.SettingsIO_ReadServicesLocked_Failed_to_read_services_locked_file, ex);
            }

            Log.Debug("All items from services.locked loaded.");
            if (!sLocked.Any())
                return;

            IFileBouquet bouquets = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => x.BouquetType == Enums.BouquetType.E1Bouquets);

            //matching locked bouquets by order number
            try
            {
                if (bouquets != null)
                {
                    Log.Debug("Matching locked bouquets.");
                    IList<IBouquetItemBouquetsBouquet> bis = bouquets.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList();
                    var query = bis.Join(sLocked.OfType<IBouquetItemBouquetsBouquet>().ToList(), bs => bs.BouquetOrderNumber,
                        x => x.BouquetOrderNumber, (bs, x) => new
                        {
                            BouquetsBouquetItem = bs,
                            LockedBouquetItem = x
                        }, StringComparer.OrdinalIgnoreCase);

                    foreach (var match in query.ToList())
                    {
                        if (match.BouquetsBouquetItem.Bouquet != null)
                        {
                            match.BouquetsBouquetItem.Bouquet.Locked = true;
                            if (Log.IsDebugEnabled) Log.Debug($"Bouquet {match.BouquetsBouquetItem.Bouquet.Name} has been marked as LOCKED");
                        }
                        else
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Bouquet_item__0__has_no_matching_Enigma1_bouquet_,
                                match.LockedBouquetItem.BouquetOrderNumber));
                        }
                        sLocked.Remove(match.LockedBouquetItem);
                    }
                }
                else
                {
                    Log.Debug("Enigma1 bouquets bouquet was not found, no locked bouquets to match.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //matching locked services
            try
            {
                Log.Debug("Matching locked services.");
                var query = settings.Services.Join(sLocked.OfType<IBouquetItemService>().ToList(), srv => srv.ServiceId,
                    bi => bi.ServiceId, (srv, x) => new
                    {
                        Service = srv,
                        LockedBouquetItem = x
                    }, StringComparer.OrdinalIgnoreCase);

                foreach (var match in query.ToList())
                {
                    match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                    if (Log.IsDebugEnabled) Log.Debug($"Service {match.Service} has been marked as LOCKED");
                    sLocked.Remove(match.LockedBouquetItem);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(Resources.Settings_MatchBouquetServices_There_was_an_error_while_matching_bouquet_items_to_services, ex);
            }

            //logging items that are not matched
            foreach (IBouquetItem bi in sLocked)
            {
                if (bi is IBouquetItemService service)
                {
                    Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Locked_bouquet_item__0__has_no_matching_service, service.ServiceId));
                }
                else
                {
                    if (bi is IBouquetItemBouquetsBouquet bouquet)
                    {
                        Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Bouquet_item__0__has_no_matching_Enigma1_bouquet_,
                            bouquet.BouquetOrderNumber));
                    }
                }
            }
        }
        #endregion
        #region "Writing"
        protected virtual void WriteSettingsFile(string directory, ISettings settings, string strLocalFilenameOverride)
        {
            Log.Debug("Writing settings file to disk");

            string fileName = settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2 ? "services" : "lamedb";

            if (!string.IsNullOrEmpty(strLocalFilenameOverride))
                fileName = strLocalFilenameOverride;

            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver5)
            {
                WriteLameDb5(directory, settings, strLocalFilenameOverride);
                return;
            }

            var sContent = new StringBuilder();
            var fi = Path.Combine(directory, fileName);
            OnFileSaving(this, fi);

            try
            {
                sContent.Append(SettingsFirstLine + GetSettingsVersion(settings.SettingsVersion) + "/" + "\n");
                sContent.Append(TranspondersToString(settings));
                sContent.Append(ServicesToString(settings));
                sContent.Append(EditorName + "\n");

                _fileHelper.WriteAllText(Path.Combine(directory, fileName), sContent.ToString());
                if (Log.IsDebugEnabled) Log.Debug($"Sucessfully written {fileName} to {Path.Combine(directory, fileName)}");
                OnFileSaved(this, fi, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fi, false);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_Save_Failed_to_save_settings_to__0_, Path.Combine(directory, fileName)), ex);
            }
        }

        protected virtual string TranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsTransponderOpenTag + "\n");

            switch (settings.SettingsVersion)
            {
                case Enums.SettingsVersion.Enigma1Ver1:
                case Enums.SettingsVersion.Enigma1Ver2:
                    sContent.Append(DVBSTranspondersToString(settings, 1));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
                    sContent.Append(IPTVTranspondersToString(settings));
                    sContent.Append(DVBSTranspondersToString(settings, -1));
                    break;
                case Enums.SettingsVersion.Enigma2Ver3:
                case Enums.SettingsVersion.Enigma2Ver4:
                case Enums.SettingsVersion.Enigma2Ver5:
                    sContent.Append(DVBSTranspondersToString(settings, 0));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
                    sContent.Append(IPTVTranspondersToString(settings));
                    break;
            }

            sContent.Append(SettingsClosingTag + "\n");
            return sContent.ToString();
        }

        protected virtual string DVBSTranspondersToString(ISettings settings, int positionOrder)
        {
            var sContent = new StringBuilder();
            foreach (
                ITransponderDVBS tran in
                    settings.Transponders.OfType<ITransponderDVBS>()
                        .Where(x => (positionOrder > 0 && x.OrbitalPositionInt >= 0) || (positionOrder < 0 && x.OrbitalPositionInt < 0) || positionOrder == 0)
                        .OrderByDescending(x => x.OrbitalPositionInt)
                        .ToList())
            {
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                    {
                        if (tran.SystemType == Enums.DVBSSystemType.DVBS)
                        {
                            sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                            sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Polarization, tran.FEC, tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture), tran.Inversion));
                            sContent.Append("\n" + "/" + "\n");
                        }
                        else
                            if (Log.IsDebugEnabled) Log.Debug($"Transponder {tran} is not DVBS transponder, not supported in Enigma1, skipping");

                        break;
                    }
                    case Enums.SettingsVersion.Enigma2Ver5:
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver3:
                    {
                        sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                        sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Polarization, tran.FEC, tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture), tran.Inversion));
                        string extraData = string.Empty;

                        if (IsVer4OrLater(settings.SettingsVersion))
                        {
                            if (tran.Flags != null)
                                extraData = ":" + tran.Flags;
                            else
                                extraData = ":0";
                        }

                        if (tran.SystemType == Enums.DVBSSystemType.DVBS2)
                        {
                            if (tran.System != null)
                                extraData += ":" + tran.System;
                            else
                                extraData += ":1";

                            if (tran.Modulation != null)
                                extraData += ":" + tran.Modulation;
                            else
                                extraData += ":0";

                            if (tran.RollOff != null)
                                extraData += ":" + tran.RollOff;
                            else
                                extraData += ":0";

                            if (tran.Pilot != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.Pilot;

                            if (tran.IsId != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.IsId;

                            if (tran.PlsCode != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.PlsCode;

                            if (tran.PlsMode != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.PlsMode;

                            if (tran.T2miPlpId != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.T2miPlpId;

                            if (tran.T2miPid != null && IsVer4OrLater(settings.SettingsVersion))
                                extraData += ":" + tran.T2miPid;
                        }

                        sContent.Append(extraData + "\n" + "/" + "\n");
                    }
                    break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string DVBCTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderDVBC tran in settings.Transponders.OfType<ITransponderDVBC>())
            {
                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                    {
                        string sRate = tran.SymbolRate;
                        string frequency = tran.Frequency;

                        if (frequency.Length < 8)
                            frequency = frequency + "00";
                        
                        sRate = sRate.Trim('0').Length > 0 ? sRate.PadRight(8, '0') : "0000";
                        sContent.Append("\t" + "s " + string.Join(":", frequency, sRate, "15", tran.FEC, "0", tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");

                        break;
                    }
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "c " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Inversion, tran.Modulation, tran.FEC));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver5:
                        sContent.Append("\t" + "c " + string.Join(":", tran.Frequency, tran.SymbolRate, tran.Inversion, tran.Modulation, tran.FEC, tran.Flags, tran.System));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string DVBTTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderDVBT tran in settings.Transponders.OfType<ITransponderDVBT>())
            {
                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1Ver1:
                    case Enums.SettingsVersion.Enigma1Ver2:
                        sContent.Append("\t" + "s " + string.Join(":", tran.Frequency, "0000", "0", "0", "0", tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "t " + string.Join(":", tran.Frequency, tran.Bandwidth, tran.FECHigh, tran.FECLow, tran.Modulation, tran.Transmission, tran.GuardInterval, tran.Hierarchy, tran.Inversion));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver5:
                        sContent.Append("\t" + "t " + string.Join(":", tran.Frequency, tran.Bandwidth, tran.FECHigh, tran.FECLow, tran.Modulation, tran.Transmission, tran.GuardInterval, tran.Hierarchy, tran.Inversion, tran.Flags, tran.System, tran.PlpId));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string IPTVTranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            foreach (ITransponderIPTV tran in settings.Transponders.OfType<ITransponderIPTV>())
            {
                // optional fields are positional, so keep every field up to the last one that is set
                string[] optional = [tran.Inversion, tran.Modulation, tran.FEC, tran.Flags, tran.System];
                int last = Array.FindLastIndex(optional, x => x != null);
                var fields = new List<string> { tran.Frequency ?? "0", tran.SymbolRate ?? "0" };
                fields.AddRange(optional.Take(last + 1).Select(x => x ?? "0"));

                sContent.Append(string.Join(":", tran.NameSpc, tran.TSID, tran.NID)).Append('\n');
                sContent.Append("\ts ").Append(string.Join(":", fields)).Append("\n/\n");
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Sort order of services in the settings file, matching the order transponders are written in
        /// </summary>
        private static int ServiceWriteGroup(IService service, bool enigma1)
        {
            switch (service.Transponder)
            {
                case null:
                    return 5;
                case ITransponderDVBS dvbs:
                    return enigma1 && dvbs.OrbitalPositionInt < 0 ? 4 : 0;
                case ITransponderDVBC:
                    return 1;
                case ITransponderDVBT:
                    return 2;
                default:
                    return 3;
            }
        }

        protected virtual string ServicesToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsServicesOpenTag + "\n");

            bool enigma1 = settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2;

            // OrderBy is stable, so the original order is kept inside each group
            foreach (IService service in settings.Services.OrderBy(x => ServiceWriteGroup(x, enigma1)))
                sContent.Append(ServiceToString(service, settings, service.Transponder == null));

            sContent.Append(SettingsClosingTag + "\n");
            return sContent.ToString();
        }

        protected virtual string ServiceToString(IService service, ISettings settings, bool bLocalTransponderIsNull = false)
        {
            var sContent = new StringBuilder();
            if (service.Transponder == null && bLocalTransponderIsNull != true)
            {
                Log.Warn(string.Format(Resources.SettingsIO_ServicesToString_Service__0__has_no_transponder_set__not_writing_it_to_settings_,
                    service.ToString().Replace("\t", "  ")));
            }
            else
            {
                bool serviceSupported = true;
                string flags = string.Empty;

                if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
                {
                    if (service.Transponder is ITransponderDVBS dvbs)
                    {
                        ITransponderDVBS tran = dvbs;
                        if (tran.SystemType != Enums.DVBSSystemType.DVBS)
                            serviceSupported = false;
                    }

                    //make sure provider name is not empty for Enigma1 settings
                    IList<IFlag> flagList = service.FlagList;
                    IFlag pFlag = flagList.SingleOrDefault(x => x.FlagType == Enums.FlagType.P);
                    if (pFlag != null && pFlag.FlagValue.Length == 0)
                    {
                        pFlag.FlagValue = "unknown";
                        flags = string.Join(",", flagList.Select(x => x.FlagString).ToArray());
                    }
                    else if (pFlag == null)
                    {
                        flags = "p:unknown," + service.Flags;
                    }
                }

                if (serviceSupported)
                {
                    var sData = bLocalTransponderIsNull
                        ? string.Join(":", service.SID.PadLeft(4, '0'), service.TransponderId, service.Type, service.ProgNumber)
                        : string.Join(":", service.SID.PadLeft(4, '0'), service.Transponder.NameSpc.PadLeft(8, '0'), service.Transponder.TSID.PadLeft(4, '0'), service.Transponder.NID.PadLeft(4, '0'), service.Type, service.ProgNumber);

                    // the optional 7th field (source id) only exists in Enigma2 lamedb
                    if (!string.IsNullOrEmpty(service.SourceID) && settings.SettingsVersion is not (Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2))
                        sData += ":" + service.SourceID;

                    sContent.Append(sData + "\n");
                    sContent.Append(service.Name + "\n");

                    if (flags.Length > 0)
                        sContent.Append(flags + "\n");
                    else
                        sContent.Append(service.Flags + "\n");
                }
                else
                    if (Log.IsDebugEnabled) Log.Debug($"Service {service.ToString().Replace("\t", "  ")} is not supported in Enigma1. Not writing it to settings.");
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Writes bouquets to corresponding files
        /// </summary>
        /// <param name="directory">Directory info of the directory all bouquet files are written to</param>
        /// <param name="settings">Settings instance to be written></param>
        /// <remarks></remarks>
        protected virtual void WriteBouquets(string directory, ISettings settings)
        {
            if (settings.SettingsVersion is Enums.SettingsVersion.Enigma1Ver1 or Enums.SettingsVersion.Enigma1Ver2)
            {
                WriteTvBouquetsE1(directory, settings);
                WriteRadioBouquetsE1(directory, settings);
                WriteEnigma1Bouquets(directory, settings);
                WriteServicesLocked(directory, settings);
            }
            else if (settings.SettingsVersion is Enums.SettingsVersion.Enigma2Ver3 or Enums.SettingsVersion.Enigma2Ver4 or Enums.SettingsVersion.Enigma2Ver5)
            {
                WriteTvBouquetsE2(directory, settings);
                WriteRadioBouquetsE2(directory, settings);
                WriteAlternativeBouquets(directory, settings);
                WriteWhiteList(directory, settings);
                WriteBlackList(directory, settings);
            }
        }

        /// <summary>
        ///     Writes text to specified fileName
        /// </summary>
        /// <param name="fileName">Full path to file location od disk</param>
        /// <param name="content">File content as text</param>
        /// <param name="bouquetName">Name of the bouquet we're saving (for log purpose)</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if write fails</exception>
        protected virtual void WriteBouquet(string fileName, string content, string bouquetName)
        {
            try
            {
                OnFileSaving(this, fileName);
                _fileHelper.WriteAllText(fileName, content);
                if (Log.IsDebugEnabled) Log.Debug($"Sucessfully written bouquet {bouquetName} to {fileName}");
                OnFileSaved(this, fileName, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fileName, false);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteTvBouquetsE2_Failed_to_write_bouquet__0__to__1_, bouquetName, fileName), ex);
            }
        }

        /// <summary>
        ///     Creates text representation of the bouquet ready to be written to disk
        /// </summary>
        /// <param name="bouquet"></param>
        /// <returns>Returns text ready to be written to disk as a bouquet</returns>
        /// <remarks></remarks>
        protected virtual string E1BouquetToString(IFileBouquet bouquet)
        {
            var sContent = new StringBuilder();
            sContent.Append("#NAME " + bouquet.Name + "\n");
            foreach (IBouquetItem bqItem in bouquet.BouquetItems)
            {
                switch (bqItem.BouquetItemType)
                {
                    case Enums.BouquetItemType.BouquetsBouquet:
                        var bib = (IBouquetItemBouquetsBouquet)bqItem;
                        Log.Debug(string.Format("Found bouquets bouquet {0} in a file bouquet, skiping write to bouquet {1}", bib.Bouquet.Name,
                            bouquet.Name));
                        break;
                    case Enums.BouquetItemType.FileBouquet:
                        var bif = (IBouquetItemFileBouquet)bqItem;
                        string fName = Path.GetFileName(bif.FileName) ?? throw new SettingsException($"File bouquet reference in bouquet {bouquet.Name} has no file name");
                        if (fName.ToLowerInvariant().StartsWith("userbouquet.", StringComparison.Ordinal))
                            fName = fName.Substring(12);
                        string reference = "#SERVICE: " + string.Join(":", bif.FavoritesTypeFlag, bif.LineSpecifierFlag, "0", fName.Split('.')[0], "0", "0", "0", "0", "0", "0", bif.FileName) + "\n";
                        reference += "#TYPE 16385" + "\n";
                        reference += bif.FileName.ToLowerInvariant() + "\n";
                        sContent.Append(reference);
                        if (Log.IsDebugEnabled) Log.Debug($"File bouquet reference {reference.Replace("\n", "\t")} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Marker:
                        var bim = (IBouquetItemMarker)bqItem;
                        sContent.Append("#SERVICE: " + string.Join(":", bim.FavoritesTypeFlag, bim.LineSpecifierFlag, bim.MarkerNumber, "0", "0", "0", "0", "0", "0", "0", "") + "\n");
                        sContent.Append("#DESCRIPTION: " + bim.Description + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Marker {bim.Description} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Service:
                        var bis = (IBouquetItemService)bqItem;
                        if (bis.Service == null)
                        {
                            Log.Warn(
                                string.Format(
                                    Resources
                                        .SettingsIO_E1BouquetToString_Service_reference__0__in_bouquet__1__is_not_matched_to_any_service__Not_writing_it_to_bouquet_,
                                    bis.ServiceId, bouquet.Name));
                        }
                        else if (bis.Service.Transponder == null)
                        {
                            Log.Warn(
                                string.Format(
                                    Resources.SettingsIO_E1BouquetToString_Service__0__in_bouquet__1__has_no_matching_transponder__Not_writing_it_,
                                    bis.Service.ToString().Replace("\t", "    "), bouquet.Name));
                        }
                        else
                        {
                            bool serviceSupported = true;
                            if (bis.Service.Transponder is ITransponderDVBS dvbs)
                            {
                                ITransponderDVBS tran = dvbs;
                                if (tran.SystemType != Enums.DVBSSystemType.DVBS)
                                {
                                    serviceSupported = false;
                                }
                            }

                            if (serviceSupported)
                            {
                                sContent.Append("#SERVICE: " + string.Join(":", bis.FavoritesTypeFlag, bis.LineSpecifierFlag, bis.Service.ServiceId, "0", "0", "0", "") + "\n");
                                if (Log.IsDebugEnabled) Log.Debug($"Service {bis.Service.ServiceId}\t{bis.Service.Name} added to bouquet {bouquet.Name}");
                            }
                            else
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources.SettingsIO_E1BouquetToString_Service__0__in_bouquet__1__is_not_supported_in_Enigma1__Not_writing_it_,
                                        bis.Service.Name, bouquet.Name));
                            }
                        }
                        break;
                    case Enums.BouquetItemType.Stream:
                        //Dim bi As IBouquetItemStream = TryCast(bqItem, IBouquetItemStream)
                        //Log.Debug(String.Format("Stream services are not supported in Enigma1. Skipping stream service {0} in bouquet {1}", bi.Description, bouquet.Name))
                        var biss = bqItem as IBouquetItemStream;

                        if (biss != null)
                            sContent.Append("#SERVICE " + string.Join(":", biss.FavoritesTypeFlag, biss.LineSpecifierFlag, biss.StreamFlag, biss.ServiceID, biss.ExtraFlag1, biss.ExtraFlag2, "0", "0", "0", "0", BouquetItemStream.EncodeUrl(biss.URL), Uri.EscapeDataString(biss.Description).Replace("'", "%27")) + "\n");

                        if (biss != null)
                        {
                            sContent.Append("#DESCRIPTION " + biss.Description + "\n");
                            if (Log.IsDebugEnabled) Log.Debug($"Stream {biss.Description}\t{biss.URL} added to bouquet {bouquet.Name}");
                        }
                        break;
                    case Enums.BouquetItemType.Unknown:
                        Log.Warn(string.Format(Resources.SettingsIO_WriteBouquets_Skipping_unknown_bouquet_item_type_in_bouquet__0_, bouquet.Name));
                        break;
                }
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Creates text representation of the bouquet ready to be written to disk
        /// </summary>
        /// <param name="bouquet"></param>
        /// <returns>Returns text ready to be written to disk as a bouquet</returns>
        /// <remarks></remarks>
        protected virtual string E2BouquetToString(IFileBouquet bouquet)
        {
            var sContent = new StringBuilder();
            sContent.Append("#NAME " + bouquet.Name + "\n");
            foreach (IBouquetItem bqItem in bouquet.BouquetItems)
            {
                switch (bqItem.BouquetItemType)
                {
                    case Enums.BouquetItemType.FileBouquet:
                        var bif = (IBouquetItemFileBouquet)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", bif.FavoritesTypeFlag, bif.LineSpecifierFlag, "1", "0", "0", "0", "0", "0", "0", "0", "FROM BOUQUET \"" + bif.FileName + "\" ORDER BY bouquet") + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"File bouquet reference {bif.FileName} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Alternative:
                        var bia = (IBouquetItemAlternative)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", bia.FavoritesTypeFlag, bia.LineSpecifierFlag, "1", "0", "0", "0", "0", "0", "0", "0", "FROM BOUQUET \"" + bia.FileName + "\" ORDER BY bouquet") + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Alternative reference {bia.FileName} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Marker:
                        var bim = (IBouquetItemMarker)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", bim.FavoritesTypeFlag, bim.LineSpecifierFlag, bim.MarkerNumber, "0", "0", "0", "0", "0", "0", "0", "") + "\n");
                        sContent.Append("#DESCRIPTION " + bim.Description + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Marker {bim.Description} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Service:
                        var bis = (IBouquetItemService)bqItem;
                        if (bis.Service == null)
                        {
                            Log.Warn(
                                string.Format(
                                    Resources
                                        .SettingsIO_E1BouquetToString_Service_reference__0__in_bouquet__1__is_not_matched_to_any_service__Not_writing_it_to_bouquet_,
                                    bis.ServiceId, bouquet.Name));
                        }
                        else if (bis.Service.Transponder == null)
                        {
                            Log.Warn(
                                string.Format(
                                    Resources.SettingsIO_E1BouquetToString_Service__0__in_bouquet__1__has_no_matching_transponder__Not_writing_it_,
                                    bis.Service.ToString().Replace("\t", "    "), bouquet.Name));
                        }
                        else
                        {
                            sContent.Append("#SERVICE " + string.Join(":", bis.FavoritesTypeFlag, bis.LineSpecifierFlag, bis.Service.ServiceId, "0", "0", "0", "") + "\n");
                            if (!string.IsNullOrEmpty(bis.Description))
                                sContent.Append("#DESCRIPTION " + bis.Description + "\n");
                            if (Log.IsDebugEnabled) Log.Debug($"Service {bis.Service.ServiceId}\t{bis.Service.Name} added to bouquet {bouquet.Name}");
                        }
                        break;
                    case Enums.BouquetItemType.Stream:
                        var biss = (IBouquetItemStream)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", biss.FavoritesTypeFlag, biss.LineSpecifierFlag, biss.StreamFlag, biss.ServiceID, biss.ExtraFlag1, biss.ExtraFlag2, "0", "0", "0", "0", BouquetItemStream.EncodeUrl(biss.URL), Uri.EscapeDataString(biss.Description).Replace("'", "%27")) + "\n");
                        sContent.Append("#DESCRIPTION " + biss.Description + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Stream {biss.Description}\t{biss.URL} added to bouquet {bouquet.Name}");
                        break;
                    case Enums.BouquetItemType.Unknown:
                        Log.Warn(string.Format(Resources.SettingsIO_WriteBouquets_Skipping_unknown_bouquet_item_type_in_bouquet__0_, bouquet.Name));
                        break;
                }
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Writes 'userbouquets.tv.epl' file and all TV bouquets to disk in Enigma1 format
        /// </summary>
        /// <param name="directory">Directory where all TV bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteTvBouquetsE1(string directory, ISettings settings)
        {
            var sBouquetsTvContent = new StringBuilder();
            sBouquetsTvContent.Append("#NAME User - bouquets (TV)" + "\n");
            int bouquetIndex = 0;
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetTv).ToList()
                )
            {
                string dbeIndex = "dbe" + bouquetIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                string enigmaPath = string.Empty;
                if (bouquet.FileName.IndexOf('/') > -1)
                {
                    enigmaPath = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf('/') + 1);
                }
                if (enigmaPath.Length == 0)
                    enigmaPath = DefaultEnigma1Path;
                bouquet.FileName = enigmaPath + "userbouquet." + dbeIndex + ".tv";
                string reference = "#SERVICE: 4097:7:0:" + dbeIndex + ":0:0:0:0:0:0:" + bouquet.FileName + "\n";
                reference += "#TYPE 16385" + "\n";
                reference += bouquet.FileName + "\n";
                sBouquetsTvContent.Append(reference);
                if (Log.IsDebugEnabled) Log.Debug($"Bouquet {bouquet.FileName} added to userbouquets.epl.tv");
                string bContent = E1BouquetToString(bouquet);
                WriteBouquet(Path.Combine(directory, Path.GetFileName(bouquet.FileName)), bContent, bouquet.Name);
                bouquetIndex += 1;
            }
            WriteBouquet(Path.Combine(directory, UserBouquetTvEpl), sBouquetsTvContent.ToString(), UserBouquetTvEpl);
        }

        /// <summary>
        ///     Writes 'bouquets.tv' file and all TV bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all TV bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks></remarks>
        protected virtual void WriteTvBouquetsE2(string directory, ISettings settings)
        {
            var sBouquetsTvContent = new StringBuilder();
            sBouquetsTvContent.Append("#NAME User - Bouquets (TV)" + "\n");
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetTv).ToList())
            {
                // sub bouquets are referenced by their parent bouquet only
                if (!bouquet.IsSubBouquet)
                {
                    sBouquetsTvContent.Append("#SERVICE 1:" + (bouquet.Hidden ? "519" : "7") + ":1:0:0:0:0:0:0:0:FROM BOUQUET \"" + Path.GetFileName(bouquet.FileName) + "\" ORDER BY bouquet\n");
                    if (Log.IsDebugEnabled) Log.Debug($"Bouquet {Path.GetFileName(bouquet.FileName)} added to bouquets.tv");
                }
                string bContent = E2BouquetToString(bouquet);
                string fName = Path.GetFileName(bouquet.FileName);
                if (fName != null) WriteBouquet(Path.Combine(directory, fName), bContent, bouquet.Name);
            }
            WriteBouquet(Path.Combine(directory, BouquetsTvFile), sBouquetsTvContent.ToString(), BouquetsTvFile);
        }

        /// <summary>
        ///     Writes 'userbouquets.radio.epl' file and all radio bouquets to disk in Enigma1 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteRadioBouquetsE1(string directory, ISettings settings)
        {
            var bouquetsRadioContent = new StringBuilder();
            bouquetsRadioContent.Append("#NAME User - bouquets (Radio)" + "\n");
            int bouquetIndex = 0;
            foreach (
                IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetRadio).ToList())
            {
                string dbeIndex = "dbe" + bouquetIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                string enigmaPath = string.Empty;
                if (bouquet.FileName.IndexOf('/') > -1)
                {
                    enigmaPath = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf('/') + 1);
                }
                if (enigmaPath.Length == 0)
                    enigmaPath = DefaultEnigma1Path;
                bouquet.FileName = enigmaPath + "userbouquet." + dbeIndex + ".radio";
                string reference = "#SERVICE: 4097:7:0:" + dbeIndex + ":0:0:0:0:0:0:" + bouquet.FileName + "\n";
                reference += "#TYPE 16385" + "\n";
                reference += bouquet.FileName + "\n";
                bouquetsRadioContent.Append(reference);
                if (Log.IsDebugEnabled) Log.Debug($"Bouquet {bouquet.FileName} added to userbouquets.epl.radio");
                string bContent = E1BouquetToString(bouquet);
                WriteBouquet(Path.Combine(directory, Path.GetFileName(bouquet.FileName)), bContent, bouquet.Name);
                bouquetIndex += 1;
            }
            WriteBouquet(Path.Combine(directory, UserBouquetRadioEpl), bouquetsRadioContent.ToString(), UserBouquetRadioEpl);
        }

        /// <summary>
        ///     Writes 'bouquets.radio' file and all radio bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're gonna writing</param>
        /// <remarks></remarks>
        protected virtual void WriteRadioBouquetsE2(string directory, ISettings settings)
        {
            var sBouquetsRadioContent = new StringBuilder();
            sBouquetsRadioContent.Append("#NAME User - Bouquets (Radio)" + "\n");
            foreach (
                IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetRadio).ToList())
            {
                // sub bouquets are referenced by their parent bouquet only
                if (!bouquet.IsSubBouquet)
                {
                    sBouquetsRadioContent.Append("#SERVICE 1:" + (bouquet.Hidden ? "519" : "7") + ":2:0:0:0:0:0:0:0:FROM BOUQUET \"" + Path.GetFileName(bouquet.FileName) + "\" ORDER BY bouquet\n");
                    if (Log.IsDebugEnabled) Log.Debug($"Bouquet {Path.GetFileName(bouquet.FileName)} added to bouquets.radio");
                }
                string bContent = E2BouquetToString(bouquet);
                string fName = Path.GetFileName(bouquet.FileName);
                if (fName != null) WriteBouquet(Path.Combine(directory, fName), bContent, bouquet.Name);
            }
            WriteBouquet(Path.Combine(directory, BouquetsRadioFile), sBouquetsRadioContent.ToString(), BouquetsRadioFile);
        }

        /// <summary>
        ///     Writes alternatives files referenced inside user bouquets (alternatives.*.tv)
        /// </summary>
        /// <param name="directory">Directory where alternatives files will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the bouquets we're writing</param>
        protected virtual void WriteAlternativeBouquets(string directory, ISettings settings)
        {
            // All user bouquets that can hold alternatives are registered at the top level by the
            // loader, so a single-level scan of settings.Bouquets covers every alternatives file.
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().ToList())
            {
                foreach (IBouquetItemAlternative alt in bouquet.BouquetItems.OfType<IBouquetItemAlternative>().ToList())
                {
                    if (alt.Bouquet == null)
                        continue;

                    string fName = Path.GetFileName(alt.FileName);
                    
                    if (fName == null)
                        continue;

                    string content = E2BouquetToString(alt.Bouquet);
                    WriteBouquet(Path.Combine(directory, fName), content, alt.Bouquet.Name);
                    if (Log.IsDebugEnabled) Log.Debug($"Alternatives file {fName} written");
                }
            }
        }

        /// <summary>
        ///     Writes Enigma2 locked services in blacklist file
        /// </summary>
        /// <param name="directory">Directory we're saving blacklist file to</param>
        /// <param name="settings">Instance of ISettings with locked services</param>
        /// <remarks></remarks>
        protected virtual void WriteBlackList(string directory, ISettings settings)
        {
            var fContent = new StringBuilder();
            IList<IService> sLocked = settings.Services.Where(x => x.ServiceSecurity == Enums.ServiceSecurity.BlackListed).ToList();
            if (sLocked.Any())
            {
                foreach (IService srv in sLocked)
                {
                    if (srv.Transponder == null)
                    {
                        Log.Warn(string.Format(
                            Resources.SettingsIO_WriteBlackList_Service__0__has_no_matching_transponder__not_adding_it_to_blacklist_file, srv));
                    }
                    else
                    {
                        fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to blacklist file");
                    }
                }
            }
            else
            {
                Log.Debug("There is no locked services to put inside blacklist file");
            }

            WriteFile(Path.Combine(directory, BlackListFile), fContent.ToString(), Utf8NoBom);
        }

        /// <summary>
        ///     Writes Enigma2 whitelisted services in whitelist file
        /// </summary>
        /// <param name="directory">Directory we're saving whitelist file to</param>
        /// <param name="settings">Instance of ISettings with whitelisted services</param>
        /// <remarks></remarks>
        protected virtual void WriteWhiteList(string directory, ISettings settings)
        {
            var fContent = new StringBuilder();
            IList<IService> sLocked = settings.Services.Where(x => x.ServiceSecurity == Enums.ServiceSecurity.WhiteListed).ToList();
            if (sLocked.Any())
            {
                foreach (IService srv in sLocked)
                {
                    if (srv.Transponder == null)
                    {
                        Log.Warn(string.Format(
                            Resources.SettingsIO_WriteWhiteList_Service__0__has_no_matching_transponder__not_adding_it_to_whitelist_file, srv));
                    }
                    else
                    {
                        fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                        if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to whitelist file");
                    }
                }
            }
            else
            {
                Log.Debug("There is no services to put inside whitelist file");
            }
            WriteFile(Path.Combine(directory, WhiteListFile), fContent.ToString(), Utf8NoBom);
        }

        /// <summary>
        ///     Writes locked services and bouquets to Enigma1 services.locked file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successful</exception>
        protected virtual void WriteServicesLocked(string directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver2)
                {
                    Log.Debug("Skipping write to services.locked file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory, ServicesLockedFile);
                var fContent = new StringBuilder();
                fContent.Append("Parentallocked Services" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        IList<IBouquetItemBouquetsBouquet> lockedBouquets =
                            existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().Where(x => x.Bouquet == null || x.Bouquet.Locked).ToList();
                        if (lockedBouquets.Any())
                        {
                            foreach (IBouquetItemBouquetsBouquet lbq in lockedBouquets)
                            {
                                if (lbq.Bouquet != null)
                                {
                                    fContent.Append(string.Join(":", "1:15:fffffffd:12", lbq.BouquetOrderNumber, "ffffffff:0:0:0:0:") + "\n");
                                    if (Log.IsDebugEnabled) Log.Debug($"Enigma1 bouquet {lbq.Bouquet.Name} added to services.locked");
                                }
                                else
                                {
                                    Log.Warn(
                                        string.Format(
                                            Resources
                                                .SettingsIO_WriteServicesLocked_Reference_to_Enigma1_bouquet__0__has_no_matching_bouquet_set__not_adding_it_to_services_locked,
                                            lbq.BouquetOrderNumber));
                                }
                            }
                        }
                        else
                        {
                            Log.Debug("There was no locked bouquets found in the list of Enigma1 Bouquets");
                        }
                    }
                    else
                    {
                        Log.Debug("There is no Enigma1 bouquets, no locked bouquets then.");
                    }
                }
                else
                {
                    Log.Debug("Enigma1 Bouquets bouquet not found in the list of bouquets, assuming no locked bouquets exists");
                }

                //adding locked services
                IList<IService> sLocked = settings.FindLockedServices().ToList();
                if (sLocked.Any())
                {
                    foreach (IService srv in sLocked)
                    {
                        if (srv.Transponder == null)
                        {
                            Log.Warn(
                                string.Format(Resources.SettingsIO_WriteServicesLocked_Service__0__has_no_transponder__not_adding_it_to_services_locked,
                                    srv));
                        }
                        else
                        {
                            fContent.Append(string.Join(":", "1:0", srv.ServiceId, "0", "0", "0", "").ToUpperInvariant() + "\n");
                            if (Log.IsDebugEnabled) Log.Debug($"Service {srv} added to services.locked");
                        }
                    }
                }
                else
                {
                    Log.Debug("There is no locked services to put inside services.locked file");
                }

                WriteFile(fName, fContent.ToString(), Utf8NoBom);
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteServicesLocked_Failed_to_write_services_locked_file_to__0_, directory), ex);
            }
        }

        /// <summary>
        ///     Writes Enigma1 bouquets to bouquets file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successful</exception>
        protected virtual void WriteEnigma1Bouquets(string directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1Ver2)
                {
                    Log.Debug("Skipping write to bouquets file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory, BouquetsFile);
                var fContent = new StringBuilder();
                fContent.Append("eDVB bouquets /2/" + "\n" + "bouquets" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => string.Equals(x.FileName, BouquetsFile, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        int bqOrderNumber = existing.BouquetItems.Count * -1;
                        foreach (IBouquetItemBouquetsBouquet bbq in existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList())
                        {
                            if (bbq.Bouquet != null)
                            {
                                //making sure all bouquets have correct order numbers 
                                bbq.BouquetOrderNumberInt = bqOrderNumber;

                                fContent.Append(string.Join("\n", bbq.BouquetOrderNumberInt.ToString(CultureInfo.InvariantCulture), bbq.Bouquet.Name, ""));
                                foreach (IBouquetItemService srv in bbq.Bouquet.BouquetItems.OfType<IBouquetItemService>())
                                {
                                    if (srv.Service != null)
                                    {
                                        if (srv.Service.Transponder != null)
                                        {
                                            fContent.Append(string.Join(":", srv.Service.SID.PadLeft(4, '0'), srv.Service.Transponder.TransponderId.ToLowerInvariant(), Convert.ToInt32(srv.Service.ServiceType, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)) + "\n");
                                            if (Log.IsDebugEnabled) Log.Debug($"Service {srv} in bouquet {bbq.Bouquet.Name} added to Enigma1 bouquets file");
                                        }
                                        else
                                        {
                                            Log.Warn(
                                                string.Format(
                                                    Resources
                                                        .SettingsIO_WriteEnigma1Bouquets_Service__0__in_Enigma1_bouqet__0__has_no_matching_transponder__not_adding_it_to_bouquets_file_,
                                                    srv.Service, bbq.Bouquet.Name));
                                        }
                                    }
                                    else
                                    {
                                        Log.Warn(
                                            string.Format(
                                                Resources
                                                    .SettingsIO_WriteEnigma1Bouquets_Service_reference__0__has_no_matching_service__not_adding_it_to_bouquet__1_,
                                                srv.ServiceId, bbq.Bouquet.Name));
                                    }
                                }
                                fContent.Append("/" + "\n");
                                bqOrderNumber += 1;
                                if (Log.IsDebugEnabled) Log.Debug($"Enigma1 bouquet {bbq.Bouquet.Name} added to Enigma1 bouquets file");
                            }
                            else
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources
                                            .SettingsIO_WriteEnigma1Bouquets_Reference_to_Enigma1_bouquet__0__has_no_matching_bouquet_set__not_adding_it_to_bouquets_file,
                                        bbq.BouquetOrderNumber));
                            }
                        }
                    }
                    else
                    {
                        Log.Debug("Enigma1 bouquets bouquet has no items.");
                    }
                }
                else
                {
                    Log.Debug("Enigma1 Bouquets bouquet not found in the list of bouquets, assuming no Enigma1 bouquets");
                }

                fContent.Append("end" + "\n" + EditorName + "\n");

                WriteFile(fName, fContent.ToString(), Utf8NoBom);
            }
            catch (SettingsException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    $"Failed to write Enigma1 bouquets file to {directory}", ex);
            }
        }
        #endregion
        #endregion
    }
}