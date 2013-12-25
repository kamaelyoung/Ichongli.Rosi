using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ichongli.Rosi
{
    public class JsonTryParse
    {
        public static string Encode<T>(T obj)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));

                using (var stream = new MemoryStream())
                {
                    json.WriteObject(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] b = new byte[stream.Length];
                    stream.Read(b, 0, b.Length);
                    return System.Text.Encoding.UTF8.GetString(b, 0, b.Length);
                }
            }
            catch { }
            return null;
        }

        public static T Parse<T>(Stream stream)
        {
            try
            {
                if (stream != null)
                {
                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                    return (T)js.ReadObject(stream);
                }
            }
            catch (Exception ex) { }
            return default(T);
        }

        public static T Parse<T>(string json)
        {
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                    {
                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                        return (T)js.ReadObject(ms);
                    }
                }
            }
            catch (Exception ex) { }
            return default(T);
        }

        public static void Write<T>(string drieName, string fileName, T items)
        {
            try
            {
                if (items != null)
                {
                    using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        string path = System.IO.Path.Combine(drieName, fileName);
                        if (!iso.DirectoryExists(drieName))
                        {
                            iso.CreateDirectory(drieName);
                        }
                        else
                        {
                            if (iso.FileExists(path))
                            {
                                iso.DeleteFile(path);
                            }
                        }

                        if (iso.FileExists(path))
                        {
                            iso.DeleteFile(path);
                        }
                        using (var istream = new IsolatedStorageFileStream(path, FileMode.Create, iso))
                        {
                            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                            js.WriteObject(istream, items);
                        }
                    }
                }
            }
            catch { }
        }

        public static T Read<T>(string dire, string file)
        {
            try
            {
                string path = System.IO.Path.Combine(dire, file);
                IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
                if (iso.FileExists(path))
                {
                    using (var istream = new IsolatedStorageFileStream(path, FileMode.Open, iso))
                    {
                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                        return (T)js.ReadObject(istream);
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }
    }
}
