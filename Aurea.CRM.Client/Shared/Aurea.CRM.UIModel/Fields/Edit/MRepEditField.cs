// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MRepEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm rep edit field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// The upm rep edit field.
    /// </summary>
    public class UPMRepEditField : UPMCatalogEditField
    {

        private List<UPRecordSelector> selectorArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRepEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMRepEditField(IIdentifier identifier)
            : base(identifier)
        {
            this.RepContainer = new UPMRepContainer();
            this.MultiSelectMode = false;
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public UPMGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the group model controller.
        /// </summary>
        public UPParticipantsGroupModelControllerDelegate GroupModelController { get; set; }

        /// <summary>
        /// Gets or sets the rep container.
        /// </summary>
        public UPMRepContainer RepContainer { get; set; }

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
        /// Gets or sets the CRM result delegate.
        /// </summary>
        /// <value>
        /// The CRM result delegate.
        /// </value>
        public IRecordSelectorEditFieldCRMResultDelegate CrmResultDelegate { get; set; }


        /// <summary>
        /// The display value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DisplayValue()
        {
            var possibleValue = this.RepContainer.PossibleValueForKey(this.StringValue);
            return possibleValue.TitleLabelField.StringValue;
        }

        /// <summary>
        /// The set field value.
        /// </summary>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public void SetFieldValue(string fieldValue)
        {
            var repContanerDelegate = this.RepContainer.Delegate;
            this.RepContainer.Delegate = null;
            if (!this.MultiSelectMode)
            {
                this.RepContainer.RepKeyDeselected((string)this.FieldValue);
            }

            this.FieldValue = fieldValue;
            this.RepContainer.RepKeySelected(fieldValue);
            this.RepContainer.Delegate = repContanerDelegate;
        }

        /// <summary>
        /// The set multi select mode.
        /// </summary>
        /// <param name="multiSelectMode">
        /// The multi select mode.
        /// </param>
        public void SetMultiSelectMode(bool multiSelectMode)
        {
            this.MultiSelectMode = multiSelectMode;
            this.RepContainer.MultiSelectMode = multiSelectMode;
        }

        /// <summary>
        /// The set rep container.
        /// </summary>
        /// <param name="repContainer">
        /// The rep container.
        /// </param>
        public void SetRepContainer(UPMRepContainer repContainer)
        {
            this.RepContainer = repContainer;
            this.MultiSelectMode = this.RepContainer.MultiSelectMode;
        }

        /// <summary>
        /// The user did change field with context.
        /// </summary>
        /// <param name="pageModelController">
        /// The page model controller.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<IIdentifier> UserDidChangeFieldWithContext(object pageModelController)
        {
            return this.GroupModelController.UserDidChangeField(this, pageModelController);
        }
    }
}
