﻿using LASI.Algorithm.FundamentalSyntacticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LASI.Algorithm
{

    /// <summary>
    /// Provides the base class, properties, and behaviors for all phrase level gramatical constructs.
    /// </summary>
    public abstract class Phrase : IPrepositionLinkable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Phrase class.
        /// </summary>
        /// <param name="composedWords">The one or more instances of the Word class of which the Phrase is composed.</param>
        protected Phrase(IEnumerable<Word> composedWords) {
            ID = IDProvider++;
            Words = composedWords;

            Weights = new Dictionary<Weighting.WeightKind, Weighting.Weight>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overrides the ToString method to augment the string representation of Phrase to include the text of the words it is composed of.
        /// </summary>
        /// <returns>a string containing the type information of the instance as well as the textual representations of the words it is composed of.</returns>
        public override string ToString() {
            return GetType().Name + " \"" + Text + "\"";
        }
        public virtual string ToString(bool verbose) {
            return ToString();
        }

        public void EstablishParent(Clause clause) {
            ParentSentence = clause.ParentSentence;
            foreach (var w in Words)
                w.EstablishParent(this);
        }






        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Prepositional construct which is lexically to the right of the Word.
        /// </summary>
        public IPrepositional PrepositionOnRight {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Prepositional construct which is lexically to the left of the Phrase.
        /// </summary>
        public IPrepositional PrepositionOnLeft {
            get;
            set;
        }

        /// <summary>
        /// Gets, lexically speaking, the next Phrase in the ParentDocument to which the instance belongs.
        /// </summary>
        public Phrase NextPhrase {
            get;
            set;
        }
        /// <summary>
        /// Gets, lexically speaking, the previous Phrase in the ParentDocument to which the instance belongs.
        /// </summary>
        public Phrase PreviousPhrase {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Sentence the Phrase belongs to.
        /// </summary>
        public Sentence ParentSentence {
            get;
            set;
        }
        /// <summary>
        /// Gets or set the Document instance to which the Phrase belongs.
        /// </summary>
        public Document ParentDocument {
            get;
            private set;
        }
        /// <summary>
        /// Gets the concatenated text content of all of the words which compose the phrase.
        /// </summary>
        public virtual string Text {
            get {
                return Words.Aggregate("", (str, word) => str + " " + word.Text).Trim();
            }
        }
        /// <summary>
        /// Gets the collection of words which comprise the phrase.
        /// </summary>
        public virtual IEnumerable<Word> Words {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the globally-unique identification number associated with the Phrase instance.
        /// </summary>
        public int ID {
            get;
            private set;
        }

        public Dictionary<Weighting.WeightKind, Weighting.Weight> Weights {
            get;
            private set;
        }


        #endregion

        #region Static Members

        private static int IDProvider;

        #endregion

    }
}