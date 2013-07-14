﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm
{
    /// <summary>
    /// Defines, very broadly, the basic kinds of Entities which are likely to be expressed.
    /// </summary>
    public enum EntityKind
    {
        UNDEFINED = 0,
        Person,
        Location,
        Organization,
        ProperUnknown,
        Thing,
        ThingUnknown,
        Activitiy,

    }
}
