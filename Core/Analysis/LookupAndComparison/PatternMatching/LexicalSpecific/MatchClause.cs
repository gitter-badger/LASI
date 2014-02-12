﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Core.Patternization
{
    /// <summary>
    /// Represents a type for the representation and free form structuring of Type based Pattern Matching expressions which match with a value of Type T and does not yield a result. 
    /// </summary>
    /// <typeparam name="T">The Type of the value which the the Pattern Matching expression will match with.</typeparam> 
    [DebuggerStepThrough]
    public abstract class MatchBase<T> where T : class, ILexical
    {
        /// <summary>
        /// Initializes a new instance of the MatchBase&lt;T&gt; class which will match against the supplied value.
        /// </summary>
        /// <param name="value">The value to match against.</param>
        protected MatchBase(T value) { this.value = value; }
        #region Fields

        /// <summary>
        /// The value indicating if a match was found or if the default value will be yielded by the Result method.
        /// </summary>
        private bool accepted;
        /// <summary>
        /// Gets or sets the value indicating if a match has succeeded.
        /// </summary>
        protected bool Accepted {
            get { return accepted; }
            set { accepted = value; }
        }
        private T value;
        /// <summary>
        /// Gets or sets the value being matched against.
        /// </summary>
        protected T Value {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion
    }
    /// <summary>
    /// Provides for the representation and free form structuring of Type based Pattern Matching expressions which match with a value of Type T and does not yield a result.
    /// </summary>
    /// <typeparam name="T">The Type of the value which the the Pattern Matching expression will match with.</typeparam> 
    /// <remarks>
    /// <para>
    /// 1. Its is important to note that null values are coalesced by the Match process. 
    /// That is to say that if the value being tested is null it will never match any of the Clauses.
    /// Because of this a null value will never produce an error.
    /// </para>
    /// </remarks>
    [DebuggerStepThrough]
    public sealed class Match<T> : MatchBase<T> where T : class, ILexical
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Match&lt;T&gt; class which will match against the supplied value.
        /// </summary>
        /// <param name="value">The value to match against.</param>
        public Match(T value) : base(value) { }
        #endregion

        #region Expression Transformations
        /// <summary>
        /// Promotes the current non result returning expression of type Case&lt;T&gt; into a result returning expression of Case&lt;T, R&gt;
        /// Such that subsequent With expressions appended are now to yield a result value of the supplied Type R.
        /// </summary>
        /// <typeparam name="TResult">The Type of the result which the match expression may now return.</typeparam>
        /// <returns>A Match&lt;T, R&gt; describing the Match expression so far.</returns> 
        public Match<T, TResult> Yield<TResult>() {
            return Match.Create<T, TResult>(Value);
        }
        #endregion

        #region When Expressions
        /// <summary>
        /// Appends a When expression to the current PatternMatching Expression. The When expression applies a predicate to the value being matched over. 
        /// It must be followed by a Then expression which is only considered if the predicate applied here returns true.
        /// </summary>
        /// <param name="predicate">The predicate to test the value being matched over.</param>
        /// <returns>The PredicatedMatch&lt;T&gt; describing the Match expression so far. This must be followed by a single Then expression.</returns>
        public PredicatedMatch<T> When(Func<T, bool> predicate) {
            return Match.Create(predicate(Value), this);
        }
        /// <summary>
        /// Appends a When expression to the current PatternMatching Expression. The When expression applies a predicate to the value being matched over. 
        /// It must be followed by a Then expression which is only considered if the predicate applied here returns true.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. That the value being matched is of this type is also necessary for the following then expression to be selected.</typeparam>
        /// <param name="predicate">The predicate to test the value being matched over.</param>
        /// <returns>The PredicatedMatch&lt;T&gt; describing the Match expression so far. This must be followed by a single Then expression.</returns>
        public PredicatedMatch<T> When<TPattern>(Func<TPattern, bool> predicate) where TPattern : class,ILexical {
            var typed = Value as TPattern;
            return Match.Create(typed != null && predicate(typed), this);
        }
        /// <summary>
        /// Appends a When expression to the current pattern. 
        /// This applies a predicate to the value being matched suched that the subsequent Then expression will only be chosen if the predicate returns true.
        /// </summary>
        /// <param name="condition">The predicate to test the value being matched.</param>
        /// <returns>The PredicatedMatch&lt;T&gt; describing the Match expression so far. This must be followed by a single Then expression</returns>     
        public PredicatedMatch<T> When(bool condition) {
            return Match.Create(condition, this);
        }
        #endregion

        #region With Expressions
        /// <summary>
        /// Appends a Match with Type expression to the current PatternMatching Expression.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. If the value being matched is of this type, this With expression will be selected and the provided action invoked.</typeparam>
        /// <param name="action">The Action which, if this With expression is Matched, will be invoked.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public Match<T> With<TPattern>(Action action) where TPattern : class ,ILexical {

            if (!Accepted && Value is TPattern) {
                Accepted = true;
                action();
            }
            return this.With<TPattern>(x => action());
        }
        /// <summary>
        /// Appends a Match with Type expression to the current PatternMatching Expression.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. If the value being matched is of this type, this With expression will be selected and the provided action invoked.</typeparam>
        /// <param name="action">The Action&lt;TPattern&gt; which, if this With expression is Matched, will be invoked on the value being matched over by the PatternMatching expression.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public Match<T> With<TPattern>(Action<TPattern> action) where TPattern : class, ILexical {
            if (Value != null) {
                if (!Accepted) {
                    var matched = Value as TPattern;
                    if (matched != null) {
                        Accepted = true;
                        action(matched);
                    }
                }
            }
            return this;
        }


        #endregion

        #region Default Expressions

        /// <summary>
        /// Appends the Default expression to the current Pattern Matching expression.
        /// </summary>
        /// <param name="action">The function to invoke if no matches in the expression succeeded.</param>

        public void Default(Action action) {
            if (!Accepted)
                action();
        }
        /// <summary>
        /// Appends the Default expression to the current Pattern Matching expression.
        /// </summary>
        /// <param name="action">The function to invoke on the match with value if no matches in the expression succeeded.</param>

        public void Default(Action<T> action) {
            if (!Accepted && Value != null) {
                action(Value);
            }
        }

        #endregion


        #region Operators




        //public static implicit operator bool(Match<T> match) { return match.Accepted; }
        #endregion



    }
    /// <summary>
    /// Provides for the representation and free form structuring of Type based Pattern Matching expressions from a match with value of Type T to a result of Type TResult.
    /// </summary>
    /// <typeparam name="T">The Type of the value which the the Pattern Matching expression will match with.</typeparam>
    /// <typeparam name="TResult">The Type of the result to be yielded by the Pattern Matching expression.</typeparam> 
    /// <remarks>
    /// <para>
    /// 1. Its is important to note that null values are coalesced by the Match process. 
    /// That is to say that if the value being tested is null it will never match any of the Clauses.
    /// Because of this a null value will always yield the default result and never produce an error.
    /// </para>
    /// </remarks>
    [DebuggerStepThrough]
    public sealed class Match<T, TResult> : MatchBase<T> where T : class, ILexical
    {
        #region Constructors

        /// <summary>
        /// Initailizes a new instance of the Case&lt;T,R&gt; which will allow for Pattern Matching with the provided value.
        /// </summary>
        /// <param name="value">The value to match with.</param>
        public Match(T value) : base(value) { }
        #endregion

        #region When Expressions
        /// <summary>
        /// Appends a When expression to the current pattern. 
        /// This applies a predicate to the value being matched suched that the subsequent Then expression will only be chosen if the predicate returns true.
        /// </summary>
        /// <param name="predicate">The predicate to test the value being matched.</param>
        /// <returns>The PredicatedMatch&lt;T, R&gt; describing the Match expression so far. This must be followed by a single Then expression.</returns>
        public PredicatedMatch<T, TResult> When(Func<T, bool> predicate) {
            return Match.Create(predicate(Value), this);
        }
        /// <summary>
        /// Appends a When expression to the current pattern. 
        /// This applies a predicate to the value being matched suched that the subsequent Then expression will only be chosen if the predicate returns true.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. That the value being matched is of this type is also necessary for the following then expression to be selected.</typeparam>
        /// <param name="predicate">The predicate to test the value being matched.</param>
        /// <returns>The PredicatedMatch&lt;T, R&gt; describing the Match expression so far. This must be followed by a single Then expression.</returns>
        public PredicatedMatch<T, TResult> When<TPattern>(Func<TPattern, bool> predicate) where TPattern : class, ILexical {
            var typed = Value as TPattern;
            return Match.Create(typed != null && predicate(typed), this);
        }
        /// <summary>
        /// Appends a When expression to the current pattern. 
        /// This applies a predicate to the value being matched suched that the subsequent Then expression will only be chosen if the predicate returns true.
        /// </summary>
        /// <param name="condition">The predicate to test the value being matched.</param>
        /// <returns>The PredicatedMatch&lt;T, R&gt; describing the Match expression so far. This must be followed by a single Then expression.</returns>
        public PredicatedMatch<T, TResult> When(bool condition) {
            return Match.Create(condition, this);
        }
        #endregion

        #region With Expressions
        /// <summary>
        /// Appends a Match with Type expression to the current PatternMatching Expression.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. If the value being matched is of this type, this With expression will be selected and executed.</typeparam>
        /// <param name="func">The function which, if this With expression is Matched, will be invoked to produce the corresponding desired result for a Match with TPattern.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public Match<T, TResult> With<TPattern>(Func<TResult> func) where TPattern : class, ILexical {
            if (Value != null && !Accepted && Value is TPattern) {
                result = func();
                Accepted = true;
            }
            return this;
        }
        /// <summary>
        /// Appends a Match with Type expression to the current PatternMatching Expression.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. If the value being matched is of this type, this With expression will be selected and executed.</typeparam>
        /// <param name="func">The function which, if this With expression is Matched, will be invoked on the value being matched with to produce the desired result for a Match with TPattern.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns> 
        public Match<T, TResult> With<TPattern>(Func<TPattern, TResult> func) where TPattern : class, ILexical {
            if (!Accepted && Value != null) {
                var matched = Value as TPattern;
                if (matched != null) {
                    result = func(matched);
                    Accepted = true;
                }
            }
            return this;
        }
        /// <summary>
        /// Appends a Match with Type expression to the current PatternMatching Expression.
        /// </summary>
        /// <typeparam name="TPattern">The Type to match with. If the value being matched is of this type, this With expression will be selected and executed.</typeparam>
        /// <param name="result">The value which, if this With expression is Matched, will be the result of the Pattern Match.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public Match<T, TResult> With<TPattern>(TResult result) where TPattern : class, ILexical {
            if (!Accepted && Value != null) {
                var matched = Value as TPattern;
                if (matched != null) {
                    this.result = result;
                    Accepted = true;
                }

            }
            return this;
        }
        #endregion

        #region Result Expressions
        /// <summary>
        /// Appends a Result Expression to the current pattern, thus specifying the default result to yield when no other patterns have been matched.
        /// Although not enforced by the compiler, Result should only be used as the last clause in the match expression, never in between With clauses.
        /// </summary>
        /// <param name="func">The factory function returning a desired default value.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns> 
        /// <remarks>Although not enforced by the compiler, Default should only be used as the last clause in the match expression, never in between With clauses.</remarks>
        public TResult Result(Func<TResult> func) {
            if (!Accepted) {
                result = func();
                Accepted = true;
            }
            return result;
        }
        /// <summary>
        /// Appends a Result Expression to the current pattern, thus specifying the default result to yield when no other patterns have been matched.
        /// Although not enforced by the compiler, Result should only be used as the last clause in the match expression, never in between With clauses.
        /// </summary>
        /// <param name="func">The factory function returning a desired default value.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public TResult Result(Func<T, TResult> func) {
            if (Value != null && !Accepted) {
                result = func(Value);
                Accepted = true;
            }
            return result;
        }
        /// <summary>
        /// Appends a Result Expression to the current pattern, thus specifying the default result to yield when no other patterns have been matched.
        /// Although not enforced by the compiler, Result should only be used as the last clause in the match expression, never in between With clauses.
        /// </summary>
        /// <param name="defaultValue">The desired default value.</param>
        /// <returns>The Match&lt;T, R&gt; describing the Match expression so far.</returns>
        public TResult Result(TResult defaultValue) {
            if (!Accepted) {
                result = defaultValue;
                Accepted = true;
            }
            return this.result;
        }
        /// <summary>
        /// Returns the result of the Pattern Matching expression. 
        /// The result will be one of 3 possibilities: 
        /// 1. The result specified for the first Match with Type expression which succeeded. 
        /// 2. If no matches succeeded, and a Default expression was provided, the result specified in the Default expression.
        /// 3. If no matches succeeded, and a Default expression was not provided, the default value for type the Result Type of the Match Expression.
        /// </summary>
        /// <returns>Returns the result of the Pattern Matching expression.</returns>
        public TResult Result() { return result; }

        #endregion

        #region Fields
        private TResult result = default(TResult);
        #endregion

        #region Operators


        //public static implicit operator bool(Match<T, TResult> match) { return match.Accepted; }


        #endregion


    }
    [DebuggerStepThrough]
    public class PredicatedMatch<T> : PredicatedMatchBase<T> where T : class, ILexical
    {

        public PredicatedMatch(bool accepted, Match<T> inner)
            : base(accepted) {
            this.inner = inner;
        }

        public Match<T> Then<TPattern>(Action action) where TPattern : class, ILexical {
            return Accepted ? inner.With<TPattern>(action) : inner;
        }

        public Match<T> Then<TPattern>(Action<TPattern> action) where TPattern : class, ILexical {
            return Accepted ? inner.With(action) : inner;
        }



        public Match<T> Then(Action action) {
            return Accepted ? inner.With<T>(action) : inner;
        }

        public Match<T> Then(Action<T> action) {
            return Accepted ? inner.With<T>(action) : inner;
        }
        #region Fields
        private Match<T> inner;
        #endregion
    }
    public abstract class PredicatedMatchBase<T> where T : class, ILexical
    {
        protected PredicatedMatchBase(bool accepted) { this.Accepted = accepted; }
        #region Fields
        protected bool Accepted { get; private set; }
        #endregion
    }
    [DebuggerStepThrough]
    public class PredicatedMatch<T, TResult> : PredicatedMatchBase<T> where T : class, ILexical
    {
        #region Constructors

        public PredicatedMatch(bool accepted, Match<T, TResult> inner)
            : base(accepted) {
            this.inner = inner;
        }
        #endregion

        #region Methods

        public Match<T, TResult> Then<TPattern>(TResult result)
             where TPattern : class, ILexical {
            return Accepted ? inner.With<TPattern>(result) : inner;
        }

        public Match<T, TResult> Then<TPattern>(Func<TResult> func)
            where TPattern : class, ILexical {
            return Accepted ? inner.With<TPattern>(func) : inner;
        }

        public Match<T, TResult> Then<TPattern>(Func<TPattern, TResult> func)
            where TPattern : class, ILexical {
            return Accepted ? inner.With<TPattern>(func) : inner;
        }


        public Match<T, TResult> Then(Func<T, TResult> func) {
            return Accepted ? inner.With<T>(func) : inner;
        }


        public Match<T, TResult> Then(TResult resultValue) {
            return Accepted ? inner.With<T>(resultValue) : inner;
        }

        public Match<T, TResult> Then(Func<TResult> func) {
            return Accepted ? inner.With<T>(func) : inner;
        }

        public TResult Result() {
            return inner.Result();
        }


        public TResult Result(TResult defaultValue) {
            return inner.Result(defaultValue);
        }

        public TResult Result(Func<TResult> func) {
            return inner.Result(func);
        }


        public TResult Result(Func<T, TResult> func) {
            return inner.Result(func);
        }
        #endregion
        #region Fields
        private Match<T, TResult> inner;
        #endregion

    }
    [DebuggerStepThrough]
    internal class Match
    {
        public static Match<T> Create<T>(T value) where T : class, ILexical {
            return new Match<T>(value);
        }
        public static Match<T, TResult> Create<T, TResult>(T value) where T : class, ILexical {
            return new Match<T, TResult>(value);
        }
        public static PredicatedMatch<T> Create<T>(bool accepted, Match<T> inner) where T : class, ILexical {
            return Match.Create(accepted, inner);
        }
        public static PredicatedMatch<T, TResult> Create<T, TResult>(bool accepted, Match<T, TResult> inner) where T : class, ILexical {
            return new PredicatedMatch<T, TResult>(accepted, inner);
        }
    }

}