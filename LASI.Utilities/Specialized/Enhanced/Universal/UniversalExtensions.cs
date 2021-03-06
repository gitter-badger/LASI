﻿using System.Collections.Generic;

namespace LASI.Utilities.Specialized.Enhanced.Universal
{
    /// <summary>
    /// Defines extension methods that apply to all types. Import with care.
    /// </summary>
    public static class UniversalExtensions
    {
        /// <summary>
        /// Lifts the given value into an enumerable. Returns a singleton sequence containing the
        /// single value unless it is <c>null</c> in which case an empty sequence will be produced.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value to lift and the return type of the resulting enumerable.
        /// </typeparam>
        /// <param name="value">The value to lift into an enumerable.</param>
        /// <returns>
        /// A singleton sequence containing the single value unless it is <c>null</c> in which
        /// case an empty sequence will be produced.
        /// </returns>
        public static IEnumerable<T> Lift<T>(this T value)
        {
            yield return value;
        }
    }
}