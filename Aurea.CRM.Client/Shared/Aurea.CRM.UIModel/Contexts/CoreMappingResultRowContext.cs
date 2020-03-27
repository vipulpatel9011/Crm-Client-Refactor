// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreMappingResultRowContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The up core mapping result row context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// The up core mapping result row context.
    /// </summary>
    public class UPCoreMappingResultRowContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCoreMappingResultRowContext"/> class.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public UPCoreMappingResultRowContext(UPCRMResultRow row, UPCoreMappingResultContext context)
        {
            this.Row = row;
            this.Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCoreMappingResultRowContext"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public UPCoreMappingResultRowContext(UPCRMResult result, UPCoreMappingResultContext context)
            : this((UPCRMResultRow)result.ResultRowAtIndex(0), context)
        {
            this.RetainedResult = result;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public UPCoreMappingResultContext Context { get; private set; }

        /// <summary>
        /// Gets the retained result.
        /// </summary>
        public UPCRMResult RetainedResult { get; private set; }

        /// <summary>
        /// Gets the row.
        /// </summary>
        public UPCRMResultRow Row { get; private set; }
    }
}
