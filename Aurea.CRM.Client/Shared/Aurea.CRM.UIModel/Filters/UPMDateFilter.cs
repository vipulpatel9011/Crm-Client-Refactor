// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMDateFilter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm date filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// The upm date filter.
    /// </summary>
    public class UPMDateFilter : UPMFilter
    {
        /// <summary>
        /// The second value.
        /// </summary>
        private DateTime secondValue;

        /// <summary>
        /// The value.
        /// </summary>
        private DateTime value;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDateFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMDateFilter(IIdentifier identifier)
            : base(identifier, UPMFilterType.Date)
        {
            this.Invalid = false;
            this.Value = DateTime.Now.Date;
            this.SecondValue = DateTime.Now.Date;
        }

        /// <summary>
        /// Gets a value indicating whether the filter has paramteres
        /// </summary>
        public override bool HasParameters => !string.IsNullOrWhiteSpace(this.ParameterName);

        /// <summary>
        /// Gets or sets the initial second value.
        /// </summary>
        public DateTime InitialSecondValue { get; set; }

        /// <summary>
        /// Gets or sets the initial value.
        /// </summary>
        public DateTime InitialValue { get; set; }

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets the raw values.
        /// </summary>
        public override List<string> RawValues
            =>
                !string.IsNullOrEmpty(this.SecondParameterName)
                    ? new List<string>
                    {
                        this.Value.CrmValueFromDate(),
                        this.SecondValue.CrmValueFromDate()
                    }
                    : new List<string> { this.Value.CrmValueFromDate() };

        /// <summary>
        /// Gets or sets the second parameter name.
        /// </summary>
        public string SecondParameterName { get; set; }

        /// <summary>
        /// Gets or sets the second value.
        /// </summary>
        public DateTime SecondValue
        {
            get => this.secondValue;
            set
            {
                this.Set(() => this.SecondValue, ref this.secondValue, value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public DateTime Value
        {
            get => this.value;
            set
            {
                this.Set(() => this.Value, ref this.value, value);
            }
        }

        /// <inheritdoc />
        public override void ClearValue()
        {
            this.Value = DateTime.Now.Date;
            this.SecondValue = DateTime.Now.Date;
            base.ClearValue();
        }
    }
}
