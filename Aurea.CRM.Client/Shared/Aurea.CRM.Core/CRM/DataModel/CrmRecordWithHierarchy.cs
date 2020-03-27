// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordWithHierarchy.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM record implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// CRM record with hierarchy
    /// </summary>
    /// <seealso cref="UPCRMRecord" />
    public class UPCRMRecordWithHierarchy : UPCRMRecord
    {
        /// <summary>
        /// The children.
        /// </summary>
        private List<object> children;

        /// <summary>
        /// The parents.
        /// </summary>
        private List<UPCRMParentRecordReference> parents;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordWithHierarchy"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        public UPCRMRecordWithHierarchy(UPCRMRecord record)
        {
            this.OnlyLink = false;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<object> Children => new List<object>(this.children);

        /// <summary>
        /// Gets a value indicating whether [only link].
        /// </summary>
        /// <value>
        /// <c>true</c> if [only link]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlyLink { get; private set; }

        /// <summary>
        /// Gets the parents.
        /// </summary>
        /// <value>
        /// The parents.
        /// </value>
        public List<object> Parents
        {
            get
            {
                if (this.parents.Count == 0)
                {
                    return null;
                }

                var parentRecords = new List<object>(this.parents.Count);
                parentRecords.AddRange(from recordReference in this.parents where recordReference.Record != null select recordReference.Record);

                return parentRecords;
            }
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="childRecord">
        /// The child record.
        /// </param>
        public void AddChild(UPCRMRecordWithHierarchy childRecord)
        {
            childRecord.AddParent(this);
            if (this.children == null)
            {
                this.children = new List<object> { childRecord };
            }
            else
            {
                this.children.Add(childRecord);
            }
        }

        /// <summary>
        /// Adds the parent.
        /// </summary>
        /// <param name="parentRecord">
        /// The parent record.
        /// </param>
        public void AddParent(UPCRMRecordWithHierarchy parentRecord)
        {
            var recordReference = new UPCRMParentRecordReference(parentRecord);
            if (this.parents != null)
            {
                this.parents.Add(recordReference);
            }
            else
            {
                this.parents = new List<UPCRMParentRecordReference> { recordReference };
            }
        }

        /// <summary>
        /// Childs the description with prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ChildDescriptionWithPrefix(string prefix)
        {
            var str = new StringBuilder();
            foreach (UPCRMRecordWithHierarchy childRecord in this.Children)
            {
                str.Append($"{prefix}{childRecord.RecordDescription()}");
                str.Append(childRecord.ChildDescriptionWithPrefix($"{prefix}  "));
            }

            return str.ToString();
        }

        /// <summary>
        /// Records the description.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordDescription()
        {
            var str = new StringBuilder($"{this.RecordIdentification} (");
            if (this.parents != null)
            {
                foreach (UPCRMRecordWithHierarchy parentRecord in this.Parents)
                {
                    str.Append($"{parentRecord.RecordIdentification},");
                }
            }

            str.Append(")\n");

            return str.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.RecordDescription()}{this.ChildDescriptionWithPrefix("  ")}\n";
        }
    }
}
