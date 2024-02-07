// <copyright file="TestAsyncEnumerable.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;

    /// <summary>
    /// The test async enumerable.
    /// </summary>
    /// <typeparam name="T">Inout type.</typeparam>
    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAsyncEnumerable{T}"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAsyncEnumerable{T}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>The IAsyncEnumerator.</returns>
        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        /// <summary>Returns an enumerator that iterates asynchronously through the collection.</summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An enumerator that can be used to iterate asynchronously through the collection.</returns>
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return this.GetEnumerator();
        }
    }
}
