﻿using System.Linq;
using LASI.WebApp.Models;
using LASI.Core;

namespace LASI.WebApp.LexicalElementStyling
{
    /// <summary>
    /// Associates syntactic elements with CSS styles.
    /// </summary>
    /// <seealso cref="Interop.Visualization.IStyleProvider{TLexical, TStylingArtifact}{TLexical, TStylingArtifact}"/>
    public class SyntacticStyleMap : Interop.Visualization.IStyleProvider<ILexical, Style>
    {
        /// <summary>
        /// Maps a Lexical element to a CSS Style based on its syntactic nature. 
        /// </summary>
        /// <param name="element">The Lexical for which to map to a CSS style based on its syntactic role.</param>
        /// <returns>A <see cref="LASI.WebApp.Models.Style"/> based on the syntactic role of the element.</returns>
        public Style this[ILexical element] => new Style
        {
            CssClass = element.Match()
                .Case((IReferencer r) => "referencer")
                .Case((NounPhrase n) => n.Words.OfProperNoun().Any() ? "entity proper" : "entity")
                .Case((InfinitivePhrase i) => "infinitive")
                .Case((PresentParticiple w) => "present-participle-gerund")
                .Case((IEntity e) => "entity")
                .Case((IVerbal v) => "verbal")
                .Case((IPrepositional p) => "prepositional")
                .Case((IDescriptor p) => "descriptor")
                .Case((IAdverbial a) => "adverbial")
                .Case((IConjunctive c) => "conjunctive")
            .Result(() => "lexical-default-style")
        };
    }
}