using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using log4net;

namespace CSM.Common.Utilities
{
    public  class XmlSerializerCache
    {
        private static object sync = new Object();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(XmlSerializerCache));
        private static readonly Dictionary<string, XmlSerializer> Cache = new Dictionary<string, XmlSerializer>();

        public static XmlSerializer Create(Type type, XmlRootAttribute root)
        {
            lock (sync)
            {
                var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, root.ElementName);

                if (!Cache.ContainsKey(key))
                {
                    //Logger.DebugFormat("O:--Create New XmlSerializer--:Key/{0}", key);
                    Cache.Add(key, new XmlSerializer(type, root));
                }
                //else
                //{
                //    Logger.DebugFormat("O:--Get Exists XmlSerializer--:Key/{0}", key);
                //}

                return Cache[key];
            }
        }
    }
}
