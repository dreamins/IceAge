using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace IceAge.type
{
    class UploadUnit
    {
        public string FileName {get; private set;}
        public string FullName {get; private set;}
        public long Size { get; private set; }
        public String Checksum {get; private set;}

        public UploadUnit(string name, string fullName, long size)
        {
            this.FileName = name;
            this.FullName = fullName;
            this.Size = size;
        }

        public String calculateChecksum() {

            if (Checksum != null)
            {
                FileStream file = null;
                try
                {
                    file = new FileStream(FullName, FileMode.Open);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] hash = md5.ComputeHash(file);
                    file.Close();
                    file = null;

                    StringBuilder sb = new StringBuilder();
                    Array.ForEach(hash, delegate(byte val) { sb.Append(val.ToString("x2")); });
                    Checksum = sb.ToString();
                }
                finally
                {
                    if (file != null)
                    {
                        file.Close();
                    }
                }
            }

            return Checksum;
        }
    }
}
