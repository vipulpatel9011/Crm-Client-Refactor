// <copyright file="ISearchViewControllerDataProvider.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.Delegates
{
    using System.Collections.Generic;

    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// The ISearchViewControllerDataProvider interface.
    /// </summary>
    public interface ISearchViewControllerDataProvider
    {
        /// <summary>
        /// Gets the search page.
        /// </summary>
        UPMSearchPage SearchPage { get; }

        /// <summary>
        /// The updated element.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="UPMElement"/>.
        /// </returns>
        UPMElement UpdatedElement(UPMElement element);

        /// <summary>
        /// The update groups for result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        void UpdateGroupsForResultRow(UPMResultRow resultRow);

        /// <summary>
        /// The update result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        void UpdateResultRow(UPMResultRow resultRow);

        /// <summary>
        /// The update expanded result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        void UpdateExpandedResultRow(UPMResultRow resultRow);

        /// <summary>
        /// The find quick actions for row check details.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <param name="checkDetails">
        /// The check details.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<UPMAction> FindQuickActionsForRowCheckDetails(UPMResultRow resultRow, bool checkDetails);

        /// <summary>
        /// The has gps field values.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool HasGPSFieldValues(UPMResultRow resultRow);
    }
}
