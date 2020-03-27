// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepeatableEditGroup.cs" company="Aurea Software Gmbh">
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
//   UPMRepeatableEditGroup
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// UPMRepeatableEditGroup
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMRepeatableEditGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRepeatableEditGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMRepeatableEditGroup(IIdentifier identifier)
             : base(identifier)
        {
            this.AddingEnabled = true;
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<UPMGroup> Groups => this.Children.Cast<UPMGroup>().ToList();

        /// <summary>
        /// Gets the number of groups.
        /// </summary>
        /// <value>
        /// The number of groups.
        /// </value>
        public int NumberOfGroups => this.Children.Count;

        /// <summary>
        /// Gets or sets the add group label text.
        /// </summary>
        /// <value>
        /// The add group label text.
        /// </value>
        public string AddGroupLabelText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [adding enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [adding enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AddingEnabled { get; set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public override List<UPMField> Fields
        {
            get
            {
                var allFieldsOfGroups = new List<UPMField>();

                foreach (UPMGroup group in this.Groups)
                {
                    allFieldsOfGroups.AddRange(group.Fields);
                }

                return allFieldsOfGroups;
            }
        }

        /// <summary>
        /// Groups at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMGroup GroupAtIndex(int index)
        {
            return this.Groups[index];
        }

        /// <summary>
        /// Groups the of field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public UPMGroup GroupOfField(UPMField field)
        {
            foreach (UPMGroup group in this.Groups)
            {
                var fields = group.Fields;
                if (fields.Contains(field))
                {
                    return group;
                }
            }

            return null;
        }
    }
}
