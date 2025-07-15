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
        ///     0=Auto, 1=QPSK, 2=QAM16, 3=8PSK
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
        ///     0=Auto, 1=Off, 1=On
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Pilot types</see>
        string Pilot { get; set; }

        /// <summary>
        ///     0=0.35, 1=0.25, 3=0.20
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
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();

    }
}