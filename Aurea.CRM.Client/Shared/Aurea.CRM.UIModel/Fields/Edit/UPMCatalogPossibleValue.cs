// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCatalogPossibleValue.cs" company="Aurea Software Gmbh">
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
//   The MCatalogEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Model the possible values for a catalog UI field
    /// </summary>
    public class UPMCatalogPossibleValue
    {
        /// <summary>
        /// Gets or sets the title label field.
        /// </summary>
        /// <value>
        /// The title label field.
        /// </value>
        public UPMStringField TitleLabelField { get; set; }

        /// <summary>
        /// Gets the title label fields string value
        /// </summary>
        public string TitleLabelFieldStringValue => this.TitleLabelField?.StringValue;

        /// <summary>
        /// Gets or sets the title label2 field.
        /// </summary>
        /// <value>
        /// The title label2 field.
        /// </value>
        public UPMStringField TitleLabel2Field { get; set; }

        /// <summary>
        /// Gets the title label fields string value
        /// </summary>
        public string TitleLabel2FieldStringValue => this.TitleLabel2Field.StringValue;

        /// <summary>
        /// Gets or sets the detail label field.
        /// </summary>
        /// <value>
        /// The detail label field.
        /// </value>
        public UPMStringField DetailLabelField { get; set; }

        /// <summary>
        /// Gets the title label fields string value
        /// </summary>
        public string DetailLabelFieldStringValue => this.DetailLabelField.StringValue;

        /// <summary>
        /// Gets or sets the color of the indicator.
        /// </summary>
        /// <value>
        /// The color of the indicator.
        /// </value>
        public AureaColor IndicatorColor { get; set; }

        /// <summary>
        /// Gets or sets the image string.
        /// </summary>
        /// <value>
        /// The image string.
        /// </value>
        public string ImageString { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Matcheses the string.
        /// </summary>
        /// <param name="theString">The string.</param>
        /// <returns></returns>
        public virtual bool MatchesString(string theString)
        {
            var range = this.TitleLabelField.StringValue.IndexOf(theString, StringComparison.OrdinalIgnoreCase);
            if (range > 0)
            {
                return true;
            }

            range = this.TitleLabel2Field.StringValue.IndexOf(theString, StringComparison.OrdinalIgnoreCase);
            if (range > 0)
            {
                return true;
            }

            range = this.DetailLabelField.StringValue.IndexOf(theString, StringComparison.OrdinalIgnoreCase);
            return range > 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"TitleLabelField: {this.TitleLabelField}, TitleLabel2Field: {this.TitleLabel2Field}, DetailLabelField: {this.DetailLabelField}, IndicatorColor: {this.IndicatorColor}, ImageString: {this.ImageString}";
        }
    }
}
