﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.ContentSystem
{
    /// <summary>
    /// The exception thrown when methods are invoked or preperties accessed on the FilaManager before a call has been made to initialize it.
    /// </summary>
    [Serializable]
    public class FileManagerNotInitializedException : FileManagerException
    {
        /// <summary>
        /// Initializes a new instance of the FileManagerException class with with its message string set to message.
        /// </summary> 
        public FileManagerNotInitializedException()
            : base("File Manager has not been initialized. No directory context in which to operate.") {
        }

        private FileManagerNotInitializedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }
    }


    #region Dirived Types
    /// <summary>
    /// The Exception thrown when an attempt is made to add a file of an ussuported type to a project.
    /// </summary>
    [Serializable]
    public class UnsupportedFileTypeAddedException : FileManagerException
    {
        private static string FormatMessage(string unsupportedFormat) {
            return string.Format(
                 "Files of type \"{0}\" are not supported. Supported types are {1}, {2}, {3}, and {4}",
                 unsupportedFormat,
                 from k in FileManager.WrapperMap.Keys.Take(4)
                 select k);
        }
        /// <summary>
        /// Initializes a new instance of the UnsupportedFileTypeAddedException class with with its message string set to message.
        /// </summary>
        /// <param name="unsupportedFormat">A description of the error. The content of message is intended to be understood</param>
        public UnsupportedFileTypeAddedException(string unsupportedFormat)
            : base(FormatMessage(unsupportedFormat)) {
        }
        /// <summary>
        /// Initializes a new instance of the UnsupportedFileTypeAddedException class with with its message string set to message.
        /// </summary>
        /// <param name="unsupportedFormat">A description of the error. The content of message is intended to be understood</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not null, the current exception is raised in a catch block that
        /// handles the inner exception.
        /// </param>
        public UnsupportedFileTypeAddedException(string unsupportedFormat, Exception inner)
            : base(FormatMessage(unsupportedFormat), inner) {

        }
        /// <summary>
        /// Initializes a new instance of the UnsupportedFileTypeAddedException class with with its message string set to message.
        /// </summary>
        /// <param name="info">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        /// <param name="context">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        private UnsupportedFileTypeAddedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }
    }

    /// <summary>
    /// The base class for all Exceptions thrown by the FileManager.
    /// </summary>
    [Serializable]
    public abstract class FileManagerException : FileSystemException
    {
        /// <summary>
        /// Initializes a new instance of the FileManagerException class with with its message string set to message.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood</param>
        protected FileManagerException(string message)
            : base(message) {
            CollectDirInfo();
        }
        /// <summary>
        /// Initializes a new instance of the FileManagerException class with with its message string set to message.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not null, the current exception is raised in a catch block that
        /// handles the inner exception.
        /// </param>
        protected FileManagerException(string message, Exception inner)
            : base(message, inner) {
            CollectDirInfo();
        }

        /// <summary>
        /// Initializes a new instance of the FileManagerException class with with its message string set to message.
        /// </summary>
        /// <param name="info">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        /// <param name="context">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        protected FileManagerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
            CollectDirInfo();
        }
        /// <summary>
        /// Sets data about the current contents of the ProjectDirectory at the time the FileManagerException is constructed.
        /// </summary>
        protected virtual void CollectDirInfo() {
            if (FileManager.Initialized)
                filesInProjectDirectories = from internalFile in new DirectoryInfo(FileManager.ProjectDir).EnumerateFiles("*", SearchOption.AllDirectories)
                                            select FileManager.WrapperMap[internalFile.Extension](internalFile.FullName);
        }

        private IEnumerable<InputFile> filesInProjectDirectories = new List<InputFile>();
        /// <summary>
        /// Gets data about the contents of the ProjectDirectory when the FileManagerException was constructed.
        /// </summary>
        public IEnumerable<InputFile> FilesInProjectDirectories {
            get {
                return filesInProjectDirectories;
            }
            protected set {
                filesInProjectDirectories = value;
            }
        }

    }

    /// <summary>
    /// The base class for all file related exceptions within the LASI framework.
    /// </summary>
    [Serializable]
    public abstract class FileSystemException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the FileSystemException class with with its message string set to message.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood</param>
        protected FileSystemException(string message)
            : base(message) {

        }
        /// <summary>
        /// Initializes a new instance of the FileSystemException class with with its message string set to message.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not null, the current exception is raised in a catch block that
        /// handles the inner exception.
        /// </param>
        protected FileSystemException(string message, Exception inner)
            : base(message, inner) {

        }

        /// <summary>
        /// Initializes a new instance of the FileSystemException class with with its message string set to message.
        /// </summary>
        /// <param name="info">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        /// <param name="context">
        /// The object that holds the serialized object data about the exception being
        /// thrown.</param>
        protected FileSystemException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }
    }

    #endregion


}