﻿using LASI.Algorithm.DocumentConstructs;
using LASI.Algorithm.Lookup;
using LASI.Utilities;
using LASI.Utilities.PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm.Weighting
{
    /// <summary>
    /// Provides static access to a comprehensive set of weighting operations which are applicable to a document.
    /// </summary>
    static public class Weighter
    {
        /// <summary>
        /// Gets an ordered collection of ProcessingTask objects which correspond to the steps required to Weight the given document.
        /// Each ProcessingTask contains a Task property which, when awaited will perform a step of the Weighting process.
        /// </summary>
        /// <param name="document">The document for which to get the ProcessingTasks for Weighting.</param>
        /// <returns>An ordered collection of ProcessingTask objects which correspond to the steps required to Weight the given document.
        /// </returns>
        /// <remarks>
        /// ProcessingTasks returned by this method may be run in an arbitrary order.
        /// However, to ensure the consistency/determinism of the Weighting process, it is recommended that they be executed (awaited) in the order
        /// in which they are hereby returned.
        /// </remarks>
        public static IEnumerable<ProcessingTask> GetWeightingProcessingTasks(Document document) {
            return new[] {  
                new ProcessingTask(document,
                    WeightWordsByLiteralFrequencyAsync (document), 
                    string.Format("{0}: Aggregating Literals", document.Name),
                    string.Format("{0}: Aggregated Literals", document.Name),
                    11),
                new ProcessingTask(document,
                    WeightPhrasesByLiteralFrequencyAsync (document),
                    string.Format("{0}: Aggregating Complex Literals", document.Name),
                    string.Format("{0}: Aggregated Complex Literals", document.Name),
                    19),
                new ProcessingTask(document,
                    ModifyNounWeightsBySynonymsAsync(document),
                    string.Format("{0}: Generalizing Nouns",document.Name), 
                    string.Format("{0}: Generalized Nouns",document.Name),
                    19),
                new ProcessingTask(document,
                    ModifyVerbWeightsBySynonymsAsync (document), 
                    string.Format("{0}: Generalizing Verbs",document.Name), 
                    string.Format("{0}: Generalized Verbs",document.Name),
                    10),
                new ProcessingTask(document,
                    WeightSimilarNounPhrasesAsync(document),
                    string.Format("{0}: Generalizing Phrases",document.Name),
                    string.Format("{0}: Generalized Phrases",document.Name),
                    20),
                new ProcessingTask(document,
                    WeightSimilarEntitiesAsync(document),
                    string.Format("{0}: Generalizing Entities",document.Name),
                    string.Format("{0}: Generalized Entities",document.Name),
                    10),
                new ProcessingTask(document,
                    HackSubjectPropernounImportanceAsync (document), 
                    string.Format("{0}: Focusing Patterns", document.Name),
                    string.Format("{0}: Focused Patterns", document.Name),
                    6),
                new ProcessingTask(document,
                    NormalizeWeightsAsync (document),
                    string.Format("{0}: Normalizing Metrics", document.Name),
                    string.Format("{0}: Normalized Metrics", document.Name),
                    3)
            };
        }

        /// <summary>
        /// Asynchronously assigns numeric Weights to each elemenet in the given Document.
        /// </summary>
        /// <param name="doc">The Document whose elements are to be assigned numeric weights.</param>
        public static async Task WeightAsync(Document doc) {
            await Task.WhenAll(GetWeightingProcessingTasks(doc).Select(procTask => procTask.Task));
        }
        /// <summary>
        /// Assigns numeric Weights to each elemenet in the given Document.
        /// </summary>
        /// <param name="doc">The Document whose elements are to be assigned numeric weights.</param>
        public static void Weight(Document doc) {

            WeightWordsBySyntacticSequence(doc);
            WeightByLiteralFrequency(doc.Words);
            WeightByLiteralFrequency(doc.Phrases);
            ModifyNounWeightsBySynonyms(doc);
            ModifyVerbWeightsBySynonyms(doc);
            WeightSimilarNounPhrases(doc);
            HackSubjectPropernounImportance(doc);
            NormalizeWeights(doc);
        }
        private static async Task NormalizeWeightsAsync(Document doc) {
            await Task.Run(() => NormalizeWeights(doc));
        }
        private static void NormalizeWeights(Document doc) {
            NewNormalizationProcedure(doc.Words);
            NewNormalizationProcedure(doc.Phrases);
        }

        private static void NewNormalizationProcedure(IEnumerable<ILexical> source) {
            if (source.Any()) {
                double maxWeight = source.Max(e => e.Weight);
                double minWeight = source.Where(e => e.Weight > 0).Min(e => e.Weight);
                double scalingFactor = (maxWeight - minWeight > 0 ? (maxWeight - minWeight) : 1.0) * (100d / maxWeight);
                source.AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                    .Where(e => e.Weight > 0)
                    .ForAll(e => e.Weight *= scalingFactor);
            }
            //    //attempting to set all non-zero weights to 0 when there isn't anything in the source.
            //else {
            //    foreach (var w in source.Where(e => e.Weight != 0)) {
            //        w.Weight = 0;
            //    }
            //}
        }




        private static async Task ModifyVerbWeightsBySynonymsAsync(Document doc) {
            await Task.Run(() => ModifyVerbWeightsBySynonyms(doc));
        }

        private static void ModifyVerbWeightsBySynonyms(Document doc) {
            var verbsToConsider = doc
                .Words.AsParallel()
                .WithDegreeOfParallelism(Concurrency.Max)
                .GetVerbs()
                .WithSubjectOrObject();
            var groups = from outerVerb in
                             verbsToConsider
                         from innerVerb in verbsToConsider
                         where outerVerb.IsSynonymFor(innerVerb)
                         group innerVerb by outerVerb;
            groups.ForAll(grp => grp.Key.Weight += 0.7 * grp.Count());
        }

        private static async Task ModifyNounWeightsBySynonymsAsync(Document doc) {
            await Task.Run(() => ModifyNounWeightsBySynonyms(doc));
        }

        /// <summary>
        /// Increase noun weights in a document by abstracting over synonyms
        /// </summary>
        /// <param name="doc">the Document whose noun weights may be modiffied</param>
        private static void ModifyNounWeightsBySynonyms(Document doc) {
            //Currently, include only those nouns which exist in relationships with some IVerbal or IPronoun.
            var toConsider = doc.Words
                .AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                .GetNouns().InSubjectOrObjectRole()
                .Concat<IEntity>(doc.Words
                .AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                .GetPronouns().InSubjectOrObjectRole().Select(e => e.ReferersTo ?? e as IEntity));
            (from outer in toConsider
             from inner in toConsider
             where outer.IsSimilarTo(inner)
             group inner by outer).AsParallel().WithDegreeOfParallelism(Concurrency.Max)
             .ForAll(grp => grp.Key.Weight += 0.7 * grp.Count());
        }

        private static async Task WeightPhrasesByLiteralFrequencyAsync(Document doc) {
            await Task.Run(() => WeightByLiteralFrequency(doc.Phrases));
        }

        private static void WeightByLiteralFrequency(IEnumerable<ILexical> syntacticElements) {
            var data = from phrase in syntacticElements
                       group phrase by new { phrase.Type, phrase.Text } into grp
                       select new { grp, count = grp.Count() };
            foreach (var grp in data) {
                foreach (var e in grp.grp) {
                    e.Weight += grp.count;
                }
            }

        }
        /// <summary>
        /// basic word count by part of speech ignoring determiners and conjunctions
        /// </summary>
        /// <param name="doc">the Document whose words to weight</param> 
        private static async Task WeightWordsByLiteralFrequencyAsync(Document doc) {
            await Task.Run(() => WeightByLiteralFrequency(doc.Words));
        }




        private static async Task WeightSimilarNounPhrasesAsync(Document doc) {
            await Task.Run(() => WeightSimilarNounPhrases(doc));
        }

        /// <summary>
        /// For each noun parent in a document that is similar to another noun parent, increase the weight of that noun
        /// </summary>
        /// <param name="doc">Document containing the componentPhrases to weight</param>
        private static void WeightSimilarNounPhrases(Document doc) {

            var nps = doc.Phrases
                .AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                .GetNounPhrases()
                .InSubjectOrObjectRole();

            foreach (var outerNP in nps) {
                var groups = from innerNP in nps.AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                             where innerNP.IsAliasFor(outerNP) || innerNP.IsSimilarTo(outerNP)
                             group innerNP by outerNP;
                foreach (var group in groups) {
                    var weightIncrease = group.Count() * .5;
                    foreach (var inner in group) {
                        inner.Weight += weightIncrease;
                    }
                }
            }

        }
        private static async Task WeightSimilarEntitiesAsync(Document doc) {
            await Task.Run(() => WeightSimilarEntities(doc));
        }
        private static void WeightSimilarEntities(Document doc) {
            var entities = doc.GetEntities().AsParallel().WithDegreeOfParallelism(Concurrency.Max).InSubjectOrObjectRole();
            //var entityLookup = entities.ToLookup(key => key,
            //                    LexicalComparers<IEntity>
            //                    .CreateCustom((L, R) => L.Text == R.Text || L.IsAliasFor(R) || L.IsSimilarTo(R)));
            foreach (var outer in entities) {
                var groups = from inner in entities.AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                             where inner.IsAliasFor(outer) || inner.IsSimilarTo(outer)
                             group inner by outer;
                foreach (var group in groups) {
                    var weightIncrease = group.Count() * .5;
                    foreach (var inner in group) {
                        inner.Weight += weightIncrease;
                    }
                }
            }

        }

        private static async Task HackSubjectPropernounImportanceAsync(Document doc) {
            await Task.Run(() => HackSubjectPropernounImportance(doc));
        }

        private static void HackSubjectPropernounImportance(Document doc) {
            doc.Phrases
                .AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                .Where(np => np.Words.Any(w => w is ProperNoun))
                .GetNounPhrases()
                .ForAll(np => np.Weight *= 2);

        }




        private static void OldNormalizationProcedure(Document doc) {
            double TotPhraseWeight = 0.0;
            double MaxWeight = 0.0;
            int NonZeroWghts = 0;
            foreach (var w in doc.Phrases) {
                TotPhraseWeight += w.Weight;

                if (w.Weight > 0)
                    NonZeroWghts++;

                if (w.Weight > MaxWeight)
                    MaxWeight = w.Weight;
            }
            if (NonZeroWghts != 0) {//Caused a devide by zero exception if document was empty.
                var AvgWght = TotPhraseWeight / NonZeroWghts;
                var ratio = 100 / MaxWeight;

                foreach (var p in doc.Phrases) {
                    p.Weight = Math.Round(p.Weight * ratio, 3);
                }
            }
        }
        private static async Task WeightWordsBySyntacticSequenceAsync(Document doc) {
            await Task.Run(() => WeightWordsBySyntacticSequence(doc));
        }








        /// <summary>
        /// SIX PHASES 
        ///PHASE 2 - word Weight based on part of speech and neighbors' (+2) part of speech
        ///PHASE 3 - Normal parent Weight based on parent part of speech (standardization) - COMPLETE
        ///PHASE 4 - Phrase Weight based on part of speech and neibhors' (full sentence) part of speech
        ///PHASE 5 - FREQUENCIES
        /// .1 - Frequency of word/Phrase in document
        /// .2 - Frequency of word/Phrase in document compared to second documents in set -EXCLUDED FOR 1-DOCUMENT DEMO
        ///PHASE 6 - SYNONYMS
        ///ALLUAN READ:            // .1 - Frequency of word (/Phrase?) in document - COMPLETE MINUS VERBS (couldn't access the adverb thesaurus in any way)
        /// .2 - Frequency of word (/Phrase?) in document compared to second documents in set -EXCLUDED FOR 1-DOCUMENT DEMO
        /// </summary>
        /// <param name="doc">The document whose contents are to be weighted,</param>
        private static void WeightWordsBySyntacticSequence(Document doc) {

            int primary, secondary, tertiary, quaternary, quinary, senary;
            int based = 20;
            primary = (secondary = (tertiary = (quaternary = (quinary = (senary = 0) + based) + based) + based) + based) + based;
            //PHASE 1 - Normal word Weight based on part of speech (standardization)
            //COMPLETE - easy peasy.

            Output.WriteLine("Normal word Weight based on POS:");
            //doc.Sentences.AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax).ForAll(s => {
            //////Output.WriteLine(subject);
            foreach (var s in doc.Sentences) {
                foreach (Word w in s.Words) {
                    //Output.WriteLine(w);

                    w.Match()
                        .With<Noun>(n => {
                            w.Weight = primary;
                        })
                        .With<Verb>(() => {
                            w.Weight = secondary;
                        })
                        .With<Adjective>(() => {
                            w.Weight = tertiary;
                        })
                        .With<Adverb>(() => {
                            w.Weight = quaternary;
                        })
                        .With<Pronoun>(() => {
                            w.Weight = quinary;
                        })
                        .Default(() => {
                            w.Weight = senary;
                        });
                }


            };





            //PHASE 2 - word Weight based on part of speech and neighbors' (+2) part of speech
            // WORKS, BUT
            // NEED FORMULAS FOR MODIFIER VARIABLES - WHAT SHOULD THESE BE?
            double modOne, modTwo;
            modOne = modTwo = 0;
            foreach (var s in doc.Sentences) {
                //////Output.WriteLine(subject);

                foreach (Word w in s.Words) {

                    Word next = w ?? w.NextWord;
                    Word nextNext = next ?? next.NextWord;

                    //cut?
                    Word prev = w ?? w.PreviousWord;
                    Word prevPrev = prev ?? prev.PreviousWord;


                    w.Match()
                       .With<Noun>(n => {
                           Noun(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Verb>(() => {
                           Verb(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Adjective>(() => {
                           Adjective(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Adverb>(() => {
                           Adverb(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Pronoun>(() => {
                           Pronoun(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Preposition>(() => {
                           Preposition(next, nextNext, out modOne, out modTwo);
                       })
                       .With<Determiner>(() => {
                           Determiner(next, nextNext, out modOne, out modTwo);
                       })
                       .Default(() => {
                           modOne = 0.1d;

                           //second (UNCAUGHT -> UNCAUGHT)
                           modTwo = UncaughtUncaught(nextNext);
                       });


                    w.Weight += (w.Weight * (modOne * modTwo)) / 3;

                }
            }
        }

        #region  Syntactic Sequence Weighting methods


        private static void Determiner(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                    .With<Noun>(() => {
                        modOne = 0.9d; //determiner-noun

                        //second (Determiner -> Noun)
                        modTwo = PronounNoun(nextNext);
                    })
                    .With<Adjective>(() => {
                        modOne = 0.8d;  //deteminer-adjective

                        //second (Determiner -> Adjective)
                        modTwo = PronounAdjective(nextNext);
                    })
                    .With<Adverb>(() => {
                        modOne = 0.7d;  //determiner-adverb

                        //second (Determiner -> Adverb)
                        modTwo = PronounAdverb(nextNext);
                    })
                    .With<Pronoun>(() => {
                        modOne = 0.9d; //determiner-pronoun

                        //second (Determiner -> Noun)
                        modTwo = PronounPronoun(nextNext);
                    })
                    .With<ToLinker>(() => {
                        modOne = 0.7d; //determiner-tolinker

                        //second (Determiner -> ToLinker)
                        modTwo = PronounToLinker(nextNext);
                    })
                    .With<Preposition>(() => {
                        modOne = 0.3d; //determiner positional

                        //second Determiner -> Preposition)
                        modTwo = PronounPreposition(nextNext);
                    })
                    .With<Determiner>(() => {
                        modOne = 0d; //determiner-determiner

                        //second (Determiner -> Determiner)
                        modTwo = PronounDeterminer(nextNext);
                    })
                    .Default(() => {
                        modOne = 0.1d;

                        //second (Determiner -> UNCAUGHT)
                        modTwo = PronounUncaught(nextNext);
                    });

            outModOne = modOne;
            outModTwo = modTwo;
        }
        private static void Preposition(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                    .With<Noun>(() => {
                        modOne = 0.8; // 

                        //second (Preposition -> Noun)
                        modTwo = PrepositionNoun(nextNext);
                    })
                    .With<Pronoun>(() => {
                        modOne = 0.8; // 

                        //second (Preposition -> Noun)
                        modTwo = PrepositionPronoun(nextNext);
                    })
                    .With<Determiner>(() => {
                        modOne = 0.7; //determiner

                        //second (Preposition -> Determiner)
                        modTwo = PrepositionDeterminer(nextNext);
                    })
                    .Default(() => {
                        modOne = 0.1;

                        //second (Preposition -> UNCAUGHT)
                        modTwo = PrepositionUncaught(nextNext);
                    });

            outModOne = modOne;
            outModTwo = modTwo;
        }

        private static void Pronoun(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                    .With<Noun>(() => {
                        modOne = 0.9; //compound noun/pronoun / possessed by pronoun

                        //second (Pronoun -> Noun)
                        modTwo = PronounNoun(nextNext);
                    })
                    .With<Adjective>(() => {
                        modOne = 0.8;  //possessed/descriptor 

                        //second (Pronoun -> Adjective)
                        modTwo = PronounAdjective(nextNext);
                    })
                    .With<Adverb>(() => {
                        modOne = 0.7;  //pronoun amplifier

                        //second (Pronoun -> Adverb)
                        modTwo = PronounAdverb(nextNext);
                    })
                    .With<Pronoun>(() => {
                        modOne = 0.9d; //compound pronoun 
                        //second (Pronoun -> Noun)
                        modTwo = PronounPronoun(nextNext);
                    })
                    .With<ToLinker>(() => {
                        modOne = 0.6d; //pronoun directional

                        //second (Pronoun -> ToLinker)
                        modTwo = PronounToLinker(nextNext);
                    })
                    .With<Preposition>(() => {
                        modOne = 0.5d; //pronoun positional

                        //second (Pronoun -> Preposition)
                        modTwo = PronounPreposition(nextNext);
                    })
                    .With<Determiner>(() => {
                        modOne = 0.7d; //determiner

                        //second (Pronoun -> Determiner)
                        modTwo = PronounDeterminer(nextNext);
                    })
                    .Default(() => {
                        modOne = 0.1d;

                        //second (Pronoun -> UNCAUGHT)
                        modTwo = PronounUncaught(nextNext);
                    });

            outModOne = modOne;
            outModTwo = modTwo;
        }

        private static void Adverb(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                   .With<Noun>(() => {
                       modOne = 0.9d; //adverbial noun

                       //second (Adverb -> Noun)
                       modTwo = AdverbNoun(nextNext);
                   })
                   .With<Adjective>(() => {
                       modOne = 0.8d;  //normal adv-adj

                       //second (Adverb -> Adjective)
                       modTwo = AdverbAdjective(nextNext);
                   })
                   .With<Adverb>(() => {
                       modOne = 0.7d;  //bi-adverbial

                       //second (Adverb -> Adverb)
                       modTwo = AdverbAdverb(nextNext);
                   })
                   .With<Pronoun>(() => {
                       modOne = 0.9d; //adverbial pronoun

                       //second (Adverb -> Noun)
                       modTwo = AdverbPronoun(nextNext);
                   })
                   .With<ToLinker>(() => {
                       modOne = 0.7d; //adverb directional

                       //second (Adverb -> ToLinker)
                       modTwo = AdverbToLinker(nextNext);
                   })
                   .With<Preposition>(() => {
                       modOne = 0.5d; //adverb positional

                       //second (Adverb -> Preposition)
                       modTwo = AdverbPreposition(nextNext);
                   })
                   .With<Determiner>(() => {
                       modOne = 0.7d; //determiner

                       //second (Adverb -> Determiner)
                       modTwo = AdverbDeterminer(nextNext);
                   })
                   .Default(() => {
                       modOne = 0.1d;

                       //second (Adverb -> UNCAUGHT)
                       modTwo = AdverbUncaught(nextNext);
                   });

            outModOne = modOne;
            outModTwo = modTwo;
        }

        private static void Adjective(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                   .With<Noun>(() => {
                       modOne = 0.7d; //noun descriptor

                       //second (Adjective -> Noun)
                       modTwo = AdjectiveNoun(nextNext);
                   })
                   .With<Adjective>(() => {
                       modOne = 0.5d;  //double descriptor

                       //second (Adjective -> Adjective)
                       modTwo = AdjectiveAdjective(nextNext);
                   })
                   .With<Adverb>(() => {
                       modOne = 0.5d;  //coloured brilliantly

                       //second (Adjective -> Adverb)
                       modTwo = AdjectiveAdverb(nextNext);
                   })
                   .With<Pronoun>(() => {
                       modOne = 0.7d; //noun descriptor

                       //second (Adjective -> Noun)
                       modTwo = AdjectivePronoun(nextNext);
                   })
                   .With<ToLinker>(() => {
                       modOne = 0.4d; //adjective directional

                       //second (Adjective -> ToLinker)
                       modTwo = AdjectiveToLinker(nextNext);
                   })
                   .With<Preposition>(() => {
                       modOne = 0.4d; //adjective positional

                       //second (Adjective -> Prepositional)
                       modTwo = AdjectivePreposition(nextNext);
                   })
                   .With<Determiner>(() => {
                       modOne = 0.4d; //determiner

                       //second (Adjective -> Determiner)
                       modTwo = AdjectiveDeterminer(nextNext);
                   })
                   .Default(() => {
                       modOne = 0.1d;

                       //second (Adjective -> UNCAUGHT)
                       modTwo = AdjectiveUncaught(nextNext);
                   });

            outModOne = modOne;
            outModTwo = modTwo;
        }

        private static void Verb(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0;
            double modTwo = 0;
            next.Match()
                   .With<Noun>(() => {
                       modOne = 0.9d; //adverb actor

                       //second (Verb -> Noun)
                       modTwo = VerbNoun(nextNext);
                   })
                   .With<PastParticipleVerb>(() => {
                       modOne = 0.7d; //adverb-adverb descriptor

                       //second (Verb -> PastParticipleVerb)
                       modTwo = VerbPastParticipleVerb(nextNext);
                   })
                   .With<Adjective>(() => {
                       modOne = 0.6d;  //adverb state

                       //second (Verb -> Adjective)
                       modTwo = VerbAdjective(nextNext);
                   })
                   .With<Adverb>(() => {
                       modOne = 0.7d;  //perfect adverb

                       //second (Verb -> Adverb)
                       modTwo = VerbAdverb(nextNext);
                   })
                   .With<Pronoun>(() => {
                       modOne = 0.9d; //adverb actor

                       //second (Verb -> Pronoun)
                       modTwo = VerbPronoun(nextNext);
                   })
                   .With<ToLinker>(() => {
                       modOne = 0.6d; //adverb directional

                       //second (Verb -> ToLinker)
                       modTwo = VerbToLinker(nextNext);
                   })
                   .With<Preposition>(() => {
                       modOne = 0.5d; //adverb-adverb positional

                       //second (Verb -> Preposition)
                       modTwo = VerbPreposition(nextNext);
                   })
                   .With<Determiner>(() => {
                       modOne = 0.4d; //determiner

                       //second (Verb -> Determiner)
                       modTwo = VerbDeterminer(nextNext);
                   })
                   .Default(() => {
                       modOne = 0.1d;

                       //second (Verb -> UNCAUGHT)
                       modTwo = VerbUncaught(nextNext);
                   });

            outModOne = modOne;
            outModTwo = modTwo;
        }

        private static void Noun(Word next, Word nextNext, out double outModOne, out double outModTwo) {
            double modOne = 0; //Renamed parameters and bound created temporary variables to pass into the switch blocks 
            double modTwo = 0;
            next.Match()
                   .With<Noun>(() => {
                       modOne = 0.9d; //compound noun

                       //second (Noun -> Noun)
                       modTwo = NounNoun(nextNext);
                   })
                   .With<Adjective>(nadj => {
                       modOne = 0.8d; //possessive

                       //second (Noun -> Adjective)
                       modTwo = NounAdjective(nextNext);
                   })
                   .With<Verb>(nv => {
                       modOne = 0.7d; //noun action or descriptor

                       //second (Noun -> Verb)
                       modTwo = NounVerb(nextNext);
                   })
                   .With<Adverb>(() => {
                       modOne = 0.7d;  //noun amplifier

                       //second (Noun -> Adverb)
                       modTwo = NounAdverb(nextNext);
                   })
                   .With<Pronoun>(() => {
                       modOne = 0.9d; //compound noun

                       //second (Noun -> Pronoun)
                       modTwo = NounPronoun(nextNext);


                   })
                   .With<ToLinker>(() => {
                       modOne = 0.6d; //noun to link

                       //second (Noun -> ToLinker)
                       modTwo = NounToLinker(nextNext);
                   })
                   .With<Preposition>(() => {
                       modOne = 0.6d; //noun positional

                       //second (Noun -> Preposition)
                       modTwo = NounPreposition(nextNext);
                   })
                   .With<Determiner>(() => {
                       modOne = 0.5d; //determiner

                       //second (Noun -> Determiner)
                       modTwo = NounDeterminer(nextNext);
                   })
                   .Default(() => {
                       modOne = 0.1d;

                       //second (Noun -> UNCAUGHT)
                       modTwo = NounUncaught(nextNext);
                   });
            outModOne = modOne;//Set parameters = temporaries
            outModTwo = modTwo;//There is a better way to handle this, but this works without any changes
        }

        private static double UncaughtUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.1d; //uncaught-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.1d; //uncaught-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0.1d;  //uncaught-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.1d;  //uncaught-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.1; //uncaught-uncaught-noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0.1d; //uncaught-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0.1d; //uncaught-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.1d; //uncaught-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0.1d; //uncaught-uncaught-uncaught (epic fail)
                });
            return modTwo;
        }

        private static double PrepositionUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //preposition-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.4d; //preposition-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0.3d;  //preposition-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.3d;  //preposition-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.5d; //preposition-uncaught-noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0.2d; //preposition-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0.2d; //preposition-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.2d; //preposition-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PrepositionDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.7d; //preposition-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.6d; //preposition-determiner-verb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.5d;  //preposition-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.4d;  //preposition-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.6d; //preposition-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.3d; //preposition-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.3d; //preposition-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.3d; //preposition-determiner determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PrepositionPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //preposition-compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //preposition-noun-verb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.3d;  //preposition-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.3d;  //preposition-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.8d; //preposition-compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.6d; //preposition-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.2d; //preposition-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.9d; //preposition-noun determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PrepositionNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //preposition-compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //preposition-noun-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.3d;  //preposition-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.3d;  //preposition-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.3d; //preposition-compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.8d; //preposition-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.6d; //preposition-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.2d; //preposition-noun determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.3d; //pronoun-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.2d; //pronoun-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0.2d;  //pronoun-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.2d;  //pronoun-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.3d; //pronoun-uncaught-noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0.2d; //pronoun-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0.2d; //pronoun-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.2d; //pronoun-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //pronoun-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.7d; //pronoun-determiner-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.6d;  //pronoun-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.5d;  //pronoun-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //pronoun-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.1d; //pronoun-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.3d; //pronoun-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //pronoun-determiner determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounPreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.8d; //pronoun-preposition-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.6d; //pronoun-preposition-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.5d;  //pronoun-preposition-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.6d;  //pronoun-preposition-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.8d; //pronoun-preposition-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.4d; //pronoun-preposition directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.4d; //pronoun-preposition positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.3d; //pronoun-preposition determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //pronoun-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //pronoun-tolinker-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.7d;  //pronoun-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.6d;  //pronoun-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //pronoun-tolinker-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //pronoun-tolinker directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.5d; //pronoun-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.7d; //pronoun-tolinker determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //triple compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //compound noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0.7d;  //compound noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.6d;  //compound noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //triple compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.7d; //compound noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.7d; //compound noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.5d; //compound noun determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.8d; //pronoun-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.7d; //pronoun-adverb-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.6d;  //pronoun-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.5d;  //pronoun-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = .8d; //pronoun-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.7d; //pronoun-adverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.6d; //pronoun-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.5d; //pronoun-adverb determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //pronoun-adjective-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //pronoun-adjective-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.7d;  //pronoun-adjective-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.6d;  //pronoun-adjective-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //pronoun-adjective-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.2d; //pronoun-adjective directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.2d; //pronoun-adjective positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.7d; //pronoun-adjective determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double PronounNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //triple compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //compound noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0.7d;  //compound noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.7d;  //compound noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //triple compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.8d; //compound noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.7d; //compound noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.5d; //compound noun determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double AdverbUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //adverb-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.4d; //adverb-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0.3d;  //adverb-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.3d;  //adverb-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.5d; //adverb-uncaught-noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0.3d; //adverb-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0.2d; //adverb-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.3d; //adverb-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double AdverbDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //adverb-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.3d; //adverb-determiner-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.8d;  //adverb-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.7d;  //adverb-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //adverb-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.3d; //adverb-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-determiner determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double AdverbPreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-preposition-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-preposition-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-preposition-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-preposition-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb-preposition-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-preposition directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-preposition positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-preposition determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdverbToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.9d; //adverb-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.8d; //adverb-tolinker-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.7d;  //adverb-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.6d;  //adverb-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.9d; //adverb-tolinker-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-tolinker directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.3d; //adverb-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.4d; //adverb-tolinker determiner
                })
                .Default(() => {
                    modTwo = 0.1d;
                });
            return modTwo;
        }

        private static double AdverbPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //adverb compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.4; //adverb-noun-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.3;  //adverb-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.5; //adverb compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.6; //adverb-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.4; //adverb-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.4; //adverb-noun determiner
                })
                .Default(() => {
                    modTwo = 0.1;
                });
            return modTwo;
        }

        private static double AdverbAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.3; //adverb-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.3; //adverb-adverb-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.3;  //adverb-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.1;  //tri adverbial
                })
                .With<Pronoun>(() => {
                    modTwo = 0.3; //adverb-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.3; //adverb-adverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.2; //adverb-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.3; //adverb-adverb determiner
                })
                .Default(() => {
                    modTwo = 0.1;
                });
            return modTwo;
        }

        private static double AdverbAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.6; //adverb-adjective-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.4; //adverb-adjective-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0.5;  //adverb-adjective-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.4;  //adverb-adjective-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.6; //adverb-adjective-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.5; //adverb-adjective directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.4; //adverb-adjective positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.3; //adverb-adjective determiner
                })
                .Default(() => {
                    modTwo = 0.1;
                });
            return modTwo;
        }

        private static double AdverbNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.5d; //adverb -> compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0.4d; //adverb-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0.3d;  //adverb-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0.3d;  //adverb-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0.5; //adverb -> compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0.3; //adverb-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0.4; //adverb-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0.2; //adverb-noun determiner
                })
                .Default(() => {
                    modTwo = 0.1;
                });
            return modTwo;
        }

        private static double AdjectiveUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0.4; //adjective-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective -> uncaught -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectiveDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-determiner descriptor 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-determiner determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectivePreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective-prepositional-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-prepositional descriptor 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-prepositional-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-prepositional-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective-prepositional-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-prepositional directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-prepositional positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-prepositional determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectiveToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-tolinker descriptor 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective-tolinker-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-tolinker directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-tolinker determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectivePronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective -> compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective -> compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-noun determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectiveAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-adverb-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-adverb-adjective (triple compound)
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-adverb-directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-adverb determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectiveAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //compound adjective -> noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //compound adjective -> adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //compound adjective -> adjective (triple compound)
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //compound adjective -> adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //compound adjective -> noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //compound adjective -> directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //compound adjective -> positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //compound adjective -> determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double AdjectiveNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adjective -> compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adjective-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adjective-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adjective-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adjective -> compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adjective-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adjective-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adjective-noun determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> uncaught -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-determiner-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> determiner -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-determiner directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-determiner determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbPreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-preposition-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-preposition-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-preposition-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-preposition-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> preposition -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-preposition directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-preposition positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-preposition determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-tolinker-pastverb (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> tolinker -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-tolinker directional (possible?)
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-tolinker determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-pronoun-noun (compound)
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-pronoun-pastverb
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-pronoun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-pronoun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> pronoun -> noun  (compound)
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-pronoun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-pronoun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-pronoun determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-adverb-pastverb
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> adverb -> noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-aadverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-adverb determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-adjective-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-adjective-pastverb
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-adjective-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-adjective-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> adjective -> noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-adjective directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-adjective positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-adjective determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbPastParticipleVerb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb-pastverb -> compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-pastverb-pastverb
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-pastverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-pastverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> pastverb -> compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-pastverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-pastverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-pastverb determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double VerbNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //adverb -> compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //adverb-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //adverb-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //adverb-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //adverb -> compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //adverb-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //adverb-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //adverb-noun determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //noun-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun -> uncaught -> noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-uncaught determiner
                })
                .Default(nuu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //noun-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-determiner-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-determiner determiner
                })
                .Default(ndu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounPreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //noun-preposition-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-preposition-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-preposition-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-preposition-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-preposition-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-preposition directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-preposition positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-preposition determiner
                })
                .Default(npu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(nlnkn => {
                    modTwo = 0; //noun-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-tolinker-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-tolinker-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-tolinker directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-tolinker determiner
                })
                .Default(nlinku => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //triple compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //compound noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //compound noun-adjective
                })
                .With<Adverb>(npnadv => {
                    modTwo = 0;  //compound noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //triple compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //compound noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //compound noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //compound noun determiner
                })
                .Default(npnu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //noun-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-adverb-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-adverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-adverb determiner
                })
                .Default(nadvu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounVerb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //noun-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-adverb-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-adverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-adverb determiner
                })
                .Default(nvu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(nadjn => {
                    modTwo = 0; //noun-adjective-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //noun-adjective-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //noun-adjective-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //noun-adjective-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //noun-adjective-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //noun-adjective directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //noun-adjective positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //noun-adjective determiner
                })
                .Default(nadju => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double NounNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //triple compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //compound noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //compound noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //compound noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //triple compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //compound noun directional
                })
                .With<Preposition>(nnpre => {
                    modTwo = 0; //compound noun positional
                })
                .With<Determiner>(nnd => {
                    modTwo = 0; //compound noun determiner
                })
                .Default(nnu => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerUncaught(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-uncaught-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-uncaught-pastverb 
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-uncaught-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-uncaught-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determineruncaught-noun 
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-uncaught directional 
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-uncaught positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-uncaught determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerDeterminer(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-determiner-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-determiner-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-determiner-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-determiner-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-determiner-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-determiner directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-determiner positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-determiner determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerPreposition(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-preposition-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-preposition-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-preposition-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-preposition-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-preposition-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-preposition directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-preposition positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-preposition determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerToLinker(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-tolinker-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-tolinker-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-tolinker-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-tolinker-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-tolinker-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-tolinker directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-tolinker positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-tolinker determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerPronoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner compound noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-noun-determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerAdverb(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-adverb-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-adverb-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-adverb-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-adverb-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-adverb-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-adverb directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-adverb positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-adverb determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerAdjective(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-adjective-noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-adjective-adverb descriptor
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-adjective-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-adjective-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-adjective-noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-adjective directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-adjective positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-adjective determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;
        }

        private static double DeterminerNoun(Word nextNext) {
            double modTwo = 0;
            nextNext.Match()
                .With<Noun>(() => {
                    modTwo = 0; //determiner-compound noun
                })
                .With<PastParticipleVerb>(() => {
                    modTwo = 0; //determiner-noun-adverb descriptor (possible?)
                })
                .With<Adjective>(() => {
                    modTwo = 0;  //determiner-noun-adjective
                })
                .With<Adverb>(() => {
                    modTwo = 0;  //determiner-noun-adverb
                })
                .With<Pronoun>(() => {
                    modTwo = 0; //determiner-compound (possessive) noun
                })
                .With<ToLinker>(() => {
                    modTwo = 0; //determiner-noun directional
                })
                .With<Preposition>(() => {
                    modTwo = 0; //determiner-noun positional
                })
                .With<Determiner>(() => {
                    modTwo = 0; //determiner-noun determiner
                })
                .Default(() => {
                    modTwo = 0;
                });
            return modTwo;

        }

        #endregion
    }


}
