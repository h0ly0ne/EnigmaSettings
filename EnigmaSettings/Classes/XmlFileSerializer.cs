// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Krkadoni.EnigmaSettings.Interfaces;

namespace Krkadoni.EnigmaSettings
{
    /// <summary>
    ///     Reads and writes satellites.xml / cables.xml so that the bytes on disk always match the
    ///     encoding declared in the XML header
    /// </summary>
    internal static class XmlFileSerializer
    {
        // Enigma ships these files as ISO-8859-1; characters outside Latin-1 are written as character references
        private static readonly Encoding FileEncoding = Encoding.Latin1;

        public static string Serialize(XmlSerializer serializer, object value)
        {
            using var memoryStream = new MemoryStream();
            var xmlSettings = new XmlWriterSettings { Indent = true, Encoding = FileEncoding };
            using (var xmlWriter = XmlWriter.Create(memoryStream, xmlSettings))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(xmlWriter, value, ns);
            }

            return FileEncoding.GetString(memoryStream.ToArray());
        }

        public static void Save(IFileHelper fileHelper, string fileName, string xml)
        {
            fileHelper.WriteAllText(fileName, xml, FileEncoding);
        }

        /// <summary>
        ///     Deserializes from the raw bytes, letting the XML reader pick the encoding from BOM / declaration
        /// </summary>
        public static T Load<T>(IFileHelper fileHelper, XmlSerializer serializer, string fileName)
        {
            using Stream stream = fileHelper.OpenRead(fileName);
            using var reader = XmlReader.Create(stream);
            return (T)serializer.Deserialize(reader);
        }
    }
}
