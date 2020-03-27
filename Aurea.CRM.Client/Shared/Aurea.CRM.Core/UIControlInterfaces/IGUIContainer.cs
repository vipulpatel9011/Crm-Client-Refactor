// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGUIContainer.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The GUIContainer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.UIControlInterfaces
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The GUIContainer interface.
    /// </summary>
    public interface IGUIContainer
    {
        /// <summary>
        /// The children added.
        /// </summary>
        /// <param name="addedChild">
        /// The added child.
        /// </param>
        void ChildrenAdded(IUPMElement addedChild);

        /// <summary>
        /// The children added at index.
        /// </summary>
        /// <param name="addedChild">
        /// The added child.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        void ChildrenAddedAtIndex(IUPMElement addedChild, int index);

        /// <summary>
        /// The children cleared.
        /// </summary>
        void ChildrenCleared();

        /// <summary>
        /// The children removed.
        /// </summary>
        /// <param name="removedChild">
        /// The removed child.
        /// </param>
        void ChildrenRemoved(IUPMElement removedChild);
    }
}
