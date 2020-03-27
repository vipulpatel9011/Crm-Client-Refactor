// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPExpandedRowContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The up expanded row context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    /// <summary>
    /// The up expanded row context.
    /// </summary>
    public class UPExpandedRowContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPExpandedRowContext"/> class.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        public UPExpandedRowContext(UPMiniDetailsResultRow resultRow)
        {
            this.ResultRow = resultRow;
        }

        /// <summary>
        /// Gets or sets the favorite record identification.
        /// </summary>
        public string FavoriteRecordIdentification { get; set; }

        /// <summary>
        /// Gets the result row.
        /// </summary>
        public UPMiniDetailsResultRow ResultRow { get; private set; }
    }
}
