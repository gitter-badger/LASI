﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm.Thesauri
{
    /// <summary>
    /// Represents entity Synonym set entry corresponding to entity line in entity WordNet thesaurus file.
    /// This type is used within the various Thesaurus implementations to compose and query the contents of the WordNet database files.
    /// This class is internal forbidding instantiation outside of the Thesaurus Namespace.
    /// </summary>
    internal class SynonymSet : IReadOnlyCollection<string>
    {
        public SynonymSet(IEnumerable<string> referencedSetIds, IEnumerable<string> memberWords, WordNetVerbLex lexName) {
            _members = new HashSet<string>(memberWords);
            _referencedIndexes = new HashSet<string>(referencedSetIds);
            LexName = lexName;
            IndexCode = referencedSetIds.First();
        }

        /// <summary>
        /// Gets the members directly contained within the SynonymSet.
        /// </summary>
        public IEnumerable<string> Members {
            get {
                return _members;
            }
        }


        private HashSet<string> _members;

        private HashSet<string> _referencedIndexes;

        /// <summary>
        /// Gets or sets the collection of SynonymSet-index-codes corresponding to referenced SynonymSets.
        /// </summary>
        public IEnumerable<string> ReferencedIndexes {
            get {
                return _referencedIndexes;
            }

        }
        /// <summary>
        /// Gets the IndexCode which identifies the SynonymSet.
        /// </summary>
        public string IndexCode {
            get;
            protected set;
        }
        /// <summary>
        /// Returns entity single string representing the members of the SynonymSet.
        /// </summary>
        /// <returns>entity single string representing the members of the SynonymSet.</returns>
        public override string ToString() {
            return "[" + IndexCode + "] " + Members.Aggregate("", (str, code) => {
                return str + "  " + code;
            });
        }

        /// <summary>
        /// Gets the number of direct members contained in the SynonymSet.
        /// </summary>
        public int Count {
            get {
                return Members.Count();
            }
        }

        /// <summary>
        /// Exposes an enumerator exposes the direct members when the SynonymSet is enumerated.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator() {
            return Members.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public WordNetVerbLex LexName {
            get;
            set;
        }
    }
}
