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
using Krkadoni.EnigmaSettings.Interfaces;
using Krkadoni.EnigmaSettings.Properties;

namespace Krkadoni.EnigmaSettings
{
    public class SettingsIO : ISettingsIO
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
        protected const string UserBouquetTvEpl = "userbouquets.tv.epl";
        protected const string UserBouquetRadioEpl = "userbouquets.radio.epl";
        protected const string BouquetsFile = "bouquets";
        protected const string ServicesLockedFile = "services.locked";
        protected const string BouquetsTvFile = "bouquets.tv";
        protected const string BouquetsRadioFile = "bouquets.radio";
        protected const string WhiteListFile = "whitelist";
        protected const string BlackListFile = "blacklist";
        protected const string DefaultEnigma1Path = "/var/tuxbox/config/enigma/";

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
        protected void OnFileLoaded(object sender, FileInfo file, bool success)
        {
            var e = new FileLoadedEventArgs(file, success);
            EventHandler<FileLoadedEventArgs> handler = FileLoaded;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises FileLoading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnFileLoading(object sender, FileInfo file)
        {
            var e = new FileLoadingEventArgs(file);
            EventHandler<FileLoadingEventArgs> handler = FileLoading;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises  FileSaved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="success"></param>
        /// <remarks></remarks>
        protected void OnFileSaved(object sender, FileInfo file, bool success)
        {
            var e = new FileSavedEventArgs(file, success);
            EventHandler<FileSavedEventArgs> handler = FileSaved;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises FileSaving event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnFileSaving(object sender, FileInfo file)
        {
            var e = new FileSavingEventArgs(file);
            EventHandler<FileSavingEventArgs> handler = FileSaving;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Rasies SettingsLoaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsLoaded(object sender, FileInfo file, bool success, ISettings settings)
        {
            var e = new SettingsLoadedEventArgs(file, success, settings);
            EventHandler<SettingsLoadedEventArgs> handler = SettingsLoaded;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises SettingsLoading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="file"></param>
        /// <remarks></remarks>
        protected void OnSettingsLoading(object sender, FileInfo file)
        {
            var e = new SettingsLoadingEventArgs(file);
            EventHandler<SettingsLoadingEventArgs> handler = SettingsLoading;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises SettingsSaved event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="folder"></param>
        /// <param name="success"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsSaved(object sender, DirectoryInfo folder, bool success, ISettings settings)
        {
            var e = new SettingsSavedEventArgs(folder, success, settings);
            EventHandler<SettingsSavedEventArgs> handler = SettingsSaved;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        ///     Raises SettingsSaving event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="folder"></param>
        /// <param name="settings"></param>
        /// <remarks></remarks>
        protected void OnSettingsSaving(object sender, DirectoryInfo folder, ISettings settings)
        {
            var e = new SettingsSavingEventArgs(folder, settings);
            EventHandler<SettingsSavingEventArgs> handler = SettingsSaving;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion

        #region "Properties"

        private readonly IInstanceFactory _factory;


        private string _editorName = "Generated by EnigmaSettings - http://www.krkadoni.com";
        private ILog _log;

        public string EditorName
        {
            get { return _editorName; }
            set
            {
                if (value == null)
                    value = string.Empty;
                if (value == _editorName) return;
                _editorName = value;
                OnPropertyChanged("EditorName");
            }
        }

        /// <summary>
        ///     Implementation of instance factory used to instatiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IInstanceFactory Factory
        {
            get { return _factory; }
        }

        /// <summary>
        ///     Log4Net logger to be used for log output
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ILog Log
        {
            get { return _log; }
            set
            {
                if (value == null) return;
                if (value == _log) return;
                _log = value;
                OnPropertyChanged("Log");
            }
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        ///     Uses custom instance factory implementation to instatiate objects
        /// </summary>
        /// <param name="factory"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if factory is null</exception>
        public SettingsIO(IInstanceFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(Resources.SettingsIO_New_Invalid_instance_factory_);
            _factory = factory;
        }

        /// <summary>
        ///     Uses default InstanceFactory to instatiate objects
        /// </summary>
        /// <remarks></remarks>
        public SettingsIO()
        {
            _factory = new InstanceFactory();
        }

        /// <summary>
        ///     Loads up and links all the settings data
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="satellitesIO">Implementation of reading/writing satellites file</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Load(FileInfo settingsFile, IXmlSatellitesIO satellitesIO)
        {
            try
            {
                if (settingsFile == null)
                {
                    Log.Error(Resources.Settings_LoadSettings_Invalid_filename_);
                    throw new SettingsException(Resources.Settings_LoadSettings_Invalid_filename_);
                }

                if (!settingsFile.Exists)
                {
                    Log.Error(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, settingsFile));
                    throw new SettingsException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, settingsFile));
                }

                OnSettingsLoading(this, settingsFile);

                Log.Info(string.Format(Resources.Settings_Load_Loading_settings_from__0_, settingsFile));

                var st = new Stopwatch();
                st.Start();

                ISettings settings = ReadSettingsFile(settingsFile);
                settings.SettingsFileName = settingsFile.FullName;
                settings.MatchServicesWithTransponders();
                IList<IXmlSatellite> sats;
                try
                {
                    sats = satellitesIO.LoadSatellitesFromFile(Path.Combine(settings.SettingsDirectory, SatelliteXmlFileName)).ToList();
                }
                catch (Exception ex)
                {
                    OnSettingsLoaded(this, settingsFile, false, null);
                    Log.Warn(string.Format("There was an error while reading {0} file.", SatelliteXmlFileName), ex);
                    sats = new List<IXmlSatellite>();
                }

                foreach (var sat in sats)
                {
                    settings.Satellites.Add(sat);
                }

                settings.MatchSatellitesWithTransponders();
                settings.AddMissingXmlSatellites(_factory);
                if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
                {
                    ReadFileBouquet(new FileInfo(Path.Combine(settings.SettingsDirectory, UserBouquetTvEpl)), ref settings);
                    ReadFileBouquet(new FileInfo(Path.Combine(settings.SettingsDirectory, UserBouquetRadioEpl)), ref settings);
                    ReadE1Bouquets(new FileInfo(Path.Combine(settings.SettingsDirectory, BouquetsFile)), ref settings);
                    settings.MatchBouquetsBouquetsServices();
                    settings.MatchFileBouquetsServices();
                    ReadServicesLocked(new FileInfo(Path.Combine(settings.SettingsDirectory, ServicesLockedFile)), ref settings);
                }
                else if (settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver3 || settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver4)
                {
                    ReadFileBouquet(new FileInfo(Path.Combine(settings.SettingsDirectory, BouquetsTvFile)), ref settings);
                    ReadFileBouquet(new FileInfo(Path.Combine(settings.SettingsDirectory, BouquetsRadioFile)), ref settings);
                    settings.MatchFileBouquetsServices();
                    ReadBlackWhiteList(new FileInfo(Path.Combine(settings.SettingsDirectory, WhiteListFile)), ref settings);
                    ReadBlackWhiteList(new FileInfo(Path.Combine(settings.SettingsDirectory, BlackListFile)), ref settings);
                }

                st.Stop();
                Log.Info(string.Format("Settings loaded in {0} ms", st.ElapsedMilliseconds));
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
        }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ISettings Load(FileInfo settingsFile)
        {
            IXmlSatellitesIO satellitesIO = _factory.InitNewXmlSatelliteIO();
            return Load(settingsFile, satellitesIO);
        }

        /// <summary>
        ///     Loads up and links all the settings data with XmlSatelliteIO from Factory asynchronusly
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public void LoadAsync(FileInfo settingsFile, AsyncCallback callback)
        {
            new Func<FileInfo, ISettings>(Load).BeginInvoke(settingsFile, callback, null);
        }

        /// <summary>
        ///     Loads up and links all the settings data asynchronusly
        /// </summary>
        /// <param name="settingsFile">Full path to lamedb or services file</param>
        /// <param name="xmlSatellitesIO"></param>
        /// <param name="callback">Async callback to be called after load finishes</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public void LoadAsync(FileInfo settingsFile, IXmlSatellitesIO xmlSatellitesIO, AsyncCallback callback)
        {
            new Func<FileInfo, IXmlSatellitesIO, ISettings>(Load).BeginInvoke(settingsFile, xmlSatellitesIO, callback, null);
        }

        /// <summary>
        ///     Saves settings to disk
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="xmlSatellitesIO">Instance of satellite.xml writer implementation</param>
        /// <remarks></remarks>
        public void Save(DirectoryInfo folder, ISettings settings, IXmlSatellitesIO xmlSatellitesIO)
        {
            Log.Info(string.Format("Saving settings version {0} to {1}", settings.SettingsVersion, folder));
            OnSettingsSaving(this, folder, settings);
            if (!folder.Exists)
            {
                try
                {
                    folder.Create();
                    Log.Debug(string.Format("Folder {0} successfully created", folder));
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
                Log.Debug(string.Format("Folder {0} exists", folder));
            }
            var st = new Stopwatch();
            st.Start();

            try
            {
                if (settings.FindBouquetsWithDuplicateFilename().Any())
                {
                    OnSettingsSaved(this, folder, false, settings);
                    throw new SettingsException(Resources.SettingsIO_Save_Bouquet_filenames_are_not_unique_);
                }

                //settings.RenumberMarkers();
                //settings.RenumberBouquetFileNames();
                WriteSettingsFile(folder, settings);
                WriteBouquets(folder, settings);

                IXmlSatellitesIO satellitesIO = Factory.InitNewXmlSatelliteIO();
                satellitesIO.SaveSatellitesToFile(Path.Combine(folder.FullName, SatelliteXmlFileName), settings);
                st.Stop();
                Log.Info(string.Format("Settings saved successfully in {0} ms.", st.ElapsedMilliseconds));
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
        ///     Saves settings to disk, initializes default satellites.xml writer
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <remarks></remarks>
        public void Save(DirectoryInfo folder, ISettings settings)
        {
            IXmlSatellitesIO satellitesIO = _factory.InitNewXmlSatelliteIO();
            Save(folder, settings, satellitesIO);
        }

        /// <summary>
        ///     Saves settings to disk, initializes default satellites.xml writer asynchronusly
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        /// <remarks></remarks>
        public void SaveAsync(DirectoryInfo folder, ISettings settings, AsyncCallback callback)
        {
            new Action<DirectoryInfo, ISettings>(Save).BeginInvoke(folder, settings, callback, null);
        }

        /// <summary>
        ///     Saves settings to disk asynchronusly
        /// </summary>
        /// <param name="folder">Directory where all settings files will be saved</param>
        /// <param name="settings">Settings instance with all the data</param>
        /// <param name="xmlSatellitesIO">Instance of satellite.xml writer implementation</param>
        /// <param name="callback">Async callback to be called after save finishes</param>
        /// <remarks></remarks>
        public void SaveAsync(DirectoryInfo folder, ISettings settings, IXmlSatellitesIO xmlSatellitesIO, AsyncCallback callback)
        {
            new Action<DirectoryInfo, ISettings, IXmlSatellitesIO>(Save).BeginInvoke(folder, settings, xmlSatellitesIO, callback, null);
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
        protected virtual string[] Read(FileInfo fileName, bool warnEmpty = false)
        {
            if (fileName == null)
                throw new ArgumentNullException(Resources.Settings_Read_Filename_cannot_be_empty_);
            if (!fileName.Exists)
                throw new ArgumentException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fileName.FullName));

            OnFileLoading(this, fileName);
            Log.Debug(string.Format(Resources.Settings_Read_Reading_file__0_, fileName.FullName));

            var st = new Stopwatch();
            string[] fileLines;
            st.Start();

            try
            {
                fileLines = File.ReadAllLines(fileName.FullName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileLoaded(this, fileName, false);
                throw new SettingsException(string.Format(Resources.Settings_Read_Failed_to_read_file__0__1__2_, fileName.FullName, "", ""), ex);
            }

            st.Stop();

            Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName.FullName, st.ElapsedMilliseconds));

            if (fileLines.Length == 0)
            {
                if (warnEmpty)
                    Log.Warn(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fileName.FullName));
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
        protected virtual string ReadText(FileInfo fileName, bool warnEmpty = true)
        {
            if (fileName == null)
                throw new ArgumentNullException(Resources.Settings_Read_Filename_cannot_be_empty_);
            if (!fileName.Exists)
                throw new ArgumentException(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fileName.FullName));
            OnFileLoading(this, fileName);
            Log.Debug(string.Format(Resources.Settings_Read_Reading_file__0_, fileName.FullName));

            var st = new Stopwatch();
            string fileText;
            st.Start();

            try
            {
                fileText = File.ReadAllText(fileName.FullName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileLoaded(this, fileName, false);
                throw new SettingsException(string.Format(Resources.Settings_Read_Failed_to_read_file__0__1__2_, fileName.FullName, "", ""), ex);
            }

            st.Stop();

            Log.Debug(string.Format(Resources.Settings_Read_File__0__read_in__1__ms, fileName.FullName, st.ElapsedMilliseconds));

            if (fileText.Length == 0)
            {
                if (warnEmpty)
                    Log.Warn(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fileName.FullName));
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
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                    Log.Debug(string.Format("Existing file {0} successfully deleted", fileName));
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
            var fi = new FileInfo(fileName);
            OnFileSaving(this, fi);
            Log.Debug(string.Format("Writing file {0} to disk", fileName));
            try
            {
                File.WriteAllText(fileName, fContent, encoding);
                Log.Debug(string.Format("File {0} successfully written.", fileName));
                OnFileSaved(this, fi, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fi, false);
                throw new SettingsException(string.Format("There was an error while writing file {0} to disk", fileName), ex);
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
                    return Enums.SettingsVersion.Enigma1V1;
                case "2":
                    return Enums.SettingsVersion.Enigma1;
                case "3":
                    return Enums.SettingsVersion.Enigma2Ver3;
                case "4":
                    return Enums.SettingsVersion.Enigma2Ver4;
                default:
                    return Enums.SettingsVersion.Unknown;
            }
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
                case Enums.SettingsVersion.Enigma1V1:
                    return 1;
                case Enums.SettingsVersion.Enigma1:
                    return 2;
                case Enums.SettingsVersion.Enigma2Ver3:
                    return 3;
                case Enums.SettingsVersion.Enigma2Ver4:
                    return 4;
                default:
                    return Convert.ToInt32(version);
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
            switch (fileName.ToLower())
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
                    if (fileName.ToLower().EndsWith(".tv"))
                    {
                        return "TV Bouquet";
                    }
                    return fileName.ToLower().EndsWith(".radio") ? "Radio Bouquet" : string.Empty;
            }
        }

        #endregion

        #region "Reading"

        /// <summary>
        ///     Reads file content from the disk, parses content, sets version number, initializes new lists of Transponders and
        ///     services
        /// </summary>
        /// <param name="fileName">Full path to settings file on the disk</param>
        /// <remarks>Does not match transponders and services</remarks>
        protected virtual ISettings ReadSettingsFile(FileInfo fileName)
        {
            try
            {
                ISettings settings = _factory.InitNewSettings();
                string[] settingsFileLines = Read(fileName);
                CheckIfSettingsValid(settingsFileLines, fileName);
                ReadSettingsVersion(settings, settingsFileLines, fileName);
                int transponderOpenIndex;
                int transponderEndIndex = 0;
                ReadTransponders(settings, settingsFileLines, fileName, out transponderOpenIndex, out transponderEndIndex);
                ReadServices(settings, settingsFileLines, fileName, transponderOpenIndex, transponderEndIndex);
                Log.Debug(string.Format("Settings loaded with {0} services and {1} transponders", settings.Services.Count, settings.Transponders.Count));
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
                throw new SettingsException(
                    string.Format(Resources.Settings_ReadServicesFile_There_was_an_error_while_reading_services_0__1_, Environment.NewLine, ex.Message), ex);
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
        protected virtual void CheckIfSettingsValid(string[] settingsFileLines, FileInfo fileName)
        {
            Log.Debug(string.Format("Checking {0} to make sure we have valid settings", fileName.FullName));
            if (settingsFileLines == null || settingsFileLines.Length == 0)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_empty_, fileName.Name));
            if (!settingsFileLines[0].ToLower().StartsWith(SettingsFirstLine.ToLower()))
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fileName.Name));
            if (!settingsFileLines[0].Trim().EndsWith("/"))
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fileName.Name));
            Log.Debug(string.Format("File {0} looks like a valid settings file, checking for version", fileName.Name));
        }

        /// <summary>
        ///     Reads settings version and sets SettingsVersion property for specified settings instance
        /// </summary>
        /// <param name="settings">Instance of ISettings we're setting settings version to</param>
        /// <param name="settingsFileLines">Content of settings file in string array of lines</param>
        /// <param name="fileName">Full path to settings file on disk</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if settings version can't be determined</exception>
        protected virtual void ReadSettingsVersion(ISettings settings, string[] settingsFileLines, FileInfo fileName)
        {
            Log.Debug("Looking for settings version");
            string sVersion = settingsFileLines[0].ToLower().Trim().Replace(SettingsFirstLine.ToLower(), "").TrimEnd('/');
            int i;
            if (sVersion == null || !Int32.TryParse(sVersion, out i))
            {
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_File__0__is_not_valid_settings_file_, fileName.Name));
            }
            settings.SettingsVersion = GetSettingsVersion(sVersion);
            if (settings.SettingsVersion == Enums.SettingsVersion.Unknown)
            {
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_Unsuported_settings_version__0_, sVersion));
            }
            Log.Debug(string.Format("Found settings version {0}", settings.SettingsVersion));
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
        protected virtual void ReadTransponders(ISettings settings, string[] settingsFileLines, FileInfo fileName, out int transponderOpenIndex,
            out int transponderEndIndex)
        {

            Log.Debug(string.Format("Reading transponders from {0}", fileName.FullName));
            //search for the begining of transponder section
            transponderOpenIndex = -1;
            transponderEndIndex = -1;
            int sCount = settingsFileLines.Count();
            for (int i = 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsTransponderOpenTag) continue;
                transponderOpenIndex = i;
                break;
            }
            if (transponderOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_No__transponder__tag_was_found_in__0_, fileName.Name));

            Log.Debug(string.Format("Transponder section found at line {0}", transponderOpenIndex + 1));

            //reading transponders
            //make sure that we can progress by 3 lines without exception at the end
            int secureLastIndex = (sCount - (transponderOpenIndex + 1));
            secureLastIndex = secureLastIndex - (secureLastIndex % 3);

            for (int i = transponderOpenIndex + 1; i <= secureLastIndex - 1; i += 3)
            {
                if (settingsFileLines[i] != SettingsClosingTag)
                {
                    switch (settingsFileLines[i + 1].Trim().ToLower().Substring(0, 1))
                    {
                        case "s":
                            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
                            {
                                //DVBT & DVBC transponders in Enigma1 are saved as satellite transponders
                                if (settingsFileLines[i].ToLower().StartsWith("ffff"))
                                {
                                    AddTransponderDVBC(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                }
                                else if (settingsFileLines[i].ToLower().StartsWith("eeee"))
                                {
                                    AddTransponderDVBT(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                }
                                else
                                {
                                    AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                                }
                            }
                            else
                            {
                                AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            }
                            break;
                        case "t":
                            AddTransponderDVBT(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            break;
                        case "c":
                            AddTransponderDVBC(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            break;
                        default:
                            Log.Warn(
                                string.Format(
                                    Resources
                                        .SettingsIO_ReadSettingsFile_Unknown_transponder_type_for_transponder__0___1___adding_it_as_satellite_transponder,
                                    settingsFileLines[i], settingsFileLines[i + 1]));
                            AddTransponderDVBS(settings, settingsFileLines[i], settingsFileLines[i + 1]);
                            break;
                    }
                }
                else
                {
                    //we have reached the end of transponders part
                    Log.Debug(string.Format("Transponder section ends at line {0}", transponderOpenIndex + i));
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
        protected virtual void ReadServices(ISettings settings, string[] settingsFileLines, FileInfo fileName, int transponderOpenIndex, int transponderEndIndex)
        {
            Log.Debug(string.Format("Reading services from {0}", fileName.FullName));

            //search for the begining of services section after transponders section
            int servicesOpenIndex = -1;
            int sCount = settingsFileLines.Count();
            for (int i = transponderEndIndex + 1; i <= sCount - 1; i++)
            {
                if (settingsFileLines[i].Trim() != SettingsServicesOpenTag) continue;
                servicesOpenIndex = i;
                break;
            }
            if (servicesOpenIndex == -1)
                throw new SettingsException(string.Format(Resources.Settings_ReadServicesFile_No__services__tag_was_found_in_file__0_, fileName.Name));

            Log.Debug(string.Format("Services section found at line {0}", transponderOpenIndex + servicesOpenIndex));

            //reading services
            //make sure that we can progress by 3 lines without exception at the end
            int secureLastIndex = (sCount - (servicesOpenIndex + 1));
            secureLastIndex = secureLastIndex - (secureLastIndex % 3);
            secureLastIndex += servicesOpenIndex;

            Log.Debug(string.Format("Services section should end at line {0}", secureLastIndex + transponderOpenIndex + 1));

            for (int i = servicesOpenIndex + 1; i <= secureLastIndex - 1; i += 3)
            {
                if (settingsFileLines[i] == SettingsClosingTag) continue;
                string line1 = settingsFileLines[i];
                string line2 = settingsFileLines[i + 1];
                string line3 = settingsFileLines[i + 2];
                Log.Debug(string.Format("Initializing new service {0}    {1}    {2}", line1, line2, line3));
                settings.Services.Add(_factory.InitNewService(line1, line2, line3));
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
            Log.Debug(string.Format("Initializing new terrestrial transponder {0} {1}", firstLine, secondLine));
            settings.Transponders.Add(_factory.InitNewTransponderDVBT(firstLine, secondLine));
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
            Log.Debug(string.Format("Initializing new cable transponder {0} {1}", firstLine, secondLine));
            settings.Transponders.Add(_factory.InitNewTransponderDVBC(firstLine, secondLine));
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
            Log.Debug(string.Format("Initializing new satellite transponder {0} {1}", firstLine, secondLine));
            settings.Transponders.Add(_factory.InitNewTransponderDVBS(firstLine, secondLine));
        }

        /// <summary>
        ///     Reads bouquet from disk and parses content. Goes recursively if bouqet has other bouquets as items.
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>Nothing if bouquet type is unknown. Type is recognized from filename</returns>
        /// <remarks>Adds bouquet to Bouquets list if not already in it, otherwise updates existing</remarks>
        protected virtual IFileBouquet ReadFileBouquet(FileInfo fileName, ref ISettings settings)
        {
            try
            {
                IFileBouquet bqt = _factory.InitNewFileBouquet();
                bqt.FileName = fileName.Name;

                //don't read bouquets that have their own methods for reading
                if (bqt.BouquetType == Enums.BouquetType.E1Bouquets || fileName.Name.ToLower() == WhiteListFile || fileName.Name.ToLower() == BlackListFile ||
                    fileName.Name.ToLower() == ServicesLockedFile)
                {
                    throw new ArgumentException();
                }

                //dont read unknown bouquet types
                if (bqt.BouquetType == Enums.BouquetType.Unknown)
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquets_File__0__is_unknown_bouquet_type, fileName.Name));
                    return null;
                }

                if (!fileName.Exists)
                {
                    Log.Warn(string.Format(Resources.Settings_LoadSettings_File__0__does_not_exist_, fileName.Name));
                    return null;
                }
                IFileBouquet existing =
                    settings.Bouquets.OfType<IFileBouquet>()
                        .SingleOrDefault(x => String.Equals(x.FileName, fileName.Name, StringComparison.InvariantCulture));
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    if (bqt.BouquetType != Enums.BouquetType.Unknown
                        && fileName.Name.ToLower() != BouquetsTvFile
                        && fileName.Name.ToLower() != BouquetsRadioFile
                        && fileName.Name.ToLower() != UserBouquetTvEpl
                        && fileName.Name.ToLower() != UserBouquetRadioEpl)
                    {
                        settings.Bouquets.Add(bqt);
                    }
                }

                Log.Debug(string.Format("Reading bouquet {0}", bqt.FileName));
                string[] bouquetLines = Read(fileName, true);

                if (bouquetLines == null || !bouquetLines.Any())
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_Bouquet__0__is_empty, bqt.FileName));
                }
                else
                {
                    DirectoryInfo dir = fileName.Directory;
                    var cnt = bouquetLines.Length;
                    for (int i = 0; i <= cnt - 1; i++)
                    {
                        string line = bouquetLines[i];
                        if (line.Trim().Length > 0)
                        {
                            DetectAndReadReference(dir, ref bqt, bouquetLines, i, settings);
                        }
                    }
                    Log.Debug(string.Format("Bouquet {0} successfully parsed with {1} items", bqt.FileName, cnt));
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
                throw new SettingsException(string.Format(Resources.Settings_ReadBouquet_Failed_to_read_bouquet__0_, fileName.Name), ex);
            }
        }

        /// <summary>
        ///     Determines type of line and initializes corresponding bouquet item type
        /// </summary>
        /// <param name="folder">Directory with all the settings files</param>
        /// <param name="bouquet">Parent bouquet that this item belongs to</param>
        /// <param name="bouquetLines">Bouquet file content as string array</param>
        /// <param name="index">Index of the current bouquet line</param>
        /// <param name="settings">Reference to existing settings instance</param>
        /// <remarks></remarks>
        protected virtual void DetectAndReadReference(DirectoryInfo folder, ref IFileBouquet bouquet, string[] bouquetLines, int index, ISettings settings)
        {
            string line = bouquetLines[index];
            string description = string.Empty;
            int bCount = bouquetLines.Count();
            string nextLine = index < (bCount - 1) ? bouquetLines[index + 1] : string.Empty;
            if (nextLine.ToUpper().StartsWith("#DESCRIPTION"))
                description = bouquetLines[index + 1];

            if (line.StartsWith("#NAME"))
            {
                bouquet.Name = line.Substring(line.Length >= 6 ? 6 : 5);
                Log.Debug(string.Format("Found bouquet NAME: {0}", line));
            }
            else if (line.StartsWith("#SERVICE"))
            {
                string serviceData = line.Substring(9).Trim();
                string[] sData = serviceData.Split(':');
                string favoritesType = sData[0];
                var lineType = (Enums.LineSpecifier)Enum.Parse(typeof(Enums.LineSpecifier), sData[1]);
                int[] directoryTypes =
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    9,
                    10,
                    11,
                    12,
                    13,
                    14,
                    15
                };
                if (lineType == Enums.LineSpecifier.Unknown
                    || favoritesType == Convert.ToInt32(Enums.FavoritesType.Unknown).ToString(CultureInfo.InvariantCulture))
                {
                    Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line,
                        Path.Combine(folder.FullName, bouquet.Name)));
                }
                else if (directoryTypes.Contains(Convert.ToInt32(sData[1]))
                         || favoritesType == Convert.ToInt32(Enums.FavoritesType.File).ToString(CultureInfo.InvariantCulture))
                {
                    Log.Debug(string.Format("Found (sub)directory item {0}", sData[10]));
                    var tmpName = sData[10];
                    //workaround for FROM BOUQUET "userbouquet.dbe01.radio" ORDER BY bouquet
                    if (tmpName.IndexOf(@"""", System.StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        Log.Debug("Non standard bouquet name, try to workaround it");
                        tmpName = tmpName.Substring(tmpName.IndexOf(@"""", System.StringComparison.InvariantCultureIgnoreCase) + 1);
                        tmpName = tmpName.Substring(0, tmpName.IndexOf(@"""", System.StringComparison.InvariantCultureIgnoreCase));
                        tmpName = tmpName.Trim();
                        sData[10] = tmpName;
                        Log.Debug(string.Format("Workaround completed with result {0}", tmpName));
                    }
                    string fName = Path.GetFileName(sData[10]);
                    if (fName == null) return;
                    IFileBouquet subBqt = ReadFileBouquet(new FileInfo(Path.Combine(folder.FullName, fName)), ref settings);
                    if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1
                        || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
                    {
                        subBqt.FileName = sData[10];
                    }
                    if (subBqt != null)
                    {
                        IBouquetItemFileBouquet bib = _factory.InitNewBouquetItemFileBouquet(subBqt);
                        bib.FavoritesTypeFlag = sData[0];
                        bib.LineSpecifierFlag = sData[1];
                        bouquet.BouquetItems.Add(bib);
                        Log.Debug(string.Format("Bouquet {0} added as bouquet item to bouquet {1} ", Path.GetFileName(sData[10]), bouquet.Name));
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
                                    Log.Debug(string.Format("Marker {0} added to bouquet {1}", bqim.Description, bouquet.Name));
                                }
                                else
                                {
                                    Log.Warn(string.Format("Invalid marker {0} not added to bouquet {1}", line, bouquet.Name));
                                }
                            }
                            break;
                        case Enums.LineSpecifier.NormalService:
                            if (favoritesType != null
                                && (
                                    (Enums.FavoritesType)Enum.Parse(typeof(Enums.FavoritesType), favoritesType) == Enums.FavoritesType.Stream
                                    || sData[10].IndexOf("//", StringComparison.InvariantCulture) > -1)
                                    || (sData.Length == 12 && sData[11] != null)
                                )
                            {
                                //it's stream
                                IBouquetItemStream bqis = ReadStreamReference(line, description);
                                if (bqis != null)
                                {
                                    bouquet.BouquetItems.Add(bqis);
                                    if (description.Length == 0)
                                    {
                                        Log.Warn(string.Format(Resources.Settings_ReadBouquet_Stream_service__1__added_to_bouquet__0__without_description,
                                            bouquet.Name, bqis.Description));
                                    }
                                    else
                                    {
                                        Log.Debug(string.Format("Stream service {0} added to bouquet {1}", bqis.Description, bouquet.Name));
                                    }
                                }
                                else
                                {
                                    Log.Warn(string.Format(Resources.SettingsIO_ReadFileBouquet_Invalid_stream_reference__0__not_added_to_bouquet__1_, line,
                                        bouquet.Name));
                                }
                            }
                            else
                            {
                                //it's regular DVB service
                                IBouquetItemService bqis = ReadServiceReference(line);
                                if (bqis != null)
                                {
                                    bouquet.BouquetItems.Add(bqis);
                                    Log.Debug(string.Format("DVB service {0} added to bouquet {1}", serviceData, bouquet.Name));
                                }
                                else
                                {
                                    Log.Warn(string.Format(Resources.SettingsIO_ReadFileBouquet_Invalid_service_reference__0__not_added_to_bouquet__1_,
                                        line, bouquet.Name));
                                }
                            }
                            break;
                    }
            }
            else if (line.StartsWith("#TYPE"))
            {
            }
            else if (line.StartsWith("#DESCRIPTION"))
            {
            }
            else if (line.StartsWith("/"))
            {
            }
            else
            {
                Log.Warn(string.Format(Resources.Settings_ReadBouquet_Unsupported_line___1___in_bouquet__0_, Path.GetFileName(bouquet.FileName), line));
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemService object from bouquetline
        /// </summary>
        /// <param name="bouquetLine">Line from bouquet file including #SERVICE tag</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="SettingsException"></exception>
        protected virtual IBouquetItemService ReadServiceReference(string bouquetLine)
        {
            try
            {
                if (bouquetLine.StartsWith("#SERVICE"))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                return Factory.InitNewBouquetItemService(bouquetLine);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);   
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_ReadServiceReference_Failed_to_initialize_new_service_from_line__0_, bouquetLine), ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemMarker object from bouquetline
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
                if (bouquetLine.ToUpper().StartsWith("#SERVICE"))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                if (description == null)
                {
                    description = string.Empty;
                }
                else if (description.ToUpper().StartsWith("#DESCRIPTION:"))
                {
                    description = description.Substring(13);
                }
                else if (description.ToUpper().StartsWith("#DESCRIPTION"))
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
                    bouquetLine),ex);
            }
        }

        /// <summary>
        ///     Initializes new IBouquetItemStream object from bouquetline
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
                if (bouquetLine.ToUpper().StartsWith("#SERVICE"))
                    bouquetLine = bouquetLine.Substring(9).Trim();
                if (description == null)
                {
                    description = string.Empty;
                }
                else if (description.ToUpper().StartsWith("#DESCRIPTION:"))
                {
                    description = description.Substring(13);
                }
                else if (description.ToUpper().StartsWith("#DESCRIPTION"))
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
                    Resources.SettingsIO_ReadStreamReference_Failed_to_initialize_new_stream_reference_from_line__0_, bouquetLine),ex);
            }
        }

        /// <summary>
        ///     Reads bouquets file from Enigma1
        /// </summary>
        /// <param name="fileName">Full path to file on disk</param>
        /// <param name="settings"></param>
        /// <returns>bouquets Bouquet with other bouquets as bouquet items</returns>
        /// <remarks></remarks>
        protected virtual IFileBouquet ReadE1Bouquets(FileInfo fileName, ref ISettings settings)
        {
            Log.Debug("Reading Enigma1 bouquets file");

            try
            {
                IFileBouquet bqt = _factory.InitNewFileBouquet();
                bqt.FileName = BouquetsFile;
                bqt.Name = BouquetsFile;

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => x.FileName.ToLower() == BouquetsFile);
                if (existing != null)
                {
                    bqt = existing;
                    existing.BouquetItems.Clear();
                }
                else
                {
                    settings.Bouquets.Add(bqt);
                }

                string bouquetString = ReadText(fileName, false);

                //remove first line
                bouquetString = bouquetString.Substring(bouquetString.IndexOf("\n", StringComparison.InvariantCulture) + 1);

                //search for the begining of bouquets
                int bouquetsOpenIndex = bouquetString.IndexOf(BouquetsOpenTag, StringComparison.InvariantCulture) + BouquetsOpenTag.Length + 1;
                int bouquetsEndIndex = bouquetString.LastIndexOf(SettingsClosingTag, StringComparison.InvariantCulture);

                if (bouquetsOpenIndex == -1)
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, BouquetsOpenTag, bqt.FileName));
                    return bqt;
                }
                bouquetString = bouquetString.Substring(bouquetsOpenIndex);

                if (bouquetsEndIndex == -1 || (bouquetsEndIndex < bouquetsOpenIndex))
                {
                    Log.Warn(string.Format(Resources.Settings_ReadBouquet_No___0___tag_was_found_in_file__1_, SettingsClosingTag, bqt.FileName));
                    return bqt;
                }
                bouquetString = bouquetString.Substring(0, bouquetsEndIndex);

                string[] bouquetsE1 = bouquetString.Split('/');

                for (int a = 0; a <= bouquetsE1.Count() - 2; a++)
                {
                    string bE1 = bouquetsE1[a].Trim();
                    string[] bouquetLines = bE1.Split('\n');
                    if (bouquetLines.Length > 1)
                    {
                        IBouquetsBouquet bq = _factory.InitNewBouquetsBouquet();
                        bq.Name = bouquetLines[1];
                        IBouquetItemBouquetsBouquet bib = _factory.InitNewBouquetItemBouquetsBouquet(bq);
                        bib.BouquetOrderNumberInt = Convert.ToInt32(bouquetLines[0]);
                        bqt.BouquetItems.Add(bib);
                        Log.Debug(string.Format("Found Enigma1 bouquet {0} in {1}", bouquetLines[1], fileName.FullName));
                        for (int i = 2; i <= bouquetLines.Count() - 1; i++)
                        {
                            if (bouquetLines[i].Trim().Length > 0)
                            {
                                if (bouquetLines[i].Split(':').Length >= 5)
                                {
                                    IBouquetItemService bis = _factory.InitNewBouquetItemService(bouquetLines[i]);
                                    bq.BouquetItems.Add(bis);
                                    Log.Debug(string.Format("Added service {0} to Enigma1 bouquet {1}", bouquetLines[i], bouquetLines[1]));
                                }
                                else
                                {
                                    Log.Warn(
                                        string.Format(
                                            Resources.Settings_ReadE1Bouquets_Ignoring_line__0__in_Enigma1_bouquet__1___doesn_t_look_like_regular_service,
                                            bouquetLines[i], bouquetLines[1]));
                                }
                            }
                        }
                    }
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
                throw new SettingsException(string.Format(Resources.SettingsIO_ReadE1Bouquets_Failed_to_read_Enigma1_bouquets_file_), ex);
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
        protected virtual void ReadBlackWhiteList(FileInfo fileName, ref ISettings settings)
        {
            var sLocked = new List<IBouquetItemService>();

            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
            {
                return;
            }
            if (fileName == null)
            {
                throw new ArgumentNullException();
            }
            if (!fileName.Exists)
            {
                return;
            }
            string[] bouquetFileLines = Read(fileName);
            sLocked.AddRange(bouquetFileLines.Select(ReadServiceReference).Where(bqis => bqis != null));

            if (!sLocked.Any()) return;
            bool blackListed = (fileName.Name.ToLower() == BlackListFile);

            //matching  services
            try
            {
                Log.Debug(!blackListed ? "Matching whitelisted services." : "Matching blacklisted services.");

                var query = settings.Services.Join(sLocked, srv => srv.ServiceId.ToLower(), bi => bi.ServiceId.ToLower(), (srv, x) => new
                {
                    Service = srv,
                    LockedBouquetItem = x
                });

                foreach (var match in query.ToList())
                {
                    if (blackListed)
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                        Log.Debug(string.Format("Service {0} has been marked as BLACKLISTED", match.Service));
                    }
                    else
                    {
                        match.Service.ServiceSecurity = Enums.ServiceSecurity.WhiteListed;
                        Log.Debug(string.Format("Service {0} has been marked as WHITELISTED", match.Service));
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
        protected virtual void ReadServicesLocked(FileInfo fileName, ref ISettings settings)
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
                        Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Invalid_line__0__found_in__1_, line, fileName.FullName));
                    }
                    else
                    {
                        string[] sData = line.Split(':');
                        IBouquetItemService srv = Factory.InitNewBouquetItemService(line);

                        if (srv.LineSpecifierType == Enums.LineSpecifier.Unknown)
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Found_unknown_reference_type__0__in__1_, line, fileName.FullName));
                        }
                        else if (srv.LineSpecifierType.ToString().ToLower().StartsWith("isdirectory"))
                        {
                            Log.Debug(string.Format("Found reference to locked bouquet {0} in {1}", line, fileName.FullName));
                            try
                            {
                                int orderNumber = Int32.Parse(sData[4], NumberStyles.HexNumber);
                                sLocked.Add(Factory.InitNewBouquetItemBouquetsBouquet(orderNumber));
                            }
                            catch (Exception ex)
                            {
                                Log.Warn(
                                    string.Format(
                                        Resources
                                            .SettingsIO_ReadServicesLocked_Unable_to_parse_bouquet_order_number__0__to_integer_for_bouquet__1__on_line__2_,
                                        sData[4], fileName.FullName, line));
                                Log.Warn(ex.ToString());
                            }
                        }
                        else if (srv.LineSpecifierType == Enums.LineSpecifier.NormalService)
                        {
                            sLocked.Add(srv);
                            Log.Debug(string.Format("Found reference to locked service {0} in {1}", line, fileName.FullName));
                        }
                        else
                        {
                            Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Unsupported_type__0__found_in_line__1__in_file__2_,
                                srv.LineSpecifierType, line, fileName.FullName));
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
                    var query = bis.Join(sLocked.OfType<IBouquetItemBouquetsBouquet>().ToList(), bs => bs.BouquetOrderNumber.ToLower(),
                        x => x.BouquetOrderNumber.ToLower(), (bs, x) => new
                        {
                            BouquetsBouquetItem = bs,
                            LockedBouquetItem = x
                        });

                    foreach (var match in query.ToList())
                    {
                        if (match.BouquetsBouquetItem.Bouquet != null)
                        {
                            match.BouquetsBouquetItem.Bouquet.Locked = true;
                            Log.Debug(string.Format("Bouquet {0} has been marked as LOCKED", match.BouquetsBouquetItem.Bouquet.Name));
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
                var query = settings.Services.Join(sLocked.OfType<IBouquetItemService>().ToList(), srv => srv.ServiceId.ToLower(),
                    bi => bi.ServiceId.ToLower(), (srv, x) => new
                    {
                        Service = srv,
                        LockedBouquetItem = x
                    });

                foreach (var match in query.ToList())
                {
                    match.Service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
                    Log.Debug(string.Format("Service {0} has been marked as LOCKED", match.Service));
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
                var service = bi as IBouquetItemService;
                if (service != null)
                {
                    Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Locked_bouquet_item__0__has_no_matching_service, service.ServiceId));
                }
                else
                {
                    var bouquet = bi as IBouquetItemBouquetsBouquet;
                    if (bouquet != null)
                    {
                        Log.Warn(string.Format(Resources.SettingsIO_ReadServicesLocked_Bouquet_item__0__has_no_matching_Enigma1_bouquet_,
                            bouquet.BouquetOrderNumber));
                    }
                }
            }
        }

        #endregion

        #region "Writing"

        protected virtual void WriteSettingsFile(DirectoryInfo directory, ISettings settings)
        {
            Log.Debug("Writing settings file to disk");
            var sContent = new StringBuilder();
            string fileName = "lamedb";
            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
            {
                fileName = "services";
            }
            var fi = new FileInfo(Path.Combine(directory.FullName, fileName));
            OnFileSaving(this, fi);
            try
            {
                sContent.Append(SettingsFirstLine + GetSettingsVersion(settings.SettingsVersion) + "/" + "\n");
                sContent.Append(TranspondersToString(settings));
                sContent.Append(ServicesToString(settings));
                sContent.Append(EditorName + "\n");

                File.WriteAllText(Path.Combine(directory.FullName, fileName), sContent.ToString());
                Log.Debug(string.Format("Sucessfully written {0} to {1}", fileName, Path.Combine(directory.FullName, fileName)));
                OnFileSaved(this, fi, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fi, false);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_Save_Failed_to_save_settings_to__0_, Path.Combine(directory.FullName, fileName)), ex);
            }
        }

        protected virtual string TranspondersToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsTransponderOpenTag + "\n");

            switch (settings.SettingsVersion)
            {
                case Enums.SettingsVersion.Enigma1V1:
                case Enums.SettingsVersion.Enigma1:
                    sContent.Append(DVBSTranspondersToString(settings, 1));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
                    sContent.Append(DVBSTranspondersToString(settings, -1));
                    break;
                case Enums.SettingsVersion.Enigma2Ver4:
                case Enums.SettingsVersion.Enigma2Ver3:
                    sContent.Append(DVBSTranspondersToString(settings, 0));
                    sContent.Append(DVBCTranspondersToString(settings));
                    sContent.Append(DVBTTranspondersToString(settings));
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
                        .Where(x => (positionOrder > 0 & x.OrbitalPositionInt > 0) || (positionOrder < 0 & x.OrbitalPositionInt < 0) || positionOrder == 0)
                        .OrderByDescending(x => x.OrbitalPositionInt)
                        .ToList())
            {
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1V1:
                    case Enums.SettingsVersion.Enigma1:
                        if (tran.SystemType == Enums.DVBSSystemType.DVBS)
                        {
                            sContent.Append(string.Join(":", new[]
                            {
                                tran.NameSpc,
                                tran.TSID,
                                tran.NID
                            }) + "\n");
                            sContent.Append("\t" + "s " + string.Join(":", new[]
                            {
                                tran.Frequency,
                                tran.SymbolRate,
                                tran.Polarization,
                                tran.FEC,
                                tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture),
                                tran.Inversion
                            }));
                            sContent.Append("\n" + "/" + "\n");
                        }
                        else
                        {
                            Log.Debug(string.Format("Transponder {0} is not DVBS transponder, not supported in Enigma1, skipping", tran));
                        }
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                    case Enums.SettingsVersion.Enigma2Ver3:
                        {
                            sContent.Append(string.Join(":", new[]
                        {
                            tran.NameSpc,
                            tran.TSID,
                            tran.NID
                        }) + "\n");
                            sContent.Append("\t" + "s " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            tran.SymbolRate,
                            tran.Polarization,
                            tran.FEC,
                            tran.OrbitalPositionInt.ToString(CultureInfo.InvariantCulture),
                            tran.Inversion
                        }));
                            string extraData = string.Empty;
                            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver4)
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
                                if (tran.Pilot != null && settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver4)
                                    extraData += ":" + tran.Pilot;
                            }
                            //End If
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
                sContent.Append(string.Join(":", new[]
                {
                    tran.NameSpc,
                    tran.TSID,
                    tran.NID
                }) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1V1:
                    case Enums.SettingsVersion.Enigma1:
                        {
                            string sRate = tran.SymbolRate;
                            string frequency = tran.Frequency;
                            if (frequency.Length < 8)
                            {
                                frequency = frequency + "00";
                                //.PadRight(8, "0"c)
                            }
                            sRate = sRate.Trim('0').Length > 0 ? sRate.PadRight(8, '0') : "0000";
                            sContent.Append("\t" + "s " + string.Join(":", new[]
                        {
                            frequency,
                            sRate,
                            "15",
                            tran.FECInner,
                            "0",
                            tran.Inversion
                        }));
                            sContent.Append("\n" + "/" + "\n");
                        }
                        break;
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "c " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            tran.SymbolRate,
                            tran.Inversion,
                            tran.Modulation,
                            tran.FECInner
                        }));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                        sContent.Append("\t" + "c " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            tran.SymbolRate,
                            tran.Inversion,
                            tran.Modulation,
                            tran.FECInner,
                            tran.Flags
                        }));
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
                sContent.Append(string.Join(":", new[]
                {
                    tran.NameSpc,
                    tran.TSID,
                    tran.NID
                }) + "\n");
                switch (settings.SettingsVersion)
                {
                    case Enums.SettingsVersion.Enigma1V1:
                    case Enums.SettingsVersion.Enigma1:
                        sContent.Append("\t" + "s " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            "0000",
                            "0",
                            "0",
                            "0",
                            tran.Inversion
                        }));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver3:
                        sContent.Append("\t" + "t " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            tran.Bandwidth,
                            tran.FECHigh,
                            tran.FECLow,
                            tran.Modulation,
                            tran.Transmission,
                            tran.GuardInterval,
                            tran.Hierarchy,
                            tran.Inversion
                        }));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                    case Enums.SettingsVersion.Enigma2Ver4:
                        sContent.Append("\t" + "t " + string.Join(":", new[]
                        {
                            tran.Frequency,
                            tran.Bandwidth,
                            tran.FECHigh,
                            tran.FECLow,
                            tran.Modulation,
                            tran.Transmission,
                            tran.GuardInterval,
                            tran.Hierarchy,
                            tran.Inversion,
                            tran.Flags
                        }));
                        sContent.Append("\n" + "/" + "\n");
                        break;
                }
            }
            return sContent.ToString();
        }

        protected virtual string ServicesToString(ISettings settings)
        {
            var sContent = new StringBuilder();
            sContent.Append(SettingsServicesOpenTag + "\n");

            switch (settings.SettingsVersion)
            {
                case Enums.SettingsVersion.Enigma1V1:
                case Enums.SettingsVersion.Enigma1:
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBS))
                    {
                        var tran = (ITransponderDVBS)service.Transponder;
                        if (tran.OrbitalPositionInt > 0)
                        {
                            sContent.Append(ServiceToString(service, settings));
                        }
                    }
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBC).ToList())
                    {
                        sContent.Append(ServiceToString(service, settings));
                    }
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBT).ToList())
                    {
                        sContent.Append(ServiceToString(service, settings));
                    }
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBS))
                    {
                        var tran = (ITransponderDVBS)service.Transponder;
                        if (tran.OrbitalPositionInt < 0)
                        {
                            sContent.Append(ServiceToString(service, settings));
                        }
                    }
                    break;
                case Enums.SettingsVersion.Enigma2Ver4:
                case Enums.SettingsVersion.Enigma2Ver3:
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBS))
                    {
                        sContent.Append(ServiceToString(service, settings));
                    }
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBC).ToList())
                    {
                        sContent.Append(ServiceToString(service, settings));
                    }
                    foreach (IService service in settings.Services.Where(x => x.Transponder.TransponderType == Enums.TransponderType.DVBT).ToList())
                    {
                        sContent.Append(ServiceToString(service, settings));
                    }
                    break;
            }

            sContent.Append(SettingsClosingTag + "\n");
            return sContent.ToString();
        }

        protected virtual string ServiceToString(IService service, ISettings settings)
        {
            var sContent = new StringBuilder();
            if (service.Transponder == null)
            {
                Log.Warn(string.Format(Resources.SettingsIO_ServicesToString_Service__0__has_no_transponder_set__not_writing_it_to_settings_,
                    service.ToString().Replace("\t", "  ")));
            }
            else
            {
                bool serviceSupported = true;
                string flags = string.Empty;

                if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
                {
                    var dvbs = service.Transponder as ITransponderDVBS;
                    if (dvbs != null)
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
                    string sData = string.Join(":", new[]
                    {
                        service.SID.PadLeft(4, '0'),
                        service.Transponder.NameSpc.PadRight(8, '0'),
                        service.Transponder.TSID.PadLeft(4, '0'),
                        service.Transponder.NID.PadLeft(4, '0'),
                        service.Type,
                        service.ProgNumber
                    });
                    sContent.Append(sData + "\n");
                    sContent.Append(service.Name + "\n");
                    if (flags.Length > 0)
                    {
                        sContent.Append(flags + "\n");
                    }
                    else
                    {
                        sContent.Append(service.Flags + "\n");
                    }
                }
                else
                {
                    Log.Debug(string.Format("Service {0} is not supported in Enigma1. Not writing it to settings.", service.ToString().Replace("\t", "  ")));
                }
            }
            return sContent.ToString();
        }

        /// <summary>
        ///     Writes bouquets to corresponding files
        /// </summary>
        /// <param name="directory">Directory info of the directory all bouquet files are written to</param>
        /// <param name="settings">Settings instance to be written></param>
        /// <remarks></remarks>
        protected virtual void WriteBouquets(DirectoryInfo directory, ISettings settings)
        {
            if (settings.SettingsVersion == Enums.SettingsVersion.Enigma1 || settings.SettingsVersion == Enums.SettingsVersion.Enigma1V1)
            {
                WriteTvBouquetsE1(directory, settings);
                WriteRadioBouquetsE1(directory, settings);
                WriteEnigma1Bouquets(directory, settings);
                WriteServicesLocked(directory, settings);
            }
            else if (settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver3 || settings.SettingsVersion == Enums.SettingsVersion.Enigma2Ver4)
            {
                WriteTvBouquetsE2(directory, settings);
                WriteRadioBouquetsE2(directory, settings);
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
            var fi = new FileInfo(fileName);
            try
            {
                OnFileSaving(this, fi);
                File.WriteAllText(fileName, content);
                Log.Debug(string.Format("Sucessfully written bouquet {0} to {1}", bouquetName, fileName));
                OnFileSaved(this, fi, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                OnFileSaved(this, fi, false);
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
                        string fName = Path.GetFileName(bif.FileName);
                        if (fName == null)
                            throw new ArgumentNullException();
                        if (fName.ToLower().StartsWith("userbouquet."))
                            fName = fName.Substring(12);
                        string reference = "#SERVICE: " + string.Join(":", new[]
                        {
                            bif.FavoritesTypeFlag,
                            bif.LineSpecifierFlag,
                            "0",
                            fName.Split('.')[0],
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            bif.FileName
                        }) + "\n";
                        reference += "#TYPE 16385" + "\n";
                        reference += bif.FileName.ToLower() + "\n";
                        sContent.Append(reference);
                        Log.Debug(string.Format("File bouquet reference {0} added to bouquet {1}", reference.Replace("\n", "\t"), bouquet.Name));
                        break;
                    case Enums.BouquetItemType.Marker:
                        var bim = (IBouquetItemMarker)bqItem;
                        sContent.Append("#SERVICE: " + string.Join(":", new[]
                        {
                            bim.FavoritesTypeFlag,
                            bim.LineSpecifierFlag,
                            bim.MarkerNumber.ToString(CultureInfo.InvariantCulture),
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            ""
                        }) + "\n");
                        sContent.Append("#DESCRIPTION: " + bim.Description + "\n");
                        Log.Debug(string.Format("Marker {0} added to bouquet {1}", bim.Description, bouquet.Name));
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
                            var dvbs = bis.Service.Transponder as ITransponderDVBS;
                            if (dvbs != null)
                            {
                                ITransponderDVBS tran = dvbs;
                                if (tran.SystemType != Enums.DVBSSystemType.DVBS)
                                {
                                    serviceSupported = false;
                                }
                            }

                            if (serviceSupported)
                            {
                                sContent.Append("#SERVICE: " + string.Join(":", new[]
                                {
                                    bis.FavoritesTypeFlag,
                                    bis.LineSpecifierFlag,
                                    bis.Service.ServiceId,
                                    "0",
                                    "0",
                                    "0",
                                    ""
                                }) + "\n");
                                Log.Debug(string.Format("Service {0}{1}{2} added to bouquet {3}", bis.Service.ServiceId, "\t", bis.Service.Name,
                                    bouquet.Name));
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
                            sContent.Append("#SERVICE " + string.Join(":", new[]
                            {
                                biss.FavoritesTypeFlag,
                                biss.LineSpecifierFlag,
                                biss.StreamFlag,
                                biss.ServiceID,
                                biss.ExtraFlag1,
                                biss.ExtraFlag2,
                                "0",
                                "0",
                                "0",
                                "0",
                                biss.URL,
                                Uri.EscapeUriString(biss.Description).Replace("'", "%27")
                            }) + "\n");
                        if (biss != null)
                        {
                            sContent.Append("#DESCRIPTION " + biss.Description + "\n");
                            Log.Debug(string.Format("Stream {0}{1}{2} added to bouquet {3}", biss.Description, "\t", biss.URL, bouquet.Name));
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
                        string reference = "#SERVICE " + string.Join(":", new[]
                        {
                            bif.FavoritesTypeFlag,
                            bif.LineSpecifierFlag,
                            "1",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            bif.FileName
                        }) + "\n";
                        Log.Debug(string.Format("File bouquet reference {0} added to bouquet {1}", reference.Replace("\n", "\t"), bouquet.Name));
                        break;
                    case Enums.BouquetItemType.Marker:
                        var bim = (IBouquetItemMarker)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", new[]
                        {
                            bim.FavoritesTypeFlag,
                            bim.LineSpecifierFlag,
                            bim.MarkerNumber.ToString(CultureInfo.InvariantCulture),
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            "0",
                            ""
                        }) + "\n");
                        sContent.Append("#DESCRIPTION " + bim.Description + "\n");
                        Log.Debug(string.Format("Marker {0} added to bouquet {1}", bim.Description, bouquet.Name));
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
                            sContent.Append("#SERVICE " + string.Join(":", new[]
                            {
                                bis.FavoritesTypeFlag,
                                bis.LineSpecifierFlag,
                                bis.Service.ServiceId,
                                "0",
                                "0",
                                "0",
                                ""
                            }) + "\n");
                            Log.Debug(string.Format("Service {0}{1}{2} added to bouquet {3}", bis.Service.ServiceId, "\t", bis.Service.Name, bouquet.Name));
                        }
                        break;
                    case Enums.BouquetItemType.Stream:
                        var biss = (IBouquetItemStream)bqItem;
                        sContent.Append("#SERVICE " + string.Join(":", new[]
                        {
                            biss.FavoritesTypeFlag,
                            biss.LineSpecifierFlag,
                            biss.StreamFlag,
                            biss.ServiceID,
                            biss.ExtraFlag1,
                            biss.ExtraFlag2,
                            "0",
                            "0",
                            "0",
                            "0",
                            biss.URL,
                            Uri.EscapeUriString(biss.Description).Replace("'", "%27")
                        }) + "\n");
                        sContent.Append("#DESCRIPTION " + biss.Description + "\n");
                        Log.Debug(string.Format("Stream {0}{1}{2} added to bouquet {3}", biss.Description, "\t", biss.URL, bouquet.Name));
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
        /// <param name="settings">Instance of ISettings that has the boquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteTvBouquetsE1(DirectoryInfo directory, ISettings settings)
        {
            var sBouquetsTvContent = new StringBuilder();
            sBouquetsTvContent.Append("#NAME User - bouquets (TV)" + "\n");
            int bouquetIndex = 0;
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetTv).ToList()
                )
            {
                string dbeIndex = "dbe" + bouquetIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                string enigmaPath = string.Empty;
                if (bouquet.FileName.IndexOf("/", StringComparison.InvariantCulture) > -1)
                {
                    enigmaPath = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
                }
                if (enigmaPath.Length == 0)
                    enigmaPath = DefaultEnigma1Path;
                bouquet.FileName = enigmaPath + "userbouquet." + dbeIndex + ".tv";
                string reference = "#SERVICE: 4097:7:0:" + dbeIndex + ":0:0:0:0:0:0:" + bouquet.FileName + "\n";
                reference += "#TYPE 16385" + "\n";
                reference += bouquet.FileName + "\n";
                sBouquetsTvContent.Append(reference);
                Log.Debug(string.Format("Bouquet {0} added to userbouquets.epl.tv", bouquet.FileName));
                string bContent = E1BouquetToString(bouquet);
                WriteBouquet(Path.Combine(directory.FullName, Path.GetFileName(bouquet.FileName)), bContent, bouquet.Name);
                bouquetIndex += 1;
            }
            WriteBouquet(Path.Combine(directory.FullName, UserBouquetTvEpl), sBouquetsTvContent.ToString(), UserBouquetTvEpl);
        }

        /// <summary>
        ///     Writes 'bouquets.tv' file and all TV bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all TV bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the boquets we're gonna writing</param>
        /// <remarks></remarks>
        protected virtual void WriteTvBouquetsE2(DirectoryInfo directory, ISettings settings)
        {
            var sBouquetsTvContent = new StringBuilder();
            sBouquetsTvContent.Append("#NAME Bouquets (TV)" + "\n");
            foreach (IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetTv).ToList()
                )
            {
                sBouquetsTvContent.Append("#SERVICE: 1:7:1:0:0:0:0:0:0:0:" + Path.GetFileName(bouquet.FileName) + "\n");
                Log.Debug(string.Format("Bouquet {0} added to bouquets.tv", Path.GetFileName(bouquet.FileName)));
                string bContent = E2BouquetToString(bouquet);
                string fName = Path.GetFileName(bouquet.FileName);
                if (fName != null) WriteBouquet(Path.Combine(directory.FullName, fName), bContent, bouquet.Name);
            }
            WriteBouquet(Path.Combine(directory.FullName, BouquetsTvFile), sBouquetsTvContent.ToString(), BouquetsTvFile);
        }

        /// <summary>
        ///     Writes 'userbouquets.radio.epl' file and all radio bouquets to disk in Enigma1 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the boquets we're gonna writing</param>
        /// <remarks>Will update bouquet names automatically to match ordinal sequence</remarks>
        protected virtual void WriteRadioBouquetsE1(DirectoryInfo directory, ISettings settings)
        {
            var bouquetsRadioContent = new StringBuilder();
            bouquetsRadioContent.Append("#NAME User - bouquets (Radio)" + "\n");
            int bouquetIndex = 0;
            foreach (
                IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetRadio).ToList())
            {
                string dbeIndex = "dbe" + bouquetIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
                string enigmaPath = string.Empty;
                if (bouquet.FileName.IndexOf("/", StringComparison.InvariantCulture) > -1)
                {
                    enigmaPath = bouquet.FileName.Substring(0, bouquet.FileName.LastIndexOf("/", StringComparison.InvariantCulture) + 1);
                }
                if (enigmaPath.Length == 0)
                    enigmaPath = DefaultEnigma1Path;
                bouquet.FileName = enigmaPath + "userbouquet." + dbeIndex + ".radio";
                string reference = "#SERVICE: 4097:7:0:" + dbeIndex + ":0:0:0:0:0:0:" + bouquet.FileName + "\n";
                reference += "#TYPE 16385" + "\n";
                reference += bouquet.FileName + "\n";
                bouquetsRadioContent.Append(reference);
                Log.Debug(string.Format("Bouquet {0} added to userbouquets.epl.radio", bouquet.FileName));
                string bContent = E1BouquetToString(bouquet);
                WriteBouquet(Path.Combine(directory.FullName, Path.GetFileName(bouquet.FileName)), bContent, bouquet.Name);
                bouquetIndex += 1;
            }
            WriteBouquet(Path.Combine(directory.FullName, UserBouquetRadioEpl), bouquetsRadioContent.ToString(), UserBouquetRadioEpl);
        }

        /// <summary>
        ///     Writes 'bouquets.radio' file and all radio bouquets to disk in Enigma2 format
        /// </summary>
        /// <param name="directory">Directory where all radio bouquets will be saved to</param>
        /// <param name="settings">Instance of ISettings that has the boquets we're gonna writing</param>
        /// <remarks></remarks>
        protected virtual void WriteRadioBouquetsE2(DirectoryInfo directory, ISettings settings)
        {
            var sBouquetsRadioContent = new StringBuilder();
            sBouquetsRadioContent.Append("#NAME Bouquets (Radio)" + "\n");
            foreach (
                IFileBouquet bouquet in settings.Bouquets.OfType<IFileBouquet>().Where(x => x.BouquetType == Enums.BouquetType.UserBouquetRadio).ToList())
            {
                sBouquetsRadioContent.Append("#SERVICE: 1:7:2:0:0:0:0:0:0:0:" + Path.GetFileName(bouquet.FileName) + "\n");
                Log.Debug(string.Format("Bouquet {0} added to bouquets.radio", Path.GetFileName(bouquet.FileName)));
                string bContent = E2BouquetToString(bouquet);
                string fName = Path.GetFileName(bouquet.FileName);
                if (fName != null) WriteBouquet(Path.Combine(directory.FullName, fName), bContent, bouquet.Name);
            }
            WriteBouquet(Path.Combine(directory.FullName, BouquetsRadioFile), sBouquetsRadioContent.ToString(), BouquetsRadioFile);
        }

        /// <summary>
        ///     Writes Enigma2 locked services in blacklist file
        /// </summary>
        /// <param name="directory">Directory we're saving blacklist file to</param>
        /// <param name="settings">Instance of ISettings with locked services</param>
        /// <remarks></remarks>
        protected virtual void WriteBlackList(DirectoryInfo directory, ISettings settings)
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
                        fContent.Append(string.Join(":", new[]
                        {
                            "1:0",
                            srv.ServiceId,
                            "0",
                            "0",
                            "0",
                            ""
                        }).ToUpper() + "\n");
                        Log.Debug(string.Format("Service {0} added to blacklist file", srv));
                    }
                }
            }
            else
            {
                Log.Debug("There is no locked services to put inside blacklist file");
            }

            WriteFile(Path.Combine(directory.FullName, BlackListFile), fContent.ToString(), Encoding.Default);
        }

        /// <summary>
        ///     Writes Enigma2 whitelisted services in whitelist file
        /// </summary>
        /// <param name="directory">Directory we're saving whitelist file to</param>
        /// <param name="settings">Instance of ISettings with whitelisted services</param>
        /// <remarks></remarks>
        protected virtual void WriteWhiteList(DirectoryInfo directory, ISettings settings)
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
                        fContent.Append(string.Join(":", new[]
                        {
                            "1:0",
                            srv.ServiceId,
                            "0",
                            "0",
                            "0",
                            ""
                        }).ToUpper() + "\n");
                        Log.Debug(string.Format("Service {0} added to whitelist file", srv));
                    }
                }
            }
            else
            {
                Log.Debug("There is no services to put inside whitelist file");
            }
            WriteFile(Path.Combine(directory.FullName, WhiteListFile), fContent.ToString(), Encoding.Default);
        }

        /// <summary>
        ///     Writes locked services and bouquets to Enigma1 services.locked file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successfull</exception>
        protected virtual void WriteServicesLocked(DirectoryInfo directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1V1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1)
                {
                    Log.Debug("Skipping write to services.locked file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory.FullName, ServicesLockedFile);
                DeleteFileIfExists(fName);

                var fContent = new StringBuilder();
                fContent.Append("Parentallocked Services" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => x.FileName.ToLower() == BouquetsFile);
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        IList<IBouquetItemBouquetsBouquet> lockedBouquets =
                            existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().Where(x => x.Bouquet.Locked).ToList();
                        if (lockedBouquets.Any())
                        {
                            foreach (IBouquetItemBouquetsBouquet lbq in lockedBouquets)
                            {
                                if (lbq.Bouquet != null)
                                {
                                    fContent.Append(string.Join(":", new[]
                                    {
                                        "1:15:fffffffd:12",
                                        lbq.BouquetOrderNumber,
                                        "ffffffff:0:0:0:0:"
                                    }) + "\n");
                                    Log.Debug(string.Format("Enigma1 bouquet {0} added to services.locked", lbq.Bouquet.Name));
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
                            fContent.Append(string.Join(":", new[]
                            {
                                "1:0",
                                srv.ServiceId,
                                "0",
                                "0",
                                "0",
                                ""
                            }).ToUpper() + "\n");
                            Log.Debug(string.Format("Service {0} added to services.locked", srv));
                        }
                    }
                }
                else
                {
                    Log.Debug("There is no locked services to put inside services.locked file");
                }

                WriteFile(fName, fContent.ToString(), Encoding.Default);
            }
            catch (SystemException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteServicesLocked_Failed_to_write_services_locked_file_to__0_, directory.FullName), ex);
            }
        }

        /// <summary>
        ///     Writes Enigma1 bouquets to bouquets file
        /// </summary>
        /// <param name="directory">Directory info of the directory all settings files are written to</param>
        /// <param name="settings">Settings instance to be written</param>
        /// <remarks></remarks>
        /// <exception cref="SettingsException">Throws SettingsException if not successfull</exception>
        protected virtual void WriteEnigma1Bouquets(DirectoryInfo directory, ISettings settings)
        {
            try
            {
                if (settings.SettingsVersion != Enums.SettingsVersion.Enigma1V1 && settings.SettingsVersion != Enums.SettingsVersion.Enigma1)
                {
                    Log.Debug("Skipping write to bouquets file, settings are not Enigma1 type");
                    return;
                }

                string fName = Path.Combine(directory.FullName, BouquetsFile);
                DeleteFileIfExists(fName);

                var fContent = new StringBuilder();
                fContent.Append("eDVB bouquets /2/" + "\n" + "bouquets" + "\n");

                IFileBouquet existing = settings.Bouquets.OfType<IFileBouquet>().SingleOrDefault(x => x.FileName.ToLower() == BouquetsFile);
                if (existing != null)
                {
                    if (existing.BouquetItems.Any())
                    {
                        int bqOrderNumber = existing.BouquetItems.Count() * -1;
                        foreach (IBouquetItemBouquetsBouquet bbq in existing.BouquetItems.OfType<IBouquetItemBouquetsBouquet>().ToList())
                        {
                            if (bbq.Bouquet != null)
                            {
                                //making sure all bouquets have correct order numbers 
                                bbq.BouquetOrderNumberInt = bqOrderNumber;

                                fContent.Append(string.Join("\n", new[]
                                {
                                    bbq.BouquetOrderNumberInt.ToString(CultureInfo.InvariantCulture),
                                    bbq.Bouquet.Name,
                                    ""
                                }));
                                foreach (IBouquetItemService srv in bbq.Bouquet.BouquetItems.OfType<IBouquetItemService>())
                                {
                                    if (srv.Service != null)
                                    {
                                        if (srv.Service.Transponder != null)
                                        {
                                            fContent.Append(string.Join(":", new[]
                                            {
                                                srv.Service.SID.PadLeft(4, '0'),
                                                srv.Service.Transponder.TransponderId.ToLower(),
                                                Convert.ToInt32(srv.Service.ServiceType).ToString(CultureInfo.InvariantCulture)
                                            }) + "\n");
                                            Log.Debug(string.Format("Service {0} in bouquet {1} added to Enigma1 bouquets file", srv, bbq.Bouquet.Name));
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
                                Log.Debug(string.Format("Enigma1 bouquet {0} added to Enigma1 bouquets file", bbq.Bouquet.Name));
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

                WriteFile(fName, fContent.ToString(), Encoding.Default);
            }
            catch (SystemException ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw new SettingsException(
                    string.Format(Resources.SettingsIO_WriteServicesLocked_Failed_to_write_services_locked_file_to__0_, directory.FullName), ex);
            }
        }

        #endregion

        #endregion
    }
}