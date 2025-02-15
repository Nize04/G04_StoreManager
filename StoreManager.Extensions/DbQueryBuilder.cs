using System.Linq.Expressions;

namespace StoreManager.Extensions
{
    public static class DbQueryBuilder
    {
        public static string GetWhereDbQuery<T>(this Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return GetDbQuery(predicate.Body);
        }

        private static string GetDbQuery(Expression exp, bool isRight = false)
        {
            if (exp is MemberExpression memberExp && !isRight) return memberExp.Member.Name;

            if (exp is ConstantExpression constantExp && !isRight) return GetDbValue(constantExp.Value!);

            if (exp is UnaryExpression unaryExp) return GetDbQuery(unaryExp.Operand);

            if (exp is BinaryExpression binaryExp) return ProcessBinary(binaryExp);

            if (exp is MemberExpression variableExp && isRight) return GetDbValue(Expression.Lambda(variableExp).Compile().DynamicInvoke()!);

            if (exp is ConstantExpression variableConstExp && isRight) return GetDbValue(variableConstExp.Value!);

            throw new NotSupportedException();
        }

        private static string GetDbOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool IsNullConstant(Expression exp) => exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null;

        private static string ProcessAndAlsoQuery(BinaryExpression andAlsoExp)
        {
            var left = GetDbQuery(andAlsoExp.Left);
            var right = GetDbQuery(andAlsoExp.Right, true);
            return $"({left} AND {right})";
        }

        private static string ProcessOrElseQuery(BinaryExpression orElseExp)
        {
            var left = GetDbQuery(orElseExp.Left);
            var right = GetDbQuery(orElseExp.Right, true);
            return $"({left} OR {right})";
        }

        private static string ProcessBinary(BinaryExpression binaryExp, string op)
        {
            var left = GetDbQuery(binaryExp.Left);
            var right = GetDbQuery(binaryExp.Right, true);
            return $"({left} {op} {right})";
        }

        private static string ProcessBinary(BinaryExpression binaryExp)
        {
            bool isNull = IsNullConstant(binaryExp.Right);

            if (binaryExp.NodeType == ExpressionType.Equal && isNull) return $"{GetDbQuery(binaryExp.Left)} IS NULL";

            if (binaryExp.NodeType == ExpressionType.NotEqual && isNull) return $"{GetDbQuery(binaryExp.Left)} IS NOT NULL";

            if (binaryExp.NodeType == ExpressionType.AndAlso) return ProcessAndAlsoQuery(binaryExp);

            if (binaryExp.NodeType == ExpressionType.OrElse) return ProcessOrElseQuery(binaryExp);

            return ProcessBinary(binaryExp, GetDbOperator(binaryExp.NodeType));
        }

        private static string GetDbValue(object value)
        {
            if (value == null) return "NULL";

            if (value is string) return $"'{value}'";

            if (value is bool) return (bool)value ? "1" : "0";

            if (value is DateTime) return $"'{(DateTime)value}'";

            return value.ToString()!;
        }
    }
}