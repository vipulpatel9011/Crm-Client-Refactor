// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebContentModelControllerUIDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The Web Content Model Controller UI Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    /// <summary>
    /// Web Content Model Controller UI Delegate
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.Delegates.IModelControllerUIDelegate" />
    public interface IWebContentModelControllerUIDelegate : IModelControllerUIDelegate
    {
        /// <summary>
        /// Shows the full screen.
        /// </summary>
        void ShowFullScreen();
    }
}
