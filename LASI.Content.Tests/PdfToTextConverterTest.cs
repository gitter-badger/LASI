﻿using LASI.Content;
using System;
using System.Threading.Tasks;
using Xunit;
using NFluent;

namespace LASI.Content.Tests
{

    /// <summary>
    ///This is a test class for PdfToTextConverterTest and is intended
    ///to contain all PdfToTextConverterTest Unit Tests
    /// </summary>
    public class PdfToTextConverterTest : FileConverterTestBase<PdfFile>
    {
        public PdfToTextConverterTest() : base("Draft_Environmental_Assessment.pdf") { }

        protected sealed override Func<string, PdfFile> SourceFactory => path => new PdfFile(path);

        /// <summary>
        ///A test for PdfToTextConverter Constructor
        /// </summary>
        [Fact]
        public void PdfToTextConverterConstructorTest()
        {
            var infile = SourceFile;
            var target = new PdfToTextConverter(infile);
            Check.That(infile).IsEqualTo(target.Original);
        }

        /// <summary>
        ///A test for ConvertFile
        /// </summary>
        [Fact]
        public void ConvertFileTest()
        {
            var infile = SourceFile;
            var target = new PdfToTextConverter(infile);
            var expected = infile.LoadText();
            string actual;
            actual = target.ConvertFile().LoadText();
            Check.That(actual).IsEqualTo(expected);
        }

        /// <summary>
        ///A test for ConvertFileAsync
        /// </summary>
        [Fact]
        public void ConvertFileAsyncTest()
        {
            var infile = SourceFile;
            var target = new PdfToTextConverter(infile);
            var expected = infile.LoadText();
            string actual;
            actual = target.ConvertFileAsync().Result.LoadText();
            Check.That(actual).IsEqualTo(expected);
        }
    }
}
