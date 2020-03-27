// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEDestinationColumnBase.cs" company="Aurea Software Gmbh">
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
//   UPSEDestinationColumnBase
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// InitialFocusMode
    /// </summary>
    public enum InitialFocusMode
    {
        /// <summary>
        /// No
        /// </summary>
        No = 0,

        /// <summary>
        /// Yes
        /// </summary>
        Yes = 1,

        /// <summary>
        /// If empty
        /// </summary>
        IfEmpty = 2
    }

    /// <summary>
    /// UPSEDestinationColumnBase
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEColumn" />
    public class UPSEDestinationColumnBase : UPSEColumn
    {
        private UPContainerFieldMetaInfo fieldMetaInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationColumnBase"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="index">The index.</param>
        /// <param name="parentColumnIndex">Index of the parent column.</param>
        /// <param name="positionInControl">The position in control.</param>
        /// <param name="destinationInfoAreaId">The destination information area identifier.</param>
        public UPSEDestinationColumnBase(UPConfigFieldControlField fieldConfig, int index, int parentColumnIndex,
            int positionInControl, string destinationInfoAreaId)
            : base(fieldConfig, index, positionInControl)
        {
            bool otherInfoArea = destinationInfoAreaId != fieldConfig.InfoAreaId;
            if (fieldConfig.Attributes.ReadOnly || otherInfoArea)
            {
                this.Readonly = true;
            }

            if (fieldConfig.Attributes.Dontsave || otherInfoArea)
            {
                this.DontSave = true;
            }

            if (fieldConfig.Attributes.DontcacheOffline || otherInfoArea)
            {
                this.DontCacheOffline = true;
            }

            if (fieldConfig.Attributes.Empty)
            {
                this.Empty = true;
            }

            string v = fieldConfig.Attributes.ExtendedOptionForKey("initialFocus");
            if (v == "ifEmpty")
            {
                this.InitialFocus = InitialFocusMode.IfEmpty;
            }
            else
            {
                this.InitialFocus = fieldConfig.Attributes.ExtendedOptionIsSet("initialFocus") ? InitialFocusMode.Yes : InitialFocusMode.No;
            }

            this.ChangeAction = fieldConfig.Attributes.ExtendedOptionForKey("onchange");
            this.ParentColumnIndex = parentColumnIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationColumnBase"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="column">The column.</param>
        public UPSEDestinationColumnBase(UPConfigFieldControlField fieldConfig, UPSEDestinationColumnBase column)
            : base(fieldConfig, column)
        {
            this.Readonly = column.Readonly;
            this.DontSave = column.DontSave;
            this.DontCacheOffline = column.DontCacheOffline;
            this.Empty = column.Empty;
            string v = fieldConfig.Attributes.ExtendedOptionForKey("initialFocus");
            if (v == "ifEmpty")
            {
                this.InitialFocus = InitialFocusMode.IfEmpty;
            }
            else
            {
                this.InitialFocus = fieldConfig.Attributes.ExtendedOptionIsSet("initialFocus") ? InitialFocusMode.Yes : column.InitialFocus;
            }

            this.ChangeAction = fieldConfig.Attributes.ExtendedOptionForKey("onchange") ?? column.ChangeAction;
            this.ParentColumnIndex = column.ParentColumnIndex;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEDestinationColumnBase"/> is readonly.
        /// </summary>
        /// <value>
        ///   <c>true</c> if readonly; otherwise, <c>false</c>.
        /// </value>
        public bool Readonly { get; private set; }

        /// <summary>
        /// Gets or sets the initial focus.
        /// </summary>
        /// <value>
        /// The initial focus.
        /// </value>
        public InitialFocusMode InitialFocus { get; set; }

        /// <summary>
        /// Gets a value indicating whether [dont save].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dont save]; otherwise, <c>false</c>.
        /// </value>
        public bool DontSave { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [dont cache offline].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dont cache offline]; otherwise, <c>false</c>.
        /// </value>
        public bool DontCacheOffline { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEDestinationColumnBase"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public bool Empty { get; private set; }

        /// <summary>
        /// Gets the change action.
        /// </summary>
        /// <value>
        /// The change action.
        /// </value>
        public string ChangeAction { get; private set; }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public UPSelector Selector { get; set; }

        /// <summary>
        /// Gets the index of the parent column.
        /// </summary>
        /// <value>
        /// The index of the parent column.
        /// </value>
        public override int ParentColumnIndex { get; }

        /// <summary>
        /// Displays the value for raw value.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <returns></returns>
        public string DisplayValueForRawValue(string rawValue)
        {
            if (this.fieldMetaInfo == null)
            {
                this.fieldMetaInfo = new UPContainerFieldMetaInfo(this.FieldConfig.Field, null, this.FieldConfig.Field);
            }

            return this.fieldMetaInfo.ValueFromRawValue(rawValue);
        }
    }
}
