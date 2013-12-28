// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponderDVBS : ITransponder
    {
        /// <summary>
        ///     Instance of satellite from satellites.xml transponder belongs to
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        IXmlSatellite Satellite { get; set; }

        /// <summary>
        ///     Transponder symbol rate
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SymbolRate { get; set; }

        /// <summary>
        ///     Orbital position value as integer
        /// </summary>
        /// <value>Position as integer number with length 3 (ie. 19.2E = 192) </value>
        /// <returns></returns>
        /// <remarks></remarks>
        int OrbitalPositionInt { get; set; }

        /// <summary>
        ///     Orbital position of corresponding satellite as Hex value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string OrbitalPositionHex { get; set; }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        string Polarization { get; set; }

        /// <summary>
        ///     0=None , 1=Auto, 2=1/2, 3=2/3, 4=3/4 5=5/6, 6=7/8, 7=3/5, 8=4/5, 9=8/9, 10=9/10
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FEC { get; set; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        string Inversion { get; set; }

        /// <summary>
        ///     Only in version 4. Field is absent in version 3
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; set; }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System values</see>
        string System { get; set; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        string Modulation { get; set; }

        /// <summary>
        ///     Only used in DVB-S2. 0=0.35, 1=0.25, 3=0.20
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string RollOff { get; set; }

        /// <summary>
        ///     Only used in DVB-S2. 0=Auto, 2=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Pilot { get; set; }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        Enums.DVBSPolarizationType PolarizationType { get; }

        /// <summary>
        ///     0=None , 1=Auto, 2=1/2, 3=2/3, 4=3/4 5=5/6, 6=7/8, 7=3/5, 8=4/5, 9=8/9, 10=9/10
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBSFECType FECType { get; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        Enums.DVBSInversionType InversionType { get; }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System values</see>
        Enums.DVBSSystemType SystemType { get; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        Enums.DVBSModulationType ModulationType { get; }

        /// <summary>
        ///     0=0.35, 1=0.25, 3=0.20
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">RollOff types</see>
        Enums.DVBSRollOffType RollOffType { get; }

        /// <summary>
        ///     0=Auto, 2=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Pilot types</see>
        Enums.DVBSPilotType PilotType { get; }

        /// <summary>
        ///     Calculated NameSpace value which can differ from NameSpc value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Calculates namespace based on Enigma algorithm</remarks>
        string CalculatedNameSpace { get; }
    }
}