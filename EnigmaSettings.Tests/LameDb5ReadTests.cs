using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class LameDb5ReadTests
    {
        [Fact]
        public void Reads_v5_fixture_into_same_model_as_v4()
        {
            var io = TestSettings.NewIO();
            var v5 = io.Load(Path.Combine(TestSettings.FixtureDir("v5"), "lamedb5"));

            Assert.Equal(Enums.SettingsVersion.Enigma2Ver5, v5.SettingsVersion);
            Assert.Equal(2, v5.Transponders.Count);
            Assert.Equal(2, v5.Services.Count);

            var one = v5.Services.Single(s => s.SID.TrimStart('0') == "1");
            Assert.Equal("Test TV One", one.Name);
            Assert.Equal("1", one.Type);
            Assert.NotNull(one.Transponder);
            Assert.Equal("c00000", one.Transponder.NameSpc.TrimStart('0'));

            // bouquets use the same format regardless of lamedb version
            Assert.Contains(v5.Bouquets.OfType<IFileBouquet>(), b => b.Name == "Test");
        }
    }
}
