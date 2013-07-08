﻿using LASI.Algorithm.DocumentConstructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm.Binding
{
    public class AttributiveNounPhraseBinder
    {
        private IEnumerable<IEnumerable<Phrase>> contiguousGroups;
        public void Bind(Sentence sentence) {
            contiguousGroups = FindContiguousNounPhrases(sentence.Phrases);
            foreach (var cg in contiguousGroups) {
                ProcessContiguous(cg);
            }
        }
        public void Bind(IEnumerable<Phrase> phrases) {
            contiguousGroups = FindContiguousNounPhrases(phrases);
            foreach (var cg in contiguousGroups) {
                ProcessContiguous(cg);
            }
        }

        private void ProcessContiguous(IEnumerable<Phrase> cnps) {
            foreach (var prepPhrase in cnps.GetPrepositionalPhrases()) {
                ProcessLinkingPrepositionalPhrase(prepPhrase);
            }
            while (cnps.Count(n => n is NounPhrase) > 1) {
                var npLeft = cnps.First(n => n is NounPhrase) as NounPhrase;
                var npRight = cnps.Skip(1).First(n => n is NounPhrase) as NounPhrase;
                var leftNPDeterminer = npLeft != null ? npLeft.GetLeadingDeterminer() : null;
                var rightNpDeterminer = npRight != null ? npLeft.GetLeadingDeterminer() : null;
                if ((leftNPDeterminer != null && rightNpDeterminer != null) && leftNPDeterminer.DeterminerKind == DeterminerKind.Definite && rightNpDeterminer.DeterminerKind == DeterminerKind.Indefinite) {
                    npLeft.InnerAttributive = npRight;
                    npRight.OuterAttributive = npLeft;
                }
                cnps = cnps.SkipWhile(n => n.PreviousPhrase != npRight);
            }
        }

        private static void ProcessLinkingPrepositionalPhrase(PrepositionalPhrase prepPhrase) {
            if (prepPhrase.PreviousPhrase != null) {
                prepPhrase.PreviousPhrase.PrepositionOnRight = prepPhrase;
            }
            if (prepPhrase.NextPhrase != null) {
                prepPhrase.NextPhrase.PrepositionOnLeft = prepPhrase;
            }
            prepPhrase.ToTheRightOf = prepPhrase.NextPhrase;
            prepPhrase.ToTheLeftOf = prepPhrase.PreviousPhrase;

            prepPhrase.Role = PrepositionRole.DiscriptiveLinker;
        }

        private IEnumerable<IEnumerable<Phrase>> FindContiguousNounPhrases(IEnumerable<Phrase> phrases) {
            var result = new List<IEnumerable<Phrase>>();
            var temp = phrases;
            while (temp.Any()) {

                result.Add(temp.TakeWhile(n => n is NounPhrase || ((n is PrepositionalPhrase || n.Words.Count(w => w is Punctuation) == n.Words.Count()) && n.NextPhrase is NounPhrase && n.PreviousPhrase is NounPhrase)));
                temp = temp.SkipWhile(n => !(n is NounPhrase)).Skip(result.Last().Count()).ToList();
            }
            return from r in result
                   where r.Count() > 1
                   select r;
        }
    }



    internal static class NounPhraseExtensions
    {
        internal static Determiner GetLeadingDeterminer(this NounPhrase nounPhrase) {
            return nounPhrase.Words.FirstOrDefault(w => w is Determiner) as Determiner;
        }
    }

}