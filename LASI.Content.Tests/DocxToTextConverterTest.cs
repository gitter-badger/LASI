﻿using System;
using System.IO;
using System.Threading.Tasks;
using NFluent;

namespace LASI.Content.Tests
{
    using Shared.Test.NFluentExtensions;
    using Fact = Xunit.FactAttribute;

    public class DocxToTextConverterTest : FileConverterTestBase<DocXFile>
    {
        public DocxToTextConverterTest() : base("Draft_Environmental_Assessment.docx") { }

        protected sealed override Func<string, DocXFile> SourceFactory => path => new DocXFile(path);

        /// <summary>
        ///A test for ConvertFileAsync
        /// </summary>
        [Fact]
        public async Task ConvertFileAsyncTest()
        {
            var infile = SourceFile;
            var target = new DocxToTextConverter(infile);
            TxtFile actual;
            actual = await target.ConvertFileAsync();
            Check.That(actual.FullPath).Satisfies(File.Exists);
        }
        /// <summary>
        ///A test for ConvertFile
        /// </summary>
        [Fact]
        public void ConvertFileTest()
        {
            var infile = SourceFile;
            var target = new DocxToTextConverter(infile);
            TxtFile actual;
            actual = target.ConvertFile();
            Check.That(actual.FullPath).Satisfies(File.Exists);
        }
        /// <summary>
        ///A test for DocxToTextConverter Constructor
        /// </summary>
        [Fact]
        public void DocxToTextConverterConstructorTest()
        {
            var infile = SourceFile;
            var target = new DocxToTextConverter(infile);
            Check.That(target.Original.FullPath).IsEqualTo(infile.FullPath);
        }
    }
}
