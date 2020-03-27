// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSerialEntryCatalogEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The UPMSerialEntryCatalogEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.SerialEntry
{
using System.Collections.Generic;
using Aurea.CRM.Core.CRM.UIModel;
using Aurea.CRM.Core.Extensions;
using Aurea.CRM.UIModel.Fields.Edit;
using Aurea.CRM.UIModel.SerialEntry;

    /// <summary>
    /// Enum UPSerialEntryCatalogStyle
    /// </summary>
    public enum UPSerialEntryCatalogStyle
    {
        /// <summary>
        /// Keyboard
        /// </summary>
        Keyboard = 0,

        /// <summary>
        /// Popover
        /// </summary>
        Popover
    }

    /// <summary>
    /// UPMSerialEntryCatalogEditField
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.UPMCatalogEditField" />
    /// <seealso cref="Aurea.CRM.UIModel.Fields.SerialEntry.UPMSerialEntryEditField" />
    public class UPMSerialEntryCatalogEditField : UPMCatalogEditField, UPMSerialEntryEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSerialEntryCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="column">The column.</param>
        /// <param name="position">The position.</param>
        /// <param name="multiSelectMode">if set to <c>true</c> [multi select mode].</param>
        public UPMSerialEntryCatalogEditField(IIdentifier identifier, UPSEColumn column, UPMSEPosition position, bool multiSelectMode = false)
            : base(identifier, multiSelectMode)
        {
            this.SerialEntryColumn = column;
            this.SerialEntryPosition = position;
            this.keysForValue = new Dictionary<string, string>();
            this.SerialEntryCatalogStyle = UPSerialEntryCatalogStyle.Keyboard;
        }

        /// <summary>
        /// The keys for value
        /// </summary>
        private Dictionary<string, string> keysForValue;

        /// <summary>
        /// Gets the serial entry column.
        /// </summary>
        /// <value>
        /// The serial entry column.
        /// </value>
        public UPSEColumn SerialEntryColumn { get; }

        /// <summary>
        /// Gets the serial entry position.
        /// </summary>
        /// <value>
        /// The serial entry position.
        /// </value>
        public UPMSEPosition SerialEntryPosition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [new line].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [new line]; otherwise, <c>false</c>.
        /// </value>
        public bool NewLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [one column].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [one column]; otherwise, <c>false</c>.
        /// </value>
        public bool OneColumn { get; set; }

        /// <summary>
        /// Gets or sets the serial entry catalog style.
        /// </summary>
        /// <value>
        /// The serial entry catalog style.
        /// </value>
        public UPSerialEntryCatalogStyle SerialEntryCatalogStyle { get; set; }

        /// <summary>
        /// Adds the possible value.
        /// </summary>
        /// <param name="possibleValue">The possible value.</param>
        /// <param name="key">The key.</param>
        public void AddPossibleValue(UPMCatalogPossibleValue possibleValue, string key)
        {
            base.AddPossibleValue(possibleValue);

            if (key != null)
            {
                this.keysForValue.SetObjectForKey(key, possibleValue.TitleLabelField.StringValue);
            }
        }

        /// <summary>
        /// Keys for value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string KeyForValue(string value)
        {
            return this.keysForValue[value];
        }
    }
}
