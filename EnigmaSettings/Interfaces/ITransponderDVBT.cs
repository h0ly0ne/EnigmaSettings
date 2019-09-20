// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponderDVBT : ITransponder
    {
        /// <summary>
        ///     0=Auto, 1=8Mhz, 2=7Mhz, 3=6Mhz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth types</see>
        string Bandwidth { get; set; }

        /// <summary>
        ///     Code rate High Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FECHigh { get; set; }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FECLow { get; set; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=QAM64
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        string Modulation { get; set; }

        /// <summary>
        ///     0=Auto, 1=2k, 3=8k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission values</see>
        string Transmission { get; set; }

        /// <summary>
        ///     0=Auto, 1=1/32, 2=1/16, 3=1/8, 4=1/4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard Interval values</see>
        string GuardInterval { get; set; }

        /// <summary>
        ///     0=Auto, 1=None, 2=1, 3=2, 4=4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy values</see>
        string Hierarchy { get; set; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion values</see>
        string Inversion { get; set; }

        /// <summary>
        ///     Transponder flags
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L793
        /// </summary>
        string System { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L793
        /// </summary>
        string PlpId { get; set; }

        /// <summary>
        ///     0=Auto, 1=8Mhz, 2=7Mhz, 3=6Mhz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth types</see>
        Enums.DVBTBandwidthType BandwidthType { get; }

        /// <summary>
        ///     Code rate High Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBTFECHighType FECHighType { get; }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBTFECLowType FECLowType { get; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=QAM64
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        Enums.DVBTModulationType ModulationType { get; }

        /// <summary>
        ///     0=Auto, 1=2k, 3=8k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission types</see>
        Enums.DVBTTransmissionType TransmissionType { get; }

        /// <summary>
        ///     0=Auto, 1=1/32, 2=1/16, 3=1/8, 4=1/4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard Interval types</see>
        Enums.DVBTGuardIntervalType GuardIntervalType { get; }

        /// <summary>
        ///     0=Auto, 1=None, 2=1, 3=2, 4=4
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy types</see>
        Enums.DVBTHierarchyType HierarchyType { get; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        Enums.DVBTInversionType InversionType { get; }
    }
}