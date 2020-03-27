// <copyright file="MRecordSelectorEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;

    /// <summary>
    /// RecordSelectorEditField Result Delegate
    /// </summary>
    public interface IRecordSelectorEditFieldCRMResultDelegate
    {
        /// <summary>
        /// Returns data from result row
        /// </summary>
        /// <param name="resultRow">Result row</param>
        /// <returns><see cref="object"/></returns>
        object DataFromResultRow(UPCRMResultRow resultRow);
    }

    /// <summary>
    /// The SearchResultSelectedDelegate interface.
    /// </summary>
    public interface ISearchResultSelectedDelegate
    {
        /// <summary>
        /// The update link from selected result
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        void UpdateLinkFromSelectedResult(UPCRMResultRow resultRow, UPMResultRow result);
    }

    /// <summary>
    /// UI control for editing a record selector field value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMRecordSelectorEditField : UPMEditField
    {
        private List<UPRecordSelector> selectorArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRecordSelectorEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMRecordSelectorEditField(IIdentifier identifier)
            : base(identifier)
        {
            this.MultiSelectMode = false;
            this.DisableEdit = false;
        }

        /// <summary>
        /// Gets the context record.
        /// </summary>
        /// <value>
        /// The context record.
        /// </value>
        public string ContextRecord => this.Delegate?.ContextRecordForEditField(this);

        /// <summary>
        /// Gets the current record.
        /// </summary>
        /// <value>
        /// The current record.
        /// </value>
        public string CurrentRecord => this.Delegate?.CurrentRecordForEditField(this);

        /// <summary>
        /// Gets or sets the result rows.
        /// </summary>
        /// <value>
        /// The result rows.
        /// </value>
        public UPRecordSelectorRowData ResultRows { get; set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public virtual string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the selector array.
        /// </summary>
        /// <value>
        /// The selector array.
        /// </value>
        public List<UPRecordSelector> SelectorArray
        {
            get
            {
                return this.selectorArray;
            }

            set
            {
                this.selectorArray = value;
                this.CurrentSelector = this.selectorArray?.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets the current selector.
        /// </summary>
        /// <value>
        /// The current selector.
        /// </value>
        public UPRecordSelector CurrentSelector { get; set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IRecordSelectorEditFieldDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable edit].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable edit]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableEdit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multi select mode is enabled
        /// </summary>
        public bool MultiSelectMode { get; set; }

        /// <summary>
        /// Gets or sets the CRM result delegate.
        /// </summary>
        /// <value>
        /// The CRM result delegate.
        /// </value>
        public IRecordSelectorEditFieldCRMResultDelegate CrmResultDelegate { get; set; }

        /// <summary>
        /// Sets the current selector from position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetCurrentSelectorFromPosition(int position)
        {
            if (position < (this.selectorArray?.Count ?? 0))
            {
                this.CurrentSelector = this.selectorArray?[position];
            }
        }
    }
}
