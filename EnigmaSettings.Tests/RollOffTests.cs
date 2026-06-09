using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class RollOffTests
    {
        // DVB-S2 roll-off coding (enigma2 db.cpp / DVB): 0=0.35, 1=0.25, 2=0.20, 3=Auto.
        [Theory]
        [InlineData("0", Enums.DVBSRollOffType.X35)]
        [InlineData("1", Enums.DVBSRollOffType.X25)]
        [InlineData("2", Enums.DVBSRollOffType.X20)]
        [InlineData("3", Enums.DVBSRollOffType.Auto)]
        public void Dvbs_rolloff_maps_per_dvb_coding(string raw, Enums.DVBSRollOffType expected)
        {
            // transponderFrequency: s freq:sr:pol:fec:pos:inv:flags:system:mod:rolloff:pilot  (rolloff at index 9)
            var t = new InstanceFactory().InitNewTransponderDVBS(
                "00c00000:0001:0001",
                "s 11214000:27500:0:3:192:2:0:1:2:" + raw + ":1");

            Assert.Equal(expected, t.RollOffType);
            Assert.Equal(raw, t.RollOff); // raw value preserved regardless of the convenience enum
        }
    }
}
