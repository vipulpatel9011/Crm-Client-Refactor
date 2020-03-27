// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchAndList.cs" company="Aurea Software Gmbh">
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
//   Search and list related configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Saerch and List related configurations
    /// Coresponds to UPConfigSearchAndList CRM.Pad implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class SearchAndList : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchAndList"/> class.
        /// </summary>
        /// <param name="defarray">
        /// The defarray.
        /// </param>
        public SearchAndList(List<object> defarray)
        {
            if (defarray == null || defarray.Count < 7)
            {
                return;
            }

            this.UnitName = (string)defarray[0];
            this.InfoAreaId = (string)defarray[1];
            this.FieldGroupName = (string)defarray[2];
            this.HeaderGroupName = (string)defarray[3];
            this.MenuLabel = (string)defarray[4];
            this.DefaultAction = (string)defarray[5];
            this.FilterName = (string)defarray[6];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchAndList"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        public SearchAndList(FieldControl fieldControl)
        {
            this.UnitName = fieldControl.FieldGroupName;
            this.InfoAreaId = fieldControl.InfoAreaId;
            this.FieldGroupName = fieldControl.FieldGroupName;
            this.HeaderGroupName = fieldControl.InfoAreaId;
            this.MenuLabel = string.Empty;
            this.DefaultAction = string.Empty;
            this.FilterName = string.Empty;
        }

        /// <summary>
        /// Gets the default action.
        /// </summary>
        /// <value>
        /// The default action.
        /// </value>
        public string DefaultAction { get; private set; }

        /// <summary>
        /// Gets the name of the field group.
        /// </summary>
        /// <value>
        /// The name of the field group.
        /// </value>
        public string FieldGroupName { get; private set; }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName { get; private set; }

        /// <summary>
        /// Gets the name of the header group.
        /// </summary>
        /// <value>
        /// The name of the header group.
        /// </value>
        public string HeaderGroupName { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the menu label.
        /// </summary>
        /// <value>
        /// The menu label.
        /// </value>
        public string MenuLabel { get; private set; }
    }
}
