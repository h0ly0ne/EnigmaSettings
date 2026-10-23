// Copyright © 2013-2026 Krkadoni.com - Released under The MIT License.
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
        /// <value>Position as integer number with length 3 (e.g. 19.2E = 192) </value>
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
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        string Polarization { get; set; }

        /// <summary>
        ///     0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8, 6=8/9, 7=3/5, 8=4/5, 9=9/10, 10=6/7, 15=None
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        string FEC { get; set; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
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
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        string System { get; set; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=8PSK, 3=QAM16, 4=16APSK, 5=32APSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        string Modulation { get; set; }

        /// <summary>
        ///     Only used in DVB-S2. 0=0.35, 1=0.25, 2=0.20, 3=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string RollOff { get; set; }

        /// <summary>
        ///     Only used in DVB-S2. 0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Pilot { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        string IsId { get; set; }
        
        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        string PlsCode { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        string PlsMode { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        string T2miPlpId { get; set; }

        /// <summary>
        /// https://github.com/OpenViX/enigma2/blob/master/lib/dvb/db.cpp#L792
        /// </summary>
        string T2miPid { get; set; }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSPolarizationType PolarizationType { get; }

        /// <summary>
        ///     0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8, 6=8/9, 7=3/5, 8=4/5, 9=9/10, 10=6/7, 15=None
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSCFECType FECType { get; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSCInversionType InversionType { get; }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSSystemType SystemType { get; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=8PSK, 3=QAM16, 4=16APSK, 5=32APSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSModulationType ModulationType { get; }

        /// <summary>
        ///     0=0.35, 1=0.25, 2=0.20, 3=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
        Enums.DVBSRollOffType RollOffType { get; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="https://github.com/OpenPLi/enigma2/blob/develop/lib/dvb/frontendparms.h">eDVBFrontendParametersSatellite</see>
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