﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Core.PatternMatching;

namespace LASI.Core.Heuristics
{
    public static partial class Lookup
    {
        /// <summary>
        /// Determines if two IEntity instances are similar.
        /// </summary>
        /// <param name="first">The first IEntity</param>
        /// <param name="second">The second IEntity</param>
        /// <returns>True if the given IEntity instances are similar; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples
        /// <code>if ( Lookup.IsSimilarTo(e1, e2) ) { ... }</code>
        /// <code>if ( e1.IsSimilarTo(e2) ) { ... }</code>
        /// Please prefer the second convention.
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this IEntity first, IEntity second) {
            return
                first.Match().Yield<SimilarityResult>()
                    .When(first.Text.ToUpper() == second.Text.ToUpper())
                        .Then(SimilarityResult.Similar)
                    .With<IAggregateEntity>(ae1 =>
                        second.Match().Yield<SimilarityResult>()
                          .With<IAggregateEntity>(ae2 => new SimilarityResult(ae1.IsSimilarTo(ae2)))
                          .With<IEntity>(e2 => new SimilarityResult(ae1.Any(entity => entity.IsSimilarTo(e2))))
                        .Result())
                    .With<Noun>(n1 =>
                        second.Match().Yield<SimilarityResult>()
                            .With<Noun>(n2 => new SimilarityResult(n1.IsSynonymFor(n2)))
                            .With<NounPhrase>(np2 => n1.IsSimilarTo(np2))
                          .Result())
                    .With<NounPhrase>(np1 =>
                        second.Match().Yield<SimilarityResult>()
                          .With<NounPhrase>(np2 => np1.IsSimilarTo(np2))
                          .With<Noun>(n2 => np1.IsSimilarTo(n2))
                        .Result())
                    .Result();
        }
        /// <summary>
        /// Determines if two IAggregateEntity instances are similar.
        /// </summary>
        /// <param name="first">The first IAggregateEntity</param>
        /// <param name="second">The second IAggregateEntity</param>
        /// <returns>True if the given IAggregateEntity instances are similar; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples
        /// <code>if ( Lookup.IsSimilarTo(e1, e2) ) { ... }</code>
        /// <code>if ( e1.IsSimilarTo(e2) ) { ... }</code>
        /// Please prefer the second convention.
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this IAggregateEntity first, IAggregateEntity second) {
            var simResults = from e1 in first
                             from e2 in second
                             select e1.IsSimilarTo(e2) into result
                             group result by (bool)result into byResult
                             let Count = byResult.Count()
                             orderby Count descending
                             select new { byResult.Key, Count };
            return new SimilarityResult(simResults.Any() && simResults.First().Key,
                simResults.Any() ? simResults
                .Aggregate(0f, (ratioSoFar, current) => ratioSoFar / current.Count) : 0);
        }
        /// <summary>
        /// Determines if the provided Noun is similar to the provided NounPhrase.
        /// </summary>
        /// <param name="first">The Noun.</param>
        /// <param name="second">The NounPhrase.</param>
        /// <returns>True if the provided Noun is similar to the provided NounPhrase; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples:
        /// <example>
        /// <code>if ( Lookup.IsSimilarTo(n1, np2) ) { ... }</code>
        /// <code>if ( n1.IsSimilarTo(np2) ) { ... }</code>
        /// </example>
        /// Please prefer the second convention.
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this Noun first, NounPhrase second) {
            var phraseNouns = second.Words.OfNoun().ToList();
            return new SimilarityResult(phraseNouns.Count == 1 && phraseNouns.First().IsSynonymFor(first));
        }
        /// <summary>
        /// Determines if the provided NounPhrase is similar to the provided Noun.
        /// </summary>
        /// <param name="first">The NounPhrase.</param>
        /// <param name="second">The Noun.</param>
        /// <returns>True if the provided NounPhrase is similar to the provided Noun; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples:
        /// <example>
        /// <code>if ( Lookup.IsSimilarTo(np1, n2) ) { ... }</code>
        /// <code>if ( np1.IsSimilarTo(n2) ) { ... }</code>
        /// </example>
        /// Please prefer the second convention.
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this NounPhrase first, Noun second) {
            return second.IsSimilarTo(first);
        }
        /// <summary>
        /// Determines if two NounPhrases are similar.
        /// </summary>
        /// <param name="first">The first NounPhrase</param>
        /// <param name="second">The second NounPhrase</param>
        /// <returns>True if the given NounPhrases are similar; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples:
        /// <example>
        /// <code>if ( Lookup.IsSimilarTo(np1, np2) ) { ... }</code>
        /// <code>if ( np1.IsSimilarTo(np2) ) { ... }</code>
        /// Please prefer the second convention.
        /// </example>
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this NounPhrase first, NounPhrase second) {
            var ratio = GetSimilarityRatio(first, second);
            return new SimilarityResult(ratio > SIMILARITY_THRESHOLD, ratio);
        }
        /// <summary>
        /// Determines if the two provided Noun instances are similar.
        /// </summary>
        /// <param name="first">The first Noun.</param>
        /// <param name="second">The second Noun.</param>
        /// <returns>True if the first Noun is similar to the second; otherwise, false.</returns>
        /// <remarks>There are two calling conventions for this method. See the following examples:
        /// <example>
        /// <code>if ( Lookup.IsSimilarTo(n1, n2) ) { ... }</code>
        /// <code>if ( n1.IsSimilarTo(n2) ) { ... }</code>
        /// </example>
        /// Please prefer the second convention.
        /// </remarks>
        public static SimilarityResult IsSimilarTo(this Noun first, Noun second) {
            return new SimilarityResult(first.IsSynonymFor(second));
        }
        /// <summary>
        /// Returns a double value indicating the degree of similarity between two NounPhrases.
        /// </summary>
        /// <param name="first">The first NounPhrase</param>
        /// <param name="second">The second NounPhrase</param>
        /// <returns>A double value indicating the degree of similarity between two NounPhrases.</returns>
        private static double GetSimilarityRatio(NounPhrase first, NounPhrase second) {
            double similarCount = 0;
            Func<NounPhrase, NounPhrase, double> overlap = (outer, inner) => {
                if (outer.Words.OfNoun().Any() && inner.Words.OfNoun().Any()) {
                    foreach (var outerNoun in outer.Words.OfNoun()) {
                        foreach (var innerNoun in inner.Words.OfNoun()) {
                            if (innerNoun.IsSynonymFor(outerNoun)) { similarCount += 0.7; }
                        }
                        var scaleFactor = inner.Words.OfNoun().Count() * outer.Words.OfNoun().Count();
                        return (similarCount / scaleFactor == 0 ? 1 : scaleFactor);

                    }
                }
                return 0;
            };
            return first.Words.Count() >= second.Words.Count() ? overlap(first, second) : overlap(second, first);
        }

    }
}
