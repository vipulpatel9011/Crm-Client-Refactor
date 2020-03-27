// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultRowEventStoreFormatter.cs" company="Aurea Software Gmbh">
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
//   The Result Row Event Store Formatter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Enum for date row Index
    /// </summary>
    public enum DateRowIndex
    {
        /// <summary>
        /// Empty
        /// </summary>
        Empty,

        /// <summary>
        /// Subject
        /// </summary>
        Subject,

        /// <summary>
        /// Date
        /// </summary>
        Date,

        /// <summary>
        /// Time
        /// </summary>
        Time,

        /// <summary>
        /// End date
        /// </summary>
        EndDate,

        /// <summary>
        /// End time
        /// </summary>
        EndTime,

        /// <summary>
        /// Organizer
        /// </summary>
        Organizer,

        /// <summary>
        /// Participants
        /// </summary>
        Participants,

        /// <summary>
        /// Status
        /// </summary>
        Status
    }

    /// <summary>
    /// Result Row Event Store Formatter
    /// </summary>
    public class UPResultRowEventStoreFormatter
    {
        private List<DateRowIndex> fieldMapper;

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the list formatter.
        /// </summary>
        /// <value>
        /// The list formatter.
        /// </value>
        public UPCRMListFormatter ListFormatter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowEventStoreFormatter"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        public UPResultRowEventStoreFormatter(FieldControl fieldControl)
        {
            this.FieldControl = fieldControl;
            if (this.FieldControl != null)
            {
                this.ListFormatter = new UPCRMListFormatter(fieldControl);
                this.fieldMapper = new List<DateRowIndex>();

                foreach (UPConfigFieldControlField field in this.FieldControl.Fields)
                {
                    switch (field.Function)
                    {
                        case "Subject":
                            this.fieldMapper.Add(DateRowIndex.Subject);
                            break;

                        case "Date":
                            this.fieldMapper.Add(DateRowIndex.Date);
                            break;

                        case "Time":
                            this.fieldMapper.Add(DateRowIndex.Time);
                            break;

                        case "EndDate":
                            this.fieldMapper.Add(DateRowIndex.EndDate);
                            break;

                        case "EndTime":
                            this.fieldMapper.Add(DateRowIndex.EndTime);
                            break;

                        case "Organizer":
                            this.fieldMapper.Add(DateRowIndex.Organizer);
                            break;

                        case "Participants":
                            this.fieldMapper.Add(DateRowIndex.Participants);
                            break;

                        case "Status":
                            this.fieldMapper.Add(DateRowIndex.Status);
                            break;

                        default:
                            this.fieldMapper.Add(DateRowIndex.Empty);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowEventStoreFormatter"/> class.
        /// </summary>
        /// <param name="fieldGroupName">Name of the field group.</param>
        public UPResultRowEventStoreFormatter(string fieldGroupName)
            : this(ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", fieldGroupName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPResultRowEventStoreFormatter"/> class.
        /// </summary>
        public UPResultRowEventStoreFormatter()
            : this("MALocalCalendarFormat")
        {
        }

#if PORTING
        string ParticipantsString(ArrayList participants)
        {
            string participantsString = string.Empty;
            foreach (EKParticipant participant in participants)
            {
                string name = participant.Name;
                if (!string.IsNullOrEmpty(name))
                {
                    participantsString = participantsString.Length == 0 ? name : $"{participantsString}, {name}";
                }
            }

            return participantsString;
        }

        public List<string> ListFieldValuesFromEvent(EKEvent theEvent)
        {
            if (this.FieldControl == null)
            {
                return new List<string> { theEvent.Title, theEvent.StartDate.LocalizedFormattedTime() };
            }

            List<string> fieldValues = new List<string>(fieldMapper.Count);
            foreach (DateRowIndex num in fieldMapper)
            {
                string fieldValue = string.Empty;
                switch (num)
                {
                    case DateRowIndex.Subject:
                        fieldValue = theEvent.Subject;
                        break;
                    case DateRowIndex.Date:
                        fieldValue = theEvent.StartDate.LocalizedFormattedDate();
                        break;
                    case DateRowIndex.Time:
                        fieldValue = theEvent.StartDate.LocalizedFormattedTime();
                        break;
                    case DateRowIndex.EndDate:
                        fieldValue = theEvent.EndDate.LocalizedFormattedDate();
                        break;
                    case DateRowIndex.EndTime:
                        fieldValue = theEvent.EndDate.LocalizedFormattedTime();
                        break;
                    case DateRowIndex.Participants:
                        fieldValue = this.ParticipantsString(theEvent.Attendees);
                        break;
                    case DateRowIndex.Organizer:
                        fieldValue = theEvent.Organizer.Name;
                        break;
                    case DateRowIndex.Status:
                        fieldValue = string.Empty;
                        break;
                }

                if (fieldValue == null)
                {
                    fieldValue = string.Empty;
                }

                fieldValues.Add(fieldValue);
            }

            return this.ListFormatter.PositionStringsFromFieldArray(fieldValues);
        }
#endif
    }
}
