// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepPossibleValue.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm rep possible value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// The upm rep possible value.
    /// </summary>
    public class UPMRepPossibleValue
    {
        /// <summary>
        /// Gets or sets the detail label field.
        /// </summary>
        public UPMStringField DetailLabelField { get; set; }

        /// <summary>
        /// Gets or sets the group reps.
        /// </summary>
        public List<UPMRepPossibleValue> GroupReps { get; set; }

        /// <summary>
        /// Gets or sets the image string.
        /// </summary>
        public string ImageString { get; set; }

        /// <summary>
        /// Gets or sets the indicator color.
        /// </summary>
        public AureaColor IndicatorColor { get; set; }

        /// <summary>
        /// Gets or sets the rep id.
        /// </summary>
        public string RepId { get; set; }

        /// <summary>
        /// Gets or sets the rep org group id.
        /// </summary>
        public string RepOrgGroupId { get; set; }

        /// <summary>
        /// Gets or sets the title label 2 field.
        /// </summary>
        public UPMStringField TitleLabel2Field { get; set; }

        /// <summary>
        /// Gets or sets the title label field.
        /// </summary>
        public UPMStringField TitleLabelField { get; set; }

        /// <summary>
        /// The matches string.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MatchesString(string str)
        {
            if (this.TitleLabelField.StringValue == str)
            {
                return true;
            }

            if (this.TitleLabel2Field.StringValue == str)
            {
                return true;
            }

            if (this.DetailLabelField.StringValue == str)
            {
                return true;
            }

            return false;
        }
    }
}
