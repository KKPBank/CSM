using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using log4net;
using Microsoft.VisualBasic.FileIO;

///<summary>
/// Class Name : StreamDataHelper
/// Purpose    : -
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date         Author           Description
/// ----         ------           -----------
///</remarks>
namespace CSM.Common.Utilities
{
    public static class StreamDataHelpers
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StreamDataHelpers));

        public static byte[] ReadDataToBytes(Stream stream)
        {
            MemoryStream ms = null;

            try
            {
                var buffer = new byte[64*1024];
                ms = new MemoryStream();
                int r = 0;
                int l = 0;
                long position = -1;
                if (stream.CanSeek)
                {
                    position = stream.Position;
                    stream.Position = 0;
                }
                while (true)
                {
                    r = stream.Read(buffer, 0, buffer.Length);
                    if (r > 0)
                    {
                        l += r;
                        ms.Write(buffer, 0, r);
                    }
                    else
                    {
                        break;
                    }
                }
                var bytes = new byte[l];
                ms.Position = 0;
                ms.Read(bytes, 0, l);
                if (position >= 0)
                {
                    stream.Position = position;
                }
                return bytes;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                    ms = null;
                }
            }
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            FileStream fs = null;

            try
            {
                // Open file for reading
                fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, FileShare.ReadWrite);

                // Writes a block of bytes to this stream using data from a byte array.
                fs.Write(byteArray, 0, byteArray.Length);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception caught in process:\n", ex);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            // error occured, return false
            return false;
        }

        public static bool TryToDelete(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public static bool TryToCopy(string filePath, string newPath)
        {
            try
            {
                File.Copy(filePath, newPath, true);
                return true;
            }
            catch (IOException ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }

            return false;
        }

        public static IEnumerable<string[]> ReadPipe(string fileName)
        {
            char[] separator = new[] { '|' };
            string currentLine;

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, Encoding.GetEncoding(874), true, 1024))
            {
                while ((currentLine = reader.ReadLine()) != null)
                {
                    yield return currentLine.Split(separator, StringSplitOptions.None);
                }
            }
        }

        public static IEnumerable<string[]> ReadCsv(string fileName)
        {
            using (var parser = new TextFieldParser(fileName, Encoding.GetEncoding(874), true))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.Delimiters = new[] { "," };
                while (parser.PeekChars(1) != null)
                {
                    string[] fields = parser.ReadFields();
                    var cleanFields = fields.Select(f => f.Trim(new[] { ' '}));
                    yield return cleanFields.ToArray();
                }                
            }
        }

        public static string GetApplicationPath(string relativeUrl)
        {
            return HostingEnvironment.MapPath(relativeUrl);
        }
    }
}