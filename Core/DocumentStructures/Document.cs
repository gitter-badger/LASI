﻿using LASI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LASI.Core.DocumentStructures
{
    /// <summary>
    /// <para> A data structure containing all of the paragraph, sentence, clause, phrase, and word objects which comprise a single document.</para>
    /// <para> Provides overalapping direct and indirect access to all of its children, </para>
    /// <para> e.g. such as myDoc.Paragraphs.Sentences.Phrases.Words will get all the words in the document in linear order 
    /// comparatively: myDoc.Words; yields the same collection. </para>
    /// </summary>
    public sealed class Document
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Document class comprised from the provided paragraphs and having the provided name.
        /// </summary>
        /// <param name="content">The collection of paragraphs which contain all text in the document.</param>
        /// <param name="name">The name for the document.</param>
        public Document(IEnumerable<Paragraph> content, string name) : this(content) { Name = name; }
        /// <summary>
        /// Initializes a new instance of the Document class comprised from the provided paragraphs.
        /// </summary>
        /// <param name="content">The collection of paragraphs which contain all text in the document.</param>
        public Document(IEnumerable<Paragraph> content) {
            this.paragraphs = content.ToList();
            paragraphsWithBulletsOrHeadings =
                (from p in this.paragraphs
                 where p.ParagraphKind == ParagraphKind.NumberedOrBullettedContent
                 select p).ToList();

            AssignMembers();
            foreach (var p in this.paragraphs) {
                p.EstablishParent(this);
            }
            LinksAdjacentElements();
        }

        private void AssignMembers() {
            sentences = (from p in this.paragraphs
                         from s in p.Sentences
                         where s.Words.OfVerb().Any()
                         select s).ToList().AsReadOnly();
            phrases = (from s in sentences
                       from r in s.Phrases
                       select r).ToList().AsReadOnly();
            words = (from s in sentences
                     from w in s.Words.Append(s.EndingPunctuation)
                     select w).ToList().AsReadOnly();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Establishes the linear linkages between all adjacent words and phrases in the Document.
        /// </summary>
        private void LinksAdjacentElements() {
            LinksAdjacentWords();
            LinksAdjacentPhrases();
        }
        private void LinksAdjacentWords() {
            if (words.Count > 1) {
                for (int i = 1; i < words.Count(); ++i) {
                    words[i].PreviousWord = words[i - 1];
                    words[i - 1].NextWord = words[i];
                }
                var lastWord = words[words.Count - 1];
                if (words.Count - 1 > 0) {
                    lastWord.PreviousWord = words[words.Count - 2];
                } else {
                    lastWord.PreviousWord = null;
                }
                lastWord.NextWord = null;
            }
        }
        private void LinksAdjacentPhrases() {
            if (phrases.Count > 1) {
                for (var i = 1; i < phrases.Count; ++i) {
                    phrases[i].PreviousPhrase = phrases[i - 1];
                    phrases[i - 1].NextPhrase = phrases[i];
                }
            }
        }


        /// <summary>
        /// Returns all of the verbals identified within the docimument.
        /// </summary>
        /// <returns>all of the verbals identified within the docimument.</returns>
        public IEnumerable<IVerbal> GetVerbals() {
            foreach (var action in words.OfType<IVerbal>())
                yield return action;
            foreach (var action in phrases.OfType<IVerbal>())
                yield return action;
            foreach (var action in Clauses.OfType<IVerbal>())
                yield return action;
        }

        /// <summary>
        /// Returns all of the entities identified in the document.
        /// </summary>
        /// <returns> All of the entities identified in the document.</returns>
        public IEnumerable<IEntity> GetEntities() {
            foreach (var entity in words.OfType<IEntity>())
                yield return entity;
            foreach (var entity in phrases.OfType<IEntity>())
                yield return entity;
            foreach (var entity in Clauses.OfType<IEntity>())
                yield return entity;

        }
        /// <summary>
        /// Returns all of lexical constructs in the document, including all words, phrases, and clauses.
        /// </summary>
        /// <returns>All of lexical constructs in the document, including all words, phrases, and clauses.</returns>
        public IEnumerable<ILexical> GetAllLexicalConstructs() {
            foreach (var lexical in words)
                yield return lexical;
            foreach (var lexical in phrases)
                yield return lexical;
            foreach (var lexical in Clauses)
                yield return lexical;

        }
        /// <summary>
        /// Returns a representation of the Document as sequence of pages based on the given number sentences per page.
        /// </summary>
        /// <param name="sentencesPerPage">The number of sentences each page can contain. This varies inversely with the number of pages in the resulting sequence.</param>
        /// <returns>A representation of the Document as sequence of pages.</returns>
        public IEnumerable<Page> Paginate(int sentencesPerPage) {
            if (sentencesPerPage < 1) {
                throw new ArgumentOutOfRangeException(
                    "sentencesPerPage",
                    "The supplied page length was invalid. A page must be allowed to have at least 1 sentence."
                );
            }
            return from subSequence in Sentences.Split(sentencesPerPage)
                   select new Page(subSequence, this);
        }
        /// <summary>
        /// Returns a string representation of the current document. The result contains the entire textual contents of the Document, thus resulting in the instance's full materialization and reification.
        /// </summary>
        /// <returns>A string representation of the current document. The result contains the entire textual contents of the Document, thus resulting in the instance's full materialization and reification.</returns>
        public override string ToString() {
            return this.GetType() + ":  " + Name + "\nParagraphs: \n" + Paragraphs.Format();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Sentences the document contains in linear, left to right order.
        /// </summary>
        public IEnumerable<Sentence> Sentences { get { return sentences; } }

        /// <summary>
        /// Gets the Paragraphs the document contains in linear, left to right order.
        /// </summary>
        public IEnumerable<Paragraph> Paragraphs { get { return paragraphs.Except(paragraphsWithBulletsOrHeadings); } }


        /// <summary>
        /// Gets the Clauses the document contains in linear, left to right order.
        /// </summary>
        public IEnumerable<Clause> Clauses { get { return from s in Sentences from c in s.Clauses select c; } }
        /// <summary>
        /// Gets the Phrases the document contains in linear, left to right order.
        /// </summary>
        public IEnumerable<Phrase> Phrases { get { return phrases; } }
        /// <summary>
        /// Gets the Words the document contains in linear, left to right order.
        /// </summary>
        public IEnumerable<Word> Words { get { return words; } }

        /// <summary>
        /// The name of the file the Document instance was parsed from.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Fields

        private IReadOnlyList<Word> words;
        private IReadOnlyList<Phrase> phrases;
        //private IList<Clause> _clauses;
        private IReadOnlyList<Sentence> sentences;
        private IReadOnlyList<Paragraph> paragraphs;
        private IReadOnlyList<Paragraph> paragraphsWithBulletsOrHeadings;



        #endregion


        /// <summary>
        /// Represents a page of a document. Pages are somewhat arbitrary segements of a Document, that contain some subset of its content.
        /// </summary>
        public sealed class Page
        {
            /// <summary>
            /// Initializes a new instance of the Page class.
            /// </summary>
            /// <param name="sentences">The Sentences which comprise the Page.</param>
            /// <param name="document">The Document to which the page belongs.</param>
            internal Page(IEnumerable<Sentence> sentences, Document document) {
                Document = document;
                Sentences = sentences;
            }

            /// <summary>
            /// Gets the Sentences which comprise the Page.
            /// </summary>
            public IEnumerable<Sentence> Sentences { get; private set; }
            /// <summary>
            /// Gets the Document to which the page belongs.
            /// </summary>
            public Document Document { get; private set; }
        }
    }


}