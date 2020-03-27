// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreMappingResultContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The up core mapping result context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;

    /// <summary>
    /// The up core mapping result context.
    /// </summary>
    public class UPCoreMappingResultContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCoreMappingResultContext"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="numberOfListFields">
        /// The number of list fields.
        /// </param>
        public UPCoreMappingResultContext(UPCRMResult result, FieldControl control, int numberOfListFields)
        {
            this.Result = result;
            this.RowDictionary = new Dictionary<string, UPCoreMappingResultRowContext>();
            this.FieldControl = control;

            this.NumberOfListFields = numberOfListFields;
            int controlFieldCount = control.NumberOfFields;
            if (numberOfListFields > controlFieldCount)
            {
                this.NumberOfListFields = controlFieldCount;
            }

            this.ListFormatter = new UPCRMListFormatter(control.TabAtIndex(0), this.NumberOfListFields);

            int dropdownLineFieldCount = controlFieldCount - numberOfListFields;
            if (dropdownLineFieldCount > 0)
            {
                List<UPConfigFieldControlField> fields = new List<UPConfigFieldControlField>(dropdownLineFieldCount);
                for (int i = numberOfListFields; i < numberOfListFields + dropdownLineFieldCount; i++)
                {
                    fields.Add(control.FieldAtIndex(i));
                }

                this.DropdownFields = fields;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.InfoAreaConfig = configStore.InfoAreaConfigById(control.InfoAreaId);

            this.SectionFieldComplete = false;

            foreach (FieldControlTab tab in control.Tabs)
            {
                if (tab.Fields != null)
                {
                    foreach (UPConfigFieldControlField field in tab.Fields)
                    {
                        if (field.Attributes.ExtendedOptionIsSet(@"SectionField"))
                        {
                            this.SectionField = field;
                            break;
                        }
                    }
                }
            }

            this.SectionFieldComplete = this.SectionField != null && this.SectionField.Field.FieldType != @"C";
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        /// The detail field.
        /// </summary>
        public UPConfigFieldControlField DetailField => this.ListFormatter.FirstFieldForPosition(1);

        /// <summary>
        /// Gets the dropdown fields.
        /// </summary>
        public List<UPConfigFieldControlField> DropdownFields { get; private set; }

        /// <summary>
        /// Gets or sets the expand mapper.
        /// </summary>
        public UPConfigExpand ExpandMapper { get; set; }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the info area config.
        /// </summary>
        public InfoArea InfoAreaConfig { get; private set; }

        /// <summary>
        /// Gets the list formatter.
        /// </summary>
        public UPCRMListFormatter ListFormatter { get; private set; }

        /// <summary>
        /// The main field.
        /// </summary>
        public UPConfigFieldControlField MainField => this.ListFormatter.FirstFieldForPosition(0);

        /// <summary>
        /// Gets the number of list fields.
        /// </summary>
        public int NumberOfListFields { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public UPCRMResult Result { get; private set; }

        /// <summary>
        /// Gets the row dictionary.
        /// </summary>
        public Dictionary<string, UPCoreMappingResultRowContext> RowDictionary { get; private set; }

        /// <summary>
        /// Gets the section field.
        /// </summary>
        public UPConfigFieldControlField SectionField { get; private set; }

        /// <summary>
        /// Gets a value indicating whether section field complete.
        /// </summary>
        public bool SectionFieldComplete { get; private set; }
    }
}
