// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGUIPage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The GUIPage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.UIControlInterfaces
{
    /// <summary>
    /// The GUIPage interface.
    /// </summary>
    public interface IGUIPage : IGUIContainer
    {
        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        string LabelText { get; set; }
    }
}
