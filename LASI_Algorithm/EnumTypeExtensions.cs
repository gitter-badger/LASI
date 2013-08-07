﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Algorithm
{
    /// <summary>
    /// Defines extension methods which augment specific enum Types for quick, centralized access to common usage patterns.
    /// </summary>
    /// <see cref="PronounKind"/>
    public static class EnumTypeExtensions
    {
        #region PronounKind value extensions

        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are thought of as explicitely male.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are thought of as explicitely male, false otherwise.</returns>
        public static bool IsMale(this PronounKind kind) {
            return kind == PronounKind.Male || kind == PronounKind.MaleReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are thought of as explicitely female.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are thought of as explicitely female, false otherwise.</returns>
        public static bool IsFemale(this PronounKind kind) {
            return kind == PronounKind.Female || kind == PronounKind.FemaleReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are thought of as explicitely gender neutral.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are thought of as explicitely gender neutral, false otherwise.</returns>
        public static bool IsGenderNeutral(this PronounKind kind) {
            return kind == PronounKind.GenderNeurtral || kind == PronounKind.GenderNeurtralReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are thought of as explicitely gender ambiguous.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are thought of as explicitely gender ambiguous, false otherwise.</returns>
        public static bool IsGenderAmbiguous(this PronounKind kind) {
            return !(kind.IsFemale() || kind.IsMale() || kind.IsGenderNeutral());
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are thought of as explicitely plural.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are thought of as explicitely plural, false otherwise.</returns>
        public static bool IsPlural(this PronounKind kind) {
            return
                kind == PronounKind.Plural ||
                kind == PronounKind.PluralReflexive ||
                kind == PronounKind.FirstPersonPlural ||
                kind == PronounKind.FirstPersonPluralReflexive ||
                kind == PronounKind.SecondPersonPluralReflexive ||
                kind == PronounKind.ThirdPersonGenderAmbiguousPlural ||
                kind == PronounKind.ThirdPersonPluralReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are reflexive.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are reflexive false, otherwise.</returns>
        public static bool IsReflexive(this PronounKind kind) {
            return
                kind == PronounKind.FemaleReflexive ||
                kind == PronounKind.FirstPersonPluralReflexive ||
                kind == PronounKind.GenderAmbiguousReflexive ||
                kind == PronounKind.GenderNeurtralReflexive ||
                kind == PronounKind.MaleReflexive ||
                kind == PronounKind.PluralReflexive ||
                kind == PronounKind.SecondPersonPluralReflexive ||
                kind == PronounKind.SecondPersonSingularReflexive ||
                kind == PronounKind.ThirdPersonPluralReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are first person.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are first person, false otherwise.</returns>
        public static bool IsFirstPerson(this PronounKind kind) {
            return
                kind == PronounKind.FirstPersonPlural ||
                kind == PronounKind.FirstPersonPluralReflexive ||
                kind == PronounKind.FirstPersonSingular;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are second person.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are second person, false otherwise.</returns>
        public static bool IsSecondPerson(this PronounKind kind) {
            return
                kind == PronounKind.SecondPerson ||
                kind == PronounKind.SecondPersonPluralReflexive ||
                kind == PronounKind.SecondPersonSingularReflexive;
        }
        /// <summary>
        /// Determines if the PronounKind is among the semantic categories which are third person.
        /// </summary>
        /// <param name="kind">The PronounKind to test.</param>
        /// <returns>True if the PronounKind is among the semantic categories which are third person, false otherwise.</returns>
        public static bool IsThirdPerson(this PronounKind kind) {
            return !(kind.IsFirstPerson() || kind.IsSecondPerson());
        }

        #endregion

        #region EntityKind <-> PronounKind Comparisons
        /// <summary>
        /// Determines if two PersonalPronoun instances have the same gender.
        /// </summary>
        /// <param name="first">The first PersonalPronoun.</param>
        /// <param name="second">The second PersonalPronoun.</param>
        /// <returns>True if both PersonalPronouns have the same gender, false otherwise.</returns>
        public static bool IsGenderEquivalentTo(this PersonalPronoun first, PersonalPronoun second) {
            var kind1 = first.PronounKind;
            var kind2 = second.PronounKind;
            return
                kind1.IsFemale() && kind2.IsFemale() ||
                kind1.IsMale() && kind2.IsMale() ||
                kind1.IsGenderNeutral() && kind2.IsGenderNeutral() ||
                kind1.IsGenderAmbiguous() && kind2.IsGenderAmbiguous();
        }

        /// <summary>
        /// Determines if a PersonalPronoun and a ProperNoun instance have the same gender.
        /// </summary>
        /// <param name="personalPronoun">The PersonalPronoun.</param>
        /// <param name="properNoun">The ProperNoun.</param>
        /// <returns>True if PersonalPronoun and the ProperNoun have the same gender, false otherwise.</returns>
        public static bool IsGenderEquivalentTo(this PersonalPronoun personalPronoun, ProperNoun properNoun) {
            var pronounKind = personalPronoun.PronounKind;
            var entityKind = properNoun.EntityKind;
            return pronounKind.IsFemale() ?
                entityKind == EntityKind.PersonFemale :
                pronounKind.IsMale() ?
                entityKind == EntityKind.PersonMale :
                false;
        }

        #endregion
    }
}