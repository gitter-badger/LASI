﻿using Xunit;

namespace LASI.Content.Tests
{
    /// <summary>
    ///This is a test class for ITaggedTextSourceTest and is intended
    ///to contain all ITaggedTextSourceTest Unit Tests
    /// </summary>
    public class ITaggedTextSourceTest
    {
        /// <summary>
        ///A test for LoadTextAsync
        /// </summary>
        [Fact]
        public void GetTextAsyncTest()
        {
            ITaggedTextSource target = CreateITaggedTextSource();
            string expected = ExpectedText;
            string actual;
            actual = target.LoadTextAsync().Result;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///A test for LoadText
        /// </summary>
        [Fact]
        public void GetTextTest()
        {
            ITaggedTextSource target = CreateITaggedTextSource();
            string expected = ExpectedText;
            string actual;
            actual = target.LoadText();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///A test for Name
        /// </summary>
        [Fact]
        public void NameTest()
        {
            ITaggedTextSource target = CreateITaggedTextSource();
            string actual;
            actual = target.Name;
            Assert.Equal("test fragment", actual);
        }

        internal virtual ITaggedTextSource CreateITaggedTextSource()
        {
            // TODO: Instantiate an appropriate concrete class.
            ITaggedTextSource target = new TaggedTextFragment(Tagger.TaggedFromRaw(new[] {
                "John enjoyed, with his usual lack of humility, consuming the object in question.",
                "Some may call him a heathen, but they are mistaken.",
                "Heathens are far less dangerous than he." }),
                "test fragment");
            return target;
        }


        private static Tagging.Tagger Tagger => new Tagging.Tagger();

        private static readonly string ExpectedText = Tagger.TaggedFromRaw(new[] {
                "John enjoyed, with his usual lack of humility, consuming the object in question.",
                "Some may call him a heathen, but they are mistaken.",
                "Heathens are far less dangerous than he." });

    }
}