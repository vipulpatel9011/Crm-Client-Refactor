// <copyright file="ISwipePageDataSourceController.cs" company="Aurea Software Gmbh">
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

    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;

    /// <summary>
    /// The SwipePageDataSourceController interface.
    /// </summary>
    public interface ISwipePageDataSourceController
    {
        /// <summary>
        /// The load table captions from index to index.
        /// </summary>
        /// <param name="fromIndex">
        /// The from index.
        /// </param>
        /// <param name="toIndex">
        /// The to index.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<UPSwipePageRecordItem> LoadTableCaptionsFromIndexToIndex(int fromIndex, int toIndex);

        /// <summary>
        /// The detail organizer for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="onlineData">
        /// The online data.
        /// </param>
        /// <returns>
        /// The <see cref="UPOrganizerModelController"/>.
        /// </returns>
        UPOrganizerModelController DetailOrganizerForRecordIdentification(string recordIdentification, bool onlineData);
    }
}
