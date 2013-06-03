﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Utilities;
using LASI.Utilities.TypedSwitch;
using LASI.Algorithm.DocumentConstructs;

namespace LASI.Algorithm.Binding
{
    class EntityReferenceBinder
    {



        public void Bind(Paragraph stream) {
            Bind(stream.Words);
        }
        public void Bind(IEnumerable<Sentence> stream) {
            Bind(from s in stream
                 from w in s.Words
                 select w);
        }


        public void Bind(IEnumerable<Word> stream) {
            foreach (var word in stream) {

                new Switch(word)
                    .Case<GenericSingularNoun>(e => {
                        genericSingular.Push(e);
                    })
                    .Case<GenericPluralNoun>(e => {
                        genericPlural.Push(e);
                    })
                    .Case<ProperSingularNoun>(e => {
                        properSingular.Push(e);
                    })
                    .Case<ProperPluralNoun>(e => {
                        properPlural.Push(e);
                    })
                    .Case<PresentParticipleGerund>(vbn => {
                        gerund.Push(vbn);
                    })
                    .Default<Word>(u => {
                        throw new UnknownEntityCompatibleWordTypeException(u);
                    });



            }
        }


        internal enum PronounGender
        {
            Male,
            Female,
            Thing,
            Group
        }



        Stack<GenericSingularNoun> genericSingular = new Stack<GenericSingularNoun>();
        Stack<GenericPluralNoun> genericPlural = new Stack<GenericPluralNoun>();
        Stack<ProperSingularNoun> properSingular = new Stack<ProperSingularNoun>();
        Stack<ProperPluralNoun> properPlural = new Stack<ProperPluralNoun>();
        Stack<PresentParticipleGerund> gerund = new Stack<PresentParticipleGerund>();
        protected readonly string[] malePronounText = new[] { "he", "him", "himself", "hisself", "his" };
        protected readonly string[] femalePronounText = new[] { "she", "her", "herself", "hers" };
    }
}
