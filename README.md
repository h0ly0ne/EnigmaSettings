EnigmaSettings
==============

Enigma1 &amp; Enigma2 settings management utility

EnigmaSettings is open source library for Enigma1 & Enigma2 settings management.
It does all heavy work with settings loading, manipulation and saving so you don’t have to.

If you’ve ever thought about writing your own settings editor (like DreamboxEdit, Dreamset…)
but didn’t know anything about Enigma settings – this library is for you.
It’s written in C# and compatible with Mono 2.8 and higher.

Contributions and bugfixes are welcomed.

Additional info and support on http://www.krkadoni.com/.

Main Features
==============

- It’s FREE
- It’s open source library licensed under permissive MIT license. Anyone can use it.
- It’s easy to use, has excellent logging support, and commented code.
- It works on Windows XP SP2 and newer (.NET framework 3.5), Linux and MacOS X (Mono 2.8 and higher). I suppose it will work on iOS and Android too when compiled as Portable Library, but I didn’t have time to test it.
- It’s fast. It will load up huge motor settings in under 500 ms.
- It supports Enigma1, Enigma2 ver3 and Enigma2 ver4 settings and automatic conversion between them.
- Implemented background threading for settings loading and saving  (Async methods)
- Full interface based objective model
- Every part of library can be replaced by custom code via custom instance factory (Inversion of Concerns)
- Fully prepared for GUI data binding (IEditable and IPropertyChanged interfaces implemented)

Examples
==============
```C#
// C#

//list location (lamedb or services file)
string fileName = "C:\\Settings\\lamedb";

//initialize default list load / save handler
Krkadoni.EnigmaSettings.SettingsIO settingsIO = new Krkadoni.EnigmaSettings.SettingsIO();

//load list
Krkadoni.EnigmaSettings.Settings settings = settingsIO.Load(fileName);

//change satellite position for Thor satellite from 1.0 W to 0.8 W
//Changes satellite position to new position for satellite and belonging transponders
var satelliteThor = settings.Satellites.Single(x => Int32.Parse(x.Position) == -10);
settings.ChangeSatellitePosition(satelliteThor, -8);

//remove all stream services
settings.RemoveStreams();

//remove empty bouquets
settings.RemoveEmptyBouquets();

//remove satellite on position 23.0 E
settings.RemoveSatellite(230);

//update service parameters
var service = settings.Services.First(); //take first service as an example
service.Name = "Service name";
service.ProgNumber = "0";
service.ServiceSecurity = Enums.ServiceSecurity.BlackListed;
service.SID = "24";
//etc...

//save settings to current folder
settingsIO.Save(new DirectoryInfo(AppSettings.CurrentDir), settings );
```

```VB.NET
'VB.net 

'list location (lamedb or services file)
Dim fileName As String = "C:\Settings\lamedb"

'initialize default list load / save handler
Dim settingsIO As New Krkadoni.EnigmaSettings.SettingsIO()

'load list
Dim settings As Krkadoni.EnigmaSettings.Settings = settingsIO.Load(fileName)

'change satellite position for Thor satellite from 1.0 W to 0.8 W
'Changes satellite position to new position for satellite and belonging transponders
Dim satelliteThor = settings.Satellites.[Single](Function(x) Int32.Parse(x.Position) = -10)
settings.ChangeSatellitePosition(satelliteThor, -8)

'remove all stream services
settings.RemoveStreams()

'remove empty bouquets
settings.RemoveEmptyBouquets()

'remove satellite on position 23.0 E
settings.RemoveSatellite(230)

'update service parameters
Dim service = settings.Services.First()
'take first service as an example
service.Name = "Service name"
service.ProgNumber = "0"
service.ServiceSecurity = Enums.ServiceSecurity.BlackListed
service.SID = "24"
'etc...

'save settings to current folder
settingsIO.Save(New DirectoryInfo(AppSettings.CurrentDir), settings)
```
