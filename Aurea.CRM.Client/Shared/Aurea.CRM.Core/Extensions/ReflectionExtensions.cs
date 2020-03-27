// <copyright file="ReflectionExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The reflection extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Finds a method of an object using reflection and invokes if it exists.
        /// </summary>
        /// <param name="objectToInvoke">Object to be invoked</param>
        /// <param name="methodName">Method name to invoke</param>
        /// <param name="args">Parameters to apply</param>
        /// <returns>Returns methods original return value</returns>
        public static object FindAndInvokeMethod(this object objectToInvoke, string methodName, object[] args)
        {
            var methodInfo = objectToInvoke?.GetType().GetTypeInfo().GetDeclaredMethod(methodName);

            if (methodInfo == null)
            {
                return null;
            }

            var arguments = new List<Expression>();
            if (args?.Length > 0)
            {
                foreach (var arg in args)
                {
                    arguments.Add(Expression.Constant(arg));
                }
            }

            var input = Expression.Parameter(typeof(object), "input");

            Func<object, object> result = Expression.Lambda<Func<object, object>>(
                Expression.Call(Expression.Convert(input, objectToInvoke.GetType()), methodInfo, arguments), input).Compile();

            return result(objectToInvoke);
        }

        /// <summary>
        /// Checks if an object has a method by given name
        /// </summary>
        /// <param name="objectToCheck">Object to be checked</param>
        /// <param name="methodName">Method name to find</param>
        /// <returns>Returns true if method exists</returns>
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            var methodInfo = objectToCheck?.GetType().GetTypeInfo().GetDeclaredMethod(methodName);
            return methodInfo != null;
        }
    }
}
