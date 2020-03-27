// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultRowCalendarItem.cs" company="Aurea Software Gmbh">
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
//   Result Row Calendar Item
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Result Row Calendar Item
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMResultRow" />
    /// <seealso cref="ICalendarItem" />
    public class ResultRowCalendarItem : UPMResultRow, ICalendarItem
    {
        static /*EKEventStore*/ object eventStore;
        static bool eventStoreAccessGranted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultRowCalendarItem" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="resultContext">The result context.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="tableCaptionResultFieldMap">The table caption result field map.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="color">The color.</param>
        /// <exception cref="Exception">StartDate is null</exception>
        public ResultRowCalendarItem(UPCRMResultRow row,
            UPCoreMappingResultContext resultContext,
            IIdentifier identifier,
            UPConfigTableCaption tableCaption, List<UPContainerFieldMetaInfo> tableCaptionResultFieldMap,
            UPConfigCatalogAttributes attributes, AureaColor color)
            : base(identifier)
        {
            this.ResultContext = resultContext;
            var fieldMapping = this.ResultContext.FieldControl.FunctionNames();
            var dict = this.ResultContext.FieldControl.FunctionNames(row);
            string startDateString = dict.ValueOrDefault("Date") as string;
            string startTimeString = dict.ValueOrDefault("Time") as string;
            string endDateString = dict.ValueOrDefault("EndDate") as string;
            string endTimeString = dict.ValueOrDefault("EndTime") as string;

            UPConfigFieldControlField personLabelControlField = fieldMapping.ValueOrDefault("PersonLabel");
            if (personLabelControlField != null)
            {
                this.PersonLabelField = UPMStringField.StringFieldWithIdentifierValue(
                        new FieldIdentifier(row.RootRecordIdentification, personLabelControlField.Field.FieldIdentification),
                        row.FormattedFieldValueAtIndex(personLabelControlField.TabIndependentFieldIndex, null, this.ResultContext.FieldControl));
            }

            UPConfigFieldControlField companyLabelControlField = fieldMapping.ValueOrDefault("CompanyLabel");
            if (companyLabelControlField != null)
            {
                this.CompanyLabelField = UPMStringField.StringFieldWithIdentifierValue(
                        new FieldIdentifier(row.RootRecordIdentification, companyLabelControlField.Field.FieldIdentification),
                        row.FormattedFieldValueAtIndex(companyLabelControlField.TabIndependentFieldIndex, null, this.ResultContext.FieldControl));
            }

            UPConfigFieldControlField field = fieldMapping.ValueOrDefault("Status");
            string status = dict.ValueOrDefault("Status") as string;
            if (field != null)
            {
                this.StatusLabelField = UPMStringField.StringFieldWithIdentifierValue(
                    new FieldIdentifier(row.RootRecordIdentification, field.Field.FieldIdentification),
                    field.Field.ValueForRawValueOptions(status, 0));
            }

            string repLabel = dict.ValueOrDefault("RepLabel") as string;
            if (string.IsNullOrEmpty(repLabel))
            {
                string repId = dict.ValueOrDefault("RepId") as string;
                if (!string.IsNullOrEmpty(repId))
                {
                    repLabel = UPCRMDataStore.DefaultStore.Reps.RepWithId(repId).RepName;
                }
            }

            this.RepLabelField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId("rep"), repLabel);
            int iType = Convert.ToInt32(dict.ValueOrDefault("Type"));
            this.Type = (UPCalendarItemType)iType;
            this.StartDate = StringExtensions.DateFromStrings(startDateString, startTimeString);
            this.EndDate = StringExtensions.DateFromStrings(endDateString, endTimeString);
            this.Identification = identifier.IdentifierAsString;
            this.Color = color;
            this.RowColor = this.Color;
            this.CrmResultRow = row;
            this.IPadCalendarItem = null;
            this.HasTime = !string.IsNullOrEmpty(startTimeString);
            this.HasEndTime = !string.IsNullOrEmpty(endTimeString);

            UPConfigCatalogValueAttributes temp = attributes?.ValuesByCode[iType];
            if (temp != null)
            {
                if (this.Color == null)
                {
                    string colorString = temp.ColorKey;
                    if (string.IsNullOrEmpty(colorString))
                    {
                        this.Color = AureaColor.ColorWithString(colorString);
                    }
                }

                this.ImageName = temp.ImageName;
            }

            if (tableCaption != null)
            {
                this.Subject = tableCaption.TableCaptionForResultRow(row, tableCaptionResultFieldMap);
            }
            else
            {
                this.Subject = dict.ValueOrDefault("Subject") as string;
            }

            if (this.StartDate == DateTime.MinValue)
            {
                throw new Exception("StartDate is null");
            }

            if (this.EndDate == DateTime.MinValue)
            {
                this.EndDate = this.StartDate;
            }

            if (this.Type < UPCalendarItemType.Color0 || this.Type >= UPCalendarItemType.ColorCount)
            {
                this.Type = UPCalendarItemType.Color0;
            }

            //this.AllDay = this.DetermineIsAllDay();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultRowCalendarItem"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="iPadCalendarItem">The i pad calendar item.</param>
        public ResultRowCalendarItem(IIdentifier identifier, /*EKEvent*/ dynamic iPadCalendarItem)
            : base(identifier)
        {
            this.IPadCalendarItem = iPadCalendarItem;
            this.CrmValue = false;
            this.StartDate = iPadCalendarItem.StartDate();
            this.EndDate = iPadCalendarItem.EndDate();
            this.HasTime = true;
            this.HasEndTime = true;
            this.RowColor = iPadCalendarItem.Color;
            this.Subject = iPadCalendarItem.Subject;
            this.Type = UPCalendarItemType.SystemCalendar;
            this.Icon = null; // UIImage.ImageGlyphWithGlyphicons(UPGlyphiconsIpad);    // CRM-5007
            this.Fields = new List<UPMField>();
            this.StyleId = "ipadCalendar";
            this.Invalid = true;
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public UPCalendarItemType Type { get; private set; }

        /// <summary>
        /// Gets or sets the result row.
        /// </summary>
        public UPMResultRow ResultRow { get; set; }

        /// <summary>
        /// Gets or sets the go to action.
        /// </summary>
        public UPMAction GoToAction { get; set; }

        /// <summary>
        /// Gets or sets the edit action.
        /// </summary>
        public UPMAction EditAction { get; set; }

        /// <summary>
        /// Gets the identification.
        /// </summary>
        public string Identification { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether all day.
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public AureaColor Color { get; private set; }

        /// <summary>
        /// Gets the image name.
        /// </summary>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets or sets the crm result row.
        /// </summary>
        public UPCRMResultRow CrmResultRow { get; set; }

        /// <summary>
        /// Gets the company label field.
        /// </summary>
        public UPMField CompanyLabelField { get; private set; }

        /// <summary>
        /// Gets the person label field.
        /// </summary>
        public UPMField PersonLabelField { get; private set; }

        /// <summary>
        /// Gets the status label field.
        /// </summary>
        public UPMField StatusLabelField { get; private set; }

        /// <summary>
        /// Gets the rep label field.
        /// </summary>
        public UPMField RepLabelField { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [CRM value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [CRM value]; otherwise, <c>false</c>.
        /// </value>
        public bool CrmValue { get; set; }

        /// <summary>
        /// Gets the result context.
        /// </summary>
        /// <value>
        /// The result context.
        /// </value>
        public UPCoreMappingResultContext ResultContext { get; private set; }

        /// <summary>
        /// Gets the i pad calendar item.
        /// </summary>
        /// <value>
        /// The i pad calendar item.
        /// </value>
        public /*EKEvent*/ dynamic IPadCalendarItem { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has time.
        /// </summary>
        public bool HasTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has end time.
        /// </summary>
        public bool HasEndTime { get; private set; }

        /// <summary>
        /// Gets or sets the calendar search.
        /// </summary>
        public UPCalendarSearch CalendarSearch { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public override string Key
        {
            get
            {
                if (this.Identifier is RecordIdentifier)
                {
                    return ((RecordIdentifier)this.Identifier).RecordIdentification;
                }

                if (this.Identifier is StringIdentifier)
                {
                    return ((StringIdentifier)this.Identifier).ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Updates the ek event values.
        /// </summary>
        public void UpdateEKEventValues()
        {
            UPResultRowEventStoreFormatter formatter = new UPResultRowEventStoreFormatter();
            List<string> stringArray = null; // formatter.ListFieldValuesFromEvent(this.IPadCalendarItem);
            List<UPMField> listFields = new List<UPMField>(stringArray.Count);
            foreach (string s in stringArray)
            {
                UPMStringField textField = new UPMStringField(StringIdentifier.IdentifierWithStringId("Field"));
                textField.FieldValue = s;
                listFields.Add(textField);
            }

            this.Fields = listFields;
            this.Invalid = false;
        }

        /// <summary>
        /// Events the store initialized.
        /// </summary>
        /// <returns></returns>
        public static bool EventStoreInitialized => eventStore != null;

#if PORTING
    static void FunctionName(EKEventStoreRequestAccessCompletionHandler completion)
    {
        if (eventStore)
        {
            completion(eventStoreAccessGranted, null);
            return;
        }

        eventStore = EKEventStore.TheNew();
        if (eventStore.RespondsToSelector(@selector(requestAccessToEntityType: completion:)))
        {
            eventStore.RequestAccessToEntityTypeCompletion(EKEntityTypeEvent, delegate (bool granted, NSError error)
            {
                eventStoreAccessGranted = granted;
                completion(granted, error);
            });
        }
        else
        {
            eventStoreAccessGranted = true;
            completion(true, null);
        }
    }

    static ArrayList EventsFromLocalCalendarFromToSearchTextCalenderIdentifiers(DateTime from, DateTime to, string searchText, ArrayList calenderIdentifiers)
    {
        ArrayList calendarItems = NSMutableArray.TheNew();
        if (!eventStore)
        {
            return null;
        }

        NSDate now = new NSDate();
        bool alwaysApplyStartStringLimit = false;
        string fromStartString = UPConfigurationUnitStore.DefaultStore().ConfigValueDefaultValue("Calendar.StartDateLimit", "$curfdMonth-1m");
        if (fromStartString.HasPrefix("!"))
        {
            alwaysApplyStartStringLimit = true;
            fromStartString = fromStartString.SubstringFromIndex(1);
        }

        if (fromStartString.Length && (alwaysApplyStartStringLimit || !from))
        {
            if (fromStartString.HasPrefix("$"))
            {
                fromStartString = fromStartString.ReplaceDateVariables();
            }

            NSDate maxFromDate = fromStartString.DateFromCrmValue();
            if (!from || (alwaysApplyStartStringLimit && from.Compare(maxFromDate) == NSOrderedAscending))
            {
                from = maxFromDate;
            }
        }

        if (!from)
        {
            NSDateComponents dayComponent = new NSDateComponents();
            dayComponent.Year = -1;
            NSCalendar theCalendar = NSCalendar.CurrentCalendar();
            from = theCalendar.DateByAddingComponentsToDateOptions(dayComponent, now, 0);
        }

        if (!to)
        {
            NSDateComponents dayComponent = new NSDateComponents();
            dayComponent.Year = 1;
            NSCalendar theCalendar = NSCalendar.CurrentCalendar();
            to = theCalendar.DateByAddingComponentsToDateOptions(dayComponent, now, 0);
        }

        ArrayList calendars = null;
        if (calenderIdentifiers != null)
        {
            calendars = new ArrayList();
            foreach (string calendarIdentifier in calenderIdentifiers)
            {
                EKCalendar calendar = eventStore.UpCalendarWithIdentifier(calendarIdentifier);
                if (calendar != null)
                {
                    calendars.Add(calendar);
                }
            }
        }

        if (calendars != null && calendars.Count == 0)
        {
            return null;
        }

        NSPredicate pred = eventStore.PredicateForEventsWithStartDateEndDateCalendars(from, to, calendars);
        ArrayList iosEvents = eventStore.EventsMatchingPredicate(pred);
        foreach (EKEvent iosEvent in iosEvents)
        {
            if (searchText.Length > 0)
            {
                NSRange range = iosEvent.Title.RangeOfStringOptions(searchText, NSCaseInsensitiveSearch);
                if (range.Length == 0)
                {
                    continue;
                }
            }

            calendarItems.Add(iosEvent);
        }

        return calendarItems.Count ? calendarItems : null;
    }

    static NSDictionary IPadCalendars()
    {
        NSMutableDictionary iPadCalendersDictinary = NSMutableDictionary.TheNew();
        ArrayList iPadCalenders = eventStore.CalendarsForEntityType(EKEntityTypeEvent);
        foreach (EKCalendar calendar in iPadCalenders)
        {
            iPadCalendersDictinary.SetObjectForKey(calendar.Title, calendar.CalendarIdentifier);
        }

        return iPadCalendersDictinary;
    }

        static AureaColor IPadCalendarColor(string calendarIdentifier)
        {
            //EKCalendar calendar = eventStore.UpCalendarWithIdentifier(calendarIdentifier);
            //return calendar != null ? UIColor.ColorWithCGColor(calendar.CGColor) : AureaColor.LightGrayColor();
            return null;
        }

        public static void RefreshEventStore()
        {
            //eventStore?.RefreshSourcesIfNecessary();
        }

        void SetIcon(UIImage icon)
        {
            UIImage image = null;
            if (type == SystemCalendar)
            {
                image = UIImage.ImageGlyphWithGlyphicons(UPGlyphiconsIpad);
            }
            else
            {
                image = UIImage.UpImageWithFileName(this.ImageName);
            }

            if (image == null)
            {
                image = icon;
            }

            this.Icon = image;
        }

        bool DetermineIsAllDay()
        {
            NSCalendar cal = NSCalendar.CurrentCalendar();
            int diffDays = (int)cal.ComponentsFromDateToDateOptions(NSCalendarUnitDay, startDate, endDate, 0).Day;
            NSDateComponents dateComp = cal.ComponentsFromDate(NSCalendarUnitHour | NSCalendarUnitMinute, endDate);
            if (dateComp.Hour == 0 && dateComp.Minute == 0)
            {
                diffDays--;
            }

            return diffDays > 0;
        }

        static string SectionKeyForDateDateFormatter(DateTime date, NSDateFormatter dateFormatter)
        {
            return dateFormatter.StringFromDate(date);
        }
    }
#endif
    }

    public static class Extensions
    {
        /// <summary>
        /// Results the sections for sorted data.
        /// </summary>
        /// <param name="calendarItems">The calendar items.</param>
        /// <param name="resultContext">The result context.</param>
        /// <param name="_delegate">The delegate.</param>
        /// <returns></returns>
        public static List<UPMResultSection> ResultSectionsForSortedData(this List<ICalendarItem> calendarItems, UPCoreMappingResultContext resultContext, IResultRowProviderDelegate _delegate)
        {
            List<UPMResultSection> resultSections = new List<UPMResultSection>();
            Dictionary<string, UPMResultSection> sectionDictionary = new Dictionary<string, UPMResultSection>();

            foreach (ICalendarItem item in calendarItems)
            {
                string currentSectionKey = item.StartDate.ReportFormattedDate();
                UPMResultSection resultSection = sectionDictionary.ValueOrDefault(currentSectionKey);
                if (resultSection == null)
                {
                    resultSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId($"Result_Section_{currentSectionKey}"), new UPMResultRowProviderForCalendarResult(_delegate, resultContext))
                    {
                        SectionField = new UPMField(StringIdentifier.IdentifierWithStringId("SectionLabel"))
                    };
                    resultSection.SectionField.FieldValue = currentSectionKey;
                    resultSection.SectionIndexKey = null;
                    resultSections.Add(resultSection);
                    sectionDictionary[currentSectionKey] = resultSection;
                }

                ((UPMResultRowProviderForCalendarResult)resultSection.ResultRowProvider).AddValue(item);
            }

            return resultSections;
        }

        /// <summary>
        /// Results the sections for data.
        /// </summary>
        /// <param name="calendarItems">The calendar items.</param>
        /// <param name="resultContext">The result context.</param>
        /// <param name="_delegate">The delegate.</param>
        /// <returns></returns>
        public static List<UPMResultSection> ResultSectionsForData(this List<ICalendarItem> calendarItems, UPCoreMappingResultContext resultContext,
            IResultRowProviderDelegate _delegate)
        {
            return ResultSectionsForSortedData(calendarItems, resultContext, _delegate);
        }

        /// <summary>
        /// Sorts the specified ascending.
        /// </summary>
        /// <param name="calendarItems">The calendar items.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        /// <returns></returns>
        public static List<ICalendarItem> Sort(this List<ICalendarItem> calendarItems, bool ascending)
        {
            return ascending
                ? calendarItems.OrderBy(x => x, new CalendarItemComparer()).ToList()
                : calendarItems.OrderByDescending(x => x, new CalendarItemComparer()).ToList();
        }

        /// <summary>
        /// Calendar Item Comparer
        /// </summary>
        /// <seealso cref="ICalendarItem" />
        private class CalendarItemComparer : IComparer<ICalendarItem>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="item1"/> and <paramref name="item2"/>, as shown in the following table.
            /// Value Meaning Less than zero<paramref name="item1"/> is less than <paramref name="item2"/>.
            /// Zero<paramref name="item1"/> equals <paramref name="item2"/>.
            /// Greater than zero<paramref name="item1"/> is greater than <paramref name="item2"/>.
            /// </returns>
            /// <param name="item1">The first object to compare.</param><param name="item2">The second object to compare.</param>
            public int Compare(ICalendarItem item1, ICalendarItem item2)
            {
                int result = DateTime.Compare(item1.StartDate, item2.StartDate);
                if (result == 0)
                {
                    bool all1 = item1.AllDay;
                    bool all2 = item2.AllDay;

                    if (all1 && !all2)
                    {
                        return -1;
                    }

                    if (all2 && !all1)
                    {
                        return 1;
                    }

                    return 0;
                }

                return result;
            }
        }
    }
}
