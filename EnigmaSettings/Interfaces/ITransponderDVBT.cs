// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponderDVBT : ITransponder
    {
        /// <summary>
        ///     0=8MHz, 1=7MHz, 2=6MHz, 3=Auto, 4=5MHz, 5=1.712MHz, 6=10MHz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth types</see>
        string Bandwidth { get; set; }

        /// <summary>
        ///     Code rate High Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FECHigh { get; set; }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FECLow { get; set; }

        /// <summary>
        ///     0=QPSK, 1=QAM16, 2=QAM64, 3=Auto, 4=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        string Modulation { get; set; }

        /// <summary>
        ///     0=2k, 1=8k, 2=Auto, 3=4k, 4=1k, 5=16k, 6=32k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission values</see>
        string Transmission { get; set; }

        /// <summary>
        ///     0=1/32, 1=1/16, 2=1/8, 3=1/4, 4=Auto, 5=1/128, 6=19/128, 7=19/256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard Interval values</see>
        string GuardInterval { get; set; }

        /// <summary>
        ///     0=None, 1=1, 2=2, 3=4, 4=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy values</see>
        string Hierarchy { get; set; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
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
        ///     0=8MHz, 1=7MHz, 2=6MHz, 3=Auto, 4=5MHz, 5=1.712MHz, 6=10MHz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Bandwidth types</see>
        Enums.DVBTBandwidthType BandwidthType { get; }

        /// <summary>
        ///     Code rate High Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBTFECHighType FECHighType { get; }

        /// <summary>
        ///     Code rate Low Pass FEC: 0=1/2, 1=2/3, 2=3/4, 3=5/6, 4=7/8, 5=Auto, 6=6/7, 7=8/9, 8=3/5, 9=4/5
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBTFECLowType FECLowType { get; }

        /// <summary>
        ///     0=QPSK, 1=QAM16, 2=QAM64, 3=Auto, 4=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        Enums.DVBTModulationType ModulationType { get; }

        /// <summary>
        ///     0=2k, 1=8k, 2=Auto, 3=4k, 4=1k, 5=16k, 6=32k
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Transmission types</see>
        Enums.DVBTTransmissionType TransmissionType { get; }

        /// <summary>
        ///     0=1/32, 1=1/16, 2=1/8, 3=1/4, 4=Auto, 5=1/128, 6=19/128, 7=19/256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Guard Interval types</see>
        Enums.DVBTGuardIntervalType GuardIntervalType { get; }

        /// <summary>
        ///     0=None, 1=1, 2=2, 3=4, 4=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Hierarchy types</see>
        Enums.DVBTHierarchyType HierarchyType { get; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        Enums.DVBTInversionType InversionType { get; }
    }
}