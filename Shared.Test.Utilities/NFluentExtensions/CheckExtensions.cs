using System;
using NFluent;
using NFluent.Helpers;
using NFluent.Extensions;
using NFluent.Extensibility;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Shared.Test.NFluentExtensions
{
    public static class NFluent
    {
        public static ICheckLink<ICheck<T>> Satisfies<T>(this ICheck<T> check, Func<T, bool> requirement)
        {
            // Every check method starts by extracting a checker instance from the check thanks to
            // the ExtensibilityHelper static class.
            var checker = ExtensibilityHelper.ExtractChecker(check);

            // Then, we let the checker's ExecuteCheck() method return the ICheckLink<ICheck<T>> result (with T as string here).
            // This method needs 2 arguments:
            //   1- a lambda that checks what's necessary, and throws a FluentAssertionException in case of failure
            //      The exception message is usually fluently build with the FluentMessage.BuildMessage() static method.
            //
            //   2- a string containing the message for the exception to be thrown by the checker when 
            //      the check fails, in the case we were running the negated version.
            //
            // e.g.:
            string requirementName = GetNominalInfo(requirement);
            return checker.ExecuteCheck(
                () =>
                {
                    if (!requirement(checker.Value))
                    {
                        var errorMessage = FluentMessage.BuildMessage($"The {{0}} does not satisfy the requirement{requirementName}.").For(typeof(T).GetType().Name).On(checker.Value).ToString();

                        throw new FluentCheckException(errorMessage);
                    }
                },
                FluentMessage.BuildMessage($"The {{0}} satisifies the requirement {requirementName} whereas it must not.").For(typeof(T).GetType().Name).On(checker.Value).ToString());
        }

        public static ICheckLink<ICheck<T>> DoesNotSatisfy<T>(this ICheck<T> check, Func<T, bool> requirement)
        {
            var checker = ExtensibilityHelper.ExtractChecker(check);

            string requirementName = GetNominalInfo(requirement);
            return checker.ExecuteCheck(
                () =>
                {
                    if (requirement(checker.Value))
                    {
                        var errorMessage = FluentMessage.BuildMessage($"The {{0}} satisfies the requirement {requirementName} whereas it must not.").For(typeof(T).Name).On(checker.Value).ToString();

                        throw new FluentCheckException(errorMessage);
                    }
                },
                FluentMessage.BuildMessage($"The {{0}} does not satisfy the requirement {requirementName}.").For(typeof(T).Name).On(checker.Value).ToString());
        }

        public static ICheckLink<ICheck<T>> IsSameReferenceAs<T>(this ICheck<T> check, T expected) => check.IsSameReferenceThan(expected);

        public static ICheckLink<ICheck<IEnumerable<T>>> StartsWith<T>(this ICheck<IEnumerable<T>> check, params T[] expectedValues)
        {
            if (expectedValues == null)
            {
                throw new FluentCheckException($"Cannot check against a null sequence of {nameof(expectedValues)}.");
            }
            var checker = ExtensibilityHelper.ExtractChecker(check);
            var actual = checker.Value.Cast<T>().Take(expectedValues.Length).ToList();
            return checker.ExecuteCheck(() =>
            {
                if (actual.Count < expectedValues.Length || !actual.Zip(expectedValues, EqualityComparer<T>.Default.Equals).All(x => x))
                {
                    var message = FluentMessage.BuildMessage($"The {{0}} does not start with:\n[ {string.Join(", ", expectedValues)} ]\nit starts with:\n[ {string.Join(", ", actual)} ]").For(typeof(IEnumerable<T>).Name).On(checker.Value).ToString();
                    throw new FluentCheckException(message);
                }
            },
            FluentMessage.BuildMessage($"The {{0}} starts with:\n[ {string.Join(", ", expectedValues)} ]\nit whereas it must not.").For(typeof(IEnumerable<T>).Name).On(checker.Value).ToString());
        }
        public static ICheckLink<ICheck<IEnumerable<T>>> EndsWith<T>(this ICheck<IEnumerable<T>> check, params T[] expectedValues)
        {
            if (expectedValues == null)
            {
                throw new FluentCheckException($"Cannot check against a null sequence of {nameof(expectedValues)}.");
            }
            var checker = ExtensibilityHelper.ExtractChecker(check);
            var actual = checker.Value.Cast<T>().Reverse().Take(expectedValues.Length).ToList();
            return checker.ExecuteCheck(() =>
            {
                if (actual.Count < expectedValues.Length || !actual.Zip(expectedValues.Reverse(), EqualityComparer<T>.Default.Equals).All(x => x))
                {
                    var message = FluentMessage.BuildMessage($"The {{0}} does not end with:\n[ {string.Join(", ", expectedValues)} ]\nit ends with:\n[ {string.Join(", ", actual)} ]").For(typeof(IEnumerable<T>).Name).On(checker.Value).ToString();
                    throw new FluentCheckException(message);
                }
            },
            FluentMessage.BuildMessage($"The {{0}} ends with:\n[ {string.Join(", ", expectedValues)} ]\nit whereas it must not.").For(typeof(IEnumerable<T>).Name).On(checker.Value).ToString());
        }

        #region Helpers

        private static string GetNominalInfo<T>(Func<T, bool> requirement) => $"{requirement.Method.DeclaringType.Name}.{requirement.Method.Name}";

        #endregion
    }
}