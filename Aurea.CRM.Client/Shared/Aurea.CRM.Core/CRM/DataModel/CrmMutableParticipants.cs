// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmMutableParticipants.cs" company="Aurea Software Gmbh">
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
//   The CRM Mutable Participants class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;
    using Delegates;

    /// <summary>
    /// Mutable Participants class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.DataModel.UPCRMParticipants" />
    public class UPCRMMutableParticipants : UPCRMParticipants
    {
        /// <summary>
        /// The mutable rep participant array
        /// </summary>
        protected List<UPCRMRepParticipant> mutableRepParticipantArray;

        /// <summary>
        /// The mutable link participant array
        /// </summary>
        protected List<UPCRMLinkParticipant> mutableLinkParticipantArray;

        /// <summary>
        /// The next new participant index
        /// </summary>
        protected int nextNewParticipantIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMMutableParticipants"/> class.
        /// </summary>
        public UPCRMMutableParticipants()
           : this(null, null, null, null, -1, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMMutableParticipants"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="rootInfoAreaId">The root information area identifier.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="linkParticipantsInfoAreaId">The link participants information area identifier.</param>
        /// <param name="linkParticipantsLinkId">The link participants link identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMMutableParticipants(ViewReference viewReference, string rootInfoAreaId, string recordIdentification,
            string linkParticipantsInfoAreaId, int linkParticipantsLinkId, UPCRMParticipantsDelegate theDelegate)
            : base(viewReference, rootInfoAreaId, linkParticipantsInfoAreaId, linkParticipantsLinkId, recordIdentification, theDelegate)
        {
            this.mutableLinkParticipantArray = new List<UPCRMLinkParticipant>();
            this.mutableRepParticipantArray = new List<UPCRMRepParticipant>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMMutableParticipants"/> class.
        /// </summary>
        /// <param name="participantString">The participant string.</param>
        public UPCRMMutableParticipants(string participantString)
            : base(participantString)
        {
            this.mutableRepParticipantArray = new List<UPCRMRepParticipant>(this.Participants.Select(x => x as UPCRMRepParticipant));
        }

        /// <summary>
        /// Gets the link participants.
        /// </summary>
        /// <value>
        /// The link participants.
        /// </value>
        public override List<UPCRMLinkParticipant> LinkParticipants => this.mutableLinkParticipantArray;

        /// <summary>
        /// Gets the rep participants.
        /// </summary>
        /// <value>
        /// The rep participants.
        /// </value>
        public override List<UPCRMRepParticipant> RepParticipants => this.mutableRepParticipantArray;

        /// <summary>
        /// Adds the rep participant with rep identifier.
        /// </summary>
        /// <param name="repId">The rep identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public UPCRMRepParticipant AddRepParticipantWithRepId(string repId, Dictionary<string, object> options)
        {
            UPCRMRepParticipant repParticipant = new UPCRMRepParticipant(repId);
            repParticipant.Options = options;
            this.AddRepParticipant(repParticipant);

            return repParticipant;
        }

        /// <summary>
        /// Adds the link participant with record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public UPCRMLinkParticipant AddLinkParticipantWithRecordIdentification(string recordIdentification, Dictionary<string, object> options)
        {
            string name = UPConfigTableCaption.TableCaptionForRecordIdentification(recordIdentification, "Default");
            UPCRMLinkParticipant linkParticipant = new UPCRMLinkParticipant(this.LinkParticipantsInfoAreaId, recordIdentification, name);
            linkParticipant.Options = options;
            this.AddLinkParticipant(linkParticipant);

            return linkParticipant;
        }

        /// <summary>
        /// Adds the rep participant.
        /// </summary>
        /// <param name="participant">The participant.</param>
        public void AddRepParticipant(UPCRMRepParticipant participant)
        {
            participant.Context = this;

            if (this.mutableRepParticipantArray == null)
            {
                this.mutableRepParticipantArray = new List<UPCRMRepParticipant>();
            }

            this.mutableRepParticipantArray.Add(participant);
        }

        /// <summary>
        /// Remove the rep participant.
        /// </summary>
        /// <param name="participant">The participant.</param>
        public void RemoveRepParticipant(UPCRMRepParticipant participant)
        {
            participant.Context = this;

            if (this.mutableRepParticipantArray.Contains(participant))
            {
                this.mutableRepParticipantArray.Remove(participant);
            }
        }

        /// <summary>
        /// Adds the link participant.
        /// </summary>
        /// <param name="participant">The participant.</param>
        public void AddLinkParticipant(UPCRMLinkParticipant participant)
        {
            participant.Context = this;

            if (this.mutableLinkParticipantArray == null)
            {
                this.mutableLinkParticipantArray = new List<UPCRMLinkParticipant>();
            }

            this.mutableLinkParticipantArray.Add(participant);
        }

        /// <summary>
        /// Adds the participants from string.
        /// </summary>
        /// <param name="participantsString">The participants string.</param>
        public void AddParticipantsFromString(string participantsString)
        {
            var participants = this.ParticipantsFromString(participantsString);
            foreach (UPCRMRepParticipant participant in participants)
            {
                participant.Context = this;
                this.AddRepParticipant(participant);
            }
        }

        /// <summary>
        /// Marks the participant as deleted.
        /// </summary>
        /// <param name="participant">The participant.</param>
        /// <returns></returns>
        public bool MarkParticipantAsDeleted(UPCRMParticipant participant)
        {
            if (participant.MayNotBeDeleted)
            {
                return false;
            }

            participant.MarkAsDeleted = true;
            return true;
        }

        /// <summary>
        /// Adds the new rep participant.
        /// </summary>
        /// <returns></returns>
        public UPCRMRepParticipant AddNewRepParticipant()
        {
            UPCRMRepParticipant participant = new UPCRMRepParticipant("rep_" + this.NextKey())
            {
                AcceptanceText = "0",
                RequirementText = "0",
                Context = this
            };
            this.AddRepParticipant(participant);

            return participant;
        }

        /// <summary>
        /// Adds the new link participant.
        /// </summary>
        /// <returns></returns>
        public UPCRMLinkParticipant AddNewLinkParticipant()
        {
            UPCRMLinkParticipant participant = new UPCRMLinkParticipant("link_" + this.NextKey())
            {
                AcceptanceText = "0",
                RequirementText = "0",
                Context = this
            };
            this.AddLinkParticipant(participant);

            return participant;
        }

        /// <summary>
        /// Changeds the rep participant acceptance records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRepParticipantAcceptanceRecords()
        {
            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();

            foreach (UPCRMRepParticipant repParticipant in this.RepParticipants)
            {
                UPCRMRecord record = repParticipant.ChangedRepParticipantAcceptanceRecord();
                if (record != null)
                {
                    changedRecords.Add(record);
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// ChangedRepParticipantRequirementRecords.
        /// </summary>
        /// <param name="repId">repId.</param>
        /// <param name="requirementValue">requirementValue.</param>
        public void ChangedRepParticipantRequirementRecords(string repId, string requirementValue)
        {
            var index = this.RepParticipants.FindIndex(r => r.RepIdString.Equals(repId));
            if (index >= 0)
            {
                this.mutableRepParticipantArray[index].RequirementText = requirementValue;
            }
        }

        /// <summary>
        /// Gets the participant string.
        /// </summary>
        /// <value>
        /// The participant string.
        /// </value>
        public override string ParticipantString => this.StringFromParticipants(this.mutableRepParticipantArray);

        private string NextKey()
        {
            return $"par_{++this.nextNewParticipantIndex}";
        }
    }
}
