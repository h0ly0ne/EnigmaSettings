using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class CrossVersionTests
    {
        [Fact]
        public void Load_v4_save_as_v5_reload_preserves_services_and_transponders()
        {
            var io = TestSettings.NewIO();
            string dir = TestSettings.CopyFixtureToTemp("v4");
            var v4 = io.Load(Path.Combine(dir, "lamedb"));

            v4.SettingsVersion = Enums.SettingsVersion.Enigma2Ver5;

            string outDir = Path.Combine(Path.GetTempPath(), "es_x_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            // bouquets/satellites needed for a clean reload
            foreach (var f in Directory.GetFiles(dir).Where(f => Path.GetFileName(f) != "lamedb"))
                File.Copy(f, Path.Combine(outDir, Path.GetFileName(f)), true);

            io.Save(outDir, v4);
            Assert.True(File.Exists(Path.Combine(outDir, "lamedb5")));

            var v5 = io.Load(Path.Combine(outDir, "lamedb5"));
            Assert.Equal(v4.Services.Count, v5.Services.Count);
            Assert.Equal(v4.Transponders.Count, v5.Transponders.Count);
            Assert.Equal(
                v4.Services.Select(s => s.ServiceId).OrderBy(x => x),
                v5.Services.Select(s => s.ServiceId).OrderBy(x => x));
        }

        [Fact]
        public void Load_v5_save_as_v4_reload_preserves_services_and_transponders()
        {
            var io = TestSettings.NewIO();
            string dir = TestSettings.CopyFixtureToTemp("v5");
            var v5 = io.Load(Path.Combine(dir, "lamedb5"));

            v5.SettingsVersion = Enums.SettingsVersion.Enigma2Ver4;

            string outDir = Path.Combine(Path.GetTempPath(), "es_x4_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            foreach (var f in Directory.GetFiles(dir).Where(f => Path.GetFileName(f) != "lamedb5"))
                File.Copy(f, Path.Combine(outDir, Path.GetFileName(f)), true);

            io.Save(outDir, v5);
            Assert.True(File.Exists(Path.Combine(outDir, "lamedb")));

            var v4 = io.Load(Path.Combine(outDir, "lamedb"));
            Assert.Equal(v5.Services.Count, v4.Services.Count);
            Assert.Equal(v5.Transponders.Count, v4.Transponders.Count);
            Assert.Equal(
                v5.Services.Select(s => s.ServiceId).OrderBy(x => x),
                v4.Services.Select(s => s.ServiceId).OrderBy(x => x));
        }
    }
}
