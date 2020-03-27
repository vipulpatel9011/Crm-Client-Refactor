using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Aurea.CRM.Client.UI.Common
{    
    public static class FileExtensions
    {
        private static readonly object _lock = new object();
        public static byte[] ReadAllBytesWithRead(string filePath)
        {
            lock (_lock)
            {
                FileStream fs = null;
                MemoryStream ms = new MemoryStream();
                try
                {
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
                catch { return ms.ToArray(); }
                finally
                {
                    if(ms != null)
                    {
                        ms.Close();
                        ms.Dispose();
                    }

                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
        }
    }
}
