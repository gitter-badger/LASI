﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.DataRepresentation
{
    public class SentenceDelimiter : Punctuator
    {
        public SentenceDelimiter(char eos)
            : base(eos) {
        }
    }
}
