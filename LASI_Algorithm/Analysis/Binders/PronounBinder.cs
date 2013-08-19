﻿using LASI;
using LASI.Algorithm;
using LASI.Algorithm.DocumentConstructs;
using LASI.Algorithm.Lookup;
using LASI.Utilities;
using LASI.Utilities.PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LASI.Algorithm.Binding
{
    /// <summary>
    /// Attempts to bind pronouns to the entities they refer to.
    /// </summary>
    public static class PronounBinder
    {
        /// <summary>
        /// Attempts to perform pronoun binding within the given Sentence.
        /// </summary>
        /// <param name="sentence">The Sentence over which to perform pronoun binding.</param>
        public static void Bind(Sentence sentence) {
            BindPosessivePronouns(sentence.Phrases);
        }


        /// <summary>
        /// Binds posessive pronouns located in the sequence of phrases sentence. 
        /// Example Sentence that this applies to:
        /// "LASI binds it's pronouns."
        /// Pronoun "it's" binds to the proper noun "LASI"
        /// </summary>
        /// <param name="phrases">The sequence of phrases to bind within.</param>
        private static void BindPosessivePronouns(IEnumerable<Phrase> phrases) {
            foreach (var vp in phrases.GetVerbPhrases().WithSubject().WithObject(o => o is PossessivePronoun)) {
                var pronouns = vp.DirectObjects.Concat(vp.IndirectObjects).OfType<PossessivePronoun>();
                foreach (var pro in pronouns) {
                    foreach (var subject in vp.Subjects) {
                        pro.PossessesFor = subject;
                    }
                }
            }
        }
    }

}
