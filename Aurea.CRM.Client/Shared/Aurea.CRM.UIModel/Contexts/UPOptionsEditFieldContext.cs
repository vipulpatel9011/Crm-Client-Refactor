// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOptionsEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for Options field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Edit field context for Options field
    /// </summary>
    /// <seealso cref="UPCatalogEditFieldContext" />
    public class UPOptionsEditFieldContext : UPCatalogEditFieldContext
    {
        /// <summary>
        /// The options.
        /// </summary>
        private readonly Dictionary<string, string> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOptionsEditFieldContext"/> class.
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
        /// <param name="childFields">
        /// The child fields.
        /// </param>
        public UPOptionsEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOptionsEditFieldContext"/> class.
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
        public UPOptionsEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
            this.options = fieldConfig.OptionDictionary;
        }

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMCatalogEditField(this.FieldIdentifier);

            foreach (var option in this.options)
            {
                var possibleValue = new UPMCatalogPossibleValue
                {
                    Key = option.Key,
                    TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                    {
                        StringValue = option.Value
                    }
                };

                field.AddPossibleValue(possibleValue);
            }

            field.NullValueKey = string.Empty;
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);

            return field;
        }
    }
}
