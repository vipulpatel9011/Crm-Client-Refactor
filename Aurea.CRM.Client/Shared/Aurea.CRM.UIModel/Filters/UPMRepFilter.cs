// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepFilter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm rep filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The upm rep filter.
    /// </summary>
    public class UPMRepFilter : UPMFilter
    {
        /// <summary>
        /// The rep contaner.
        /// </summary>
        private UPMRepContainer repContaner;

        /// <summary>
        /// The single select.
        /// </summary>
        private bool singleSelect;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRepFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMRepFilter(IIdentifier identifier)
            : base(identifier, UPMFilterType.Reps)
        {
            this.Invalid = false;
        }

        /// <summary>
        /// Gets the raw values.
        /// </summary>
        public override List<string> RawValues
        {
            get
            {
                this.repContaner.SetLastUsedRepKeys(this.repContaner.SelectedRepKeys);
                return this.repContaner.SelectedRepKeys;
            }
        }

        /// <summary>
        /// Gets or sets the rep contaner.
        /// </summary>
        public UPMRepContainer RepContaner
        {
            get
            {
                return this.repContaner;
            }

            set
            {
                this.repContaner = value;
                this.repContaner.MultiSelectMode = !this.singleSelect;
            }
        }

        /// <summary>
        /// Gets or sets the selected rep keys.
        /// </summary>
        public List<string> SelectedRepKeys
        {
            get
            {
                return this.RepContaner.SelectedRepKeys;
            }

            set
            {
                this.RepContaner.ResetSelectedKeys();
                this.SetDefaultRawValues(value?.Select(x => (object)x).ToList());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether single select.
        /// </summary>
        public bool SingleSelect
        {
            get
            {
                return this.singleSelect;
            }

            set
            {
                this.singleSelect = value;
                this.repContaner.MultiSelectMode = !this.singleSelect;
            }
        }

        /// <summary>
        /// The selected reps display value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string SelectedRepsDisplayValue()
        {
            var selectedCodesArray = this.repContaner.SelectedRepKeys;
            StringBuilder mutableString = new StringBuilder();
            for (int i = 0; i < selectedCodesArray.Count; i++)
            {
                if (i > 0)
                {
                    mutableString.Append("; ");
                }

                mutableString.Append(
                    this.repContaner.PossibleValueForKey(selectedCodesArray[i]).TitleLabelField.StringValue);
            }

            return mutableString.ToString();
        }

        /// <summary>
        /// The set default raw values.
        /// </summary>
        /// <param name="defaultRawValues">
        /// The default raw values.
        /// </param>
        public override void SetDefaultRawValues(List<object> defaultRawValues)
        {
            foreach (string repKey in defaultRawValues)
            {
                this.repContaner.RepKeySelected(repKey);
            }
        }
    }
}
