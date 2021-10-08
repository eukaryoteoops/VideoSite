using System;
using System.Linq.Expressions;

namespace Comic.Common.ExtensionMethods
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> exp1,
            Expression<Func<T, bool>> exp2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(exp1.Parameters[0], parameter);
            var left = leftVisitor.Visit(exp1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(exp2.Parameters[0], parameter);
            var right = rightVisitor.Visit(exp2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }
    }

    public class ReplaceExpressionVisitor
        : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
