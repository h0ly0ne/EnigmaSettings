using System.IO;
using System.Linq;
using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    /// <summary>
    ///     The cleanup/remove utilities on <see cref="Settings" /> must reach services and streams that
    ///     live inside an alternative's inner bouquet (which is intentionally kept out of
    ///     <c>Settings.Bouquets</c>), and must preserve SPACE (832) spacer markers.
    /// </summary>
    public class SettingsUtilitiesTests
    {
        // userbouquet.alt.tv (a normal, top-level bouquet) holds a shared service AND an alternative.
        // alternatives.sample.tv (reachable only through the alternative) holds the same shared service.
        private static string BuildDirWithSharedServiceInAlternative()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            File.WriteAllText(Path.Combine(dir, "bouquets.tv"),
                "#NAME User - Bouquets (TV)\n" +
                "#SERVICE 1:7:1:0:0:0:0:0:0:0:FROM BOUQUET \"userbouquet.alt.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "userbouquet.alt.tv"),
                "#NAME AltTest\n" +
                "#SERVICE 1:0:1:1:3e9:1:c00000:0:0:0:\n" +
                "#DESCRIPTION Shared Channel\n" +
                "#SERVICE 1:134:1:0:0:0:0:0:0:0:FROM BOUQUET \"alternatives.sample.tv\" ORDER BY bouquet\n");
            File.WriteAllText(Path.Combine(dir, "alternatives.sample.tv"),
                "#NAME sample\n" +
                "#SERVICE 1:0:1:1:3e9:1:c00000:0:0:0:\n" +
                "#DESCRIPTION Shared Channel\n");
            return dir;
        }

        [Fact]
        public void RemoveService_also_removes_the_service_from_inside_an_alternative()
        {
            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(BuildDirWithSharedServiceInAlternative(), "lamedb"));

            var top = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "AltTest");
            var alt = top.BouquetItems.OfType<IBouquetItemAlternative>().Single();
            var inner = alt.Bouquet;
            var shared = inner.BouquetItems.OfType<IBouquetItemService>().Single().Service;
            Assert.NotNull(shared); // matched against lamedb on load

            settings.RemoveService(shared);

            Assert.Empty(inner.BouquetItems.OfType<IBouquetItemService>());  // reached through the alternative
            Assert.Empty(top.BouquetItems.OfType<IBouquetItemService>());    // and from the top-level bouquet
            Assert.DoesNotContain(shared, settings.Services);
        }

        [Fact]
        public void RemoveStreams_also_removes_streams_from_inside_an_alternative()
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
                "#SERVICE 5001:0:1:0:0:0:0:0:0:0:http%3a//example.test/live.m3u8:Live\n" +
                "#DESCRIPTION Live\n");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            var alt = settings.Bouquets.OfType<IFileBouquet>().Single(b => b.Name == "AltTest")
                .BouquetItems.OfType<IBouquetItemAlternative>().Single();
            Assert.NotEmpty(alt.Bouquet.BouquetItems.OfType<IBouquetItemStream>()); // present before

            settings.RemoveStreams();

            Assert.Empty(alt.Bouquet.BouquetItems.OfType<IBouquetItemStream>()); // removed through the alternative
        }

        [Fact]
        public void RemoveInvalidBouquetItems_drops_dangling_alternative_and_cleans_inside_valid_one()
        {
            var f = new InstanceFactory();
            var settings = f.InitNewSettings();
            var main = f.InitNewFileBouquet();

            // a dangling alternative: references a file that was never read, so Bouquet stays null
            var dangling = f.InitNewBouquetItemAlternative("alternatives.gone.tv");
            main.BouquetItems.Add(dangling);

            // a valid alternative whose inner bouquet holds an unmatched (Service == null) reference
            var inner = f.InitNewFileBouquet();
            var unmatched = f.InitNewBouquetItemService("1:0:1:dead:beef:1:c00000:0:0:0:");
            inner.BouquetItems.Add(unmatched);
            var valid = f.InitNewBouquetItemAlternative(inner);
            main.BouquetItems.Add(valid);

            settings.Bouquets.Add(main);

            settings.RemoveInvalidBouquetItems();

            Assert.DoesNotContain(dangling, main.BouquetItems);             // dangling alternative removed
            Assert.Contains(valid, main.BouquetItems);                      // valid alternative kept
            Assert.Empty(inner.BouquetItems.OfType<IBouquetItemService>()); // unmatched inner service removed
        }

        [Fact]
        public void RemoveEmptyMarkers_treats_a_trailing_space_as_an_empty_marker()
        {
            // In the removal logic a SPACE (832) is treated exactly like a marker: a marker that is
            // followed only by a space is empty, and a trailing space is itself empty - both go.
            var f = new InstanceFactory();
            var settings = f.InitNewSettings();
            var bq = f.InitNewFileBouquet();

            var channel = f.InitNewBouquetItemService("1:0:1:1:3e9:1:c00000:0:0:0:");
            var header = f.InitNewBouquetItemMarker("Trailing", "0");
            header.LineSpecifierFlag = "64";
            var space = f.InitNewBouquetItemMarker("spacer", "1");
            space.LineSpecifierFlag = "832";

            bq.BouquetItems.Add(channel);
            bq.BouquetItems.Add(header); // followed only by a space -> empty
            bq.BouquetItems.Add(space);  // trailing -> empty
            settings.Bouquets.Add(bq);

            settings.RemoveEmptyMarkers();

            Assert.DoesNotContain(header, bq.BouquetItems); // removed: a marker followed only by a space
            Assert.DoesNotContain(space, bq.BouquetItems);  // removed: trailing space treated as a marker
            Assert.Contains(channel, bq.BouquetItems);
        }

        [Fact]
        public void RemoveEmptyMarkers_removes_a_marker_followed_by_a_space_but_keeps_a_space_before_a_channel()
        {
            // marker = space: the header is immediately "followed by another marker" (the space) so it
            // is empty and removed, while the space precedes a real channel and is therefore kept.
            var f = new InstanceFactory();
            var settings = f.InitNewSettings();
            var bq = f.InitNewFileBouquet();

            var header = f.InitNewBouquetItemMarker("News", "0");
            header.LineSpecifierFlag = "64";
            var space = f.InitNewBouquetItemMarker("spacer", "1");
            space.LineSpecifierFlag = "832";
            var channel = f.InitNewBouquetItemService("1:0:1:1:3e9:1:c00000:0:0:0:");

            bq.BouquetItems.Add(header);
            bq.BouquetItems.Add(space);
            bq.BouquetItems.Add(channel);
            settings.Bouquets.Add(bq);

            settings.RemoveEmptyMarkers();

            Assert.DoesNotContain(header, bq.BouquetItems); // empty: immediately followed by a (space) marker
            Assert.Contains(space, bq.BouquetItems);        // kept: precedes a real channel
            Assert.Contains(channel, bq.BouquetItems);
        }
    }
}
