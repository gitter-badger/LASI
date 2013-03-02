﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace LASI.Algorithm
{
    public class Clause : ILexical
    {
        System.Xml.Linq.XElement ILexical.Serialize() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This class is currently experimental and is not a tier in the ParentDocument objects created by the tagged file parsers
        /// Initializes a new instnace of the Clause class, by composing the given linear sequence of phrases.
        /// </summary>
        /// <param name="phrases">The linear sequence of Phrases which compose to form the Clause.</param>
        public Clause(IEnumerable<Phrase> phrases) {
            Phrases = phrases;
        }
        public Clause(IEnumerable<Phrase> phrases, ClauseTypes clauseType) {
            Phrases = phrases;
            Kind = clauseType;
        }
        /// <summary>
        ///Initializes a new instnace of the Clause class, by composing the given linear sequence of words       
        ///As the words are bare in this context, that is not members of a known phrase object, they are subsequently implanted in an UndeterminedPhrase instance whose syntactic role should be determined contextually in the future.
        /// </summary>
        /// <param name="words">The linear sequence of Words which compose to form the single UndeterminedPhrase which will comprise the Clause.</param>
        public Clause(IEnumerable<Word> words) {
            Phrases = new List<Phrase>(new[] { new UndeterminedPhrase(words) });
        }
        /// <summary>
        /// Gets the collection of Phrases which comprise the Clause.
        /// </summary>
        public IEnumerable<Phrase> Phrases {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the concatenated text content of all of the Phrases which compose the Clause.
        /// </summary>
        public string Text {
            get {
                return Phrases.Aggregate(" ", (txt, phrase) => txt + phrase.Text) + " CLAUSE Tag ";
            }
        }
        internal void EstablishParent(Sentence sentence) {
            ParentDocument = sentence.ParentDocument;
            ParentSentence = sentence;
            foreach (var P in Phrases)
                P.EstablishParent(this);
        }

        /// <summary>
        /// Gets or set the Document instance to which the Clause belongs.
        /// </summary>
        public Document ParentDocument {
            get;
            protected set;
        }

        public Punctuator ClauseDelimiter {
            get;
            protected set;
        }

        public ClauseTypes Kind {
            get;
            set;
        }

        string ILexical.Text {
            get {
                throw new NotImplementedException();
            }
        }

        Dictionary<Weighting.WeightKind, Weighting.Weight> ILexical.Weights {
            get {
                throw new NotImplementedException();
            }
        }

        public Sentence ParentSentence {
            get;
            set;
        }
    }

    public enum ClauseTypes
    {
        S,
        SINV,
        SBAR,
        SBARQ,
        SQ
    }
}