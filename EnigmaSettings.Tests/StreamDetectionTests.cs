using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class StreamDetectionTests
    {
        [Theory]
        [InlineData("4097")]
        [InlineData("5001")]
        [InlineData("5002")]
        [InlineData("8193")]
        [InlineData("8739")]
        public void Stream_types_are_detected_and_first_field_is_preserved(string streamType)
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            string bq = Path.Combine(dir, "userbouquet.stream.tv");
            File.WriteAllText(bq,
                "#NAME Streams\n" +
                "#SERVICE " + streamType + ":0:1:0:0:0:0:0:0:0:http%3a//example.test/live.m3u8:Sample\n" +
                "#DESCRIPTION Sample\n");
            File.WriteAllText(Path.Combine(dir, "bouquets.tv"),
                "#NAME User - Bouquets (TV)\n" +
                "#SERVICE 1:7:1:0:0:0:0:0:0:0:FROM BOUQUET \"userbouquet.stream.tv\" ORDER BY bouquet\n");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            var streamBq = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "Streams");
            var item = streamBq.BouquetItems.Single();
            var stream = Assert.IsAssignableFrom<IBouquetItemStream>(item);
            Assert.Equal(streamType, stream.FavoritesTypeFlag);
            Assert.Equal("Sample", stream.Description);
        }
    }
}
