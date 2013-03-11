﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm
{
    /// <summary>
    /// Defines the broad role requiements for the weightable, countable textual elements, of a written work.
    /// Along with the other interfaces in the Syntactic Interfaces Library, the ILexical interface provides for generalization and abstraction over many otherwise disparate element types and type heirarchies.
    /// </summary>
    public interface ILexical
    {
        /// <summary>
        /// Gets the literal text of the ILexical.
        /// </summary>
        string Text {
            get;
        }
        /// <summary>
        /// Gets the dictionary of Weight objects, indexed by weight kind, which stores all of the computed weights of the ILexical.
        /// </summary>
        Dictionary<Weighting.WeightKind, Weighting.Weight> Weights {
            get;
        }
    }
}