﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LASI.Algorithm
{
    /// <summary>
    /// Defines the role requirements for constructs; generally Nouns, Nounphrases or Pronouns; which are semantically capable of "possessing" other Entities.
    /// Along with the other interfaces in the Syntactic Interfaces Library, the IPossesser interface provides for generalization and abstraction over word and Phrase types.
    /// </summary>
    public interface IPossesser
    {
        /// <summary>
        /// Gets all of the IEntity constructs which the IPossesser "owns".
        /// </summary>
        IEnumerable<IEntity> Possessed {
            get;
        }
        /// <summary>
        /// Adds an IPossessible construct, such as a person place or thing, to the collection of IEntity instances the IPossesser "Owns",
        /// and sets its owner to be the IPossesser.
        /// If the item is already possessed by the current instance, this method has no effect.
        /// </summary>
        /// <param name="possession">The possession to add.</param>
        void AddPossession(IEntity possession);
    }
}
