// <copyright file="ISwipePageRecordController.cs" company="Aurea Software Gmbh">
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
    /// <summary>
    /// The SwipePageRecordController interface.
    /// </summary>
    public interface ISwipePageRecordController
    {
        /// <summary>
        /// Gets current index
        /// </summary>
        int CurrentIndex { get; }

        /// <summary>
        /// The switch to detail at index offset 
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="uiDelegate">
        /// The UI delegate.
        /// </param>
        void SwitchToIndex(int index, IModelControllerUIDelegate uiDelegate);
    }
}
