using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using log4net;
using CONST = CSM.Common.Utilities.Constants;

///<summary>
/// Class Name : XMLTransformer
/// Purpose    : -
/// Author     : -
///</summary>
///<remarks>
/// Change History:
/// Date            Author           Description
/// 28-07-2014      Neda Peyrone     Modified GetDeserializeResult(..) method
///</remarks>
namespace CSM.Common.Utilities
{
    public static class XMLTransformer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (XMLTransformer));

        public static dynamic GetDeserializeResult<T>(T srcObject, Type targetObject)
        {
            Stream data = null;

            try
            {
                data = new MemoryStream();
                var xRoot = new XmlRootAttribute();
                xRoot.ElementName = typeof (T).Name;
                xRoot.IsNullable = true;

                XmlSerializer serializer = XmlSerializerCache.Create(typeof(T), xRoot);
                serializer.Serialize(data, srcObject);

                // USE FOR TRANSFORM STREAM TO BE READABLE
                data.Position = 0;

                data.Seek(0, SeekOrigin.Begin);

                XmlSerializer deserializer = XmlSerializerCache.Create(targetObject, xRoot);
                return deserializer.Deserialize(data);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                throw;
            }
            finally
            {
                if (data != null) { data.Dispose(); }
            }
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            if (toSerialize != null)
            {
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = typeof(T).Name;
                xRoot.IsNullable = true;

                XmlSerializer xmlSerializer = XmlSerializerCache.Create(typeof(T), xRoot);
                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }

            return "There is no response body.";
        }
        
        public static string ToXml(this DataSet ds)
        {
            using (var sw = new StringWriter())
            {
                ds.WriteXml(sw);
                return sw.ToString();
            }
        }
    }
}