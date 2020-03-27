// <copyright file="GeoSearchPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Structs;
    using Aurea.CRM.Services.Delegates;
    using Core.Configuration;
    using Core.CRM.DataModel;
    using Core.CRM.Features;
    using Core.CRM.Query;
    using Core.CRM.UIModel;
    using Core.Extensions;
    using GalaSoft.MvvmLight.Ioc;
    using UIModel;
    using UIModel.Contexts;
    using UIModel.Fields;
    using UIModel.Filters;
    using UIModel.Identifiers;
    using UIModel.Pages;
    using UIModel.Status;

    /// <summary>
    /// Implementation of geosearch page model controller
    /// </summary>
    public class GeoSearchPageModelController : GlobalSearchPageModelController, ILocationServiceDelegate, IGeoViewControllerDataProvider, UPCopyFieldsDelegate
    {
        private const string ModusGeoSearch = "GetSearch";
        private Location currentUserLocation;
        private ILocationService locationManager;
        private Dictionary<string, UPMFilter> usedFilters;
        private UPMResultSection geoSection;
        private List<GeoUPMResultRow> resultRowsToSort;
        private List<UPSearchPageModelControllerPreparedSearch> availableSearches;
        private Location initialTarget;
        private UPCopyFields copyFields;
        private Dictionary<string, object> copyFieldsDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoSearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public GeoSearchPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.locationManager = SimpleIoc.Default.GetInstance<ILocationService>();
            this.locationManager.GetCurrentLocation(this);
            this.BuildPreparedSearches(false);
        }

        /// <inheritdoc/>
        public UPMLocationModeType LocationMode { get; set; }

        /// <inheritdoc/>
        public bool AnyResultWithImageField { get; set; }

        /// <inheritdoc/>
        public override void BuildPage()
        {
            string copyFieldGroupName = this.ViewReference.ContextValueForKey("CopyFieldGroup");
            string copyFieldRecord = this.ViewReference.ContextValueForKey("CopyFieldRecord");
            this.copyFields = null;
            if (copyFieldRecord.IsRecordIdentification() && !string.IsNullOrEmpty(copyFieldGroupName))
            {
                var listFieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", copyFieldGroupName);
                if (listFieldControl != null)
                {
                    this.copyFields = new UPCopyFields(listFieldControl);
                }
            }

            if (this.copyFields != null)
            {
                var page = this.CreatePageInstance();
                page.Invalid = true;
                this.TopLevelElement = page;
                this.copyFields.CopyFieldValuesForRecordIdentification(copyFieldRecord, false, this);
            }
            else
            {
                this.CopyFieldsLoaded(null);
            }
        }

        /// <inheritdoc/>
        public override void BuildPageDetails()
        {
            base.BuildPageDetails();
            this.SearchPage.Invalid = true;
        }

        /// <inheritdoc/>
        public override UPMSearchPage CreatePageInstance()
        {
            InfoAreaId = ViewReference.ContextValueForKey("InfoArea");
            ConfigName = ViewReference.ContextValueForKey("Config1Name");
            var configStore = ConfigurationUnitStore.DefaultStore;
            var searchTypeString = ViewReference.ContextValueForKey("InitialSearchType");
            var searchType = SearchPageSearchType.OfflineSearch;

            if (searchTypeString != null)
            {
                searchType = (SearchPageSearchType)searchTypeString.ToInt();
                if (searchType == 0 || (int)searchType > 2)
                {
                    searchType = SearchPageSearchType.OfflineSearch;
                }
            }

            if (string.IsNullOrWhiteSpace(InfoAreaId) && ViewReference.ContextValueForKey("Modus") == ModusGeoSearch)
            {
                if (string.IsNullOrWhiteSpace(ConfigName))
                {
                    ConfigName = "default";
                }
            }

            var page = CreatePageInstanceWorker(configStore, searchType);

            if (searchType == SearchPageSearchType.OnlineSearch)
            {
                page.InitiallyOnline = true;
            }

            page.HideSearchBar = true;
            page.ViewType = SearchPageViewType.Geo;
            return page;
        }

        /// <inheritdoc/>
        public override UPContainerMetaInfo CreateContainerMetaInfoWithValuePreparedSearch(string searchValue, UPSearchPageModelControllerPreparedSearch preparedSearch)
        {
            var firstFilter = this.SearchPage.AvailableFilters[0] as UPMDistanceFilter;
            var vFilter = this.usedFilters.ValueOrDefault(preparedSearch.SearchConfiguration.UnitName) as UPMDistanceFilter;
            List<UPConfigFilter> configFilter = null;
            if (vFilter != null)
            {
                vFilter.Active = true;
                vFilter.Radius = firstFilter.Radius;
                vFilter.LocationResult(this.currentUserLocation);
                configFilter = UPMFilter.ActiveFiltersForFilters(new List<UPMFilter> { vFilter });
            }

            UPContainerMetaInfo container = preparedSearch.CrmQueryForValue(searchValue, configFilter, false);
            return container;
        }

        /// <inheritdoc/>
        public override void Search(object searchPage)
        {
            if (this.currentUserLocation != null)
            {
                if (this.currentUserLocation?.Latitude != 0 && this.currentUserLocation?.Longitude != 0)
                {
                    this.PreparedSearches.Clear();
                    foreach (var search in this.availableSearches)
                    {
                        var vFilter = this.usedFilters.ValueOrDefault(search.SearchConfiguration.UnitName) as UPMDistanceFilter;
                        if (vFilter.Active)
                        {
                            this.PreparedSearches.Add(search);
                        }
                    }

                    base.Search(searchPage);
                }
            }
        }

        /// <inheritdoc/>
        public override void BuildPreparedSearches(bool forServer)
        {
            this.PreparedSearches = new List<UPSearchPageModelControllerPreparedSearch>();
            this.availableSearches = new List<UPSearchPageModelControllerPreparedSearch>();
            UPSearchPageModelControllerPreparedSearch prepareSearch = new UPSearchPageModelControllerPreparedSearch(null, this.ConfigName, null);
            this.PreparedSearches.Add(prepareSearch);
            this.availableSearches.Add(prepareSearch);
            for (int i = 2; i < 99; i++)
            {
                string configNameKey = $"Config{i}Name";
                string configName2 = this.ViewReference.ContextValueForKey(configNameKey);
                if (!string.IsNullOrEmpty(configName2))
                {
                    var vPrepareSearch = new UPSearchPageModelControllerPreparedSearch(null, configName2, null);
                    this.PreparedSearches.Add(vPrepareSearch);
                    this.availableSearches.Add(vPrepareSearch);
                }
                else
                {
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public override void CreateNewSearchPageWithResult(List<UPCRMResult> results)
        {
            this.geoSection = null;
            this.resultRowsToSort = new List<GeoUPMResultRow>();
            base.CreateNewSearchPageWithResult(results ?? new List<UPCRMResult>());
        }

        /// <inheritdoc/>
        public override UPMResultSection ResultSectionForSearchResult(UPSearchPageModelControllerPreparedSearch preparedSearch, UPCRMResult result)
        {
            UPCoreMappingResultContext resultContext = new UPCoreMappingResultContext(result, preparedSearch.CombinedControl, preparedSearch.ListFieldControl.NumberOfFields);
            this.SectionContexts.SetObjectForKey(resultContext, preparedSearch.InfoAreaId);
            bool newSection = this.geoSection == null;
            if (newSection)
            {
                this.geoSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId($"Search_{preparedSearch.InfoAreaId}"));
            }

            var count = result.RowCount;
            for (var j = 0; j < count; j++)
            {
                UPCRMResultRow dataRow = result.ResultRowAtIndex(j) as UPCRMResultRow;
                var identifier = new RecordIdentifier(preparedSearch.InfoAreaId, dataRow.RecordIdAtIndex(0));
                UPMResultRow resultRow = new UPMResultRow(identifier);
                resultRow.DataValid = true;
                resultContext.RowDictionary.SetObjectForKey(new UPCoreMappingResultRowContext(dataRow, resultContext), resultRow.Key);
                resultContext.ExpandMapper = preparedSearch.ExpandSettings;
                resultRow = (UPMResultRow)this.UpdatedElement(resultRow);

                this.AddDistanceFieldToRow(resultRow);
                this.geoSection.AddResultRow(resultRow);
                if (resultRow.RecordImageDocument != null)
                {
                    this.AnyResultWithImageField = true;
                }
            }

            List<GeoUPMResultRow> sortedArray = this.resultRowsToSort.OrderBy(a => a.Distance).ToList();
            this.geoSection.RemoveAllChildren();
            foreach (var row in sortedArray)
            {
                if (row.ResultRow != null)
                {
                    this.geoSection.AddResultRow(row.ResultRow);
                }
                else
                {
                }
            }

            return newSection ? this.geoSection : null;
        }

        /// <inheritdoc/>
        public void SetUserLocationWithManualAddress(string address)
        {
        }

        /// <inheritdoc/>
        public void SetUserLocationWithManualLocation(Location newLocation)
        {
            this.locationManager.StopUpdatingLocation();
            this.LocationMode = UPMLocationModeType.Manual;
            this.LocationResult(newLocation);
        }

        /// <inheritdoc/>
        public void StartAutoLocation()
        {
            if (this.initialTarget.Longitude != 0 && this.initialTarget.Latitude != 0)
            {
                this.SetUserLocationForInitialGps();
            }
            else
            {
                this.LocationMode = UPMLocationModeType.Auto;
                this.locationManager.StartUpdatingLocation();
            }
        }

        /// <inheritdoc/>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            copyFields = null;
            this.CopyFieldsLoaded(dictionary);
        }

        /// <inheritdoc/>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            copyFields = null;
            this.ReportError(error, false);
        }

        /// <inheritdoc/>
        public void LocationResult(Location newLocation)
        {
            this.currentUserLocation = newLocation;
            this.SearchPage.CurrentLocation = this.currentUserLocation;
            this.Search(this.SearchPage);
        }

        /// <inheritdoc/>
        public void LocationError(string error)
        {
            this.locationManager.StopUpdatingLocation();
            this.SearchPage.Status = UPMMessageStatus.MessageStatusWithMessageDetails(error, LocalizedString.TextDistanceFilterNoLocation);
            this.InformAboutDidFailTopLevelElement(this.SearchPage);
        }

        /// <inheritdoc/>
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            if (this.SearchPage.AvailableFilters.Count < 1)
            {
                this.SearchPage.Status = UPMErrorStatus.ErrorStatusWithMessageDetails(string.Empty, LocalizedString.TextErrorConfiguration);
                return;
            }

            string configValue = this.ViewReference.ContextValueForKey("AdditionalParameters");
            Dictionary<string, object> additionalParametersDictionary = null;
            if (this.LocationMode == UPMLocationModeType.Manual)
            {
                return;
            }
            else
            {
                this.LocationMode = UPMLocationModeType.Auto;
            }

            if (configValue?.Length > 0)
            {
                additionalParametersDictionary = configValue.JsonDictionaryFromString();
            }

            if (this.copyFieldsDictionary?.Count > 0)
            {
                if (additionalParametersDictionary?.Count > 0)
                {
                    var dict = new Dictionary<string, object>(additionalParametersDictionary);
                    foreach (var key in this.copyFieldsDictionary.Keys)
                    {
                        dict.Add(key, this.copyFieldsDictionary[key]);
                    }

                    additionalParametersDictionary = dict;
                }
                else
                {
                    additionalParametersDictionary = this.copyFieldsDictionary;
                }
            }

            if (additionalParametersDictionary?.Count > 0)
            {
                string gpsY = additionalParametersDictionary.ValueOrDefault("GpsY") as string;
                string gpsX = additionalParametersDictionary.ValueOrDefault("GpsX") as string;
                if ((gpsX != null && gpsX != "0") && (gpsY != null && gpsY != "0"))
                {
                    this.initialTarget = new Location(gpsX.ToDouble(), gpsY.ToDouble());
                    if (this.LocationMode != UPMLocationModeType.Manual)
                    {
                        this.LocationMode = UPMLocationModeType.Initial;
                    }
                }
                else
                {
                    string gpsCountry = additionalParametersDictionary.ValueOrDefault("GpsCountry") as string;
                    string gpsCity = additionalParametersDictionary.ValueOrDefault("GpsCity") as string;
                    string gpsStreet = additionalParametersDictionary.ValueOrDefault("GpsStreet") as string;
                    string address = null;
                    if (gpsStreet != null)
                    {
                        address = gpsStreet;
                    }

                    if (gpsCity != null)
                    {
                        if (address != null)
                        {
                            address = $"{address},{gpsCity}";
                        }
                        else
                        {
                            address = gpsCity;
                        }
                    }

                    if (gpsCountry != null)
                    {
                        if (address != null)
                        {
                            address = $"{address}, {gpsCountry}";
                        }
                        else
                        {
                            address = gpsCountry;
                        }
                    }

                    if (address != null)
                    {
                        this.LocationMode = UPMLocationModeType.Initial;
                    }

                    this.LocationMode = UPMLocationModeType.Auto;
                    this.StartAutoLocation();
                }
            }

            if (this.LocationMode == UPMLocationModeType.Auto || this.LocationMode == UPMLocationModeType.Initial)
            {
                this.StartAutoLocation();
            }
        }

        /// <inheritdoc/>
        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();
            this.locationManager.StopUpdatingLocation();
        }

        private void AddDistanceFieldToRow(UPMResultRow resultRow)
        {
            UPMGpsXField gpsXField = null;
            UPMGpsYField gpsYField = null;

            foreach (UPMField field in resultRow.Fields)
            {
                if (field is UPMGpsXField)
                {
                    gpsXField = (UPMGpsXField)field;
                }
                else if (field is UPMGpsYField)
                {
                    gpsYField = (UPMGpsYField)field;
                }
            }

            var target = new Location(gpsXField.StringValue.ToDouble(), gpsYField.StringValue.ToDouble());
            var locA = new Location(target.Longitude, target.Latitude);
            var locB = new Location(this.currentUserLocation.Longitude, this.currentUserLocation.Latitude);
            var distance = locA.DistanceFromLocation(locB);
            this.resultRowsToSort.Add(new GeoUPMResultRow(resultRow, distance));
            uint fieldCount = 0;
            foreach (UPMField field in resultRow.Fields)
            {
                if (field.Hidden == false)
                {
                    fieldCount++;
                }
            }

            var rowIdentifier = (RecordIdentifier)resultRow.Identifier;
            UPMStringField distanceField = new UPMStringField(rowIdentifier.IdentifierWithFieldId("Distance"));

            // TODO: Localization not working and will be handled in CRM-5629
            // distanceField.FieldValue = $"{(distance / 1000).ToString("0.##")} {LocalizationKeys.upTextDistanceFilterKmValue}";
            distanceField.FieldValue = $"{(distance / 1000).ToString("0.##")} km";

            resultRow.Fields.Add(distanceField);
        }

        private void RemainderOfMethodHereUsingReturnAddress(string returnAddress)
        {
            var page = this.SearchPage;
            page.SearchGeoAddress = returnAddress;
            this.InformAboutDidChangeTopLevelElement(page, page, null, null);
        }

        private void SetUserLocationForInitialGps()
        {
            this.locationManager.StopUpdatingLocation();
            var locA = this.initialTarget;
            this.LocationResult(locA);
        }

        private void CopyFieldsLoaded(Dictionary<string, object> copyFieldsDictionary)
        {
            this.copyFieldsDictionary = copyFieldsDictionary;
            var page = this.CreatePageInstance();
            page.Invalid = true;
            this.TopLevelElement = page;
            this.BuildPageDetails();
            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            this.SearchPage.SearchAction = searchAction;
        }

        /// <summary>
        /// Internal method to create a Page Instance
        /// </summary>
        /// <param name="configStore">
        /// IConfigurationUnitStore
        /// </param>
        /// <param name="searchType">
        /// SearchPageSearchType
        /// </param>
        /// <returns>
        /// UPMSearchPage
        /// </returns>
        private UPMSearchPage CreatePageInstanceWorker(IConfigurationUnitStore configStore, SearchPageSearchType searchType)
        {
            var defaultRadiusMeter = 100;
            var defaultRadius = ViewReference.ContextValueForKey("DefaultRadius");

            if (!string.IsNullOrWhiteSpace(defaultRadius))
            {
                defaultRadiusMeter = defaultRadius.ToInt();
            }

            usedFilters = new Dictionary<string, UPMFilter>();
            var identifiers = new List<IIdentifier>();
            var filters = new List<UPMFilter>();

            PopulatePageFilterAndIdentifierLists(configStore, filters, identifiers, defaultRadiusMeter);

            var multipleIdentifier = new MultipleIdentifier(identifiers);
            var page = new UPMSearchPage(multipleIdentifier)
            {
                SearchType = searchType,
                AvailableFilters = filters,
                Style = UPMTableStyle.UPMStandardTableStyle,
                AvailableOnlineSearch = !ViewReference.ContextValueIsSet("hideOnlineOfflineButton")
            };
            return page;
        }

        /// <summary>
        /// Populates filters and Identifiers for Page
        /// </summary>
        /// <param name="configStore">
        /// IConfigurationUnitStore
        /// </param>
        /// <param name="filters">
        /// List&lt;UPMFilter&gt;
        /// </param>
        /// <param name="identifiers">
        /// List&lt;IIdentifier&gt;
        /// </param>
        /// <param name="defaultRadiusMeter">
        /// defaultRadiusMeter
        /// </param>
        private void PopulatePageFilterAndIdentifierLists(
            IConfigurationUnitStore configStore,
            List<UPMFilter> filters,
            List<IIdentifier> identifiers,
            int defaultRadiusMeter)
        {
            PopulatePageFiltersAndIdentifiersForConfig1Filter(configStore, filters, identifiers, defaultRadiusMeter);

            PopulatePageFiltersAndIdentifiersForNonConfig1Filter(configStore, filters, identifiers, defaultRadiusMeter);
        }

        /// <summary>
        /// Populates filters and Identifiers for filter = Config1Filter
        /// </summary>
        /// <param name="configStore">
        /// IConfigurationUnitStore
        /// </param>
        /// <param name="filters">
        /// List&lt;UPMFilter&gt;
        /// </param>
        /// <param name="identifiers">
        /// List&lt;IIdentifier&gt;
        /// </param>
        /// <param name="defaultRadiusMeter">
        /// defaultRadiusMeter
        /// </param>
        private void PopulatePageFiltersAndIdentifiersForConfig1Filter(
            IConfigurationUnitStore configStore,
            List<UPMFilter> filters,
            List<IIdentifier> identifiers,
            int defaultRadiusMeter)
        {
            var infoAreaId = string.Empty;
            var geoFilter1 = ViewReference.ContextValueForKey("Config1Filter");
            var filter = UPMFilter.FilterForName(geoFilter1) as UPMDistanceFilter;
            if (filter != null)
            {
                var expand = configStore.ExpandByName(ConfigName);
                var imageName = expand.ImageName;
                var searchConfiguration = configStore.SearchAndListByName(ConfigName);
                infoAreaId = searchConfiguration != null ? searchConfiguration.InfoAreaId : ConfigName;
                if (string.IsNullOrWhiteSpace(imageName))
                {
                    imageName = configStore.InfoAreaConfigById(searchConfiguration.InfoAreaId).ImageName;
                }

                var colorKey = expand.ColorKey;
                if (string.IsNullOrWhiteSpace(colorKey))
                {
                    colorKey = configStore.InfoAreaConfigById(searchConfiguration.InfoAreaId).ColorKey;
                }

                filter.ImageName = imageName;
                filter.ColorKey = colorKey;
                filter.Radius = defaultRadiusMeter;
                filter.Active = true;
                usedFilters.SetObjectForKey(filter, ConfigName);
                filters.Add(filter);
                identifiers.Add(new RecordIdentifier(infoAreaId, null));
            }
        }

        /// <summary>
        /// Populates filters and Identifiers for filter != Config1Filter
        /// </summary>
        /// <param name="configStore">
        /// IConfigurationUnitStore
        /// </param>
        /// <param name="filters">
        /// List&lt;UPMFilter&gt;
        /// </param>
        /// <param name="identifiers">
        /// List&lt;IIdentifier&gt;
        /// </param>
        /// <param name="defaultRadiusMeter">
        /// defaultRadiusMeter
        /// </param>
        private void PopulatePageFiltersAndIdentifiersForNonConfig1Filter(
            IConfigurationUnitStore configStore,
            List<UPMFilter> filters,
            List<IIdentifier> identifiers,
            int defaultRadiusMeter)
        {
            var infoAreaId = string.Empty;
            for (int configNo = 2; configNo < 99; configNo++)
            {
                var configNameKey = $"Config{configNo}Name";
                var configName = ViewReference.ContextValueForKey(configNameKey);
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    var configFilterKey = $"Config{configNo}Filter";
                    var geoFilter = ViewReference.ContextValueForKey(configFilterKey);
                    var filter = UPMFilter.FilterForName(geoFilter) as UPMDistanceFilter;
                    if (filter == null)
                    {
                        continue;
                    }

                    var expand = configStore.ExpandByName(configName);
                    var imageName = expand.ImageName;
                    var searchConfiguration = configStore.SearchAndListByName(configName);
                    var colorKey = configStore.InfoAreaConfigById(searchConfiguration.InfoAreaId).ColorKey;
                    if (string.IsNullOrWhiteSpace(imageName))
                    {
                        imageName = configStore.InfoAreaConfigById(searchConfiguration.InfoAreaId).ImageName;
                    }

                    filter.ImageName = imageName;
                    filter.ColorKey = colorKey;
                    filter.Radius = (int)defaultRadiusMeter;
                    filter.Active = true;
                    usedFilters.SetObjectForKey(filter, configName);
                    infoAreaId = (searchConfiguration != null) ? searchConfiguration.InfoAreaId : configName;
                    identifiers.Add(new RecordIdentifier(infoAreaId, null));
                    filters.Add(filter);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
