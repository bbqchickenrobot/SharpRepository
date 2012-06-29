using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SharpRepository.ODataRepository.Linq
{
    public class Query<T> : IOrderedQueryable<T>, IHasCollection
    {
        public string Collection { get; private set; }
        readonly QueryProvider _provider;
        readonly Expression _expression;

        public Query(QueryProvider provider, string collection)
        {
            Collection = collection;
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this._provider = provider;
            this._expression = Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this._provider = provider;
            this._expression = expression;
        }

        public Expression Expression
        {
            get { return this._expression; }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return this._provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var execute = _provider.Execute<IEnumerable<T>>(Expression);
            return execute.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IHasCollection
    {
        string Collection { get; }
    }
}