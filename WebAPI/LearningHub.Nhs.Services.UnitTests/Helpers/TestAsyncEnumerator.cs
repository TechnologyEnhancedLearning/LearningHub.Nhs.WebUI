// <copyright file="TestAsyncEnumerator.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services.UnitTests.Helpers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The test async enumerator.
    /// </summary>
    /// <typeparam name="T">Input type.</typeparam>
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        /// <summary>
        /// The inner.
        /// </summary>
        private readonly IEnumerator<T> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAsyncEnumerator{T}"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public T Current
        {
            get
            {
                return this.inner.Current;
            }
        }

        /// <summary>Advances the enumerator asynchronously to the next element of the collection.</summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.ValueTask`1" /> that will complete with a result of <c>true</c> if the enumerator
        /// was successfully advanced to the next element, or <c>false</c> if the enumerator has passed the end
        /// of the collection.</returns>
        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(Task.FromResult(this.inner.MoveNext()));
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.inner.Dispose();
        }

        /// <summary>
        /// The move next.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.inner.MoveNext());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>The <see cref="ValueTask"/>.</returns>
        public ValueTask DisposeAsync()
        {
            this.inner.Dispose();
            return new ValueTask(Task.CompletedTask);
        }
    }
}
