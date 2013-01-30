﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.DataRepresentation
{
    public class SimpleDeclarativePhrase : Phrase
    {
        public SimpleDeclarativePhrase(IEnumerable<Word> composedWords)
            : base(composedWords) {
        }

        public override Word HeadWord {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
    }
}
