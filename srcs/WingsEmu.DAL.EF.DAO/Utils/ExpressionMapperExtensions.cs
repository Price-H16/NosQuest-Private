// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Linq.Expressions;

namespace WingsEmu.DAL.EF.DAO.Utils
{
    public static class ExpressionMapperExtensions
    {
        public static Expression<Func<TTarget, bool>> Convert<TSource, TTarget>(this Expression<Func<TSource, bool>> root)
        {
            ParameterTypeVisitor<TSource, TTarget> visitor = new ParameterTypeVisitor<TSource, TTarget>();
            Expression<Func<TTarget, bool>> expression = (Expression<Func<TTarget, bool>>)visitor.Visit(root);
            return expression;
        }

        public static Expression<Func<TTarget, bool>> Convert<TSource, TTarget>(this Func<TSource, bool> root)
        {
            Expression<Func<TSource, bool>> tmp = target => root(target);
            return tmp.Convert<TSource, TTarget>();
        }
    }
}