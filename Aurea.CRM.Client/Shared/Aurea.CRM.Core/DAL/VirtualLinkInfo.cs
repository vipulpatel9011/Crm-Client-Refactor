// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VirtualLinkInfo.cs" company="Aurea Software Gmbh">
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
//   Virtual link type
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// Virtual link type
    /// </summary>
    public enum VirtualLinkType
    {
        /// <summary>
        /// The dont move.
        /// </summary>
        DontMove = 0,

        /// <summary>
        /// The move from source.
        /// </summary>
        MoveFromSource = 1,

        /// <summary>
        /// The move from target.
        /// </summary>
        MoveFromTarget = 2
    }

    /// <summary>
    /// Virtual link info
    /// </summary>
    public class VirtualLinkInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualLinkInfo"/> class.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="moveLinks">
        /// The move links.
        /// </param>
        /// <param name="intermediateInfoAreaId">
        /// The intermediate information area identifier.
        /// </param>
        /// <param name="sourceLinkId">
        /// The source link identifier.
        /// </param>
        /// <param name="targetLinkId">
        /// The target link identifier.
        /// </param>
        public VirtualLinkInfo(
            DataModel dataModel,
            string infoAreaId,
            string targetInfoAreaId,
            int linkId,
            VirtualLinkType moveLinks,
            string intermediateInfoAreaId,
            int sourceLinkId,
            int targetLinkId)
        {
            this.InfoAreaId = infoAreaId;
            this.TargetInfoAreaId = targetInfoAreaId;
            this.IntermediateInfoAreaId = intermediateInfoAreaId;
            this.LinkId = linkId < 0 ? 0 : linkId;
            this.MoveLinks = moveLinks;
            this.SourceLinkId = sourceLinkId;
            this.TargetLinkId = targetLinkId;

            if (dataModel != null)
            {
                var tableInfo = dataModel.InternalGetTableInfo(intermediateInfoAreaId);
                if (tableInfo != null)
                {
                    this.LinkToSource = tableInfo.GetLink(infoAreaId, sourceLinkId);
                    this.LinkToTarget = tableInfo.GetLink(targetInfoAreaId, targetLinkId);
                }
                else
                {
                    this.LinkToSource = this.LinkToTarget = null;
                }
            }

            this.IsValid = this.LinkToSource != null && this.LinkToTarget != null && this.LinkToTarget.HasColumn
                           && this.LinkToSource.HasColumn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualLinkInfo"/> class.
        /// </summary>
        /// <param name="sourceLink">
        /// The source link.
        /// </param>
        /// <param name="targetLink">
        /// The target link.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="moveLinks">
        /// The move links.
        /// </param>
        public VirtualLinkInfo(LinkInfo sourceLink, LinkInfo targetLink, int linkId, VirtualLinkType moveLinks)
        {
            this.MoveLinks = moveLinks;
            this.LinkToSource = sourceLink;
            this.LinkToTarget = targetLink;
            this.LinkId = linkId;

            if (this.LinkToSource != null)
            {
                this.InfoAreaId = this.LinkToSource.TargetInfoAreaId;
                this.IntermediateInfoAreaId = this.LinkToSource.InfoAreaId;
                this.SourceLinkId = this.LinkToSource.LinkId;

                if (this.LinkToTarget != null && !Equals(this.IntermediateInfoAreaId, this.LinkToTarget.InfoAreaId))
                {
                    this.TargetInfoAreaId = this.LinkToTarget.TargetInfoAreaId;
                    this.TargetLinkId = this.LinkToTarget.LinkId;
                }
            }

            this.IsValid = this.LinkToSource != null && this.LinkToTarget != null && this.LinkToTarget.HasColumn
                           && this.LinkToSource.HasColumn;
        }

        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the intermediate information area identifier.
        /// </summary>
        /// <value>
        /// The intermediate information area identifier.
        /// </value>
        public string IntermediateInfoAreaId { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; set; }

        /// <summary>
        /// Gets or sets the link to source.
        /// </summary>
        /// <value>
        /// The link to source.
        /// </value>
        public LinkInfo LinkToSource { get; set; }

        /// <summary>
        /// Gets or sets the link to target.
        /// </summary>
        /// <value>
        /// The link to target.
        /// </value>
        public LinkInfo LinkToTarget { get; set; }

        /// <summary>
        /// Gets or sets the move links.
        /// </summary>
        /// <value>
        /// The move links.
        /// </value>
        public VirtualLinkType MoveLinks { get; set; }

        /// <summary>
        /// Gets or sets the source link identifier.
        /// </summary>
        /// <value>
        /// The source link identifier.
        /// </value>
        public int SourceLinkId { get; set; }

        /// <summary>
        /// Gets or sets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public string TargetInfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the target link identifier.
        /// </summary>
        /// <value>
        /// The target link identifier.
        /// </value>
        public int TargetLinkId { get; set; }
    }
}
