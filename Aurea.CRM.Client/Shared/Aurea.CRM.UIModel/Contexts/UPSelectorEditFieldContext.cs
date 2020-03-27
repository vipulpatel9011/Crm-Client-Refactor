// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSelectorEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for selector field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Edit field context for selector field
    /// </summary>
    /// <seealso cref="UPCatalogEditFieldContext" />
    public class UPSelectorEditFieldContext : UPCatalogEditFieldContext
    {
        /// <summary>
        /// The selector.
        /// </summary>
        protected UPSelector selector;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSelectorEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="selector">
        /// The _selector.
        /// </param>
        public UPSelectorEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            UPSelector selector)
            : base(fieldConfig, fieldIdentifier, value, null)
        {
            this.selector = selector;
        }

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMCatalogSelectorEditField(this.FieldIdentifier);
            var dict = this.FieldConfig.Attributes.Selector;
            var descriptionFormat = dict?.ValueOrDefault("DescriptionFormat") as string;
            var detailsFormat = dict?.ValueOrDefault("DetailsFormat") as string;

            foreach (var p in this.selector.PossibleValues)
            {
                var possibleValue = new UPMCatalogPossibleValue { Key = p.Key };

                var valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"titleLabel-{p.Key}"));

                var selectorOption = p.Value;
                valueField.StringValue = selectorOption.Name;
                possibleValue.TitleLabelField = valueField;
                if (!string.IsNullOrEmpty(descriptionFormat))
                {
                    var title2 = selectorOption.LabelWithFormat(descriptionFormat);
                    if (!string.IsNullOrEmpty(title2))
                    {
                        var title2Field =
                            new UPMStringField(StringIdentifier.IdentifierWithStringId($"title2Label-{p.Key}"))
                            {
                                StringValue = title2
                            };

                        possibleValue.TitleLabel2Field = title2Field;
                    }
                }

                if (!string.IsNullOrEmpty(detailsFormat))
                {
                    var details = selectorOption.LabelWithFormat(detailsFormat);
                    var detailsField =
                        new UPMStringField(StringIdentifier.IdentifierWithStringId($"detailLabel-{p.Key}"))
                        {
                            StringValue = details
                        };

                    possibleValue.DetailLabelField = detailsField;
                }

                field.AddPossibleValue(possibleValue);
            }

            field.ExplicitKeyOrder = this.selector.ExplicitKeyOrder;
            field.NullValueKey = "0";
            field.ContinuousUpdate = true;
            field.Selector = this.selector;
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            return field;
        }
    }
}
