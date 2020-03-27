// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMContactTimesGroup.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes, Serdar Tepeyurt
// </author>
// <summary>
//   Contact times group
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel;

    /// <summary>
    /// Contact Times Group
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMContactTimesGroup : UPMGroup
    {
        private Dictionary<string, string> timeTypeTitleMapping;
        private List<string> timeTypeTitleSortedKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContactTimesGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMContactTimesGroup(IIdentifier identifier)
            : base(identifier)
        {
            this.timeTypeTitleMapping = new Dictionary<string, string>();
            this.timeTypeTitleSortedKeys = new List<string>();
        }

        /// <summary>
        /// Gets contact times list
        /// </summary>
        /// <returns><see cref="List{ContactTimes}"/></returns>
        public List<UPMContactTime> ContactTimes => this.Children.Cast<UPMContactTime>().ToList();

        /// <summary>
        /// Gets the time typ title sorted keys.
        /// </summary>
        /// <value>
        /// The time typ title sorted keys.
        /// </value>
        public List<string> TimeTypeTitleSortedKeys => this.timeTypeTitleSortedKeys;

        /// <summary>
        /// Adds the type of the title for time.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="timeType">Type of the time.</param>
        public void AddTitleForTimeType(string title, string timeType)
        {
            this.timeTypeTitleMapping[timeType] = title;
            if (!this.timeTypeTitleSortedKeys.Contains(timeType))
            {
                this.timeTypeTitleSortedKeys.Add(timeType);
            }
        }

        /// <summary>
        /// Contacts the time for week day time typ.
        /// </summary>
        /// <param name="weekDay">Week day</param>
        /// <param name="timeType">Time type</param>
        /// <returns><see cref="List{ContactTime}"/></returns>
        public List<UPMContactTime> ContactTimeForWeekDayTimeType(int weekDay, string timeType)
        {
            return this.ContactTimes.Where(a => a.WeekDay == weekDay && a.TimeType == timeType).ToList();
        }

        /// <summary>
        /// Times the type title for time type.
        /// </summary>
        /// <param name="timeType">Time type.</param>
        /// <returns>Time type title</returns>
        public string TimeTypeTitleForTimeType(string timeType)
        {
            return this.timeTypeTitleMapping.ValueOrDefault(timeType);
        }
    }
}
