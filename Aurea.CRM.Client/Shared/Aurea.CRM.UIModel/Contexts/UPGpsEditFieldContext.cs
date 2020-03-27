// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPGpsEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for GPS field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Edit field context for GPS field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPGpsEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// The child gps context.
        /// </summary>
        protected UPChildGpsEditFieldContext childGpsContext;

        /// <summary>
        /// The first field longtitude.
        /// </summary>
        protected bool firstFieldLongtitude;

        /// <summary>
        /// The original child value applied.
        /// </summary>
        protected bool originalChildValueApplied;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGpsEditFieldContext"/> class.
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
        public UPGpsEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGpsEditFieldContext"/> class.
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
        public UPGpsEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGpsEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPGpsEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => ((UPMGpsEditField)this.editField).Longtitude.ToString();

        /// <summary>
        /// Gets the y value.
        /// </summary>
        /// <value>
        /// The y value.
        /// </value>
        public string YValue => ((UPMGpsEditField)this.editField).Latitude.ToString();

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var gpsField = new UPMGpsEditField(this.FieldIdentifier);
            this.firstFieldLongtitude = this.FieldConfig.Attributes?.ExtendedOptionForKey("GPS") == "X";
            if (!this.ChildFields.Any())
            {
                return gpsField;
            }

            var context = this.ChildFields.FirstOrDefault() as UPChildGpsEditFieldContext;
            if (context != null)
            {
                this.childGpsContext = context;
            }

            return gpsField;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (this.firstFieldLongtitude)
            {
                ((UPMGpsEditField)this.editField).Longtitude = double.Parse(value);
            }
            else
            {
                ((UPMGpsEditField)this.editField).Latitude = double.Parse(value);
            }

            if (this.childGpsContext == null)
            {
                return;
            }

            var childGpsValue = this.editField != null && this.originalChildValueApplied
                                    ? null
                                    : this.childGpsContext.OriginalValue;

            if (string.IsNullOrEmpty(childGpsValue))
            {
                return;
            }

            if (this.firstFieldLongtitude)
            {
                ((UPMGpsEditField)this.editField).Latitude = double.Parse(childGpsValue);
            }
            else
            {
                ((UPMGpsEditField)this.editField).Longtitude = double.Parse(childGpsValue);
            }
        }

        /// <summary>
        /// Sets the y value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetYValue(string value) => ((UPMGpsEditField)this.editField).Latitude = double.Parse(value);
    }
}
