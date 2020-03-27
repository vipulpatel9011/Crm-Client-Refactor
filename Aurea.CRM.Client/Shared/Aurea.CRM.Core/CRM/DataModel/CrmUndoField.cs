// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmUndoField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The CRM Undo Field class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    /// <summary>
    /// Undo Field
    /// </summary>
    public class UPCRMUndoField
    {
        /// <summary>
        /// Gets the json array.
        /// </summary>
        /// <value>
        /// The json array.
        /// </value>
        public List<string> JsonArray
        {
            get
            {
                List<string> arr = new List<string> { this.FieldName, this.Value };

                if (this.OldValue != null)
                {
                    arr.Add(this.OldValue);
                }

                return arr;
            }
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public string OldValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMUndoField"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">The old value.</param>
        public UPCRMUndoField(string fieldName, string value, string oldValue)
        {
            this.FieldName = fieldName;
            this.Value = value;
            this.OldValue = oldValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMUndoField"/> class.
        /// </summary>
        /// <param name="jsonArray">The json array.</param>
        public UPCRMUndoField(List<object> jsonArray)
        {
            this.FieldName = jsonArray[0] as string;

            if (jsonArray[1] != null)
            {
                this.Value = jsonArray[1] as string;
            }

            if (jsonArray.Count > 2 && jsonArray[2] != null)
            {
                this.OldValue = jsonArray[2] as string;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.OldValue)
                ? $"{this.FieldName}:{this.Value} ({this.OldValue})"
                : $"{this.FieldName}:{this.Value}";
        }
    }
}
