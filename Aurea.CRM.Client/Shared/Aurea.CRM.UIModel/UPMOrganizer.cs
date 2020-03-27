// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMOrganizer.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm organizer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// The upm organizer.
    /// </summary>
    public class UPMOrganizer : UPMContainer, ITopLevelElement
    {
        /// <summary>
        /// The pages.
        /// </summary>
        public List<Page> Pages => this.Children.Select(x => x as Page).ToList();

        /// <summary>
        /// The number of pages.
        /// </summary>
        public int NumberOfPages => this.Children.Count;

        /// <summary>
        /// Gets or sets a value indicating whether expand found.
        /// </summary>
        public bool ExpandFound { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether displays title text.
        /// </summary>
        public bool DisplaysTitleText { get; set; }

        /// <summary>
        /// Gets or sets the line count additional titletext.
        /// </summary>
        public int LineCountAdditionalTitletext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether displays image.
        /// </summary>
        public bool DisplaysImage { get; set; }

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public string TitleText { get; set; }

        /// <summary>
        /// Gets or sets the subtitle text.
        /// </summary>
        public string SubtitleText { get; set; }

        /// <summary>
        /// Gets or sets the additional title text.
        /// </summary>
        public string AdditionalTitleText { get; set; }

        /// <summary>
        /// Gets or sets the image document.
        /// </summary>
        public UPMDocument ImageDocument { get; set; }

        /// <summary>
        /// Gets or sets the organizer color.
        /// </summary>
        public AureaColor OrganizerColor { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public UPMStatus Status { get; set; }

        // public UIImage StatusIndicatorIcon { get; set; }  // CRM-5007

        /// <summary>
        /// Gets or sets the favorite record identification.
        /// </summary>
        public string FavoriteRecordIdentification { get; set; }

        public List<UPMTile> Tiles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMOrganizer"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMOrganizer(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// The add page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public void AddPage(Page page)
        {
            this.AddChild(page);
        }

        /// <summary>
        /// The replace page with new page.
        /// </summary>
        /// <param name="oldPage">
        /// The old page.
        /// </param>
        /// <param name="newPage">
        /// The new page.
        /// </param>
        public void ReplacePageWithNewPage(Page oldPage, Page newPage)
        {
            var index = this.Children.IndexOf(oldPage);
            if (index >= 0)
            {
                this.Children[index] = newPage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has changes.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                foreach (Page page in this.Pages)
                {
                    var editPage = page as EditPage;
                    if (editPage != null && editPage.HasChanges)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
