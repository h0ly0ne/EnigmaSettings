using Krkadoni.EnigmaSettings;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    // Exposes the protected version-mapping methods for direct unit testing.
    public class VersionProbe : SettingsIO
    {
        public VersionProbe() : base(new InstanceFactory()) { }
        public Enums.SettingsVersion FromToken(string token) => GetSettingsVersion(token);
        public int ToToken(Enums.SettingsVersion version) => GetSettingsVersion(version);
    }

    public class VersionMappingTests
    {
        [Fact]
        public void File_token_5_maps_to_Enigma2Ver5()
        {
            Assert.Equal(Enums.SettingsVersion.Enigma2Ver5, new VersionProbe().FromToken("5"));
        }

        [Fact]
        public void Enigma2Ver5_maps_back_to_file_token_5()
        {
            Assert.Equal(5, new VersionProbe().ToToken(Enums.SettingsVersion.Enigma2Ver5));
        }

        [Theory]
        [InlineData("3", Enums.SettingsVersion.Enigma2Ver3)]
        [InlineData("4", Enums.SettingsVersion.Enigma2Ver4)]
        public void Existing_tokens_still_map(string token, Enums.SettingsVersion expected)
        {
            Assert.Equal(expected, new VersionProbe().FromToken(token));
        }
    }
}
