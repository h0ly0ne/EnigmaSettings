// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    /// <summary>
    ///     Used to load or save cables from/to cables.xml file
    /// </summary>
    /// <remarks></remarks>
    public class XmlCablesIO : IXmlCablesIO
    {
        private static IFileHelper _fileProvider;

        /// <summary>
        ///     Initializes new instance with custom factory implementation
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="fileProvider"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if factory is null</exception>
        public XmlCablesIO(IInstanceFactory factory, IFileHelper fileProvider)
        {
            Factory = factory ?? throw new ArgumentNullException(Resources.SettingsIO_New_Invalid_instance_factory_);
            _fileProvider = fileProvider ?? throw new ArgumentNullException(Resources.XmlSatellitesIO_XmlSatellitesIO_Invalid_file_provider);
        }

        /// <summary>
        ///     Implementation of instance factory used to instantiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IInstanceFactory Factory { get; }

        /// <summary>
        ///     Loads cables and transponders from cables.xml file
        /// </summary>
        /// <param name="fileName">Full path to cables.xml file on disk</param>
        /// <returns>List of IXmlCable objects and corresponding transponders for each cable</returns>
        /// <remarks></remarks>
        public IList<IXmlCable> LoadCablesFromFile(string fileName)
        {
            var cables = new List<IXmlCable>();
            var sc = SerializerCables.LoadFromFile(fileName);

            foreach (var sCab in sc.cab)
            {
                var cab = new XmlCable
                {
                    Name = sCab.name,
                    Flags = sCab.flags,
                    SatFeed = sCab.satfeed,
                    CountryCode = sCab.countrycode
                };

                foreach (var sTran in sCab.transponders)
                {
                    var xmlTran = Factory.InitNewXmlTransponder();

                    xmlTran.FEC = sTran.fec;
                    xmlTran.Frequency = sTran.frequency;
                    xmlTran.Modulation = sTran.modulation;
                    xmlTran.SymbolRate = sTran.symbol_rate;

                    cab.Transponders.Add(xmlTran);
                }
                cables.Add(cab);
            }
            return cables;
        }

        /// <summary>
        ///     Saves cables and transponders to cables.xml file
        /// </summary>
        /// <param name="fileName">Full path to cables.xml file on disk</param>
        /// <param name="settings">
        ///     Settings instance with the list of IXmlCable objects and corresponding transponders for each
        ///     cable
        /// </param>
        /// <remarks></remarks>
        public void SaveCablesToFile(string fileName, ISettings settings)
        {
            // cloning objects to avoid modifying referenced ones while saving
            var sc = new SerializerCables();

            foreach (var xCab in settings.Cables)
            {
                var sCab = new SerializerCables.SerializerCable
                {
                    flags = xCab.Flags,
                    name = xCab.Name,
                    satfeed = xCab.SatFeed,
                    countrycode = xCab.CountryCode
                };

                foreach (var xTran in xCab.Transponders)
                {
                    var serTran = new SerializerCables.SerializerTransponder
                    {
                        frequency = xTran.Frequency,
                        symbol_rate = xTran.SymbolRate,
                    };

                    switch (settings.SettingsVersion)
                    {
                        case Enums.SettingsVersion.Enigma1Ver1:
                        case Enums.SettingsVersion.Enigma1Ver2:
                        {
                            serTran.fec = xTran.FEC switch
                            {
                                "0" or "1" or "2" or "3" or "4" or "5" => xTran.FEC,
                                _ => "0"
                            };

                            serTran.modulation = null;
                            break;
                        }
                        case Enums.SettingsVersion.Enigma2Ver3:
                        case Enums.SettingsVersion.Enigma2Ver4:
                        case Enums.SettingsVersion.Enigma2Ver5:
                            {
                            serTran.fec = xTran.FEC;
                            serTran.modulation = xTran.Modulation;
                            break;
                        }
                    }
                    sCab.transponders.Add(serTran);
                }
                sc.cab.Add(sCab);
            }
            sc.SaveToFile(fileName);
        }
        #region "Serialization"

        /// <summary>
        /// Internal helper class that contains all deserialized objects and performs (de)serialization
        /// </summary>
        [XmlRoot("cables"), XmlType("cables")]
        public class SerializerCables
        {
            private static XmlSerializer _cSerializer;

            /// <summary>
            /// Helper class representing cable item from list of cables
            /// </summary>
            [XmlElement("cable", Form = XmlSchemaForm.Unqualified)]
            public List<SerializerCable> cab { get; set; } = [];

            private static XmlSerializer Serializer => _cSerializer ??= new XmlSerializer(typeof(SerializerCables));

            #region "Serialize/Deserialize"
            /// <summary>
            ///     Serializes current SerializerCables object into an XML document
            /// </summary>
            /// <returns>string XML value</returns>
            public virtual string Serialize()
            {
                var memoryStream = new MemoryStream();
                var xmlSettings = new XmlWriterSettings {Indent = true, Encoding = Encoding.GetEncoding("ISO-8859-1")};
                var xmlTextWriter = XmlWriter.Create(memoryStream, xmlSettings);
                var ns = new XmlSerializerNamespaces();

                ns.Add("", "");
                Serializer.Serialize(xmlTextWriter, this, ns);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using var streamReader = new StreamReader(memoryStream);

                return streamReader.ReadToEnd();
            }

            public static SerializerCables Deserialize(string xml)
            {
                return (SerializerCables)Serializer.Deserialize(XmlReader.Create(new StringReader(xml)));
            }

            public virtual void SaveToFile(string fileName)
            {
                _fileProvider.WriteAllText(fileName, Serialize());
            }

            public static SerializerCables LoadFromFile(string fileName)
            {
                string strXMLSerialized;
                using (var srCurrentStreamReader = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), Encoding.GetEncoding("ISO-8859-1")))
                {
                    strXMLSerialized = srCurrentStreamReader.ReadToEnd();
                }
                return Deserialize(strXMLSerialized);
            }
            #endregion

            #region "Subclases"
            [XmlType("cable")]
            public class SerializerCable
            {
                [XmlElement("transponder", Form = XmlSchemaForm.Unqualified)]
                public List<SerializerTransponder> transponders { get; set; } = [];

                [XmlAttribute]
                public string name { get; set; }

                [XmlAttribute]
                public string flags { get; set; }

                [XmlAttribute]
                public string satfeed { get; set; }

                [XmlAttribute]
                public string countrycode { get; set; }
            }

            [DataContract, XmlType("transponder")]
            public class SerializerTransponder
            {
                [XmlAttribute]
                public string frequency { get; set; }

                [XmlAttribute]
                public string symbol_rate { get; set; }

                [XmlAttribute]
                public string fec { get; set; }

                [XmlAttribute]
                public string modulation { get; set; }
            }
            #endregion
        }
        #endregion
    }
}