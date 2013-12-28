// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT
     
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;  

namespace Krkadoni.EnigmaSettings
{

    /// Provides a method for performing a deep copy of an object.
    /// Binary Serialization is used to perform the copy.
    /// Taken from http://www.codeproject.com/Articles/23832/Implementing-Deep-Cloning-via-Serializing-objects
    /// Copyright by Stephen Inglish, licensed under CPOL license
    public static class ObjectCopier
    {

        /// Perform a deep Copy of the object.
        /// <span class="code-SummaryComment"><typeparam name="T">The type of object being copied.</typeparam></span>
        /// <span class="code-SummaryComment"><param name="source">The object instance to copy.</param></span>
        /// <span class="code-SummaryComment"><returns>The copied object.</returns></span>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
