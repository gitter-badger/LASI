﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.DataRepresentation
{
    public class PresentPrtcplVPhrase : VerbPhrase, IActionSubject, IActionObject
    {
        public PresentPrtcplVPhrase(IEnumerable<Word> composedWords)
            : base(composedWords) {
        }

        public ITransitiveAction DirectObjectOf {
            get;
            set;
        }

        public ITransitiveAction IndirectObjectOf {
            get;
            set;
        }

        public IIntransitiveAction SubjectOf {
            get;
            set;
        }
    }
}
