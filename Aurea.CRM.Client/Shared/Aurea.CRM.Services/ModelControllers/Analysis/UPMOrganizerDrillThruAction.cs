﻿// <copyright file="UPMOrganizerDrillThruAction.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Services.ModelControllers.Analysis
{
    using Aurea.CRM.Core.Analysis.Result;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel;

    /// <summary>
    /// Implementation of organizer drill thru action
    /// </summary>
    public class UPMOrganizerDrillThruAction : UPMOrganizerAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMOrganizerDrillThruAction"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        public UPMOrganizerDrillThruAction(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets analysis row
        /// </summary>
        public AnalysisRow AnalysisRow { get; set; }
    }
}
