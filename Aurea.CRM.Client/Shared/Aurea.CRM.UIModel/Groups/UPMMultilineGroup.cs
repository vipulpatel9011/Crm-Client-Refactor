// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MMultilineGroup.cs" company="Aurea Software Gmbh">
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
//   IMplements a group that spans multiple lines
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// IMplements a group that spans multiple lines
    /// </summary>
    /// <seealso cref="UPMGroup" />
    public class UPMMultilineGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMMultilineGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMMultilineGroup(IIdentifier identifier)
            : base(identifier)
        {
            this.Html = false;
        }

        /// <summary>
        /// </summary>
        /// <value>
        ///   <c>true</c> if HTML; otherwise, <c>false</c>.
        /// </value>
        public bool Html { get; set; }

        /// <summary>
        /// Gets or sets the multiline string field.
        /// </summary>
        /// <value>
        /// The multiline string field.
        /// </value>
        public UPMStringField MultilineStringField
        {
            get
            {
                if (this.Children.Count > 0)
                {
                    return (UPMStringField)this.FieldAtIndex(0);
                }

                return null;
            }

            set
            {
                if (this.Children.Count > 0)
                {
                    this.RemoveAllChildren();
                }

                this.AddChild(value);
            }
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public override void AddField(UPMField field)
        {
            if (this.Children == null || this.Children.Count > 0)
            {
                return;
            }

            this.AddChild(field);
        }
    }
}
