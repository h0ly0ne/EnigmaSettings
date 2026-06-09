// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class SatellitesXmlTests
    {
        [Fact]
        public void Reads_s2x_and_t2mi_transponder_attributes()
        {
            // Arrange: copy v4 fixture to a temp dir and overwrite satellites.xml with S2X/T2MI fixture
            string dir = TestSettings.CopyFixtureToTemp("v4");
            File.Copy(
                Path.Combine(TestSettings.FixtureDir("sat"), "satellites.xml"),
                Path.Combine(dir, "satellites.xml"),
                overwrite: true);

            // Act: load through the public SettingsIO pipeline
            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            // Assert: the Test 13E satellite is present and its single transponder carries all S2X/T2MI attributes
            var sat = settings.Satellites.Single(s => s.Name == "Test 13E");
            var tr = sat.Transponders.Single();

            Assert.Equal("1", tr.System);
            Assert.Equal("2", tr.Modulation);
            Assert.Equal("1", tr.PlsMode);
            Assert.Equal("16416", tr.PlsCode);
            Assert.Equal("0", tr.IsId);
            Assert.Equal("0", tr.T2miPlpId);
            Assert.Equal("4096", tr.T2miPid);
        }

        [Fact]
        public void V5_save_preserves_s2x_and_t2mi_satellite_attributes()
        {
            string dir = TestSettings.CopyFixtureToTemp("v5");
            File.Copy(Path.Combine(TestSettings.FixtureDir("sat"), "satellites.xml"),
                      Path.Combine(dir, "satellites.xml"), true);

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb5"));
            Assert.Equal(Enums.SettingsVersion.Enigma2Ver5, settings.SettingsVersion);

            string outDir = Path.Combine(Path.GetTempPath(), "es_sat5_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            io.Save(outDir, settings);

            var reloaded = io.Load(Path.Combine(outDir, "lamedb5"));
            var tr = reloaded.Satellites.Single(s => s.Name == "Test 13E").Transponders.Single();
            Assert.Equal("1", tr.System);
            Assert.Equal("2", tr.Modulation);
            Assert.Equal("1", tr.PlsMode);
            Assert.Equal("16416", tr.PlsCode);
            Assert.Equal("0", tr.IsId);
            Assert.Equal("4096", tr.T2miPid);
        }
    }
}
