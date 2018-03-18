using System;
using System.Collections;
using System.IO;
using log4net;

namespace CSM.Common.Mail
{
    public class TextGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TextGenerator));
        private Hashtable _hashData;

        private void SetHashData(Hashtable hashData)
        {
            _hashData = hashData;
        }

        private void GenText(string pathName, StringWriter sw)
        {
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                string line = sr.ReadLine();

                while (line != null)
                {
                    if (line.IndexOf("[") >= 0 && line.IndexOf("]") >= 0)
                    {
                        foreach (string key in _hashData.Keys)
                        {
                            try
                            {
                                line = line.Replace("[" + key + "]", (string)_hashData[key]);
                            }
                            catch (Exception)
                            {
                                Logger.Debug("Missing Message Key[" + key + "]");
                            }
                        }
                    }

                    sw.WriteLine(line);
                    line = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                //if (fs != null)
                //    fs.Close();
            }
        }

        public string GenText(string pathName, Hashtable hData)
        {
            StringWriter sw = null;

            try
            {
                sw = new StringWriter();
                SetHashData(hData);
                GenText(pathName, sw);
                sw.Flush();
                return sw.ToString();
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw = null;
                }
            }
        }
    }
}