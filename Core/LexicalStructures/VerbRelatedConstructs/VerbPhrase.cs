﻿using LASI.Core.Heuristics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System;

namespace LASI.Core
{
    /// <summary>
    /// Represents a Verb Phrase, a Phrase with the syntactic role of a verb.
    /// </summary>
    public class VerbPhrase : Phrase, IVerbal, IAdverbialModifiable, IModalityModifiable
    {
        public IEnumerable<IAdverbial> AttributedBy => AdverbialModifiers;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the VerbPhrase class.
        /// </summary>
        /// <param name="words">The words which compose to form the VerbPhrase.</param>
        public VerbPhrase(IEnumerable<Word> words)
            : base(words) {
            PrevailingForm = (from verb in Words.OfVerb()
                              group verb.VerbForm by verb.VerbForm into byForm
                              orderby byForm.Count() descending
                              select byForm.Key)
                .FirstOrDefault();
        }

        /// <summary>
        /// Initializes a new instance of the VerbPhrase class.
        /// </summary>
        /// <param name="first">The first Word of the VerbPhrase.</param>
        /// <param name="rest">The rest of the Words comprise the VerbPhrase.</param>
        /// <remarks>
        /// This constructor overload reduces the syntactic overhead associated with the manual construction of Phrases. Thus, its purpose
        /// is to simplifiy test code.
        /// </remarks>
        public VerbPhrase(Word first, params Word[] rest) : this(rest.Prepend(first)) { }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Attaches an Adverbial construct, such as an Adverb or AdverbPhrase, as a modifier of the Verb.
        /// </summary>
        /// <param name="modifier">The Adverbial construct by which to modify the AdjectivePhrase.</param>
        public void ModifyWith(IAdverbial modifier) {
            modifiers = modifiers.Add(modifier);
            modifier.ModifiedBy = this;
        }

        /// <summary>
        /// <para>Binds the VerbPhrase to an object via a prepositional construct such as a Preposition or PrepositionalPhrase.</para>
        /// <para>Example: He "ran" to work. where "work" is the object of ran via the prepositional construct "to"</para>
        /// </summary>
        /// <param name="prepositional">The IPrepositional construct through which the Object is associated.</param>
        public virtual void AttachObjectViaPreposition(IPrepositional prepositional) {
            ObjectOfThePreposition = prepositional.BoundObject;
            PrepositionalToObject = prepositional;
        }

        /// <summary>
        /// Binds the given Entity as a subject of the VerbPhrase instance.
        /// </summary>
        /// <param name="subject">The Entity to attach to the VerbPhrase as a subject.</param>
        public virtual void BindSubject(IEntity subject) {
            if (subject != null) {
                subjects = subjects.Add(subject);
                subject.SubjectOf = this;
                if (PostpositiveDescriptor != null) { subject.BindDescriptor(postpositiveDescriptor); }
                foreach (var v in Words.OfVerb()) { v.BindSubject(subject); }
            }
        }

        /// <summary>
        /// Binds the given Entity as a direct object of the VerbPhrase instance.
        /// </summary>
        /// <param name="directObject">The Entity to attach to the VerbPhrase as a direct object.</param>
        public virtual void BindDirectObject(IEntity directObject) {
            if (directObject != null) {
                directObjects = directObjects.Add(directObject);
                directObject.DirectObjectOf = this;
                foreach (var v in Words.OfVerb()) { v.BindDirectObject(directObject); }
                if (IsPossessive) {
                    foreach (var subject in Subjects) {
                        subject.AddPossession(directObject);
                    }
                } else if (IsClassifier) {
                    foreach (var subject in Subjects) {
                        AliasLookup.DefineAlias(subject, directObject);
                    }
                }
            }
        }

        /// <summary>
        /// Binds the given Entity as an indirect object of the VerbPhrase instance.
        /// </summary>
        /// <param name="indirectObject">The Entity to attach to the VerbPhrase as an indirect object.</param>
        public virtual void BindIndirectObject(IEntity indirectObject) {
            if (indirectObject != null) {
                indirectObjects = indirectObjects.Add(indirectObject);
                indirectObject.IndirectObjectOf = this;
                foreach (var v in Words.OfVerb()) { v.BindIndirectObject(indirectObject); }
            }
        }

        /// <summary>
        /// Returns a string representation of the VerbPhrase.
        /// </summary>
        /// <returns>A string representation of the VerbPhrase.</returns>
        public override string ToString() {
            return !Phrase.VerboseOutput ? base.ToString() :
            string.Join("\n", base.ToString(),
                Subjects.Any() ? "Subjects: " + Subjects.Format(s => s.Text) : string.Empty,
                DirectObjects.Any() ? "Direct Objects: " + DirectObjects.Format(o => o.Text) : string.Empty,
                IndirectObjects.Any() ? "Indirect Objects: " + IndirectObjects.Format(o => o.Text) : string.Empty,
                ObjectOfThePreposition != null ? "Via Preposition Object: " + ObjectOfThePreposition.Text : string.Empty,
                Modality != null ? "Modality: " + Modality.Text : string.Empty,
                AdverbialModifiers.Any() ? "Modifiers: " + AdverbialModifiers.Format(m => m.Text) : string.Empty,
                "\nPossessive: [\{IsPossessive}]",
                "\nClassifier: [\{IsClassifier}]",
                "\nPrevailing Form: [\{PrevailingForm}]"
            );
        }

        /// <summary>
        /// Determines if the VerbPhrase implies a possession relationship. E.g. in the sentence "They certainly have a lot of ideas." the
        /// VerbPhrase "certainly have" asserts a possessor possessee relationship between "They" and "a lot of ideas".
        /// </summary>
        /// <returns><c>true</c> if the VerbPhrase is a possessive relationship specifier; otherwise, <c>false</c>.</returns>
        private bool DetermineIsPossessive() => Words.OfVerb().Any() && Words.OfVerb().Last().IsPossessive;

        /// <summary>
        /// Determines if the VerbPhrase acts as a classifier. E.g. in the sentence "Rodents are definitely prey animals." the VerbPhrase
        /// "are definitely" acts as a classifier because it states that rodents are a subset of prey animals.
        /// </summary>
        /// <returns><c>true</c> if the VerbPhrase is a classifier; otherwise, <c>false</c>.</returns>
        private bool DetermineIsClassifier() {
            return !IsPossessive &&
                //AdverbialModifiers.None() &&
                Words.OfVerb().Any() &&
                Words.OfVerb().All(v => v.IsClassifier);
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets an IAggregateEntity implementation composed from all of the VerbPhrase's subjects.
        /// </summary>
        public IAggregateEntity AggregateSubject => subjects.ToAggregate();

        /// <summary>
        /// Gets an IAggregateEntity implementation composed from all of the VerbPhrase's direct objects.
        /// </summary>
        public IAggregateEntity AggregateDirectObject => directObjects.ToAggregate();

        /// <summary>
        /// Gets an IAggregateEntity implementation composed from all of the VerbPhrase's indirect objects.
        /// </summary>
        public IAggregateEntity AggregateIndirectObject => indirectObjects.ToAggregate();

        /// <summary>
        /// Gets the collection of IAdverbial modifiers which modify the VerbPhrase.
        /// </summary>
        public IEnumerable<IAdverbial> AdverbialModifiers => modifiers;

        /// <summary>
        /// Gets or sets the IDescriptor which modifies, by way of the Verbal, its Subject.
        /// </summary>
        public IDescriptor PostpositiveDescriptor {
            get {
                return postpositiveDescriptor;
            }
            set {
                postpositiveDescriptor = value;
                foreach (var described in Subjects) { described.BindDescriptor(value); }
            }
        }

        /// <summary>
        /// Gets the prevailing Tense of the VerbPhrase.
        /// </summary>
        /// <see cref="VerbForm" />
        public VerbForm PrevailingForm { get; }

        /// <summary>
        /// Gets or sets the ModalAuxilary word which modifies the VerbPhrase.
        /// </summary>
        public ModalAuxilary Modality { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the VerbPhrase has possessive semantics. E.g. "A (has) a B"
        /// </summary>
        public bool IsPossessive => isPossessive ?? (isPossessive = DetermineIsPossessive()) ?? false;

        /// <summary>
        /// Gets a value indicating whether or not the VerbPhrase has classifying semantics. E.g. "A (is) a B"
        /// </summary>
        public bool IsClassifier => isClassifier ?? (isClassifier = DetermineIsClassifier()) ?? false;

        /// <summary>
        /// Gets the subjects of the VerbPhrase.
        /// </summary>
        public IEnumerable<IEntity> Subjects => subjects;

        /// <summary>
        /// Gets the direct objects of the VerbPhrase.
        /// </summary>
        public IEnumerable<IEntity> DirectObjects => directObjects;

        /// <summary>
        /// Gets the indirect objects of the VerbPhrase.
        /// </summary>
        public IEnumerable<IEntity> IndirectObjects => indirectObjects;

        /// <summary>
        /// Gets the VerbPhrases's object, If the VerbPhrase has an object bound via a Prepositional. This can be any ILexical construct
        /// including a word, phrase, or clause.
        /// </summary>
        public ILexical ObjectOfThePreposition { get; protected set; }

        /// <summary>
        /// Gets the IPrepositional object which links the VerbPhrase to the ObjectOfThePreoposition.
        /// </summary>
        public IPrepositional PrepositionalToObject { get; protected set; }

        #endregion Properties

        #region Fields

        private IImmutableSet<IAdverbial> modifiers = ImmutableHashSet<IAdverbial>.Empty;
        private IImmutableSet<IEntity> subjects = ImmutableHashSet<IEntity>.Empty;
        private IImmutableSet<IEntity> directObjects = ImmutableHashSet<IEntity>.Empty;
        private IImmutableSet<IEntity> indirectObjects = ImmutableHashSet<IEntity>.Empty;
        private IDescriptor postpositiveDescriptor;

        private bool? isClassifier;
        private bool? isPossessive;

        #endregion Fields
    }
}