// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordParticipantWriter.cs" company="Aurea Software Gmbh">
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
//   Record Participant Writer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Sync
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Record Participant Writer
    /// </summary>
    public class UPCRMRecordParticipantWriter
    {
        /// <summary>
        /// The delete statement SQL
        /// </summary>
        protected string DeleteStatementSql;

        /// <summary>
        /// The insert statement SQL
        /// </summary>
        protected string InsertStatementSql;

        /// <summary>
        /// The database
        /// </summary>
        protected DatabaseBase Database;

        /// <summary>
        /// The insert statement
        /// </summary>
        protected DatabaseStatement InsertStatement;

        /// <summary>
        /// The delete statement
        /// </summary>
        protected DatabaseStatement DeleteStatement;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets or sets the index of the result.
        /// </summary>
        /// <value>
        /// The index of the result.
        /// </value>
        public int ResultIndex { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordParticipantWriter"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="database">The database.</param>
        public UPCRMRecordParticipantWriter(string infoAreaId, FieldInfo fieldInfo, DatabaseBase database)
        {
            this.InfoAreaId = infoAreaId;
            this.FieldId = fieldInfo.FieldId;
            this.Database = database;
            string tableName = $"CRM_{this.InfoAreaId}_PART_{fieldInfo.DatabaseFieldName}";
            this.InsertStatementSql = $"INSERT INTO {tableName} (recid, nr, repId, repOrgGroupId, attendance) VALUES (?,?,?,?,?)";
            this.DeleteStatementSql = $"DELETE FROM {tableName} WHERE recid = ?";
        }

        /// <summary>
        /// Deletes the rows for record identifier.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns>0 if success, else error number</returns>
        public int DeleteRowsForRecordId(string recordId)
        {
            DatabaseStatement statement;
            if (this.DeleteStatement == null)
            {
                statement = new DatabaseStatement(this.Database);
                if (!statement.Prepare(this.DeleteStatementSql))
                {
                    return -1;
                }

                this.DeleteStatement = statement;
            }
            else
            {
                statement = this.DeleteStatement;
                statement.Reset();
            }

            statement.Bind(1, recordId);
            return statement.Execute();
        }

        /// <summary>
        /// Adds the value for record identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <returns>0 if success, else error number</returns>
        public int AddValueForRecordId(string value, string recordId)
        {
            DatabaseStatement statement;
            int ret = 0;
            if (this.InsertStatement == null)
            {
                statement = new DatabaseStatement(this.Database);
                if (!statement.Prepare(this.InsertStatementSql))
                {
                    return -1;
                }

                this.InsertStatement = statement;
            }
            else
            {
                statement = this.InsertStatement;
            }

            UPCRMParticipants participantFormatter = new UPCRMParticipants(value);
            List<UPCRMRepParticipant> participants = participantFormatter.RepParticipants;
            int participantCount = 0;
            foreach (UPCRMRepParticipant participant in participants)
            {
                statement.Reset();
                statement.Bind(1, recordId);
                statement.Bind(2, participantCount++);
                statement.Bind(3, participant.RepId);
                statement.Bind(4, participant.RepGroupId);
                statement.Bind(5, participant.RequirementText);
                ret = statement.Execute();
                if (ret != 0)
                {
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Updates the value for record identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <returns>0 if success, else error number</returns>
        public int UpdateValueForRecordId(string value, string recordId)
        {
            int ret = this.DeleteRowsForRecordId(recordId);
            return ret != 0 ? ret : this.AddValueForRecordId(value, recordId);
        }
    }
}
