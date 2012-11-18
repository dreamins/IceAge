using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Linq.Expressions;

namespace IceAge.type
{
    // This class is abused a bit, TODO: refactor into multiple classes
    // for ex: Progress/InSync have nothing to do with DB
    class UploadUnit : ObservableObject, IEquatable<UploadUnit>
    {
        public long Id { get; set; }
        public string FileName {get; private set;}
        public string FullName {get; private set;}
        public long Size { get; private set; }
        public long UploadedModTimestamp { get; set; }


        public String Checksum { get; set;}

        // refactor those out
        private long _Progress;
        public long Progress { get { return _Progress; } set { _Progress = value; OnPropertyChanged(() => this.Progress); } }
        
        // hmm
        private String _Speed;
        public String Speed { get { return _Speed; } set { _Speed = value; OnPropertyChanged(() => this.Speed); } }

        private bool _InSync;
        public bool InSync { get { return _InSync; } set { _InSync = value; OnPropertyChanged(() => this.InSync); } }

        public long FileTimestamp { get; private set; }
        public long UploadTimestamp { get; set; }

        public UploadUnit(string name, string fullName, long size, long timestamp)
        {
            this.Id = -1;
            this.FileName = name;
            this.FullName = fullName;
            this.Size = size;
            this.FileTimestamp = timestamp;
            this.InSync = false;
            this.Progress = 0;
        }

        public String calculateChecksum() {
            FileStream file = null;
            try
            {
                file = new FileStream(FullName, FileMode.Open);
                SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
                byte[] hash = sha256.ComputeHash(file);
                file.Close();
                file = null;

                StringBuilder sb = new StringBuilder();
                Array.ForEach(hash, delegate(byte val) { sb.Append(val.ToString("x2")); });
                return sb.ToString();
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        public bool Equals(UploadUnit other)
        {
            if (other == null)
                return false;

            return this.FileName.Equals(other.FileName) &&
                   this.FullName.Equals(other.FullName) &&
                   this.Size == other.Size &&
                   this.FileTimestamp == other.FileTimestamp;
        }
    }
}
