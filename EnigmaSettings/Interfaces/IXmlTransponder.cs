// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IXmlTransponder : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     Frequency in Hertz
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Frequency { get; set; }

        /// <summary>
        ///     Symbol rate
        /// </summary>
        /// <value>Usually 8 digit integer</value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SymbolRate { get; set; }

        /// <summary>
        ///     0=Horizontal, 1=Vertical, 2=Circular Left, 3=Circular right
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Polarization values</see>
        string Polarization { get; set; }

        /// <summary>
        ///     0=Auto, 1=1/2, 2=2/3, 3=3/4, 4=5/6, 5=7/8, 6=8/9, 7=3/5, 8=4/5, 9=9/10, 10=6/7, 15=None
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FEC { get; set; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        string Inversion { get; set; }

        /// <summary>
        ///     0=Auto, 1=QPSK, 2=8PSK, 3=QAM16, 4=16APSK, 5=32APSK
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        string Modulation { get; set; }

        /// <summary>
        ///     0=DVB-S 1=DVB-S2
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">System types</see>
        string System { get; set; }

        /// <summary>
        ///     0=Off, 1=On, 2=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Pilot types</see>
        string Pilot { get; set; }

        /// <summary>
        ///     0=0.35, 1=0.25, 2=0.20, 3=Auto
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">RollOff types</see>
        string RollOff { get; set; }

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
        ///     Transport stream id (satellites.xml attribute "tsid"), optional
        /// </summary>
        string Tsid { get; set; }

        /// <summary>
        ///     Original network id (satellites.xml attribute "onid"), optional
        /// </summary>
        string Onid { get; set; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}