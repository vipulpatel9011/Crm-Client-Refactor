// <copyright file="NumberExtensions.cs" company="Aurea Software Gmbh">
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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Number extensions
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Remaps a double range to a new range.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="from1">
        /// The from 1.
        /// </param>
        /// <param name="to1">
        /// The to 1.
        /// </param>
        /// <param name="from2">
        /// The from 2.
        /// </param>
        /// <param name="to2">
        /// The to 2.
        /// </param>
        /// <returns>
        /// Returns methods original return value
        /// </returns>
        public static double Remap(this double value, double from1, double to1, double from2, double to2)
        {
            return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
        }
    }
}
