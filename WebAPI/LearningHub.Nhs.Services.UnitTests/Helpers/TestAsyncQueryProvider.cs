namespace LearningHub.Nhs.Services.UnitTests.Helpers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using Microsoft.EntityFrameworkCore.Query;

    /// <summary>
    /// The test async query provider.
    /// </summary>
    /// <typeparam name="TEntity">Inout type.</typeparam>
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        /// <summary>
        /// The inner.
        /// </summary>
        private readonly IQueryProvider inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAsyncQueryProvider{TEntity}"/> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            this.inner = inner;
        }

        /// <summary>
        /// The create query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            switch (expression)
            {
                case MethodCallExpression m:
                    {
                        var resultType = m.Method.ReturnType; // it shoud be IQueryable<T>
                        var tElement = resultType.GetGenericArguments()[0];
                        var queryType = typeof(TestAsyncEnumerable<>).MakeGenericType(tElement);
                        return (IQueryable)Activator.CreateInstance(queryType, expression);
                    }
            }

            return new TestAsyncEnumerable<TEntity>(expression);
        }

        /// <summary>
        /// The create query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <typeparam name="TElement">Input type.</typeparam>
        /// <returns>The <see cref="IQueryable"/>.</returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object Execute(Expression expression)
        {
            return this.inner.Execute(expression);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <typeparam name="TResult">Inout type.</typeparam>
        /// <returns>The TResult.</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            return this.inner.Execute<TResult>(expression);
        }

        /// <inheritdoc/>
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return this.Execute<TResult>(expression);
        }
    }
}
