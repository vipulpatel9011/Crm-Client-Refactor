// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkInfo.cs" company="Aurea Software Gmbh">
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
//   Link info definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Linq;

    /// <summary>
    /// Link info definition
    /// </summary>
    public class LinkInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="reverseLinkId">
        /// The reverse link identifier.
        /// </param>
        /// <param name="relationType">
        /// Type of the relation.
        /// </param>
        /// <param name="sourceFieldId">
        /// The source field identifier.
        /// </param>
        /// <param name="destFieldId">
        /// The dest field identifier.
        /// </param>
        /// <param name="linkFieldCount">
        /// The link field count.
        /// </param>
        /// <param name="sourceLinkFieldIds">
        /// The source link field ids.
        /// </param>
        /// <param name="destinationLinkFieldIds">
        /// The destination link field ids.
        /// </param>
        /// <param name="linkFlag">
        /// The link flag.
        /// </param>
        /// <param name="sourceValues">
        /// The source values.
        /// </param>
        /// <param name="destValues">
        /// The dest values.
        /// </param>
        public LinkInfo(
            string infoAreaId,
            string targetInfoAreaId,
            int linkId,
            int reverseLinkId,
            LinkType relationType,
            int sourceFieldId,
            int destFieldId,
            int linkFieldCount,
            int[] sourceLinkFieldIds,
            int[] destinationLinkFieldIds,
            int linkFlag,
            string[] sourceValues,
            string[] destValues)
        {
            this.InfoAreaId = infoAreaId;
            this.TargetInfoAreaId = targetInfoAreaId;
            this.LinkId = linkId;
            this.ReverseLinkId = reverseLinkId;
            this.RelationType = relationType;

            this.IdentName = $"LINK_{targetInfoAreaId}_{linkId}";

            if (sourceFieldId >= 0 && destFieldId >= 0)
            {
                this.SourceFieldId = sourceFieldId;
                this.DestFieldId = destFieldId;
                this.ColumnName = this.IdentName;
                this.HasColumn = false;
            }
            else
            {
                this.SourceFieldId = -1;
                this.DestFieldId = -1;
                if (linkId == 126 || linkId == 127)
                {
                    this.HasColumn = linkId == 126;
                    this.InfoAreaColumnName = "LINK_INFOAREA";
                    this.ColumnName = "LINK_RECORDID";
                }
                else
                {
                    this.HasColumn = this.RelationType == LinkType.MANYTOONE || this.RelationType == LinkType.ONETOONE;
                    this.ColumnName = this.IdentName;
                    this.InfoAreaColumnName = null;
                }
            }

            this.LinkFieldCount = linkFieldCount;

            if (this.LinkFieldCount > 0)
            {
                this.SourceFieldIds = sourceLinkFieldIds.ToArray();
                this.DestFieldIds = destinationLinkFieldIds.ToArray();
            }

            this.LinkFlag = linkFlag;

            if (sourceValues != null)
            {
                this.SourceValues = new string[this.LinkFieldCount];

                for (var i = 0; i < this.LinkFieldCount; i++)
                {
                    var sourceValue = sourceValues[i];

                    if (!string.IsNullOrEmpty(sourceValue))
                    {
                        if (sourceValue == targetInfoAreaId)
                        {
                            sourceValue = infoAreaId; // strcmp (infoAreaId, "KP") ? infoAreaId : "CP";
                        }

                        this.SourceValues[i] = sourceValue;
                    }
                    else
                    {
                        this.SourceValues[i] = null;
                    }
                }
            }
            else
            {
                this.SourceValues = null;
            }

            if (destValues != null)
            {
                this.DestValues = new string[this.LinkFieldCount];

                for (var i = 0; i < this.LinkFieldCount; i++)
                {
                    var destValue = destValues[i];

                    if (!string.IsNullOrEmpty(destValues[i]))
                    {
                        if (!destValue.Equals(infoAreaId))
                        {
                            destValue = targetInfoAreaId; // strcmp (targetInfoAreaId, "KP") ? targetInfoAreaId : "CP";
                        }

                        this.DestValues[i] = destValue;
                    }
                    else
                    {
                        this.DestValues[i] = null;
                    }
                }
            }
            else
            {
                this.DestValues = null;
            }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets the dest field identifier.
        /// </summary>
        /// <value>
        /// The dest field identifier.
        /// </value>
        public int DestFieldId { get; private set; }

        /// <summary>
        /// Gets or sets the dest field ids.
        /// </summary>
        /// <value>
        /// The dest field ids.
        /// </value>
        public int[] DestFieldIds { get; set; }

        /// <summary>
        /// Gets or sets the dest values.
        /// </summary>
        /// <value>
        /// The dest values.
        /// </value>
        public string[] DestValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has column; otherwise, <c>false</c>.
        /// </value>
        public bool HasColumn { get; set; }

        /// <summary>
        /// Gets or sets the name of the ident.
        /// </summary>
        /// <value>
        /// The name of the ident.
        /// </value>
        public string IdentName { get; set; }

        /// <summary>
        /// Gets or sets the name of the information area column.
        /// </summary>
        /// <value>
        /// The name of the information area column.
        /// </value>
        public string InfoAreaColumnName { get; set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is child link.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is child link; otherwise, <c>false</c>.
        /// </value>
        public bool IsChildLink => this.RelationType == LinkType.ONETOMANY;

        /// <summary>
        /// Gets a value indicating whether this instance is field link.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is field link; otherwise, <c>false</c>.
        /// </value>
        public bool IsFieldLink => this.SourceFieldId >= 0;

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is generic; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneric => this.LinkId == 126 || this.LinkId == 127;

        /// <summary>
        /// Gets a value indicating whether this instance is ident link.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is ident link; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentLink => this.RelationType == LinkType.IDENT;

        /// <summary>
        /// Gets or sets the link field count.
        /// </summary>
        /// <value>
        /// The link field count.
        /// </value>
        public int LinkFieldCount { get; set; }

        /// <summary>
        /// Gets or sets the link flag.
        /// </summary>
        /// <value>
        /// The link flag.
        /// </value>
        public int LinkFlag { get; set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets or sets the type of the relation.
        /// </summary>
        /// <value>
        /// The type of the relation.
        /// </value>
        public LinkType RelationType { get; set; }

        /// <summary>
        /// Gets the reverse link identifier.
        /// </summary>
        /// <value>
        /// The reverse link identifier.
        /// </value>
        public int ReverseLinkId { get; private set; }

        /// <summary>
        /// Gets the source field identifier.
        /// </summary>
        /// <value>
        /// The source field identifier.
        /// </value>
        public int SourceFieldId { get; private set; }

        /// <summary>
        /// Gets or sets the source field ids.
        /// </summary>
        /// <value>
        /// The source field ids.
        /// </value>
        public int[] SourceFieldIds { get; set; }

        /// <summary>
        /// Gets or sets the source values.
        /// </summary>
        /// <value>
        /// The source values.
        /// </value>
        public string[] SourceValues { get; set; }

        /// <summary>
        /// Gets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public string TargetInfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use link fields].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use link fields]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLinkFields => this.LinkFlag == 1;

        /// <summary>
        /// Gets the type of the reverse relation.
        /// </summary>
        /// <param name="relationType">
        /// Type of the relation.
        /// </param>
        /// <returns>
        /// The <see cref="LinkType"/>.
        /// </returns>
        public static LinkType GetReverseRelationType(LinkType relationType)
        {
            switch (relationType)
            {
                case LinkType.IDENT:
                    return LinkType.IDENT;
                case LinkType.PARENT:
                    return LinkType.CHILD;
                case LinkType.CHILD:
                    return LinkType.PARENT;
                case LinkType.ONETOONE:
                    return LinkType.PARENT;
                default:
                    return LinkType.UNKNOWN;
            }
        }

        /// <summary>
        /// To the type of the link.
        /// </summary>
        /// <param name="relationType">
        /// Type of the relation.
        /// </param>
        /// <returns>
        /// The <see cref="LinkType"/>.
        /// </returns>
        public static LinkType ToLinkType(string relationType)
        {
            switch (relationType)
            {
                case "OneToMany":
                    return LinkType.ONETOMANY;

                case "ManyToOne":
                    return LinkType.MANYTOONE;

                case "OneToOne":
                    return LinkType.ONETOONE;

                case "Identity":
                    return LinkType.IDENT;

                case "Child":
                    return LinkType.CHILD;

                case "Parent":
                    return LinkType.PARENT;
            }

            return LinkType.UNKNOWN;
        }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo CreateCopy()
        {
            return new LinkInfo(
                this.InfoAreaId,
                this.TargetInfoAreaId,
                this.LinkId,
                this.ReverseLinkId,
                this.RelationType,
                this.SourceFieldId,
                this.DestFieldId,
                this.LinkFieldCount,
                this.SourceFieldIds,
                this.DestFieldIds,
                this.LinkFlag,
                this.SourceValues,
                this.DestValues);
        }

        /// <summary>
        /// Creates the virtual reverse link.
        /// </summary>
        /// <returns>
        /// The <see cref="LinkInfo"/>.
        /// </returns>
        public LinkInfo CreateVirtualReverseLink()
        {
            // creates a reverselink in case the reverse link does not exist in the data model
            var reverseRelationType = GetReverseRelationType(this.RelationType);
            if (reverseRelationType == LinkType.UNKNOWN)
            {
                return null;
            }

            return new LinkInfo(
                this.TargetInfoAreaId,
                this.InfoAreaId,
                this.ReverseLinkId,
                this.LinkId,
                reverseRelationType,
                this.DestFieldId,
                this.SourceFieldId,
                this.LinkFieldCount,
                this.DestFieldIds,
                this.SourceFieldIds,
                this.LinkFlag,
                this.DestValues,
                this.SourceValues);
        }

        /// <summary>
        /// Drops the virtual reverse link.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        public void DropVirtualReverseLink(LinkInfo link)
        {
            // delete link;
        }

        /// <summary>
        /// Gets the index of the destination field identifier with.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetDestinationFieldIdWithIndex(int index)
        {
            return this.DestFieldIds[index];
        }

        /// <summary>
        /// Gets the index of the destination value with.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDestinationValueWithIndex(int index)
        {
            return this.DestValues?[index];
        }

        /// <summary>
        /// Gets the name of the physical column.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetPhysicalColumnName()
        {
            return this.RelationType == LinkType.IDENT ? "recid" : this.ColumnName;
        }

        /// <summary>
        /// Gets the index of the source field identifier with.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetSourceFieldIdWithIndex(int index)
        {
            return this.SourceFieldIds[index];
        }

        /// <summary>
        /// Gets the index of the source value with.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetSourceValueWithIndex(int index)
        {
            return this.SourceValues?[index];
        }
    }
}
