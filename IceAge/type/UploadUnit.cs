using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace IceAge.type
{
    class UploadUnit: IEquatable<UploadUnit>
    {
        public long Id { get; set; }
        public string FileName {get; private set;}
        public string FullName {get; private set;}
        public long Size { get; private set; }
        public long Timestamp { get; private set; }
        public String Checksum {get; set;}
        public bool InSync { get; set; }

        public UploadUnit(string name, string fullName, long size, long timestamp)
        {
            this.Id = -1;
            this.FileName = name;
            this.FullName = fullName;
            this.Size = size;
            this.Timestamp = timestamp;
            this.InSync = false;
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

        public bool Equals(UploadUnit other)
        {
            if (other == null)
                return false;

            return this.FileName.Equals(other.FileName) &&
                   this.FullName.Equals(other.FullName) &&
                   this.Size == other.Size &&
                   this.Timestamp == other.Timestamp;
        }
    }
}
