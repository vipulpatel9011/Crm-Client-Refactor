// <copyright file="MapGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Map group model controller class implementation
    /// </summary>
    public class UPMapGroupModelController : UPFieldControlBasedGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMapGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">
        /// The form item.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPMapGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMapGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPMapGroupModelController(IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMapGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="tabIndex">
        /// The tab index.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPMapGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <inheritdoc/>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            var group = this.GroupFromRow(row);
            group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }

        /// <summary>
        /// Creates and returns an instance of <see cref="UPMGroup"/> from given <see cref="UPCRMResultRow"/>.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        private UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            var location = UPGeoLocation.Create(resultRow, this.TabConfig);
            var recordIdentification = resultRow.RootRecordIdentification;

            if (location != null)
            {
                var mapGroup = new UPMMapGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                mapGroup.LabelText = this.TabLabel;
                var locationField = new UPMLocationField(FieldIdentifier.IdentifierWithRecordIdentificationFieldId(recordIdentification, "locationField"), null);

                if (location.ValidGPS)
                {
                    locationField.Longitude = location.GpsXString;
                    locationField.Latitude = location.GpsYString;
                }

                if (!string.IsNullOrEmpty(location.AddressTitle))
                {
                    locationField.AddressTitle = location.AddressTitle;
                }

                if (!string.IsNullOrEmpty(location.Address))
                {
                    locationField.Address = location.Address;
                }

                mapGroup.AddChild(locationField);
                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = mapGroup;
                return mapGroup;
            }

            this.ControllerState = GroupModelControllerState.Empty;
            return null;
        }
    }
}
