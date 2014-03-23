﻿
using LASI.Core.DocumentStructures;
using LASI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LASI.Core
{

    /// <summary>
    /// Provides the base class, properties, and behaviors for all Phrase level grammatical constructs.
    /// </summary>
    public abstract class Phrase : ILexical
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Phrase class.
        /// </summary>
        /// <param name="composedWords">The one or more instances of the word class of which the Phrase is composed.</param>
        protected Phrase(IEnumerable<Word> composedWords) {
            Words = composedWords;
            Weight = 1;
            MetaWeight = 1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overrides the ToString method to augment the string representation of Phrase to include the text of the words it is composed of.
        /// </summary>
        /// <returns>A string containing the Type information of the instance as well as the textual representations of the words it is composed of.</returns>
        public override string ToString() {
            return Type.Name + " \"" + Text + "\"";
        }
        /// <summary>
        /// Establish the nested links between the Phrase, its parent Clause, and the Words comprising it.
        /// </summary>
        /// <param name="parent">The Clause to which the Phrase belongs.</param>
        public void EstablishParent(Clause parent) {
            Clause = parent;
            Sentence = parent.Sentence;
            Document = Sentence.Document;
            foreach (var w in Words) { w.EstablishParent(this); }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Prepositional construct which is lexically to the right of the word.
        /// </summary>
        public IPrepositional PrepositionOnRight { get; set; }
        /// <summary>
        /// Gets or sets the Prepositional construct which is lexically to the left of the Phrase.
        /// </summary>
        public IPrepositional PrepositionOnLeft { get; set; }

        /// <summary>
        /// Gets, lexically speaking, the next Phrase in the Document to which the instance belongs.
        /// </summary>
        public Phrase NextPhrase { get; internal set; }
        /// <summary>
        /// Gets, lexically speaking, the previous Phrase in the Document to which the instance belongs.
        /// </summary>
        public Phrase PreviousPhrase { get; internal set; }
        /// <summary>
        /// Gets or sets the Clause to which the Phrase belongs.
        /// </summary>
        public Clause Clause { get; private set; }
        /// <summary>
        /// Gets or sets the Sentence to which the Phrase belongs.
        /// </summary>
        public Sentence Sentence { get; private set; }
        /// <summary>
        /// Gets or the Paragraph to which the Phrase belongs.
        /// </summary>
        public Paragraph Paragraph { get { return Sentence != null ? Sentence.Paragraph : null; } }
        /// <summary>
        /// Gets or set the Document instance to which the Phrase belongs.
        /// </summary>
        public Document Document { get; private set; }
        /// <summary>
        /// Gets the concatenated text content of all of the words which comprise the Phrase.
        /// </summary>
        public string Text {
            get {
                text = text ?? string.Join(" ", Words.Select(w => w.Text));
                return text;
            }
        }

        /// <summary>
        /// Gets the collection of words which comprise the Phrase.
        /// </summary>
        public IEnumerable<Word> Words { get; private set; }



        /// <summary>
        /// Gets the System.Type of the current Instance.
        /// </summary>
        public Type Type {
            get {
                return GetType();
            }
        }

        /// <summary>
        /// Gets or sets the numeric Weight of the Phrase within the context of its document.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the numeric Weight of the Phrase over the context of all extant documents.
        /// </summary>
        public double MetaWeight { get; set; }

        #region Fields

        private string text;

        #endregion

        #endregion

        #region Static Members

        #region Static Properties
        /// <summary>
        /// Controls the level detail of the information provided by the ToString method of all instances of the Phrase class.
        /// </summary>
        public static bool VerboseOutput { get; set; }
        #endregion

        #endregion
    }
}