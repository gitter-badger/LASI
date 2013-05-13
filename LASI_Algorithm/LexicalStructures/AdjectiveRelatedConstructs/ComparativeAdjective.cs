﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LASI.Algorithm
{
    /// <summary>
    /// entity specialization of the Adjective class, ComparativeAdjective represents adjectives such as "greener" and "better".
    /// </summary>
    public class ComparativeAdjective : Adjective
    {
        /// <summary>
        /// Initializes entity new instance of the ComparativeAdjective class
        /// </summary>
        /// <param name="text">The literal text content of the verb.</param>
        public ComparativeAdjective(string text)
            : base(text) {
        }
    }
}