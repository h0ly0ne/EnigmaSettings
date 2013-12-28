// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Krkadoni.EnigmaSettings.Interfaces;
using Krkadoni.EnigmaSettings.Properties;

namespace Krkadoni.EnigmaSettings
{

    /// <summary>
    ///     Used to load or save satellites from/to satellites.xml file
    /// </summary>
    /// <remarks></remarks>
    public class XmlSatellitesIO : IXmlSatellitesIO
    {
        private readonly IInstanceFactory _factory;

        /// <summary>
        ///     Initialites new instance with custom factory implementation
        /// </summary>
        /// <param name="factory"></param>
        /// <remarks></remarks>
        /// <exception cref="ArgumentNullException">Throws argument null exception if factory is null</exception>
        public XmlSatellitesIO(IInstanceFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(Resources.SettingsIO_New_Invalid_instance_factory_);
            _factory = factory;
        }

        /// <summary>
        ///     Implementation of instance factory used to instatiate objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public IInstanceFactory Factory
        {
            get { return _factory; }
        }

        /// <summary>
        ///     Loads satellites and transponders from satellites.xml file
        /// </summary>
        /// <param name="fileName">Full path to satellites.xml file on disk</param>
        /// <returns>List of IXMLSatellite objects and corresponding transponders for each satellite</returns>
        /// <remarks></remarks>
        public IList<IXmlSatellite> LoadSatellitesFromFile(string fileName)
        {
            var sats = new BindingList<IXmlSatellite>();
            SerializerSatellites ss = SerializerSatellites.LoadFromFile(fileName);
            foreach (SerializerSatellites.SerializerSatellite sSat in ss.sat)
            {
                var sat = new XmlSatellite
                {
                    Name = sSat.name,
                    Flags = sSat.flags,
                    Position = sSat.position
                };
                foreach (SerializerSatellites.SerializerTransponder sTran in sSat.transponders)
                {
                    IXmlTransponder xmlTran = Factory.InitNewxmlTransponder();
                    xmlTran.FECInner = sTran.fec_inner;
                    xmlTran.Frequency = sTran.frequency;
                    xmlTran.Inversion = sTran.inversion;
                    xmlTran.Modulation = sTran.modulation;
                    xmlTran.Pilot = sTran.pilot;
                    xmlTran.Polarization = sTran.polarization;
                    xmlTran.RollOff = sTran.rolloff;
                    xmlTran.SymbolRate = sTran.symbol_rate;
                    xmlTran.System = sTran.system;
                    sat.Transponders.Add(xmlTran);
                }
                sats.Add(sat);
            }
            return sats;
        }

        /// <summary>
        ///     Saves satellites and transponders to satellites.xml file
        /// </summary>
        /// <param name="fileName">Full path to satellites.xml file on disk</param>
        /// <param name="settings">
        ///     Settings instance with the list of IXMLSatellite objects and corresponding transponders for each
        ///     satellite
        /// </param>
        /// <remarks></remarks>
        public void SaveSatellitesToFile(string fileName, ISettings settings)
        {
            // cloning objects to avoid modifying referenced ones while saving
            var ss = new SerializerSatellites();
            foreach (IXmlSatellite xSat in settings.Satellites)
            {
                var sSat = new SerializerSatellites.SerializerSatellite
                {
                    flags = xSat.Flags,
                    name = xSat.Name,
                    position = xSat.Position
                };
                foreach (IXmlTransponder xTran in xSat.Transponders)
                {
                    var serTran = new SerializerSatellites.SerializerTransponder
                    {
                        frequency = xTran.Frequency,
                        symbol_rate = xTran.SymbolRate,
                        polarization = xTran.Polarization
                    };
                    switch (settings.SettingsVersion)
                    {
                        case Enums.SettingsVersion.Enigma1:
                        case Enums.SettingsVersion.Enigma1V1:
                            switch (xTran.FECInner)
                            {
                                case "0":
                                case "1":
                                case "2":
                                case "3":
                                case "4":
                                case "5":
                                    serTran.fec_inner = xTran.FECInner;
                                    break;
                                default:
                                    serTran.fec_inner = "0";
                                    break;
                            }
                            serTran.inversion = null;
                            serTran.modulation = null;
                            serTran.system = null;
                            serTran.pilot = null;
                            serTran.rolloff = null;
                            break;
                        case Enums.SettingsVersion.Enigma2Ver4:
                        case Enums.SettingsVersion.Enigma2Ver3:
                            serTran.fec_inner = xTran.FECInner;
                            serTran.inversion = xTran.Inversion ?? "2";
                            serTran.modulation = xTran.Modulation ?? "1";
                            serTran.system = xTran.System ?? "0";
                            serTran.pilot = xTran.Pilot;
                            serTran.rolloff = xTran.RollOff;
                            break;
                    }
                    sSat.transponders.Add(serTran);
                }
                ss.sat.Add(sSat);
            }
            ss.SaveToFile(fileName);
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable CSharpWarnings::CS1591
        #region "Serialization"

        /// <summary>
        /// Internal helper class that contains all deserialized objects and performs (de)serialization
        /// </summary>
        [XmlRoot("satellites"), XmlType("satellites")]
        public class SerializerSatellites
        {
            private static XmlSerializer _sSerializer;

            public SerializerSatellites()
            {
                sat = new List<SerializerSatellite>();
            }

            /// <summary>
            /// Helper class representing satellite item from list of satellites
            /// </summary>
            [XmlElement("sat", Form = XmlSchemaForm.Unqualified)]
            public List<SerializerSatellite> sat { get; set; }

            private static XmlSerializer Serializer
            {
                get
                {
                    if ((_sSerializer == null))
                    {
                        _sSerializer = new XmlSerializer(typeof(SerializerSatellites));
                    }
                    return _sSerializer;
                }
            }

            #region "Serialize/Deserialize"

            /// <summary>
            ///     Serializes current SerializerSatellites object into an XML document
            /// </summary>
            /// <returns>string XML value</returns>
            public virtual string Serialize()
            {
                var memoryStream = new MemoryStream();
                var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.GetEncoding("ISO-8859-1")) { Formatting = Formatting.Indented };
                // XmlTextWriter xmlTextWriter = New XmlTextWriter(memoryStream, New UTF8Encoding(True))
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                Serializer.Serialize(xmlTextWriter, this, ns);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }

            public static SerializerSatellites Deserialize(string xml)
            {
                using (var stringReader = new StringReader(xml))
                {
                    return (SerializerSatellites)Serializer.Deserialize(XmlReader.Create(stringReader));
                }
            }

            public virtual void SaveToFile(string fileName)
            {
                string xmlString = Serialize();
                var xmlFile = new FileInfo(fileName);
                using (var streamWriter = xmlFile.CreateText())
                {
                    streamWriter.WriteLine(xmlString);
                }
            }

            public static SerializerSatellites LoadFromFile(string fileName)
            {
                var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                string xmlString;
                using (var sr = new StreamReader(file))
                {
                    xmlString = sr.ReadToEnd();
                }
                return Deserialize(xmlString);
            }

            #endregion

            #region "Subclases"

            [XmlType("sat")]
            public class SerializerSatellite
            {
                public SerializerSatellite()
                {
                    transponders = new List<SerializerTransponder>();
                }

                [XmlElement("transponder", Form = XmlSchemaForm.Unqualified)]
                public List<SerializerTransponder> transponders { get; set; }

                [XmlAttribute]
                public string position { get; set; }

                [XmlAttribute]
                public string flags { get; set; }

                [XmlAttribute]
                public string name { get; set; }
            }

            [Serializable, XmlType("transponder")]
            public class SerializerTransponder
            {
                [XmlAttribute]
                public string frequency { get; set; }

                [XmlAttribute]
                public string symbol_rate { get; set; }

                [XmlAttribute]
                public string polarization { get; set; }

                [XmlAttribute]
                public string fec_inner { get; set; }

                [XmlAttribute]
                public string inversion { get; set; }

                [XmlAttribute]
                public string modulation { get; set; }

                [XmlAttribute]
                public string system { get; set; }

                [XmlAttribute]
                public string pilot { get; set; }

                [XmlAttribute]
                public string rolloff { get; set; }
            }

            #endregion
        }

        #endregion
        // ReSharper restore CSharpWarnings::CS1591
        // ReSharper restore InconsistentNaming

    }
}