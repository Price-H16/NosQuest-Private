﻿// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenNos.Core
{
    public class DelegateBuilder
    {
        #region Methods

        public static T BuildDelegate<T>(MethodInfo method, params object[] missingParamValues)
        {
            Queue<object> queueMissingParams = new Queue<object>(missingParamValues);

            MethodInfo dgtMi = typeof(T).GetMethod("Invoke");
            ParameterInfo[] dgtParams = dgtMi.GetParameters();

            ParameterExpression[] paramsOfDelegate = dgtParams
                .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
                .ToArray();

            ParameterInfo[] methodParams = method.GetParameters();

            if (method.IsStatic)
            {
                Expression[] paramsToPass = methodParams
                    .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
                    .ToArray();

                Expression<T> expr = Expression.Lambda<T>(
                    Expression.Call(method, paramsToPass),
                    paramsOfDelegate);

                return expr.Compile();
            }
            else
            {
                UnaryExpression paramThis = Expression.Convert(paramsOfDelegate[0], method.DeclaringType);

                Expression[] paramsToPass = methodParams
                    .Select((p, i) => CreateParam(paramsOfDelegate, i + 1, p, queueMissingParams))
                    .ToArray();

                Expression<T> expr = Expression.Lambda<T>(
                    Expression.Call(paramThis, method, paramsToPass),
                    paramsOfDelegate);

                return expr.Compile();
            }
        }

        private static Expression CreateParam(IReadOnlyList<ParameterExpression> paramsOfDelegate, int i, ParameterInfo callParamType, Queue<object> queueMissingParams)
        {
            if (i < paramsOfDelegate.Count)
            {
                return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);
            }

            return queueMissingParams.Any()
                ? Expression.Constant(queueMissingParams.Dequeue())
                : Expression.Constant(callParamType.ParameterType.IsValueType ? Activator.CreateInstance(callParamType.ParameterType) : null);
        }

        #endregion
    }
}