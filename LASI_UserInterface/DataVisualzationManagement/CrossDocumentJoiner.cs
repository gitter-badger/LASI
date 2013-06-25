﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Algorithm;
using LASI.Algorithm.DocumentConstructs;

using LASI.Algorithm.Thesauri;

namespace LASI.UserInterface
{
    public class CrossDocumentJoiner
    {


        public CrossDocumentJoiner(IEnumerable<Document> documents) {
            Documents = documents;

        }
        public async Task<IEnumerable<RelationshipTuple>> JoinDocumentsAsnyc() {

            return await await Task.Factory.ContinueWhenAll(
                new[]{  Task.Run(()=> GetCommonalitiesByVerbals()),
                        Task.Run(()=> GetCommonalitiesByBoundEntities())}, async tasks => {
                            var results = new List<RelationshipTuple>();
                            foreach (var t in tasks) {
                                results.AddRange(await t);
                            }
                            return results.Distinct();
                        });



        }

        private async Task<IEnumerable<RelationshipTuple>> GetCommonalitiesByBoundEntities() {
            var topNPsByDoc = from doc in Documents.AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                              select GetTopNounPhrases(doc);
            var nounCommonalities = (from outerSet in topNPsByDoc
                                         .AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                                     from np in outerSet
                                     from innerSet in topNPsByDoc
                                     where innerSet != outerSet
                                     where innerSet.Contains(np, (L, R) => L.Text == R.Text || L.IsAliasFor(R) || L.IsSimilarTo(R))
                                     select np).Distinct((L, R) => L.Text == R.Text || L.IsAliasFor(R) || L.IsSimilarTo(R));
            var results = from n in nounCommonalities
                              .InSubjectRole()
                              .AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                          select new RelationshipTuple {
                              Subject = n,
                              Verbal = n.SubjectOf,
                              Direct = n.SubjectOf
                                  .DirectObjects
                                  .OfType<NounPhrase>()
                                  .FirstOrDefault(),
                              Indirect = n.SubjectOf
                                  .IndirectObjects
                                  .OfType<NounPhrase>()
                                  .FirstOrDefault(),
                              Prepositional = n.SubjectOf.ObjectOfThePreoposition
                          } into result
                          group result by result.Subject.Text into resultGrouped
                          select resultGrouped.First() into result
                          orderby result.Subject.Weight
                          select result;
            await Task.Yield();
            return results.Distinct();

        }
        private async Task<IEnumerable<RelationshipTuple>> GetCommonalitiesByVerbals() {
            var topVerbalsByDoc = await Task.WhenAll(from doc in Documents.AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                                                     select GetTopVerbPhrases(doc));
            var verbalCominalities = from verbPhraseSet in topVerbalsByDoc.AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                                     from vp in verbPhraseSet
                                     where (from verbs in topVerbalsByDoc
                                            select verbs.Contains(vp, (L, R) => L.Text == R.Text || R.IsSimilarTo(R)))
                                            .Aggregate(true, (product, result) => product &= result)
                                     select vp;
            return (from v in verbalCominalities.AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                    select new RelationshipTuple {
                        Verbal = v,
                        Direct = v.DirectObjects
                            .OfType<NounPhrase>()
                            .Select(s => (s as IPronoun) == null ? s : (s as IPronoun).BoundEntity as IEntity)
                            .FirstOrDefault(),
                        Indirect = v.IndirectObjects
                            .OfType<NounPhrase>()
                            .Select(s => (s as IPronoun) == null ? s : (s as IPronoun).BoundEntity as IEntity)
                            .FirstOrDefault(),
                        Subject = v.Subjects
                            .OfType<NounPhrase>()
                            .Select(s => (s as IPronoun) == null ? s : (s as IPronoun).BoundEntity as IEntity)
                            .FirstOrDefault(),
                        Prepositional = v.ObjectOfThePreoposition
                    } into result
                    where result.Subject != null
                    group result by result.Verbal.Text into resultGrouped
                    select resultGrouped.First() into result
                    orderby result.Verbal.Weight
                    select result);
        }

        public async Task<IEnumerable<NounPhrase>> GetTopNounPhrasesAsync(Document document) {
            return await Task.Run(() => from distinctNP in
                                            (from np in document
                                                 .GetEntities().GetPhrases()
                                                 .AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                                                 .GetNounPhrases()
                                             where np.SubjectOf != null && (np.DirectObjectOf != null || np.IndirectObjectOf != null)
                                             select np).Distinct((L, R) => L.Text == R.Text || L.IsAliasFor(R) || L.IsSimilarTo(R))
                                        orderby distinctNP.Weight
                                        select distinctNP);

        }
        public IEnumerable<NounPhrase> GetTopNounPhrases(Document document) {
            return from distinctNP in
                       (from np in document
                            .GetEntities().GetPhrases()
                            .AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax)
                            .GetNounPhrases()
                        where np.SubjectOf != null && (np.DirectObjectOf != null || np.IndirectObjectOf != null)
                        select np
                        ).Distinct((L, R) => L.Text == R.Text || L.IsAliasFor(R) || L.IsSimilarTo(R))
                   orderby distinctNP.Weight
                   select distinctNP;

        }
        public async Task<IEnumerable<VerbPhrase>> GetTopVerbPhrases(Document document) {
            return await Task.Run(() => {
                var vpsWithSubject = document.Phrases.GetVerbPhrases().AsParallel().WithDegreeOfParallelism(Concurrency.CurrentMax).WithSubject();
                return from vp in vpsWithSubject.WithDirectObject().Concat(vpsWithSubject.WithIndirectObject()).Distinct()
                       orderby vp.Weight +
                       vp.Subjects.Sum(e => e.Weight) +
                       vp.DirectObjects.Sum(e => e.Weight) +
                       vp.IndirectObjects.Sum(e => e.Weight)
                       select vp as VerbPhrase;
            });
        }


        public IEnumerable<Document> Documents {
            get;
            protected set;
        }

    }

}