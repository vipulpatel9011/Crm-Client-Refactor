// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCRMRepParticipant.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Max Menezes
// </author>
// <summary>
//   Query configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using Extensions;

    /// <summary>
    /// Rep Participant class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.DataModel.UPCRMParticipant" />
    public class UPCRMRepParticipant : UPCRMParticipant
    {
        private string _participantString;

        /// <summary>
        /// The original acceptance
        /// </summary>
        protected string _originalAcceptance;

        /// <summary>
        /// Gets the rep identifier.
        /// </summary>
        /// <value>
        /// The rep identifier.
        /// </value>
        public int RepId { get; private set; }

        /// <summary>
        /// Gets the rep group identifier.
        /// </summary>
        /// <value>
        /// The rep group identifier.
        /// </value>
        public int RepGroupId { get; private set; }

        /// <summary>
        /// Gets the participant string.
        /// </summary>
        /// <value>
        /// The participant string.
        /// </value>
        public string ParticipantString
        {
            get
            {
                this.UpdateParticipantString();
                return this._participantString;
            }
        }

        /// <summary>
        /// Gets or sets the requirement text.
        /// </summary>
        /// <value>
        /// The requirement text.
        /// </value>
        public override string RequirementText
        {
            get
            {
                return base.RequirementText;
            }

            set
            {
                base.RequirementText = value;
                if (!string.IsNullOrEmpty(value))
                {
                    this.UpdateParticipantString();
                }
            }
        }

        /// <summary>
        /// Gets the rep identifier string.
        /// </summary>
        /// <value>
        /// The rep identifier string.
        /// </value>
        public string RepIdString { get; private set; }

        /// <summary>
        /// Gets the acceptance record identification.
        /// </summary>
        /// <value>
        /// The acceptance record identification.
        /// </value>
        public string AcceptanceRecordIdentification { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRepParticipant"/> class.
        /// </summary>
        /// <param name="participantString">The participant string.</param>
        public UPCRMRepParticipant(string participantString)
            : base(GetKey(participantString))
        {
            string[] arr = participantString.Split(':');

            var requirementText = arr.Length > 1 ? arr[1] : "0";

            arr = arr[0].Split(',');

            this.RequirementText = requirementText;
            this.AcceptanceText = "0";
            this._participantString = participantString;

            this.RepGroupId = arr.Length > 1 ? arr[1].RepId() : 0;

            this.RepId = this.Key.RepId();
            this.RepIdString = UPCRMReps.FormattedRepId($"{this.RepId}");
        }

        /// <summary>
        /// Changes the rep.
        /// </summary>
        /// <param name="_repIdString">The rep identifier string.</param>
        /// <returns></returns>
        public bool ChangeRep(string _repIdString)
        {
            UPCRMRep rep = UPCRMDataStore.DefaultStore.Reps.RepWithId(_repIdString);
            if (rep == null)
            {
                return false;
            }

            this.RepId = rep.RepId.RepId();
            this.RepGroupId = rep.RepOrgGroupId.RepId();
            if (!string.IsNullOrEmpty(this._participantString))
            {
                this.UpdateParticipantString();
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this instance can change acceptance state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can change acceptance state; otherwise, <c>false</c>.
        /// </value>
        public override bool CanChangeAcceptanceState => !string.IsNullOrEmpty(this.AcceptanceRecordIdentification);

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMParticipant" /> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public override bool Empty => this.RepId == 0;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => UPCRMDataStore.DefaultStore.Reps.NameOfRepId(this.RepId);

        /// <summary>
        /// Acceptances from record identification.
        /// </summary>
        /// <param name="acceptance">The acceptance.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void AcceptanceFromRecordIdentification(string acceptance, string recordIdentification)
        {
            this.AcceptanceRecordIdentification = recordIdentification;
            this._originalAcceptance = acceptance;
            this.AcceptanceText = acceptance;
        }

        /// <summary>
        /// Changeds the rep participant acceptance record.
        /// </summary>
        /// <returns></returns>
        public UPCRMRecord ChangedRepParticipantAcceptanceRecord()
        {
            if (this.Context.AcceptanceFieldId >= 0 && !string.IsNullOrEmpty(this.AcceptanceRecordIdentification) && this._originalAcceptance != this.AcceptanceText)
            {
                UPCRMRecord record = new UPCRMRecord(this.AcceptanceRecordIdentification);
                record.AddValue(new UPCRMFieldValue(this.AcceptanceText, this._originalAcceptance, record.InfoAreaId, this.Context.AcceptanceFieldId));
                return record;
            }

            return null;
        }

        private static string GetKey(string participantString)
        {
            if (string.IsNullOrEmpty(participantString))
            {
                throw new ArgumentNullException();
            }

            string[] arr = participantString.Split(':');

            arr = arr[0].Split(',');

            return arr[0];
        }

        private void UpdateParticipantString()
        {
            this._participantString = string.IsNullOrEmpty(this.RequirementText) || this.RequirementText == "0"
                ? $"{StringExtensions.NineDigitStringFromRep(this.RepId)},{StringExtensions.NineDigitStringFromRep(this.RepGroupId)}"
                : $"{StringExtensions.NineDigitStringFromRep(this.RepId)},{StringExtensions.NineDigitStringFromRep(this.RepGroupId)}:{this.RequirementText}";
        }
    }
}
