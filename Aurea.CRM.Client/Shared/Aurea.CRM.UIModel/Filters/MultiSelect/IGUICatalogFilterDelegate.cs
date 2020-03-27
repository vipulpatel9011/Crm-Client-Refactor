// <copyright file="IGUICatalogFilterDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel.Filters.MultiSelect
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// UI Model Class
    /// </summary>
    public interface IGUICatalogFilterDelegate
    {
        /// <summary>
        /// Gets gets or sets the items.
        /// </summary>
        ObservableCollection<SelectableItem> Items { get; }
    }
}
