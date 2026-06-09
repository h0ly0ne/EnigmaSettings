using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class SpaceHiddenTests
    {
        [Fact]
        public void Space_entry_is_preserved_as_marker()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            File.WriteAllText(Path.Combine(dir, "bouquets.tv"),
                "#NAME User - Bouquets (TV)\n" +
                "#SERVICE 1:7:1:0:0:0:0:0:0:0:FROM BOUQUET \"userbouquet.sp.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "userbouquet.sp.tv"),
                "#NAME SpaceTest\n" +
                "#SERVICE 1:832:D:0:0:0:0:0:0:0:\n" +
                "#SERVICE 1:0:1:1:3e9:1:c00000:0:0:0:\n" +
                "#DESCRIPTION Test TV One\n");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));
            var bq = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "SpaceTest");

            Assert.Equal(2, bq.BouquetItems.Count);
            Assert.Contains(bq.BouquetItems, i => i.LineSpecifierFlag == "832");

            // round-trips on write: the 832 spacer is re-emitted
            string outDir = Path.Combine(Path.GetTempPath(), "es_sp_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            io.Save(outDir, settings);
            Assert.Contains("1:832:", File.ReadAllText(Path.Combine(outDir, "userbouquet.sp.tv")));
        }

        [Fact]
        public void Hidden_bouquet_reference_is_preserved_and_round_trips()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            File.WriteAllText(Path.Combine(dir, "bouquets.tv"),
                "#NAME User - Bouquets (TV)\n" +
                "#SERVICE 1:519:1:0:0:0:0:0:0:0:FROM BOUQUET \"userbouquet.hidden.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "userbouquet.hidden.tv"),
                "#NAME HiddenTest\n" +
                "#SERVICE 1:0:1:1:3e9:1:c00000:0:0:0:\n" +
                "#DESCRIPTION Test TV One\n");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            var hidden = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "HiddenTest");
            Assert.True(hidden.Hidden);
            Assert.Single(hidden.BouquetItems); // the channel survived

            string outDir = Path.Combine(Path.GetTempPath(), "es_hid_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            io.Save(outDir, settings);
            string bouquetsTv = File.ReadAllText(Path.Combine(outDir, "bouquets.tv"));
            Assert.Contains("1:519:", bouquetsTv); // hidden flag re-emitted
        }
    }
}
