// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickSearchEntry.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Defines the quick search entry configurations
//   corresponds to UPConfigQuickSearchEntry in CRM.Pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Defines the quick search entry configurations
    /// corresponds to UPConfigQuickSearchEntry in CRM.Pad
    /// </summary>
    public class QuickSearchEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSearchEntry"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        public QuickSearchEntry(List<object> def)
        {
            this.InfoAreaId = (string)def[0];
            this.FieldId = JObjectExtensions.ToInt(def[1]);
            this.MenuName = (string)def[2];
            if (string.IsNullOrWhiteSpace(this.MenuName))
            {
                this.MenuName = "SHOWRECORD";
            }
        }

        /// <summary>
        /// Gets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public UPCRMField CrmField => UPCRMField.FieldWithFieldIdInfoAreaId(this.FieldId, this.InfoAreaId);

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the name of the menu.
        /// </summary>
        /// <value>
        /// The name of the menu.
        /// </value>
        public string MenuName { get; private set; }
    }
}
