﻿// <copyright file="MDashboardPage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Implements the dashboard page model
// </summary>
namespace Aurea.CRM.UIModel.Pages
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// IMplements the details view page
    /// </summary>
    /// <seealso cref="Page" />
    public class MDashboardPage : MDetailPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MDashboardPage"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public MDashboardPage(IIdentifier identifier)
            : base(identifier)
        {
        }
    }
}
