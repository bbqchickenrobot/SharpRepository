using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// Code from: https://code.google.com/p/linq-for-odata/
namespace SharpRepository.ODataRepository.Linq
{
    public abstract class QueryProvider : IQueryProvider
    {
        public IQueryable<S> CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public abstract TResult Execute<TResult>(Expression expression);

        public object Execute(Expression expression)
        {
            return Execute<object>(expression);
        }
    }
}