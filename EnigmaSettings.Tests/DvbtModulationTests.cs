using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class DvbtModulationTests
    {
        [Theory]
        [InlineData("0", Enums.DVBTModulationType.QPSK)]
        [InlineData("1", Enums.DVBTModulationType.Qam16)]
        [InlineData("2", Enums.DVBTModulationType.Qam64)]
        [InlineData("3", Enums.DVBTModulationType.Auto)]
        [InlineData("4", Enums.DVBTModulationType.Qam256)]
        public void Dvbt_modulation_maps_per_dvb_constellation(string raw, Enums.DVBTModulationType expected)
        {
            var t = new InstanceFactory().InitNewTransponderDVBT(
                "eeee0000:0001:0001",
                "t 474000000:3:0:0:" + raw + ":1:1:1:2");
            Assert.Equal(expected, t.ModulationType);
            Assert.Equal(raw, t.Modulation); // raw preserved
        }
    }
}
