﻿using LASI.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LASI.UnitTests
{


    /// <summary>
    ///This is a test class for PastParticipleVerbTest and is intended
    ///to contain all PastParticipleVerbTest Unit Tests
    ///</summary>
    [TestClass]
    public class PastParticipleVerbTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PastParticipleVerb Constructor
        ///</summary>
        [TestMethod]
        public void PastParticipleVerbConstructorTest() {
            string text = "abided";
            PastParticiple target = new PastParticiple(text);
            Assert.AreEqual(text, target.Text);
            Assert.AreEqual(VerbForm.PastParticiple, target.VerbForm);
        }
    }
}
