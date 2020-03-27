// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmServerResultPointer.cs" company="Aurea Software Gmbh">
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
//   CRM server result pointer
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.DataModel
{
    /// <summary>
    /// CRM server result pointer
    /// </summary>
    public class UPCRMServerResultPointer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMServerResultPointer"/> class.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="recordIndex">
        /// Index of the record.
        /// </param>
        public UPCRMServerResultPointer(int fieldIndex, int recordIndex)
        {
            this.FieldIndex = fieldIndex;
            this.RecordIndex = recordIndex;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; }

        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>
        /// The index of the record.
        /// </value>
        public int RecordIndex { get; }
    }
}
