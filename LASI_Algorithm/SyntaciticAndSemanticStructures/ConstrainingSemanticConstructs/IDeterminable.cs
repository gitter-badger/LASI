﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm
{
    interface IDeterminable
    {
        Determiner DeterminedBy {
            get;
        }
        void BindDeterminer(Determiner determiner);
    }
}
 