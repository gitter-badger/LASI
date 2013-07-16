﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm
{
    /// <summary>
    /// Exposes the behaviors of a source of raw text and an associated name
    /// </summary>
    public interface IRawTextSource
    {
        /// <summary>
        /// Returns a string containing all of the text in the IRawTextSource.
        /// </summary>
        /// <returns>A string containing all of the text in the IRawTextSource.</returns>
        string GetText();
        /// <summary>
        /// Returns a Task which when awaited yields all of the text in the IRawTextSource.
        /// </summary>
        /// <returns>A Task which when awaited yields all of the text in the IRawTextSource.</returns>
        Task<string> GetTextAsync();
        /// <summary>
        /// Gets the name associated with the IRawTextSource.
        /// </summary>
        string Name { get; }
    }
}
