﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace LASI.Algorithm
{
    /// <summary>
    /// Represents a foriegn adverb embedded in an english written work.
    /// </summary>
    public class ForeignWord : Word
    {
        /// <summary>
        /// Initializes an instance of the ForeignWord class.
        /// </summary>
        /// <param name="text">The key text content of the ForeignWord.</param>
        public ForeignWord(string text)
            : base(text) {
        }
        /// <summary>
        /// Gets or sets the equivalent English adverb Noun if it can be inferred from the ForeignWord'subject usage.
        /// </summary>
        public virtual Type UsedAsType {
            get;
            set;
        }

    }

}