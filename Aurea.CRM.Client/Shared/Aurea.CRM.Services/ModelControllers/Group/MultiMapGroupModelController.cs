// <copyright file="MultiMapGroupModelController.cs" class="UPMultiMapGroupModelController" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// The multi map group model controller.
    /// </summary>
    public class UPMultiMapGroupModelController : UPListResultGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMultiMapGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">
        /// The form item.
        /// </param>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="controllerDelegate">
        /// The controller delegate.
        /// </param>
        public UPMultiMapGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate controllerDelegate)
            : base(formItem, identifier, controllerDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMultiMapGroupModelController"/> class.
        /// </summary>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="headerSwipeEnabled">if set to <c>true</c> [header swipe enabled].</param>
        /// <param name="cellStyleAsString">The cell style as string.</param>
        /// <param name="disablePaging">if set to <c>true</c> [disable paging].</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPMultiMapGroupModelController(string searchAndListConfigurationName, int linkId, bool headerSwipeEnabled,
            string cellStyleAsString, bool disablePaging, IGroupModelControllerDelegate theDelegate)
            : base(searchAndListConfigurationName, linkId, headerSwipeEnabled, cellStyleAsString, disablePaging, theDelegate)
        {
        }

        /// <summary>
        /// The perform location field action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void PerformLocationFieldAction(object sender)
        {
            var locationField = (UPMLocationField)sender;
            var configStore = ConfigurationUnitStore.DefaultStore;
            Menu defaultAction = null;
            if (!string.IsNullOrEmpty(this.SearchAndList.DefaultAction))
            {
                defaultAction = configStore.MenuByName(this.SearchAndList.DefaultAction);
            }

            if (defaultAction == null)
            {
                defaultAction = configStore.MenuByName("SHOWRECORD");
            }

            var recordIdentifier = (RecordIdentifier)locationField.Identifier;
            var locationFieldViewReference = defaultAction.ViewReference.ViewReferenceWith(recordIdentifier.RecordIdentification);
            this.Delegate.PerformOrganizerAction(this, locationFieldViewReference);
        }

        /// <summary>
        /// The group from result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        protected override UPMGroup GroupFromResult(UPCRMResult result)
        {
            int count = result.RowCount;

            if (this.MaxResults > 0 && count > this.MaxResults)
            {
                count = this.MaxResults;
            }

            if (count > 0)
            {
                var mapGroup = new UPMMapGroup(this.ExplicitTabIdentifier) { LabelText = this.TabLabel };

                if (this.FormItem != null)
                {
                    var optionValue = this.FormItem.Options.ValueOrDefault("Interaction") as string;

                    mapGroup.UserInteractionOnlyEnabledInFullscreenMode = optionValue == "FullScreen";

                    mapGroup.RecommendedHeight = this.FormItem.CellAttributes.ValueOrDefault("Height").ToInt();
                }
                else
                {
                    mapGroup.RecommendedHeight = 350;
                }

                var tabConfig = this.FieldControl.TabAtIndex(0);

                for (var i = 0; i < count; i++)
                {
                    var resultRow = (UPCRMResultRow)result.ResultRowAtIndex(i);
                    var geoLocation = UPGeoLocation.Create(resultRow, tabConfig);
                    if (geoLocation == null)
                    {
                        continue;
                    }

                    var identifier = new RecordIdentifier(this.FieldControl.InfoAreaId, resultRow.RecordIdentificationAtIndex(0).RecordId());
                    var locationFieldAction = new UPMAction(identifier.IdentifierWithFieldId("locationActionId"));
                    locationFieldAction.SetTargetAction(this, this.PerformLocationFieldAction);
                    var locationField = new UPMLocationField(identifier, locationFieldAction);
                    if (geoLocation.ValidGPS)
                    {
                        locationField.Longitude = geoLocation.GpsXString;
                        locationField.Latitude = geoLocation.GpsYString;
                    }

                    if (!string.IsNullOrEmpty(geoLocation.AddressTitle))
                    {
                        locationField.AddressTitle = geoLocation.AddressTitle;
                    }

                    if (!string.IsNullOrEmpty(geoLocation.Address))
                    {
                        locationField.Address = geoLocation.Address;
                    }

                    mapGroup.AddChild(locationField);
                }

                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = mapGroup;
                return mapGroup;
            }

            this.ControllerState = GroupModelControllerState.Empty;
            this.Group = null;
            return null;
        }
    }
}
