// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface ITransponderDVBC : ITransponder
    {
        /// <summary>
        ///     Transponder symbol rate
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string SymbolRate { get; set; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        string Inversion { get; set; }

        /// <summary>
        ///     0=Auto, 1=QAM16, 2=QAM32, 3=QAM64, 4=QAM128, 5=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        string Modulation { get; set; }

        /// <summary>
        ///     0=None, 1=Auto, 2=1/2, 3=2/3, 4=3/4, 5=5/6, 6=7/8, 7=8/9
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        string FECInner { get; set; }

        /// <summary>
        ///     Transponder flag value
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        string Flags { get; set; }

        /// <summary>
        ///     0=Auto, 1=On, 2=Off
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Inversion types</see>
        Enums.DVBCInversionType InversionType { get; }

        /// <summary>
        ///     0=Auto, 1=QAM16, 2=QAM32, 3=QAM64, 4=QAM128, 5=QAM256
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">Modulation types</see>
        Enums.DVBCModulationType ModulationType { get; }

        /// <summary>
        ///     0=None, 1=Auto, 2=1/2, 3=2/3, 4=3/4, 5=5/6, 6=7/8, 7=8/9
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <see href="http://www.satsupreme.com/showthread.php/194074-Lamedb-format-explained">FEC types</see>
        Enums.DVBCFECInnerType FECInnerType { get; }
    }
}