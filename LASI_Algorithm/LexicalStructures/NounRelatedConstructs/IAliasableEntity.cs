﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm.LexicalStructures.NounRelatedConstructs
{
    public interface IAliasableEntity : LASI.Algorithm.SyntacticInterfaces.IEntity
    {
        bool IsAliasFor(IAliasableEntity other);
    }
}