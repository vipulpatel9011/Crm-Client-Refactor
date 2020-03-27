// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedResultField.cs" company="Aurea Software Gmbh">
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
//   A combined result field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// A combined result field
    /// </summary>
    public class UPCombinedResultField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCombinedResultField"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        public UPCombinedResultField(FieldControl fieldControl, int position)
        {
            var first = true;
            var count = fieldControl?.NumberOfFields ?? 0;
            List<int> fieldArray = null;
            for (var i = 0; i < count; i++)
            {
                var field = fieldControl?.FieldAtIndex(i);
                if (field?.TargetFieldNumber != position)
                {
                    continue;
                }

                if (first)
                {
                    first = false;
                    this.FirstField = field;
                    this.Attributes = field.Attributes;
                }

                if (fieldArray == null)
                {
                    fieldArray = new List<int>();
                }

                fieldArray.Add(field.TabIndependentFieldIndex);
            }

            this.FieldIndices = fieldArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCombinedResultField"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public UPCombinedResultField(UPConfigFieldControlField field)
        {
            if (field == null)
            {
                return;
            }

            this.FieldIndices = new List<int> { field.TabIndependentFieldIndex };
            this.Attributes = field.Attributes;
            this.FirstField = field;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public FieldAttributes Attributes { get; private set; }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>
        /// The field count.
        /// </value>
        public int FieldCount => this.FieldIndices?.Count ?? 0;

        /// <summary>
        /// Gets the field indices.
        /// </summary>
        /// <value>
        /// The field indices.
        /// </value>
        public List<int> FieldIndices { get; private set; }

        /// <summary>
        /// Gets the first field.
        /// </summary>
        /// <value>
        /// The first field.
        /// </value>
        public UPConfigFieldControlField FirstField { get; private set; }
    }
}
