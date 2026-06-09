using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class LameDb5WriteTests
    {
        [Fact]
        public void Save_v5_writes_lamedb5_with_v5_header_and_round_trips()
        {
            var io = TestSettings.NewIO();
            string dir = TestSettings.CopyFixtureToTemp("v5");
            var settings = io.Load(Path.Combine(dir, "lamedb5"));

            string outDir = Path.Combine(Path.GetTempPath(), "es_w5_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            io.Save(outDir, settings);

            string written = Path.Combine(outDir, "lamedb5");
            Assert.True(File.Exists(written));
            Assert.False(File.Exists(Path.Combine(outDir, "lamedb")));
            Assert.StartsWith("eDVB services /5/", File.ReadAllText(written));

            var reloaded = io.Load(written);
            Assert.Equal(settings.Services.Count, reloaded.Services.Count);
            Assert.Equal(settings.Transponders.Count, reloaded.Transponders.Count);
            Assert.Equal(
                settings.Services.Select(s => s.Name).OrderBy(n => n),
                reloaded.Services.Select(s => s.Name).OrderBy(n => n));
        }
    }
}
