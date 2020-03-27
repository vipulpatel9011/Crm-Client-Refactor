// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPFixedCatalogEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit feld context for a fixed catalog field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Edit feld context for a fixed catalog field
    /// </summary>
    /// <seealso cref="UPCatalogEditFieldContext" />
    public class UPFixedCatalogEditFieldContext : UPCatalogEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPFixedCatalogEditFieldContext"/> class.
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
        public UPFixedCatalogEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFixedCatalogEditFieldContext"/> class.
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
        public UPFixedCatalogEditFieldContext(
            WebConfigLayoutField fieldConfig,
            IIdentifier fieldIdentifier,
            string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Gets the catalog.
        /// </summary>
        /// <value>
        /// The catalog.
        /// </value>
        public override UPCatalog Catalog
            => UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(this.FieldConfig.Field.CatNo);

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMCatalogEditField(this.FieldIdentifier, this.MultiSelect);
            if (this.MultiSelect)
            {
                field.MultiSelectMaxCount = this.ChildFields.Count + 1;
            }

            var possibleValues = this.Catalog?.TextValuesForFieldValues(true);
            var explicitKeyOrder = this.Catalog != null
                                   && ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("FixedCatalog.SortByCode")
                                       ? this.Catalog.ExplicitKeyOrderByCodeEmptyValueIncludeHidden(false, false)
                                       : this.Catalog?.ExplicitKeyOrderEmptyValueIncludeHidden(false, false);

            var attributes =
                ConfigurationUnitStore.DefaultStore.CatalogAttributesForInfoAreaIdFieldId(
                    this.FieldConfig.InfoAreaId,
                    this.FieldConfig.FieldId);
            var valueForCode0 = possibleValues.ValueOrDefault("0");
            if (string.IsNullOrEmpty(valueForCode0))
            {
                field.NullValueKey = "0";
            }
            else
            {
                field.NullValueText = valueForCode0;
            }

            foreach (var p in possibleValues ?? new Dictionary<string, string>())
            {
                var possibleValue = new UPMCatalogPossibleValue
                {
                    Key = p.Key,
                    TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                    {
                        StringValue = p.Value
                    }
                };

                if (!this.MultiSelect || !p.Key.Equals(field.NullValueKey))
                {
                    field.AddPossibleValue(possibleValue);
                }
            }

            var allKeys = field.AllKeysFromPossibleValues;
            foreach (var theObject in allKeys)
            {
                var temp = attributes?.ValuesByCode?.ValueOrDefault(JObjectExtensions.ToInt(theObject));
                if (temp == null)
                {
                    continue;
                }

                var possibleValue = field.PossibleValueForKey(theObject);
                var colorString = temp.ColorKey;
                if (!string.IsNullOrEmpty(colorString))
                {
                    var color = AureaColor.ColorWithString(colorString);
                    possibleValue.IndicatorColor = color;
                }

                possibleValue.ImageString = temp.ImageName;
            }

            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            if (explicitKeyOrder != null)
            {
                field.ExplicitKeyOrder = explicitKeyOrder;
            }

            return field;
        }
    }
}
