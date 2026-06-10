using System.IO;
using System.Linq;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    /// <summary>
    ///     Regression: newer enigma2 / DemonEdit lamedb (still tagged "eDVB services /4/") writes service
    ///     reference lines with a 7th field, the source id:
    ///     sid:namespace:tsid:onid:type:number:sourceid
    ///     enigma2 parses it with sscanf "%x:%x:%x:%x:%d:%d:%x" and writes it back, so we must accept it
    ///     (instead of throwing "Invalid service data") and preserve it on save.
    /// </summary>
    public class SevenFieldServiceTests
    {
        private static void WriteLamedb(string dir, string srcOne, string srcTwo)
        {
            File.WriteAllText(Path.Combine(dir, "lamedb"),
                "eDVB services /4/\n" +
                "transponders\n" +
                "00c00000:03e9:0001\n" +
                "\ts 11214000:23500000:0:3:192:2:0:1:2:2:2\n" +
                "/\n" +
                "end\n" +
                "services\n" +
                "0001:00c00000:03e9:0001:1:0" + srcOne + "\n" +
                "Test TV One\n" +
                "p:TestProv,C:0001\n" +
                "0002:00c00000:03e9:0001:1:0" + srcTwo + "\n" +
                "Test TV Two\n" +
                "p:TestProv\n" +
                "end\n");
        }

        [Fact]
        public void Service_line_with_trailing_seventh_field_loads()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            WriteLamedb(dir, ":0", ":0");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));

            Assert.Equal(2, settings.Services.Count);
            var svc = settings.Services.First();
            Assert.Equal("0001", svc.SID);
            Assert.Equal("1", svc.Type);
            Assert.Equal("0", svc.ProgNumber);
            Assert.Equal("0", svc.SourceID);
        }

        [Fact]
        public void Classic_six_field_line_keeps_six_fields_on_save()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            WriteLamedb(dir, "", "");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));
            Assert.All(settings.Services, s => Assert.Equal(string.Empty, s.SourceID));

            string outDir = Path.Combine(dir, "out");
            io.Save(outDir, settings);
            string saved = File.ReadAllText(Path.Combine(outDir, "lamedb"));

            Assert.Contains("0001:00c00000:03e9:0001:1:0\n", saved);
            Assert.DoesNotContain("0001:00c00000:03e9:0001:1:0:", saved);
        }

        [Fact]
        public void Source_id_round_trips_on_save_including_non_zero_value()
        {
            string dir = TestSettings.CopyFixtureToTemp("v4");
            WriteLamedb(dir, ":0", ":a1b2");

            var io = TestSettings.NewIO();
            var settings = io.Load(Path.Combine(dir, "lamedb"));
            Assert.Equal("0", settings.Services.First().SourceID);
            Assert.Equal("a1b2", settings.Services.Skip(1).First().SourceID);

            string outDir = Path.Combine(dir, "out");
            io.Save(outDir, settings);
            string saved = File.ReadAllText(Path.Combine(outDir, "lamedb"));

            Assert.Contains("0001:00c00000:03e9:0001:1:0:0\n", saved);
            Assert.Contains("0002:00c00000:03e9:0001:1:0:a1b2\n", saved);
        }
    }
}
