﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LASI.DataRepresentation
{
    /// <summary>
    /// Represents determiner words such as "the", "a", and "those".
    /// </summary>
    public class Determiner : Word
    {
        /// <summary>
        /// Initializes a new instance of the Determiner class.
        /// </summary>
        /// <param name="text">the literal text content of the word.</param>
        public Determiner(string text)
            : base(text) {
        }
        /// <summary>
        /// Gets or sets the Entity Determined by the Determiner.
        /// </summary>
        public virtual IEntity Determines {
            get;
            set;
        }
    }
}
