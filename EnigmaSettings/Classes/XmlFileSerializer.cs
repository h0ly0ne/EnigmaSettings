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
    ///     Common (de)serialization of the satellites.xml / cables.xml root objects
    /// </summary>
    /// <typeparam name="T">The serializable root type deriving from this class</typeparam>
    public abstract class XmlFileRoot<T> where T : XmlFileRoot<T>
    {
        // one XmlSerializer per root type, creating it is expensive
        private static XmlSerializer _serializer;

        private static XmlSerializer Serializer => _serializer ??= new XmlSerializer(typeof(T));

        /// <summary>
        ///     Serializes the current object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize()
        {
            return XmlFileSerializer.Serialize(Serializer, this);
        }

        public static T Deserialize(string xml)
        {
            using var reader = XmlReader.Create(new StringReader(xml));
            return (T)Serializer.Deserialize(reader);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, new FileHelper());
        }

        public virtual void SaveToFile(string fileName, IFileHelper fileHelper)
        {
            XmlFileSerializer.Save(fileHelper, fileName, Serialize());
        }

        public static T LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, new FileHelper());
        }

        public static T LoadFromFile(string fileName, IFileHelper fileHelper)
        {
            return XmlFileSerializer.Load<T>(fileHelper, Serializer, fileName);
        }
    }

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
