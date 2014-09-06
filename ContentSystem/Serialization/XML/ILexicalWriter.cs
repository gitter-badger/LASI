﻿using LASI;
using LASI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LASI.ContentSystem.Serialization.Xml
{
    /// <summary>
    /// Defines the behavioral requirements for outputting an XML string from sequences of ILexical elements.
    /// </summary> 
    /// <typeparam name="T">The Type of the elements in the generic collections. This Type parameter is covariant.</typeparam> 
    public interface ILexicalXmlSerializer<in T>

        where T : class, ILexical
    {
        /// <summary>
        /// Writes a sequence of elements to the underlying xmlWriter, using the provided title string and Degree of output.
        /// </summary>
        /// <param name="elements">The sequence S&lt;T&gt; containing the values to serialize.</param>
        /// <param name="resultSetTitle">The title string which will represent a parent node of which the serialized elements will be child nodes.</param>
        /// <param name="degreeOfOutput">Controls the level of output detail to which elements will be serialized.</param>
        System.Xml.Linq.XElement Serialize(IEnumerable<T> elements, string resultSetTitle, DegreeOfOutput degreeOfOutput);

    }
    /// <summary>
    /// Controls the quantity and level detail which will be serialized into the XML representations of each lexical element.
    /// </summary>
    public enum DegreeOfOutput
    {
        /// <summary>
        /// Only the top weighted elements in the input set will be serialized.
        /// </summary>
        TopWeighted,
        /// <summary>
        /// All elements will in the input set will be serialized.
        /// </summary>
        Comprehensive,
    }
}
