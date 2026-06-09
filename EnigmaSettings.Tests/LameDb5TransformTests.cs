using Krkadoni.EnigmaSettings;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    // Exposes the protected v5 transforms for direct unit testing.
    public class ProbeSettingsIO : SettingsIO
    {
        public ProbeSettingsIO() : base(new InstanceFactory()) { }
        public string Transponders(string v4) => TranspondersToLameDb5(v4);
        public string Services(string v4) => ServicesToLameDb5(v4);
        public bool ParseTransponder(string l, out string a, out string b) => TryParseLameDb5Transponder(l, out a, out b);
        public bool ParseService(string l, out string d, out string n, out string f) => TryParseLameDb5Service(l, out d, out n, out f);
    }

    public class LameDb5TransformTests
    {
        [Fact]
        public void Transponder_block_becomes_t_line()
        {
            var io = new ProbeSettingsIO();
            string v4 = "00c00000:03e9:0001\n\ts 11214000:23500000:0:3:192:2\n/\n";
            Assert.Equal("t:00c00000:03e9:0001,s:11214000:23500000:0:3:192:2\n", io.Transponders(v4));
        }

        [Fact]
        public void Services_section_becomes_s_lines_with_quoted_name_and_flags()
        {
            var io = new ProbeSettingsIO();
            string v4 = "services\n0001:00c00000:03e9:0001:1:0\nTest One\np:Prov,C:0001\nend\n";
            Assert.Equal("s:0001:00c00000:03e9:0001:1:0,\"Test One\",p:Prov,C:0001\n", io.Services(v4));
        }

        [Fact]
        public void Bare_empty_package_flag_is_omitted_in_s_line()
        {
            var io = new ProbeSettingsIO();
            string v4 = "services\n0001:00c00000:03e9:0001:1:0\nName\np:\nend\n";
            Assert.Equal("s:0001:00c00000:03e9:0001:1:0,\"Name\"\n", io.Services(v4));
        }

        [Fact]
        public void Parses_t_line_back_to_v4_shape()
        {
            var io = new ProbeSettingsIO();
            Assert.True(io.ParseTransponder("t:00c00000:03e9:0001,s:11214000:23500000:0:3:192:2", out var a, out var b));
            Assert.Equal("00c00000:03e9:0001", a);
            Assert.Equal("s 11214000:23500000:0:3:192:2", b);
        }

        [Fact]
        public void Parses_s_line_back_to_three_parts()
        {
            var io = new ProbeSettingsIO();
            Assert.True(io.ParseService("s:0001:00c00000:03e9:0001:1:0,\"Test One\",p:Prov,C:0001", out var d, out var n, out var f));
            Assert.Equal("0001:00c00000:03e9:0001:1:0", d);
            Assert.Equal("Test One", n);
            Assert.Equal("p:Prov,C:0001", f);
        }

        [Fact]
        public void Parses_s_line_with_no_flags_defaults_package()
        {
            var io = new ProbeSettingsIO();
            Assert.True(io.ParseService("s:0002:00c00000:0408:0001:1:0,\"Two\"", out var d, out var n, out var f));
            Assert.Equal("Two", n);
            Assert.Equal("p:", f);
        }
    }
}
