// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPRepEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Field edit context for Rep field. Deriving from UPCatalogEditFieldContext currently for basic functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts.Reps;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Field edit context for Rep field. Deriving from UPCatalogEditFieldContext currently for basic functionality.
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPRepEditFieldContext : UPCatalogEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPRepEditFieldContext"/> class.
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
        public UPRepEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRepEditFieldContext"/> class.
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
        public UPRepEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// The create edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            UPMRepEditField field = new UPMRepEditField(this.FieldIdentifier);
            UPCRMRepType repType = UPCRMReps.RepTypeFromString(this.FieldConfig.Field.RepType);
            var possibleValues = UPCRMDataStore.DefaultStore.Reps.AllRepsOfTypes(repType);
            var explicitKeyOrder = UPCRMDataStore.DefaultStore.Reps.AllRepIdsOfTypes(repType);
            var repContainer = UPRepsService.CreateRepContainerForRepType(repType);
            field.RepContainer = repContainer;

            // Adding all rep values from UPCRMDataStore to the PossibleValues list.
            foreach (var obj in possibleValues)
            {
                UPMCatalogPossibleValue possibleValue = new UPMCatalogPossibleValue();
                UPMStringField valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                UPCRMRep rep = obj;
                valueField.StringValue = rep.RepName;
                possibleValue.TitleLabelField = valueField;
                possibleValue.Key = rep.RepId;
                field.AddPossibleValue(possibleValue);
            }

            field.SetFieldValue(this.OriginalValue);
            field.NullValueKey = "0";
            field.ExplicitKeyOrder = explicitKeyOrder;
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);

            return field;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            if (value?.Length > 0 && value.Length != 9)
            {
                base.SetValue(UPCRMReps.FormattedRepId(value));
            }
            else
            {
                base.SetValue(value);
            }
        }
    }
}
