using System.Linq.Expressions;

namespace SharpRepository.ODataRepository.Linq
{
    public static class Extensions
    {
        public static string GetCollectionName(this Expression expression)
        {
            return GetCollectionName(expression as MethodCallExpression);
        }

        public static string GetCollectionName(MethodCallExpression methodCallExpression)
        {
            var constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;

            if (constantExpression != null)
            {
                return GetCollectionName(constantExpression);
            }

            return GetCollectionName(methodCallExpression.Arguments[0] as MethodCallExpression);
        }

        public static string GetCollectionName(ConstantExpression constantExpression)
        {
            return (constantExpression.Value as IHasCollection).Collection;
        }


        public static bool IsCount(this Expression expression)
        {
            if (expression is MethodCallExpression)
                return ((MethodCallExpression)expression).Method.Name == "Count";

            return false;
        }

        public static bool IsEmptyCount(this Expression expression)
        {
            if (expression is MethodCallExpression)
                return IsCount(expression) && ((MethodCallExpression)(expression)).Arguments.Count == 1;

            return false;
        }

        public static bool IsSkip(this Expression expression)
        {
            return expression.ToString().Contains(".Skip(");
        } 
    }
}