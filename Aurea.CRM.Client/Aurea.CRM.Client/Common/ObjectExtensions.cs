// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Object extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns true if object is a subclass of type
        /// </summary>
        /// <param name="obj">object to check</param>
        /// <param name="typeToCheck">type to check</param>
        /// <returns>value indicating if object is subclass of type</returns>
        public static bool IsSubclass(this object obj, Type typeToCheck)
        {
            if (obj == null || typeToCheck == null)
            {
                return false;
            }

            return obj.GetType().GetTypeInfo().IsSubclassOf(typeToCheck);
        }
    }
}
