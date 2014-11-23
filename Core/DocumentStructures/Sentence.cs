﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Utilities;

namespace LASI.Core
{
    /// <summary>
    /// Represents a single sentence.
    /// </summary>
    public sealed class Sentence : IReifiedTextual
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Sentence class.
        /// </summary>
        /// <param name="phrases">The sequence of Phrase elements which comprise the Sentence.</param>
        /// <param name="ending">The SentenceEnding which terminates the Sentence. If not provided, a period will be assumed, and an instance of SentenceEnding created to represent it.</param>
        public Sentence(IEnumerable<Phrase> phrases, SentenceEnding ending) {
            Clauses = new[] { new Clause(phrases) };
            EndingPunctuator = ending ?? SentenceEnding.Period;
        }

        /// <summary>
        /// Initializes a new instance of the Sentence class.
        /// </summary>
        /// <param name="clauses">The sequence of Clause elements which comprise the Sentence.</param>
        /// <param name="ending">The SentenceEnding which terminates the Sentence. If not provided, a period will be assumed, and an instance of SentenceEnding created to represent it.</param>
        public Sentence(IEnumerable<Clause> clauses, SentenceEnding ending) {
            Clauses = clauses;
            EndingPunctuator = ending ?? SentenceEnding.Period;
        }
        /// <summary>
        /// Returns the Phrase elements in the Sentence, following and not including the given Phrase. 
        /// </summary>
        /// <param name="phrase">The Phrase from which to start.</param>
        /// <returns>The Phrase elements in the Sentence, following and not including the given Phrase. </returns>
        public IEnumerable<Phrase> GetPhrasesAfter(Phrase phrase) => Phrases.SkipWhile(r => r != phrase).Skip(1);
        #endregion

        #region Methods
        /// <summary>
        /// Returns a string representation of the Sentence.
        /// </summary>
        /// <returns>A string representation of the Sentence.</returns>
        public override string ToString() => "\{base.ToString()} \"\{Text}\"";

        #endregion

        #region Properties
        /// <summary>
        /// Gets the ending punctuation character of the sentence.
        /// </summary>
        public SentenceEnding EndingPunctuator { get; }

        /// <summary>
        /// Establishes the linkages between the Sentence, its parent Paragraph, and its child Clauses.
        /// </summary>
        /// <param name="parent">The Paragraph to which the Sentence belongs.</param>
        public void EstablishParenthood(Paragraph parent) {
            EndsParagraph = this == parent.Sentences.Last();
            BeginsParagraph = this == parent.Sentences.First();
            Paragraph = parent; Clauses.ToList().ForEach(c => c.EstablishParent(this));
        }



        /// <summary>
        /// Gets the sequence of Clauses which comprise the sentence.
        /// </summary>
        public IEnumerable<Clause> Clauses { get; }
        /// <summary>
        /// Gets the sequence of Phrases which comprise the sentence.
        /// </summary>
        /// 
        public IEnumerable<Phrase> Phrases {
            get {
                return from clause in Clauses
                       from phrase in clause.Phrases
                       select phrase;
            }
        }
        /// <summary>
        /// Gets the sequence of Words which comprise the sentence.
        /// </summary>
        public IEnumerable<Word> Words => Clauses.SelectMany(clause => clause.Phrases.SelectMany(phrase => phrase.Words));

        /// <summary>
        /// Gets the concatenated text content of all of the Words which compose the Sentence.
        /// </summary>
        public string Text => string.Join(" ", Phrases.Select(e => e.Text)).Trim() + EndingPunctuator.Text;



        /// <summary>
        /// Gets the Paragraph to which the Sentence belongs.
        /// </summary>
        public Paragraph Paragraph { get; private set; }
        /// <summary>
        /// Gets the Document to which the Sentence Belongs.
        /// </summary>
        public Document Document => Paragraph?.Document;

        /// <summary>
        /// Gets or sets a value indicating whether the Sentence is an inverted sentence.
        /// That is a sentence in which the Verbal precedes its subject. ex: <para>
        /// inverted -> "Vastly overblown were the rumors."
        /// standard -> "The rumors were vastly overblown."
        /// </para>
        /// </summary>
        public bool IsInverted { get; set; }

        public IEnumerable<ILexical> Lexicals {
            get {
                foreach (var clause in Clauses) {
                    yield return clause;
                    foreach (var phrase in clause.Phrases) {
                        yield return phrase; foreach (var word in phrase.Words) {
                            yield return word;
                        }
                    }
                }
            }
        }

        public IEnumerable<IEntity> Entities => Lexicals.OfEntity();

        public IEnumerable<IVerbal> Verbals => Lexicals.OfVerbal();

        #endregion

        /// <summary>
        /// Gets a value indicating whether the Sentence is the first Sentence in its Paragraph.
        /// </summary>
        public bool BeginsParagraph { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the Sentence is the Last Sentence in its Paragraph.
        /// </summary>
        public bool EndsParagraph { get; private set; }


    }

}
