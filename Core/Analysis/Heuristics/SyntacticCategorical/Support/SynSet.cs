﻿using LASI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Core.Heuristics.WordNet
{
    abstract class SynSet<TSetRelationship> : IEquatable<SynSet<TSetRelationship>>
    {
        protected SynSet(int id, IEnumerable<string> words, IEnumerable<KeyValuePair<TSetRelationship, int>> pointerRelationships) {
            Id = id;
            Words = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
            relatedSetsByRelationKindSource = pointerRelationships;
            ReferencedSet = new HashSet<int>(pointerRelationships.Select(p => p.Value));
        }
        /// <summary>
        /// Gets the ID of the SynSet.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets all of the words belonging to the SynSet.
        /// </summary>
        public HashSet<string> Words { get; }
        /// <summary>
        /// Gets the IDs of all sets referenced by the SynSet.
        /// </summary>
        public HashSet<int> ReferencedSet { get; }
        /// <summary>
        /// Returns the IDs of all other SynSets which are referenced from the current SynSet in the indicated fashion. 
        /// </summary>
        /// <param name="relationships">The kinds of external set relationships to consider return.</param>
        /// <returns>The IDs of all other SynSets which are referenced from the current SynSet in the indicated fashion.</returns>
        public IEnumerable<int> this[params TSetRelationship[] relationships] {
            get {
                if (referencedSetsByLinkType == null)
                    referencedSetsByLinkType = relatedSetsByRelationKindSource.ToLookup(p => p.Key, p => p.Value);
                foreach (var r in relationships) {
                    foreach (var related in referencedSetsByLinkType[r]) { yield return related; }
                }
            }
        }
        private ILookup<TSetRelationship, int> referencedSetsByLinkType;
        private IEnumerable<KeyValuePair<TSetRelationship, int>> relatedSetsByRelationKindSource;
        /// <summary>
        /// Returns the IDs of all other SynSets which are referenced from the current SynSet in the indicated fashion. 
        /// </summary>
        /// <returns>The IDs of all other SynSets which are referenced from the current SynSet in the indicated fashion.</returns>
        public ILookup<TSetRelationship, int> RelatedSetIdsByRelationKind {
            get { if (referencedSetsByLinkType == null) referencedSetsByLinkType = relatedSetsByRelationKindSource.ToLookup(p => p.Key, p => p.Value); return referencedSetsByLinkType; }
        }
        /// <summary>
        /// Returns a value indicating whether the given word is directly contained within the SynSet.
        /// </summary>
        /// <param name="word">The word to find.</param>
        /// <returns> <c>true</c> if the given word is directly contained within the Synset; otherwise false.</returns>
        public bool ContainsWord(string word) => Words.Contains(word);
        /// <summary>
        /// Returns a value indicating whether the current SynSet directly links to the given SynSet.
        /// </summary>
        /// <param name="other">The SynSet to find.</param>
        /// <returns> <c>true</c> if the current SynSet directly links to given SynSet; otherwise false.</returns>
        public bool DirectlyReferences(SynSet<TSetRelationship> other) => ReferencedSet.Contains(other.Id);

        public override int GetHashCode() => Id;

        public bool Equals(SynSet<TSetRelationship> other) => this == other;

        public override bool Equals(object obj) => obj as SynSet<TSetRelationship> == this;

        /// <summary>
        /// Returns a single string representing the SynSet.
        /// </summary>
        /// <returns>A single string representing the SynSet.</returns>
        public override string ToString() {
            return "[" + Id + "] " + Words
                .Format(Tuple.Create(' ', ',', ' '));
        }
        public static bool operator ==(SynSet<TSetRelationship> left, SynSet<TSetRelationship> right) {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);
            if (ReferenceEquals(right, null))
                return ReferenceEquals(left, null);
            return left.Id == right.Id;
        }
        public static bool operator !=(SynSet<TSetRelationship> left, SynSet<TSetRelationship> right) {
            return !(left == right);
        }


    }




    /// <summary>
    /// Represents a synset parsed from a line of the data.noun file of the WordNet package.
    /// </summary>
    internal sealed class NounSynSet : SynSet<NounLink>
    {
        public NounSynSet(int id, IEnumerable<string> words, IEnumerable<KeyValuePair<NounLink, int>> pointerRelationships, NounCategory category)
            : base(id, words, pointerRelationships) {
            Category = category;
        }

        public NounCategory Category { get; }
    }
    /// <summary>
    /// Represents a synset parsed from a line of the data.verb file of the WordNet package.
    /// </summary>
    internal sealed class VerbSynSet : SynSet<VerbLink>
    {
        public VerbSynSet(int id, IEnumerable<string> words, IEnumerable<KeyValuePair<VerbLink, int>> referencedSets, VerbCategory category)
            : base(id, words, referencedSets) {
            Category = category;
        }
        public VerbCategory Category { get; }
    }
    /// <summary>
    /// Represents a synset parsed from the data.adj file of the WordNet package.
    /// </summary>
    internal sealed class AdjectiveSynSet : SynSet<AdjectiveLink>
    {
        public AdjectiveSynSet(int id, IEnumerable<string> words, IEnumerable<KeyValuePair<AdjectiveLink, int>> pointerRelationships, AdjectiveCategory category)
            : base(id, words, pointerRelationships) {
            Category = category;
        }

        public AdjectiveCategory Category { get; }
    }
    /// <summary>
    /// Represents a synset parsed from a line of the data.adv file of the WordNet package.
    /// </summary>
    internal sealed class AdverbSynSet : SynSet<AdverbLink>
    {
        public AdverbSynSet(int id, IEnumerable<string> words, IEnumerable<KeyValuePair<AdverbLink, int>> pointerRelationships, AdverbCategory category)
            : base(id, words, pointerRelationships) {
            Category = category;
        }
        public AdverbCategory Category { get; }
    }

}