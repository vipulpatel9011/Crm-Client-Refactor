// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTableCaptionSpecialDefs.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   table caption special configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// table caption special configurations
    /// </summary>
    public class UPConfigTableCaptionSpecialDefs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigTableCaptionSpecialDefs"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigTableCaptionSpecialDefs(List<object> definition)
        {
            this.EmptyFieldArray = (definition[0] as JArray)?.ToObject<List<object>>();
            this.FormatString = definition[1] as string;
        }

        /// <summary>
        /// Gets the empty field array.
        /// </summary>
        /// <value>
        /// The empty field array.
        /// </value>
        public List<object> EmptyFieldArray { get; private set; }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>
        /// The format string.
        /// </value>
        public string FormatString { get; private set; }

        /// <summary>
        /// Alls the array fields are empty.
        /// </summary>
        /// <param name="fieldValues">
        /// The field values.
        /// </param>
        /// <returns>
        /// true if all fields are empty; else false
        /// </returns>
        public bool AllArrayFieldsAreEmpty(List<string> fieldValues)
        {
            foreach (var n in this.EmptyFieldArray)
            {
                var num = JObjectExtensions.ToInt(n);
                if (fieldValues.Count > num && !string.IsNullOrEmpty(fieldValues[num]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
