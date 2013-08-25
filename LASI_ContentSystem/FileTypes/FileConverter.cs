﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.ContentSystem
{
    /// <summary>
    /// The base class from which file format conversion objects are derrived.
    /// Provides a small set of common methods, properties, and attributes which all file conversion objects.
    /// Any new file converters should be derrived from this base class.
    /// <see cref="DocToDocXConverter"/>
    /// <see cref="DocxToTextConverter"/>
    /// </summary>
    public abstract class FileConverter<TSource, TDestination>
        where TSource : InputFile
        where TDestination : InputFile
    {
        /// <summary>
        /// The location where the converted file will be saved.
        /// </summary>
        protected string destinationDir;
        /// <summary>
        /// The location of the source file.
        /// </summary>
        protected string sourcePath;
        /// <summary>
        /// Initializes a new instance of the FileConverter class.
        /// </summary>
        /// <param name="infile">The file to convert.</param>
        protected FileConverter(TSource infile) {
            sourcePath = infile.FullPath;
            destinationDir = infile.Directory;
            Original = infile;
        }
        /// <summary>
        /// Initializes a new instance of the FileConverter class.
        /// </summary>
        /// <param name="infile">The file to convert.</param>
        /// <param name="targetDir">The location in which to save the converted file.</param>
        protected FileConverter(TSource infile, string targetDir) {
            sourcePath = infile.FullPath;
            destinationDir = targetDir;
            Original = infile;
        }

        /// <summary>
        /// When overriden in a derrived class, this method invokes the file conversion routine on the file which the instance governs.
        /// </summary>
        /// <returns>An instance of input file representing the file in its converted format.</returns>
        public abstract TDestination ConvertFile();

        /// <summary>
        /// When overridden in a dirrrived class, this method invokes the file conversion routine asynchronously, gernerally in a serparate thread.
        /// Use with the await operator in an asnyc method to retrieve the new file object and specify a continuation function to be executed when the conversion is complete.
        /// </summary>
        /// <returns>A Task&lt;InputFile&gt; object which functions as a proxy for the actual InputFile while the conversion routine is in progress.
        /// Access the internal input file encapsulated by the Task by using syntax such as : var file = await myConverter.ConvertFileAsync()
        /// </returns>
        public abstract Task<TDestination> ConvertFileAsync();


        /// <summary>
        /// Gets the document which is to be converted to the destination format
        /// </summary>
        public TSource Original {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the document object which is the fruit of the conversion process
        /// This additional method of accessing the new document is primarily provided to facilitate asynchronous programming
        /// and any access attempts before the conversion is complete will raise a NullReferenceException.
        /// </summary>
        public abstract TDestination Converted {
            get;
            protected set;
        }
    }
}
