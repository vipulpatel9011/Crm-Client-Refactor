// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildMultiMapGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The ChildMultiMapGroupModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Services.Delegates;

    /// <summary>
    /// ChildMultiMapGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.ChildListGroupModelController" />
    public class ChildMultiMapGroupModelController : ChildListGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildListGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public ChildMultiMapGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <summary>
        /// Creates the child controller.
        /// </summary>
        /// <param name="swipeDetailRecords">if set to <c>true</c> [swipe detail records].</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <returns></returns>
        public UPListResultGroupModelController CreateChildController(bool swipeDetailRecords, int linkId, string searchAndListConfigurationName)
        {
            return new UPMultiMapGroupModelController(searchAndListConfigurationName, linkId, swipeDetailRecords, this.ListStyle, this.DisablePaging, this);
        }
    }
}
