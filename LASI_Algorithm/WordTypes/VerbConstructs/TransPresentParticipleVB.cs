﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm
{
    public class TransPresentPrtcplVB : TransitiveVerb, IActionObject, IActionSubject
    {
        public TransPresentPrtcplVB(string text)
            : base(text) {
        }

        public IAction SubjectOf {
            get;
            set;
        }

        public ITransitiveAction DirectObjectOf {
            get;
            set;
        }

        public ITransitiveAction IndirectObjectOf {
            get;
            set;
        }
    }
}
