using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class AlternativesTests
    {
        private static string BuildDirWithAlternative()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            File.WriteAllText(Path.Combine(dir, "bouquets.tv"),
                "#NAME User - Bouquets (TV)\n" +
                "#SERVICE 1:7:1:0:0:0:0:0:0:0:FROM BOUQUET \"userbouquet.alt.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "userbouquet.alt.tv"),
                "#NAME AltTest\n" +
                "#SERVICE 1:134:1:0:0:0:0:0:0:0:FROM BOUQUET \"alternatives.sample.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "alternatives.sample.tv"),
                "#NAME sample\n" +
                "#SERVICE 1:0:1:1:3e9:1:c00000:0:0:0:\n" +
                "#DESCRIPTION Test TV One\n");
            return dir;
        }

        [Fact]
        public void Reads_alternative_reference_as_alternative_item()
        {
            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(BuildDirWithAlternative(), "lamedb"));

            var bq = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "AltTest");
            var alt = Assert.IsAssignableFrom<IBouquetItemAlternative>(bq.BouquetItems.Single());
            Assert.Equal("134", alt.LineSpecifierFlag);
            Assert.Equal("alternatives.sample.tv", Path.GetFileName(alt.FileName));
            Assert.NotNull(alt.Bouquet);
            Assert.Single(alt.Bouquet.BouquetItems);
            // alternatives files must NOT leak into the top-level bouquet list
            Assert.DoesNotContain(settings.Bouquets.OfType<IFileBouquet>(),
                b => b.FileName != null && b.FileName.StartsWith("alternatives."));
            // inner service must be matched (Fix 1)
            var innerService = Assert.IsAssignableFrom<IBouquetItemService>(alt.Bouquet.BouquetItems.Single());
            Assert.NotNull(innerService.Service);
        }

        [Fact]
        public void Writes_alternative_reference_line_and_alternatives_file()
        {
            var io = TestSettings.NewIO();
            string dir = BuildDirWithAlternative();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            string outDir = Path.Combine(Path.GetTempPath(), "es_alt_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(outDir);
            io.Save(outDir, settings);

            string userBq = File.ReadAllText(Path.Combine(outDir, "userbouquet.alt.tv"));
            Assert.Contains("1:134:", userBq);
            Assert.Contains("FROM BOUQUET \"alternatives.sample.tv\"", userBq);
            Assert.True(File.Exists(Path.Combine(outDir, "alternatives.sample.tv")));

            // full round-trip: reload the saved directory and confirm the alternative + its inner service survived
            var reloaded = io.Load(Path.Combine(outDir, "lamedb"));
            var reBq = reloaded.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "AltTest");
            var reAlt = Assert.IsAssignableFrom<IBouquetItemAlternative>(reBq.BouquetItems.Single());
            Assert.NotNull(reAlt.Bouquet);
            var reInner = Assert.IsAssignableFrom<IBouquetItemService>(reAlt.Bouquet.BouquetItems.Single());
            Assert.NotNull(reInner.Service);
        }
    }
}
