// <copyright file="UPMInsightBoardItem.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Insight Board Item Type
    /// </summary>
    public enum UPMInsightBoardItemType
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0, // colored action zb. login

        /// <summary>
        /// Count
        /// </summary>
        Count = 1,

        /// <summary>
        /// Action
        /// </summary>
        Action = 2,

        /// <summary>
        /// Ex menu
        /// </summary>
        ExMenu = 3,
    }

    /// <summary>
    /// Insight Board Item
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMInsightBoardItem : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMInsightBoardItem"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMInsightBoardItem(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMInsightBoardItem"/> is countable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if countable; otherwise, <c>false</c>.
        /// </value>
        public bool Countable { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        // public UIImage DefaultIcon { get; set; }     // CRM-5007

        // public UIImage Icon      // CRM-5007
        // {
        //    get
        //    {
        //        if (this.DefaultIcon != null)
        //        {
        //            return this.DefaultIcon;
        //        }
        //        else if (this.ImageName.Length > 0)
        //        {
        //            return UIImage.UpImageWithFileName(this.ImageName);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        // }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public UPMAction Action { get; set; }

        /// <summary>
        /// Gets or sets the index of the sort.
        /// </summary>
        /// <value>
        /// The index of the sort.
        /// </value>
        public int SortIndex { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show image].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show image]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowImage { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public AureaColor Color { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public UPMInsightBoardItemType Type { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{base.ToString()}, {this.Title}]";
        }
    }
}
