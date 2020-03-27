// <copyright file="UPMContactTime.cs" company="Aurea Software Gmbh">
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
//   Contact time implementation
// </summary>

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Contact Time
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMContactTime : UPMElement
    {
        private UPMDateTimeEditField fromTime;
        private string timeType;
        private UPMDateTimeEditField toTime;
        private int weekDay;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContactTime"/> class.
        /// </summary>
        /// <param name="weekDay">The week day.</param>
        /// <param name="timeType">The time type.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        public UPMContactTime(int weekDay, string timeType, UPMDateTimeEditField fromTime, UPMDateTimeEditField toTime)
            : base(StringIdentifier.IdentifierWithStringId("ContactTime"))
        {
            this.weekDay = weekDay;
            this.timeType = timeType;
            this.fromTime = fromTime;
            this.toTime = toTime;

            if (this.fromTime == null)
            {
                this.fromTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("fromTime"));
            }

            if (this.toTime == null)
            {
                this.toTime = new UPMDateTimeEditField(StringIdentifier.IdentifierWithStringId("toTime"));
            }
        }

        /// <summary>
        /// Gets from time.
        /// </summary>
        /// <value>
        /// From time.
        /// </value>
        public UPMDateTimeEditField FromTime => this.fromTime;

        /// <summary>
        /// Gets or sets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; set; }

        /// <summary>
        /// Gets the time typ.
        /// </summary>
        /// <value>
        /// The time typ.
        /// </value>
        public string TimeType => this.timeType;

        /// <summary>
        /// Gets to time.
        /// </summary>
        /// <value>
        /// To time.
        /// </value>
        public UPMDateTimeEditField ToTime => this.toTime;

        /// <summary>
        /// Gets the week day.
        /// </summary>
        /// <value>
        /// The week day.
        /// </value>
        public int WeekDay => this.weekDay;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            UPMContactTime contactTime = (UPMContactTime)obj;
            return this.FromTime.DateValue == contactTime.FromTime.DateValue && this.ToTime.DateValue == contactTime.ToTime.DateValue;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"WeekDay: {this.weekDay}, TimeTyp: {this.timeType}, {this.fromTime.DateValue} - {this.toTime.DateValue}";
        }
    }
}
