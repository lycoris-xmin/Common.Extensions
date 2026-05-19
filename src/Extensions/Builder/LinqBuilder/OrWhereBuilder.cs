using System.Linq.Expressions;

namespace Lycoris.Common.Extensions.Builder.LinqBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrWhereBuilder<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IOrWhereBuilder<T> Or(Expression<Func<T, bool>> condition);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQueryable<T> AsQueryable();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <summary>
    /// OR 条件构建器
    /// </summary>
    public class OrWhereBuilder<T> : IOrWhereBuilder<T>
    {
        private readonly IQueryable<T> _source;
        private readonly List<Expression<Func<T, bool>>> _expressions = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="first"></param>
        public OrWhereBuilder(IQueryable<T> source, Expression<Func<T, bool>> first)
        {
            _source = source;
            _expressions.Add(first);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IOrWhereBuilder<T> Or(Expression<Func<T, bool>> condition)
        {
            _expressions.Add(condition);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return BuildQuery().Where(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            var query = BuildQuery();
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> AsQueryable()
        {
            return BuildQuery();
        }

        /// <summary>
        /// 构建合并后的 IQueryable
        /// </summary>
        private IQueryable<T> BuildQuery()
        {
            if (_expressions.Count == 0)
                return _source;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? body = null;

            foreach (var expr in _expressions)
            {
                var replaced = ReplaceParameter(expr.Body, expr.Parameters[0], parameter);
                body = body == null ? replaced : Expression.OrElse(body, replaced);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(body!, parameter);
            return _source.Where(lambda);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="oldParam"></param>
        /// <param name="newParam"></param>
        /// <returns></returns>
        private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam, ParameterExpression newParam)
        {
            return new ReplaceExpressionVisitor(oldParam, newParam).Visit(body)!;
        }

        /// <summary>
        /// 
        /// </summary>
        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="oldParam"></param>
            /// <param name="newParam"></param>
            public ReplaceExpressionVisitor(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
