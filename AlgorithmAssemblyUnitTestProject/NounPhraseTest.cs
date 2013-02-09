﻿using LASI.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AlgorithmAssemblyUnitTestProject
{


    /// <summary>
    ///This is a test class for NounPhraseTest and is intended
    ///to contain all NounPhraseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NounPhraseTest
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
        ///A test for NounPhrase Constructor
        ///</summary>
        [TestMethod()]
        public void NounPhraseConstructorTest() {
            IEnumerable<Word> composedWords = new Word[] { new ProperPluralNoun("Americans"), new Conjunction("and"), new ProperPluralNoun("Canadians") };
            NounPhrase target = new NounPhrase(composedWords);
            Assert.AreEqual(target.Words, composedWords);
        }


        /// <summary>
        ///A test for BindDescriber
        ///</summary>
        [TestMethod()]
        public void BindDescriberTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            IDescriber adj = null; // TODO: Initialize to an appropriate value
            target.BindDescriber(adj);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for BindPronoun
        ///</summary>
        [TestMethod()]
        public void BindPronounTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            IEntityReferencer pro = null; // TODO: Initialize to an appropriate value
            target.BindPronoun(pro);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for DetermineHeadWord
        ///</summary>
        [TestMethod()]
        public void DetermineHeadWordTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            target.DetermineHeadWord();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            IEntity other = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DescribedBy
        ///</summary>
        [TestMethod()]
        public void DescribedByTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            ICollection<IDescriber> actual;
            actual = target.DescribedBy;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DirectObjectOf
        ///</summary>
        [TestMethod()]
        public void DirectObjectOfTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            ITransitiveAction expected = null; // TODO: Initialize to an appropriate value
            ITransitiveAction actual;
            target.DirectObjectOf = expected;
            actual = target.DirectObjectOf;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IndirectObjectOf
        ///</summary>
        [TestMethod()]
        public void IndirectObjectOfTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            ITransitiveAction expected = null; // TODO: Initialize to an appropriate value
            ITransitiveAction actual;
            target.IndirectObjectOf = expected;
            actual = target.IndirectObjectOf;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IndirectReferences
        ///</summary>
        [TestMethod()]
        public void IndirectReferencesTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            ICollection<IEntityReferencer> actual;
            actual = target.IndirectReferences;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Possessed
        ///</summary>
        [TestMethod()]
        public void PossessedTest() {
            IEnumerable<Word> composedWords = null; // TODO: Initialize to an appropriate value
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            ICollection<IEntity> actual;
            actual = target.Possessed;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Possesser
        ///</summary>
        [TestMethod()]
        public void PossesserTest() {
            IEnumerable<Word> composedWords = new Word[] { new ProperPluralNoun("Americans"), new Conjunction("and"), new ProperPluralNoun("Canadians") };
            NounPhrase target = new NounPhrase(composedWords);
            IEntity expected = new NounPhrase(new[] { new ProperSingularNoun("North"), new ProperSingularNoun("America") });
            IEntity actual;
            target.Possesser = expected;
            actual = target.Possesser;
            Assert.AreEqual(expected, actual);
   
        }

        /// <summary>
        ///A test for SubjectOf
        ///</summary>
        [TestMethod()]
        public void SubjectOfTest() {
            IEnumerable<Word> composedWords = new Word[] { new ProperPluralNoun("Americans"), new Conjunction("and"), new ProperPluralNoun("Canadians") };
            NounPhrase target = new NounPhrase(composedWords); // TODO: Initialize to an appropriate value
            IAction expected = new Verb("are");
            IAction actual;
            target.SubjectOf = expected;
            actual = target.SubjectOf;
            Assert.AreEqual(expected, actual);

        }
    }
}
