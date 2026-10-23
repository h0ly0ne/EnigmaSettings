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
        [Obsolete("No longer used by SettingsIO, will be removed in a future version")]
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
        [Obsolete("No longer used by SettingsIO, will be removed in a future version")]
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
        [Obsolete("No longer used by SettingsIO, will be removed in a future version")]
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
        #endregion
    }
}