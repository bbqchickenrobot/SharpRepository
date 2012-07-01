using System;
using System.Linq.Expressions;

namespace SharpRepository.ODataRepository.Linq
{
    public class ODataExpressionVisitor : ExpressionVisitor
    {
        private bool _isCount;
        private string _filter = string.Empty;
        private string _orderBy = string.Empty;
        private int? _skip;
        private int? _take;
        private bool _isFirst = false;

        public string Parse(Expression expression)
        {
            Visit(expression);

            var result = string.Empty;

            if (_isCount)
                result += "/$count";

            if (!string.IsNullOrWhiteSpace(_filter))
                result = result.AddParameter("$filter=" + _filter);

            if (!string.IsNullOrWhiteSpace(_orderBy))
                result = result.AddParameter("$orderby=" + _orderBy);

            if (_skip.HasValue)
                result = result.AddParameter("$skip=" + _skip.Value);

            if (_take.HasValue)
                result = result.AddParameter("$top=" + _take.Value);

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
                    _isFirst = true;
                    break;

                case "OrderBy":
                case "OrderByDescending":
                    SetOrderBy(m, m.Method.Name.Contains("Descending"));
                    break;

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
                    SetSkipQuery(m);
                    break;

                case "Take":
                    SetTakeQuery(m);
                    break;

                case "Max":
                case "Min":
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }

            return base.VisitMethodCall(m);
        }

        private void SetSkipQuery(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
                return;

            var arg = m.Arguments[1];

            if (arg.NodeType == ExpressionType.Constant)
            {
                _skip = (int) ((ConstantExpression) arg).Value;
            }

            // TODO: handle variable expression
        }

        private void SetTakeQuery(MethodCallExpression m)
        {
            if (m.Arguments.Count == 1)
                return;

            var arg = m.Arguments[1];

            if (arg.NodeType == ExpressionType.Constant)
            {
                _take = (int)((ConstantExpression)arg).Value;
            }
        }

        private void SetOrderBy(MethodCallExpression m, bool isDescending)
        {
            if (m.Arguments.Count == 1)
                return;

            var arg = m.Arguments[1];

            var unExpr = arg as UnaryExpression;
            var op = unExpr.Operand as LambdaExpression;
            var prop = op.Body as MemberExpression;

            _orderBy = prop.Member.Name + (isDescending ? " desc" : " asc");
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
            }
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
            var left = body.Left as MemberExpression;
            var right = body.Right;

            switch (body.NodeType)
            {
                case ExpressionType.Equal:
                    return string.Format("{0} eq {1}", left.Member.Name, GetValue(right));

                case ExpressionType.NotEqual:
                    return string.Format("{0} ne {1}", left.Member.Name, GetValue(right));

                case ExpressionType.GreaterThan:
                    return string.Format("{0} gt {1}", left.Member.Name, GetValue(right));

                case ExpressionType.GreaterThanOrEqual:
                    return string.Format("{0} ge {1}", left.Member.Name, GetValue(right));

                case ExpressionType.LessThan:
                    return string.Format("{0} lt {1}", left.Member.Name, GetValue(right));

                case ExpressionType.LessThanOrEqual:
                    return string.Format("{0} le {1}", left.Member.Name, GetValue(right));

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return string.Format("{0} and {1}", left.Member.Name, GetValue(right));

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return string.Format("{0} or {1}", left.Member.Name, GetValue(right));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public string DynamicUnary(ExpressionType nodeType, MemberExpression left, Expression right)
        //{
            
        //}

        private static string CreateOdataOperand(MethodCallExpression methodCall)
        {
            switch (methodCall.Method.Name)
            {
                case "StartsWith":
                    return string.Format("{0}({1},'{2}')", "startswith", ((MemberExpression)methodCall.Object).Member.Name, GetValue(methodCall.Arguments[0]));

                case "EndsWith":
                    return string.Format("{0}({1},'{2}')", "endswith", ((MemberExpression)methodCall.Object).Member.Name, GetValue(methodCall.Arguments[0]));

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static object GetValue(Expression memberExpression)
        {
            return Expression.Lambda(memberExpression).Compile().DynamicInvoke();
        }

        //private string DynamicUnary(string name, MemberExpression left, Expression right)
        //{
        //    return string.Format("{0}({1},'{2}')", name, left.Member.Name, GetValue(right));
        //}
    }
}