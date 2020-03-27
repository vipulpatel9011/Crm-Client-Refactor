// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchToFirstCoiIndex.cs" company="Aurea Software Gmbh">
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
//   SwitchToFirstCoiIndex
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Services.ModelControllers.Organizer;

    /// <summary>
    /// SwitchToFirstCoiIndex
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerViewSwitchToIndex" />
    public class SwitchToFirstCoiIndex : UPOrganizerViewSwitchToIndex
    {
        /// <summary>
        /// Index the of view to switch.
        /// </summary>
        /// <param name="pageModelControllers">The page model controllers.</param>
        /// <returns>Index of the view</returns>
        public int IndexOfViewToSwitch(List<UPPageModelController> pageModelControllers)
        {
            for (int index = 0; index < pageModelControllers.Count; index++)
            {
                if (pageModelControllers[index] is UPCoIBasePageModelController)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
