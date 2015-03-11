﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace LASI.Utilities.Tests
{
    [TestClass]
    public class CharExtensionsTest
    {
        private const string Consonants = "bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";
        private const string Vowels = "aeiouyAEIOUY";
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [TestMethod]
        public void IsConsonantTest()
        {
            foreach (var c in Consonants)
            {
                Assert.IsTrue(c.IsConsonant());
            }
        }

        [TestMethod]
        public void IsVowelTest()
        {
            foreach (var c in Vowels)
            {
                Assert.IsTrue(c.IsVowel());
            }
        }

        [TestMethod]
        public void IsEnglishLetterTest()
        {
            for (var c = char.MinValue; c < char.MaxValue; ++c)
            {
                if (Alphabet.Contains(c))
                {
                    Assert.IsTrue(c.IsEnglishLetter());
                }
                else
                {
                    Assert.IsFalse(c.IsEnglishLetter());
                }
            }
        }
        [TestMethod]
        public void IsUpperTest()
        {
            for (char c = char.MinValue; c < char.MaxValue; ++c) { Assert.AreEqual(c.IsUpper(), char.IsUpper(c)); }
        }
        [TestMethod]
        public void IsLowerTest()
        {
            for (char c = char.MinValue; c < char.MaxValue; ++c) { Assert.AreEqual(c.IsLower(), char.IsLower(c)); }
        }

        [TestMethod]
        public void EqualsIgnoreCaseTest()
        {
            var lowerCaseLetters = Alphabet.Take(26);
            foreach (char lower in lowerCaseLetters)
            {
                char upper = char.ToUpper(lower);
                Assert.IsTrue(upper.EqualsIgnoreCase(lower));
                Assert.IsTrue(lower.EqualsIgnoreCase(upper));
                Assert.IsTrue(upper.EqualsIgnoreCase(upper));
                Assert.IsTrue(lower.EqualsIgnoreCase(lower));
                Assert.AreEqual(50, Alphabet.Count(c => !c.EqualsIgnoreCase(lower)));
                Assert.AreEqual(2, Alphabet.Count(c => c.EqualsIgnoreCase(lower)));
                Assert.AreEqual(50, Alphabet.Count(c => !c.EqualsIgnoreCase(upper)));
                Assert.AreEqual(2, Alphabet.Count(c => c.EqualsIgnoreCase(upper)));
            }
            var random = new Random();
            Func<int> rand = () => random.Next(char.MinValue, char.MaxValue + 1);
            var randomCharacters = Enumerable
                .Range(0, int.MaxValue)
                .Select(n => (char)rand())
                .Where(c => !(c.IsEnglishLetter() || char.ToUpper(c).Equals(c)))
                .Take(100);
            foreach (var character in randomCharacters)
            {
                Assert.IsFalse(character.EqualsIgnoreCase(character.ToUpper()), $"{character}({(int)character}) IS equal to {character.ToUpper()}({(int)character.ToUpper()})");
            }
        }
    }
}