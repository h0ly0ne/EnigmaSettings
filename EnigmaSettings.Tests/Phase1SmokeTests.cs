using System.IO;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
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
