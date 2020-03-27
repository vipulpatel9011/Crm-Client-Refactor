// <copyright file="UPMGridCategory.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.UIModel.Pages
{
    /// <summary>
    /// Implementation of grid category
    /// </summary>
    public class UPMGridCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridCategory"/> class.
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="action">Action</param>
        /// <param name="isCurrentCategory">Is current category</param>
        public UPMGridCategory(string label, UPMAction action, bool isCurrentCategory)
        {
            this.Label = label;
            this.Action = action;
            this.IsCurrentCategory = isCurrentCategory;
        }

        /// <summary>
        /// Gets action
        /// </summary>
        public UPMAction Action { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is current category
        /// </summary>
        public bool IsCurrentCategory { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }
    }
}
