﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm.StateMachines
{
    enum StateKind
    : byte
    {
        Initial,
        InitialOrAccept,
        Accept,
        Continue,
        ContinueOrAccept,
        Failed
    }
}
