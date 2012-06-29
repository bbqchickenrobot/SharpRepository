using System;
using System.Linq.Expressions;

namespace SharpRepository.ODataRepository.Linq
{
    public class ODataExpressionVisitor : ExpressionVisitor
    {
        private bool _isCount;
        private string _filter = string.Empty;

        public string Parse(Expression expression)
        {
            Visit(expression);

            var result = string.Empty;

            if (_isCount)
                result += "/$count";

            if (!string.IsNullOrWhiteSpace(_filter))
                result = result.AddParameter("$filter=" + _filter);

            if (!_isCount)
                result = result.AddParameter("$format=json");

            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "First":
                case "FirstOrDefault":
                    throw new NotSupportedException();

                case "OrderBy":
                case "OrderByDescending":
                    throw new NotSupportedException();

                case "Count":
                    _isCount = true;
                    break;

                case "GroupBy":
                case "StartsWith":
                case "Contains":
                case "Where":
                    SetWhereQuery(m);
                    break;

                case "Skip":
                    throw new NotSupportedException();

                case "Take":
                    throw new NotSupportedException();

                case "Max":
                case "Min":
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }

            return base.VisitMethodCall(m);
        }

        private void SetWhereQuery(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
                return;

            Expression arg = m.Arguments[1];
            var unExpr = arg as UnaryExpression;
            var op = unExpr.Operand as LambdaExpression;

            AppendFilter(CreateOperand(op.Body));
        }

        private void AppendFilter(string param)
        {
            if (string.IsNullOrWhiteSpace(_filter))
            {
                _filter = param;
                return;
            }

            throw new NotImplementedException();
        }

        private string CreateOperand(Expression expression)
        {
            var binary = expression as BinaryExpression;

            if (binary != null)
            {
                return CreateOdataOperand(binary);
            }

            var methodCall = expression as MethodCallExpression;

            if (methodCall != null)
            {
                return CreateOdataOperand(methodCall);
            }

            throw new NotImplementedException();
        }

        private string CreateOdataOperand(BinaryExpression body)
        {
            switch (body.NodeType)
            {
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    return DynamicUnary(body.NodeType, body.Left as MemberExpression, body.Right);
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    throw new ArgumentOutOfRangeException();
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string DynamicUnary(ExpressionType nodeType, MemberExpression left, Expression right)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return string.Format("{0} eq {1}", left.Member.Name, GetValue(right));

                case ExpressionType.GreaterThan:
                    return string.Format("{0} gt {1}", left.Member.Name, GetValue(right));


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object GetValue(Expression memberExpression)
        {
            return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
        }


        private string CreateOdataOperand(MethodCallExpression methodCall)
        {
            switch (methodCall.Method.Name)
            {
                case "StartsWith":
                    return DynamicUnary("startswith", methodCall.Object as MemberExpression, methodCall.Arguments[0]);


                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private string DynamicUnary(string name, MemberExpression left, Expression right)
        {
            return string.Format("{0}({1},'{2}')", name, left.Member.Name, GetValue(right));
        }
    }
}