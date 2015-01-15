﻿namespace LASI.UnitTests
{
    public sealed class ExpectedFileTypeWrapperMismatchExceptionAttribute : Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedExceptionBaseAttribute
    {
        protected override void Verify(System.Exception exception) {
            RethrowIfAssertException(exception);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(exception, typeof(Content.FileTypeWrapperMismatchException));
        }
    }
}