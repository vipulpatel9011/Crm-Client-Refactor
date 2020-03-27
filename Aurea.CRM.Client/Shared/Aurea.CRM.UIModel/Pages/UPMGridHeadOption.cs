// <copyright file="UPMGridHeadOption.cs" company="Aurea Software Gmbh">
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
    /// Implementation of grid head option
    /// </summary>
    public class UPMGridHeadOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridHeadOption"/> class.
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="action">Action</param>
        public UPMGridHeadOption(string label, UPMAction action)
        {
            this.Action = action;
            this.Label = label;
        }

        /// <summary>
        /// Gets action
        /// </summary>
        public UPMAction Action { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether has reset visible text
        /// </summary>
        public bool HasResetVisibleText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is reset
        /// </summary>
        public bool IsReset { get; set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }
    }
}
