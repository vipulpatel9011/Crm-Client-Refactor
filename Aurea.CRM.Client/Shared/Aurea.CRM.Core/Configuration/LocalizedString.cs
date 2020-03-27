// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizedString.cs" company="Aurea Software Gmbh">
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
//   Provides the localization functionality.
//   Simmilar to the UPLocalizedString functionality of CRM.pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Provides the localization functionality.
    /// Simmilar to the UPLocalizedString functionality of CRM.pad
    /// </summary>
    public static class LocalizedString
    {
        /// <summary>
        /// Localizes the specified text group key.
        /// </summary>
        /// <param name="textGroupKey">
        /// The text group key.
        /// </param>
        /// <param name="textIndex">
        /// Index of the text.
        /// </param>
        /// <param name="configUnitStore">
        /// The configuration unit store.
        /// </param>
        /// <param name="defaultString">
        /// The default string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Localize(string textGroupKey, int textIndex, IConfigurationUnitStore configUnitStore, string defaultString = "<Not localized>")
        {
            TextGroup textGroup = configUnitStore?.TextgroupByName(textGroupKey);
            var localizedText = textGroup?.TextAtIndexDefaultText(textIndex, null);
            if (localizedText != null)
            {
                return localizedText;
            }

            var localizationKey = $"{textGroupKey}_{textIndex}";
            // localizedText = SimpleIoc.Default.GetInstance<ILocalizationService>().GetString(localizationKey, localizationKey);
            localizedText = SimpleIoc.Default.GetInstance<ILocalizationService>().GetString(localizationKey, localizationKey);

            return localizedText == localizationKey ? defaultString : localizedText;
        }

        /// <summary>
        /// Localizes the specified text group key.
        /// </summary>
        /// <param name="textGroupKey">
        /// The text group key.
        /// </param>
        /// <param name="textIndex">
        /// Index of the text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Localize(string textGroupKey, int textIndex)
        {
            var configUnitStore = ServerSession.CurrentSession?.ConfigUnitStore;
            return Localize(textGroupKey, textIndex, configUnitStore);
        }

        /// <summary>
        /// Localizes the specified text group key.
        /// </summary>
        /// <param name="textGroupKey">
        /// The text group key.
        /// </param>
        /// <param name="textIndex">
        /// Index of the text.
        /// </param>
        /// <param name="defaultString">
        /// The default string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Localize(string textGroupKey, int textIndex, string defaultString)
        {
            var configUnitStore = ServerSession.CurrentSession?.ConfigUnitStore;
            return configUnitStore != null ? Localize(textGroupKey, textIndex, configUnitStore, defaultString) : defaultString;
        }

        /* Short Macros for Login Group */

        public static string TextWelcomeMessageWithoutName => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginWelcomeMessageWithoutName);
        public static string TextWelcomeMessageWithName => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginWelcomeMessageWithName);
        public static string TextErrorTitleLoginFailed => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorTitleLoginFailed);
        public static string TextErrorMessageCheckLoginData => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorMessageCheckLoginData);
        public static string TextErrorMessageLoginGeneral => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorMessageGeneral);
        public static string TextErrorTitleMissingInternetConnection => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorTitleMissingInternetConnection);
        public static string TextErrorTitleLoginGeneral => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorTitleGeneral);
        public static string TextErrorUserLoggedIn => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyTextErrorUserLoggedIn);

        /// <summary>
        /// Gets or sets the Error message about height being too small
        /// </summary>
        public static string TextErrorsHeightTooSmall => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsHeightTooSmall);

        /// <summary>
        /// Gets or sets a value indicating the Error message about width being too small
        /// </summary>
        public static string TextErrorsWidthTooSmall => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsWidthTooSmall);

        /// <summary>
        /// Gets or sets a value indicating the Error message about resolution not supported
        /// </summary>
        public static string TextErrorsResolutionNotSupported => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsResolutionNotSupported);

        public static string TextErrorSameServer => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyTextErrorSameServer);
        public static string TextErrorSameServerUrl => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyTextErrorSameServerUrl);
        public static string TextErrorTitleSyncFailed => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorTitleSyncFailed);
        public static string TextErrorMessageErrorOccuredDuringSync => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorMessageErrorOccuredDuringSync);
        public static string TextMessageUserCancelledSync => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorMessageUserCancelledSync);
        public static string TextSynchronisationInProgress => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginSynchronisationInProgress);
        public static string TextPleaseChooseYourLanguage => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginPleaseChooseYourLanguage);
        public static string TextErrorNoLanguagesReturned => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorNoLanguagesReturned);
        public static string TextApplyLanguage => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginApplyLanguage);

        public static string TextNewPassword => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginNewPassword);
        public static string TextConfirmNewPassword => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginConfirmNewPassword);
        public static string TextErrorMessageNewPasswordAndConfirmPasswordDontMatch => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginErrorNewPasswordAndConfirmPasswordDontMatch);
        public static string TextPasswordExpired => Localize(LocalizationKeys.TextGroupLogin, LocalizationKeys.KeyLoginPasswordExpired);

        /* Short Macros for Basic Text Group */

        public static string TextTabOverview => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicTabOverview);
        public static string TextDropdownNoDetails => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDropdownNoDetails);
        public static string TextTabAll => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicTabAll);
        public static string TextHeadlineGlobalSearch => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHeadlineGlobalSearch);
        public static string TextHeadlineLoading => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHeadlineLoading);
        public static string TextYes => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicYes);
        public static string TextNo => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNo);
        public static string TextSyncCore => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncCore);
        public static string TextSyncCatalogs => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncCatalogs);
        public static string TextSyncPlaceHolder => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncPlaceHolder);
        public static string TextSyncResources => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncResources);
        public static string TextAddNewGroup => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicAddNewGroup);
        public static string TextLoadingData => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLoadingData);
        public static string TextMoreData => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMoreData);
        public static string TextFilter => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicFilter);
        public static string TextOnline => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOnline, "Online");
        public static string TextOffline => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOffline, "Offline");
        public static string TextEmptyCatalog => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEmptyCatalog);
        public static string TextSelectAll => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSelectAll);
        public static string TextSelectNone => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSelectNone);
        public static string TextSelectedCount => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSelectedCount);
        public static string TextClearAll => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicClearAll);
        public static string TextSettings => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSettings, "View Settings");
        public static string TextSave => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSave);
        public static string TextEdit => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEdit);
        public static string TextCancel => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCancel, "Cancel");
        public static string TextClear => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicClear);
        public static string TextErrorTitleNoMailAccount => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorTitleNoMailAccount);
        public static string TextErrorMessageNoMailAccount => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorMessageNoMailAccount);
        public static string TextErrorTitleNoInternet => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorTitleNoInternet);
        public static string TextErrorMessageNoInternet => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorMessageNoInternet);
        public static string TextErrorTitleReportError => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorTitleReportError);
        public static string TextErrorMessageLoadingReport => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorMessageLoadingReport);
        public static string TextMessageLoadingReport => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMessageLoadingReport);
        public static string TextErrorTitleCouldNotOpenUrl => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorTitleCouldNotOpenUrl);
        public static string TextErrorMessageCouldNotOpenUrl => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorMessageCouldNotOpenUrl);
        public static string TextOk => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOK);
        public static string TextNewInfoArea => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNewInfoArea);

        public static string TextPreviousItem => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicPreviousItem);
        public static string TextPreviousField => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicPreviousField);
        public static string TextNextItem => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNextItem);
        public static string TextEnter => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEnter);
        public static string TextClose => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicClose);
        public static string TextCreate => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCreate);

        public static string TextNoCharacteristics => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNoCharacteristics);

        public static string TextDistanceFilterKmValue => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDistanceFilterKmValue);
        public static string TextDistanceFilterMValue => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDistanceFilterMValue);
        public static string TextDistanceFilterDetail => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDistanceFilterDetail);
        public static string TextDistanceFilterSummary => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDistanceFilterSummary);
        public static string TextDistanceFilterNoLocation => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDistanceFilterNoLocation);
        public static string TextSearchWithRadius => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSearchWithRadius);
        public static string TextCurrentLocation => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCurrentLocation, "Akt. Position");
        public static string TextStartLocation => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicStartLocation, "Starting Position");
        public static string TextWithinRadius => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicWithinRadius, "Search within a radius of %@");
        public static string TextLocation => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLocation, "Location");
        public static string TextMapStandard => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMapStandard, "Standard");
        public static string TextMapSatellite => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMapSatellite, "Satellite");
        public static string TextMapHybrid => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMapHybrid, "Hybrid");
        public static string TextMapNavigateTo => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicMapNavigateTo, "Navigate to");
        public static string TextMapResultSectionHeader => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicGeoResultSectionHeader, "Result");

        public static string TextDelete => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDelete);

        public static string TextOrderReportEmailSubject => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOrderReportEmailSubject);

        public static string TextSearchResultsOffline => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSearchResultsOffline);
        public static string TextSearchResultsOnline => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSearchResultsOnline);

        public static string TextOfflineNotAvailable => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOfflineNotAvailable, "Offline not available");
        public static string TextStart => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicStart, "Start");
        public static string TextEnd => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEnd, "End");
        public static string TextChange => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicChange, "Change");
        public static string TextLanguage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLanguage, "Language");
        public static string TextShowRecord => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicShowRecord, "Show Record");
        public static string TextCoINumberOfChildren => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCoINumberOfChildren, "%i/%i");
        public static string TextCoIUnknownNumberOfChildren => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCoIUnknownNumberOfChildren, "?");
        public static string TextEmployee => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEmployee, "Employee");
        public static string TextLastUsed => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLastUsed, "Last used");
        public static string TextRecentlyUsed => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicRecentlyUsed, "Recently used");
        public static string TextShowAllRecords => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicShowAllRecords, "Show all records");
        public static string TextiPadCalendar => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasiciPadCalendar, "iPad calendar");
        public static string TextSelected => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSelected, "Selected");
        public static string TextChangePassword => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicChangePassword);
        public static string TextPasswordNotChanged => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicPasswordNotChanged);
        public static string TextPasswordSuccessfullChanged => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicPasswordChanged);
        public static string TextFrom => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicFrom, "From");
        public static string TextTo => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicTo, "To");
        public static string TextSelectedXOfY => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSelectedXOfY, "Selected %i of %i");
        public static string TextNotSpecified => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNotSpecified, "Not specified");
        public static string TextOpeningTimes => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOpeningTimes, "Opening Times");
        public static string TextVisitTimes => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicVisitTimes, "Visit Times");
        public static string TextPhoneTimes => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicPhoneTimes, "PhoneTimes");
        public static string TextClosed => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicClosed, "-");
        public static string TextAllDay => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicAllDay, "All day");

        public static string TextDebugOrganizerTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugOrganizerTitle, "Debug");
        public static string TextDebugCrmDbPage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugCrmDbPage, "CRMDataStore");
        public static string TextDebugConfigDbPage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugConfigDbPage, "ConfigUnitStore");
        public static string TextDebugOfflineDbPage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugOfflineDbPage, "OfflineStorage");
        public static string TextDebugLoggingPage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugLoggingPage, "Logging");
        public static string TextDebugDefaultMenuName => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDebugDefaultMenuName, "Debug");

        public static string TextSwitchToEditOrganizer => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSwitchToEditOrganizer, "Switch to edit");
        public static string TextEditOrganizerWillBeClosed => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicEditOrganizerWillBeClosed, "All open edit pages will be closed!");
        public static string TextOnlyOneEditIsAllowed => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicOnlyOneEditIsAllowed, "Only one edit page is allowed!");
        public static string TextDeleteRecordTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDeleteRecordTitle, "Delete record?");
        public static string TextDeleteRecordMessage => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDeleteRecordMessage, "Do you really want to delete this record?");

        public static string TextCustomProductName => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCustomProductName, "CRM.pad");
        public static string TextHomeBaseFullSync => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHomeBaseFullSync, "You arrived your homebase. Full sync?");
        public static string TextSyncConflictsTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsTitle, "Sync Conflicts");

        public static string TextRetryAll => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSyncConflictsRetryAll, "Retry All");
        public static string TextTimeline => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicTimeline, "Timeline");
        public static string TextTextInBracket => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicTextInBracket, "(%@)");
        public static string TextDownloadingDocuments => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicDownloadingDocuments, "Downloading documents (%i remaining)");
        public static string TextLogout => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLogout, "Logout");
        public static string TextGlobal => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicGlobal, "GLOBAL");
        public static string TextSync => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSync, "SYNC");
        public static string TextSystem => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSystem, "SYSTEM");
        public static string TextBack => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicBackButton, "Back");
        public static string TextActions => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicActions, "Actions");
        public static string TextBrowseBy => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicBrowseBy, "Browse By");
        public static string TextErrorProtocolMenuTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorProtocolMenuTitle, "Error Protocol");
        public static string TextError => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHTMLPageDefaultOrganizerTitle, "Error");
        public static string TextDefaultError => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicErrorTitle, "Error");
        public static string TextSearch => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSearch, "Search");
        public static string TextRetry => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicRetry, "Retry");
        public static string TextFavorites => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicFavorites, "Favorites");
        public static string TextLabelFullSync => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicLabelFullSync, "Full Data Synchronization");
        public static string TextSublabelFullSync => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSublabelFullSync, "Switching to %@");
        public static string TextCloseOpenEditOrganizer => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicCloseOpenEditOrganizer, "Close opened edit page");
        public static string TextScanQrCode => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicScanQrCode, "Scan QR Code");
        public static string TextSendMail => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicSendMail, "Send by email");
        public static string TextName => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicName, "Name");
        public static string TextNew => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicNew, "New");
        public static string TextUpload => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicUpload, "Upload");
        public static string TextInboxUploadTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicInboxUploadTitle, "Choose File");
        public static string TextInboxTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicInboxTitle, "Document Inbox");
        public static string TextInboxAdditionalTitle => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicInboxAdditionalTitle, "Manage your central Document Repository");
        public static string TextInboxDescription => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicInboxDescription, "You can add this file to your Document Inbox, in order to upload it to a record.");
        public static string TextAddFile => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicAddFile, "Add File");
        public static string TextAddFileToInbox => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicAddFileToInbox, "Add File to Document Inbox");

        /* Short Macros for Date Text Group */

        public static string TextToday => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateToday);
        public static string TextYesterday => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateYesterday);
        public static string TextTomorrow => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateTomorrow);
        public static string TextNow => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateNow);
        public static string TextDateRange => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateRange, "%@ - %@");
        public static string TextDate => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateDate, "Date");
        public static string TextYear => Localize(LocalizationKeys.TextGroupDate, LocalizationKeys.KeyDateYear, "Year");

        /* Short Macros for Serial Entry Text Group */

        public static string TextQuantityXCurrency => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryQuantityXCurrency, "%@ x %@");
        public static string TextSerialEntryComplete => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryComplete, "Complete");
        public static string TextSerialEntryPackageSize => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryPackageSize, "Quantity per packing unit: %i");
        public static string TextSerialEntryQuota => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryQuota, "Quota: %i");

        public static string TextSerialEntryMinQuantity => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryMinQuantity, "Minimum Quantity (per Delivery): %i");
        public static string TextSerialEntryMaxQuantity => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryMaxQuantity, "Maximum Quantity (per Delivery): %i");
        public static string TextSerialEntryMinMaxQuantity => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryMinMaxQuantity, "Quantity (per Delivery): %i - %i");
        public static string TextSerialEntryDuplicate => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryDuplicate, "Duplicate");
        public static string TextSerialEntryPricing => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryPricing, "Pricing");
        public static string TextSerialEntryDiscount => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryDiscount, "Discount");
        public static string TextSerialEntryUnitPrice => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryUnitPrice, "Unit Price");
        public static string TextSerialEntryFreeGoods => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryFreeGoods, "Free Goods");
        public static string TextSerialEntryPriceRange => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryPriceRange, "Price Range");
        public static string TextSerialEntryQuantity => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryQuantity, "Quantity");
        public static string TextSerialEntryScanningEAN => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryScanningEAN, "Scanning EAN");
        public static string TextSerialEntryNoListing => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryNoListing, "No Listing");
        public static string TextSerialEntrySerialEntry => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntrySerialEntry, "Serial Entry");
        public static string TextSerialEntrySerialEntryAutoCreate => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntrySerialEntryAutoCreate, "%d positions will be created");
        public static string TextSerialEntrySummaryTitle => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntrySummaryTitle, "SUMMARY");
        public static string TextSerialEntryAllItemsButtonTitle => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntryAllItemsButtonTitle, "All Items");
        public static string TextSerialEntrySummaryButtonTitle => Localize(LocalizationKeys.TextGroupSerialEntry, LocalizationKeys.KeySerialEntrySummaryButtonTitle, "Summary");

        /* Short Macros for Processes Text Group */

        public static string TextEditCharacteristics => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesEditCharacteristics);
        public static string TextSyncing => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSyncing, "Synchronizing...");
        public static string TextWaitForChanges => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesWaitForChanges);
        public static string TextYouMadeChanges => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesYouMadeChanges);
        public static string TextReallyAbortAndLoseChanges => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesReallyAbortAndLoseChanges);
        public static string TextProcessCrashReport => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCrashReports);
        public static string TextProcessSyncErrors => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSyncErrors);
        public static string TextEditObjectives => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesEditObjectives);
        public static string TextProcessFilterAll => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesFilterAll);
        public static string TextProcessFilterListings => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesFilterListings);
        public static string TextProcessDocuments => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesDocuments, "Documents");
        public static string TextProcessRecordDataWasSync => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesRecordDataWasSync, "Record has been synchronized");
        public static string TextProcessAddToFavorites => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesAddToFavorites, "Add to Favorites");
        public static string TextProcessDeleteFromFavorites => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesDeleteFromFavorites, "Delete from Favorites");
        public static string TextProcessOrderWasApproved => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesOrderWasApproved, "Order was just approved");
        public static string TextProcessShowDocument => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesShowDocument, "Show document");
        public static string TextProcessSendDocument => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSendDocument, "Send document");
        public static string TextProcessReloadDocument => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesReloadDocument, "Reload document");
        public static string TextProcessUploadPhotoTakePhoto => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoTakePhoto, "Take a photo");
        public static string TextProcessUploadPhotoLibrary => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoLibrary, "Library");
        public static string TextProcessUploadPhotoSizes => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoSizes, "small,medium,large,actual");
        public static string TextProcessImagesAvailableOnline => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesImagesAvailableOnline, "%d images available online");
        public static string TextProcessUploadPhotoUploadAlertTitle => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoUploadAlertTitle, "Upload photo");
        public static string TextProcessUploadPhotoUploadAlertMessage => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoUploadAlertMessage, "The photo you chose is being uploaded. You can wait until the upload is finished, cancel the upload or put it to background.");
        public static string TextProcessUploadPhotoUploadAlertBackground => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoUploadAlertBackground, "Background");
        public static string TextProcessSerialEntrySumItems => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSerialEntrySumItems, "Items:");
        public static string TextProcessSerialEntrySumTotal => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSerialEntrySumTotal, "Total:");
        public static string TextProcessSerialEntryFreeGoodsSumTotal => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSerialEntryFreeGoodsSumTotal, "Free Goods: Î£");
        public static string TextProcessUploadPhotoChooseSizeTitle => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoChooseSizeTitle, "The image has a size of %1. You can reduce the size by scaling the image to one of the sizes below.");
        public static string TextProcessUploadPhotoPhotoNamePlaceholder => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesUploadPhotoPhotoNamePlaceholder, "Name of photo");
        public static string TextProcessCalendarWeekLabel => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarWeekLabel, "CW");
        public static string TextProcessCalendarMoreItems => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarMoreItems, "%d more ...");
        public static string TextProcessSignatureTitle => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSignatureTitle, "Signature");
        public static string TextProcessSignatureConfirmButtonTitle => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSignatureConfirmButtonTitle, "Confirm");
        public static string TextProcessCalendarDayView => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarDayView, "Day");
        public static string TextProcessCalendarWeekView => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarWeekView, "Week");
        public static string TextProcessCalendarWorkWeekView => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarWorkWeekView, "5 Days");
        public static string TextProcessCalendarMonthView => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarMonthView, "Month");
        public static string TextProcessCalendarListView => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCalendarListView, "List");
        public static string TextProcessCredentialsWereRejected => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCredentialsWereRejected, "Your username or password was rejected by the server.");
        public static string TextProcessCredentialsWorkOffline => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCredentialsWorkOffline, "Continue working offline");
        public static string TextProcessCredentialsEnterPassword => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCredentialsEnterPassword, "Enter password");
        public static string TextProcessCredentialsEnterBoth => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCredentialsEnterBoth, "Enter username and password");
        public static string TextProcessCredentialsRetry => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesCredentialsRetry, "Retry");
        public static string TextProcessSerialEntrySaveErrors => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSerialEntrySaveErrors, "Your changes (or part of it) could not be saved. Please either correct your data or remove your changes");
        public static string TextBasicDiscardChanges => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesDiscard, "Discard");
        public static string TextProcessQuestionnaireSurveyAllQuestions => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireSurveyAllQuestions, "All questions");
        public static string TextProcessQuestionnaireMandatory => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireMandatory, "Only mandatory questions");
        public static string TextProcessQuestionnaireSummary => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireSummary, "Summary");
        public static string TextProcessQuestionnaireFinalize => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireFinalize, "Finalize questionnaire");
        public static string TextProcessQuestionnaireFinalized => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireFinalized, "Finalized questionnaire");
        public static string TextProcessSurvey => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSurvey, "Survey");
        public static string TextProcessExecute => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesExecute, "Execute");
        public static string TextProcessExecuting => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesExecuting, "Executing...");
        public static string TextProcessAction => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesAction, "Action");
        public static string TextProcessQuestionnaireNotAvailable => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesQuestionnaireNotAvailable, "Questionnaire not available");
        public static string TextProcessDone => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesDone, "Done");
        public static string TextProcessPortfolioResult => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesPortfolioResult, "Result");
        public static string TextObjectivesTitle => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesObjectivesTitle, "Objectives");
        public static string TextObjectivesSaveWarning => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesObjectivesSaveChanges, "You must save your previous changes before leaving. Do you want to Save or Cancel?");
        public static string TextObjectivesSaveError => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesObjectivesSaveError, "Your changes could not be saved. If you proceed anyway you lose your unchanged data if you don't navigate back afterward and save it. Do you want to proceed?");
        public static string TextLogfileMailContent => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesLogfileMailContent, "Your detailed description here:");
        public static string TextSendReport => Localize(LocalizationKeys.TextGroupProcesses, LocalizationKeys.KeyProcessesSendReport, "Send Report");

        /* Short Macros for Errors Text Group */
        public static string TextErrorConfiguration => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsConfigurationError);
        public static string TextErrorExpandMissing => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsExpandMissing);
        public static string TextErrorSearchAndListMissing => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsSearchAndListMissing);
        public static string TextErrorFilterMissing => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsFilterMissing);
        public static string TextErrorFieldControlMissing => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsFieldControlMissing);
        public static string TextErrorActionNotAllowed => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsActionNotAllowed);
        public static string TextErrorRightsFilter => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsRightsFilter);
        public static string TextErrorParameterEmpty => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsParameterEmpty);
        public static string TextErrorActionNotPossible => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsActionNotPossible);
        public static string TextErrorActionPending => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsActionPending);
        public static string TextErrorRecordDoesNotExist => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsRecordDoesNotExist);
        public static string TextErrorErrorLog => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsErrorLog);
        public static string TextErrorSend => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsSend);
        public static string TextErrorCouldNotBeSaved => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsCouldNotBeSaved);
        public static string TextErrorGeneralServerError => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsGeneralServerError);
        public static string TextErrorServerTimeout => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsServerTimeout);
        public static string TextErrorDetailMessage => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsCrashReportDetailMessage);
        public static string TextErrorClientConstraintError => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsClientConstraintError, "invalid value");
        public static string TextErrorNoExternalParticipantSelected => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsNoExternalParticipantSelected, "No external participants selected");
        public static string TextErrorRecordNotFound => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsRecordNotFound, "Record was not found.");
        public static string TextErrorNoDetailInfo => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsNoDetailInfo, "No detail info.");
        public static string TextErrorDataNotSaved => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsDataNotSaved, "Your entry could not saved (partially).");
        public static string TextErrorErrorInSqlStatement => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsErrorInSqlStatement, "SQL statement error");
        public static string TextErrorOnlySelectAllowed => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsErrorOnlSelectAllowed, "Only select statements are allowed");
        public static string TextErrorNoResults => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsNoResults, "No results!");
        public static string TextErrorWarning => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsWarning, "Warning");
        public static string TextErrorWarningPasswordChangeNeeded => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsWarningPasswordChangeNeeded, "You must change your password in the next 24 hours.");
        public static string TextErrorWarningHTMLFormatWillBeLost => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsWarningHTMLFormatWillBeLost, "HTML formatting will be lost");
        public static string TextErrorOtherErrors => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsOtherErrors, "Other Errors:");
        public static string TextErrorDiscardChangesAndContinue => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsDiscardChangesAndContinue, "A mandatory field was not set. Do you want to discard all changes and continue?");
        public static string TextErrorInboxNotConfigured => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsInboxNotConfigured, "Document Inbox not configured");
        public static string TextErrorInboxFileNotSupported => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsInboxFileNotSupported, "You canÂ´t add this file type to your Document Inbox");
        public static string TextErrorInboxFileSizeNotSupported => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsInboxFileSizeNotSupported, "File is too large for your Document Inbox");
        public static string TextAnalysesEmptyOptions => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesEmptyOptions, "No options selected.");
        public static string TextAnalysesAnalysis => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesAnalysis, "Analysis");
        public static string TextAnalysesAnalysisWithName => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesAnalysisWithName, "Analysis %@");
        public static string TextAnalysesBackToAnalysis => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesBackToAnalysis, "Back to Analysis");
        public static string TextAnalysesDrilldown => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesDrilldown, "Drilldown:%@");
        public static string TextAnalysesDetails => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesDetails, "Details");
        public static string TextAnalysesOther => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesOther, "Other");
        public static string TextAnalysesNoValue => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesNoValue, "no value");
        public static string TextAnalysesShowParam => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesShowParam, "Show %@");
        public static string TextAnalysesQueryWithName => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesQueryWithName, "Query %@");
        public static string TextAnalysesQuery => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesQuery, "Query");
        public static string TextAnalysesReset => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesReset, "Reset");
        public static string TextAnalysesSum => Localize(LocalizationKeys.TextGroupAnalyses, LocalizationKeys.KeyAnalysesSum, "SUM");
        public static string TextErrorInvalidTime => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsInvalidTimeSelection, "Start time must not be greater than end time");
        public static string TextErrorInvalidDate => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsInvalidDateSelection, "Start date must not be greater than end date");
        public static string TextErrorFieldUpdateDenied => Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsFileAccessDenied, "Update for field(s) %@ denied due to access rights");
        public static string TextAddPhoto => Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicAddPhoto, "Add Photo");

        /// <summary>
        /// The text error log purge.
        /// </summary>
        public static string TextErrorLogPurge =>
            Localize(
                LocalizationKeys.TextGroupClientUIText,
                LocalizationKeys.KeyClientUIErrorLogPurge,
                "Old log records have been purged");

        /// <summary>
        /// The text error log purge detail.
        /// </summary>
        public static string TextErrorLogPurgeDetail =>
            Localize(
                LocalizationKeys.TextGroupClientUIText,
                LocalizationKeys.KeyClientUIErrorLogPurgeDetail,
                "The size of Log File had reached its limit of {0} MBytes, so old log records have been purged");

        /// <summary>
        /// Gets localized text for Check all button in filter page
        /// </summary>
        public static string KeyClientUICheckAll =>
            Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUICheckAll, "Check All");

        /// <summary>
        /// Gets localized text for Check all button in filter page
        /// </summary>
        public static string KeyClientUIClearAll =>
            Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIClearAll, "Clear All");

        /// <summary>
        /// Gets localized text for Check all button in filter page
        /// </summary>
        public static string KeyClientUIEnableAll =>
            Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIEnableAll, "Enable All");

        /// <summary>
        /// Gets localized text for Check all button in filter page
        /// </summary>
        public static string KeyClientUIDisableAll =>
            Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIDisableAll, "Disable All");

        /// <summary>
        /// Gets localized text matches count in filter page
        /// </summary>
        public static string KeyClientUIMatchesFound =>
            Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIMatchesFound, "matches found");

        /// <summary>
        /// Gets a value of Discard All button text
        /// </summary>
        public static string TextDiscardAll => Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIDiscardAll, "Discard All");

        /// <summary>
        /// Gets a value of Delete Selected button text
        /// </summary>
        public static string TextDeleteSelected => Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIDeleteSelected, "Delete Selected");

        /// <summary>
        /// Gets a value of edit Selected button text
        /// </summary>
        public static string TextEditSelected => Localize(LocalizationKeys.TextGroupClientUIText, LocalizationKeys.KeyClientUIEditSelected, "Edit Selected");
    }
}
