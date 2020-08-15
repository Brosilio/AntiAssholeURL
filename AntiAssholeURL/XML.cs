using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AntiAssholeURL
{
    public static class XML
    {
        public static T Read<T>(string xml) where T : class
        {
            XmlSerializer serial = new XmlSerializer(typeof(T));
            StringReader read = new StringReader(xml);
            return serial.Deserialize(read) as T;
        }

        public static string Write<T>(T obj) where T : class
        {
            XmlSerializer serial = new XmlSerializer(typeof(T));
            StringWriter write = new StringWriter();
            serial.Serialize(write, obj);
            return write.GetStringBuilder().ToString();
        }
    }
}
