﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LASI.FileSystem
{
    /// <summary>
    /// This class serves as entity wrapper around entity file newPath, providing for direct access to the indvidual components of the newPath.
    /// </summary>
    public abstract class InputFile
    {
        /// <summary>
        /// Initializes entity new instance of the InputFile class.
        /// </summary>
        /// <param name="newPath"></param>
        public InputFile(string path) {
            if (!System.IO.File.Exists(path))
                throw new System.IO.FileNotFoundException();
            FInfo = new FileData(path);
        }
        private FileData FInfo {
            get;
            set;
        }
        /// <summary>
        /// Gets the full newPath, including the file name and extension of the file.
        /// </summary>
        public string FullPath {
            get {
                return FInfo.FullPathAndExt;
            }
        }
        /// <summary>
        /// Gets the newPath, including the file name, but not the extension, of the file.
        /// </summary>
        public string PathSansExt {
            get {
                return FInfo.FullPathSansExt;
            }
        }
        /// <summary>
        /// Gets the filename, including its extension.
        /// </summary>
        public string Name {
            get {
                return FInfo.FileNameWithExt;
            }
        }
        /// <summary>
        /// Gets the filename, not including its extension.
        /// </summary>
        public string NameSansExt {
            get {
                return FInfo.FileNameSansExt;
            }
        }
        /// <summary>
        /// Gets the extension of the file.
        /// </summary>
        public string Ext {
            get {
                return FInfo.FileExt;
            }
        }
        /// <summary>
        /// Gets the newPath of the full newPath of the directory in which the file resides.
        /// </summary>
        public string Directory {
            get {
                return FInfo.Directory;
            }
        }

        public override bool Equals(object obj) {
            return this == obj as InputFile;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return FullPath;
        }
        public static bool operator ==(InputFile lhs, InputFile rhs) {
            if (lhs as object == null && rhs as object == null)
                return true;
            else if (rhs as object == null || lhs as object == null)
                return false;
            else
                return lhs.Directory == rhs.Directory && lhs.Ext == rhs.Ext && lhs.FullPath == rhs.FullPath && lhs.Name == rhs.Name && lhs.NameSansExt == rhs.NameSansExt && lhs.PathSansExt == rhs.PathSansExt;
        }
        public static bool operator !=(InputFile lhs, InputFile rhs) {
            return !(lhs == rhs);
        }


    }
}
