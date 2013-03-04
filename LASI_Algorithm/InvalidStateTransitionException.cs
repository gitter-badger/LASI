﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm
{
    class InvalidStateTransitionException : Exception
    {
        public InvalidStateTransitionException(int stateNumber, ILexical errorOn)
            : base(String.Format("Invalid Transition\nAt State {0}\nOn {1}", stateNumber, errorOn)) {
        }
    }
}
