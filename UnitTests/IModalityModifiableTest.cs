﻿using LASI.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LASI.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for IModalityModifiableTest and is intended
    ///to contain all IModalityModifiableTest Unit Tests
    ///</summary>
    [TestClass]
    public class IModalityModifiableTest
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


        internal virtual IModalityModifiable CreateIModalityModifiable() {

            IModalityModifiable target = new SimpleVerb("laugh");
            return target;
        }

        /// <summary>
        ///A test for Modality
        ///</summary>
        [TestMethod]
        public void ModalityTest() {
            IModalityModifiable target = CreateIModalityModifiable();
            ModalAuxilary expected = new ModalAuxilary("might");
            ModalAuxilary actual;
            target.Modality = expected;
            actual = target.Modality;
            Assert.AreEqual(expected, actual); 
        }

        
    }
}
