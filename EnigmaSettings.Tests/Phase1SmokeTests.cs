using System.IO;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    // Fixtures/v5 (lamedb5) are intentionally not exercised here; they will be
    // covered once lamedb5 parsing support is implemented in a later phase.
    public class Phase1SmokeTests
    {
        [Fact]
        public void Loads_v4_fixture_with_expected_counts()
        {
            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(TestSettings.FixtureDir("v4"), "lamedb"));

            Assert.Equal(2, settings.Services.Count);
            Assert.Equal(2, settings.Transponders.Count);
            Assert.Contains(settings.Services, s => s.Name == "Test TV One");
        }
    }
}
