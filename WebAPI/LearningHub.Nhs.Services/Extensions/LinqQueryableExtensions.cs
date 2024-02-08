namespace LearningHub.Nhs.Services.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The linq queryable extensions.
    /// </summary>
    public static class LinqQueryableExtensions
    {
        /// <summary>
        /// Modifies the query to use LIKE where the Contains method is used.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        public static IQueryable<T> ContainsWithLikeQuery<T>(this IQueryable<T> query)
        {
            return query.Provider.CreateQuery<T>(
                new ContainsWithLikeQueryVisitor().Visit(query.Expression));
        }

        /// <summary>
        /// The fix query visitor.
        /// </summary>
        internal class ContainsWithLikeQueryVisitor : ExpressionVisitor
        {
            private readonly MethodInfo likeMethod = ExtractMethod(() => EF.Functions.Like(string.Empty, string.Empty));
            private readonly MethodInfo stringConcatMethod = ExtractMethod(() => string.Concat(string.Empty, string.Empty, string.Empty));
            private readonly Expression percentageExpression = Expression.Constant("%");

            /// <summary>
            /// The VisitMethodCall.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <returns>The expression.</returns>
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "Contains")
                {
                    var arg = node.Arguments[0];
                    var modifiedArg = Expression.Call(
                        this.stringConcatMethod,
                        this.percentageExpression,
                        arg,
                        this.percentageExpression);
                    return Expression.Call(
                        this.likeMethod,
                        Expression.Constant(EF.Functions),
                        node.Object,
                        modifiedArg);
                }

                return base.VisitMethodCall(node);
            }

            private static MethodInfo ExtractMethod(Expression<Action> expr)
            {
                MethodCallExpression body = (MethodCallExpression)expr.Body;

                return body.Method;
            }
        }
    }
}
