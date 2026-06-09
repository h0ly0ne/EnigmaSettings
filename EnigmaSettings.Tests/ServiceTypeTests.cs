using Krkadoni.EnigmaSettings.Interfaces;
using Xunit;

namespace Krkadoni.EnigmaSettings.Tests
{
    public class ServiceTypeTests
    {
        [Theory]
        [InlineData("28", Enums.ServiceType.AcStereoHdtv)]
        [InlineData("29", Enums.ServiceType.AcStereoHdNvodTimeShifted)]
        [InlineData("30", Enums.ServiceType.AcStereoHdNvodReference)]
        [InlineData("31", Enums.ServiceType.Hevc)]
        [InlineData("32", Enums.ServiceType.HevcUhd)]
        public void New_service_types_map_to_enum(string raw, Enums.ServiceType expected)
        {
            var svc = new InstanceFactory().InitNewService("0001:00c00000:03e9:0001:" + raw + ":0", "X", "p:");
            Assert.Equal(expected, svc.ServiceType);
            Assert.Equal(raw, svc.Type); // raw value preserved regardless
        }

        [Fact]
        public void Unknown_service_type_keeps_raw_value()
        {
            var svc = new InstanceFactory().InitNewService("0001:00c00000:03e9:0001:200:0", "X", "p:");
            Assert.Equal(Enums.ServiceType.Unknown, svc.ServiceType);
            Assert.Equal("200", svc.Type);
        }
    }
}
