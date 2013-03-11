﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm
{
    /// <summary>
    /// Defines the role requirements for constructs; generally Nouns, Nounphrases or Pronouns; which are semantically capable of "possessing" other Entities.
    /// Along with the other interfaces in the Syntactic Interfaces Library, the IPossesser interface provides for generalization and abstraction over Word and Phrase types.
    /// </summary>
    public interface IPossesser
    {
        IEnumerable<IEntity> Possessed {
            get;
        }
        void AddPossession(IEntity possession);
    }
}