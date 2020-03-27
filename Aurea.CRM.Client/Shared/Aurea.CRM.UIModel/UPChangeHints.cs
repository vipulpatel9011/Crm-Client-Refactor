// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChangeHints.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Change hints infomation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    /// <summary>
    /// Change hints infomation
    /// </summary>
    public class UPChangeHints
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeHints"/> class.
        /// </summary>
        /// <param name="generalHint">
        /// The general hint.
        /// </param>
        /// <param name="detailHints">
        /// The detail hints.
        /// </param>
        public UPChangeHints(string generalHint, Dictionary<string, object> detailHints)
        {
            this.GeneralChangeHint = generalHint;
            this.DetailHints = detailHints;
        }

        /// <summary>
        /// Gets the detail hints.
        /// </summary>
        /// <value>
        /// The detail hints.
        /// </value>
        public Dictionary<string, object> DetailHints { get; private set; }

        /// <summary>
        /// Gets the general change hint.
        /// </summary>
        /// <value>
        /// The general change hint.
        /// </value>
        public string GeneralChangeHint { get; private set; }

        /// <summary>
        /// Changes the hints with hint.
        /// </summary>
        /// <param name="hint">
        /// The hint.
        /// </param>
        /// <returns>
        /// The <see cref="UPChangeHints"/>.
        /// </returns>
        public static UPChangeHints ChangeHintsWithHint(string hint)
        {
            return ChangeHintsWithHintDetailHints(hint, null);
        }

        /// <summary>
        /// Changes the hints with hint detail hints.
        /// </summary>
        /// <param name="hint">
        /// The hint.
        /// </param>
        /// <param name="detailHints">
        /// The detail hints.
        /// </param>
        /// <returns>
        /// The <see cref="UPChangeHints"/>.
        /// </returns>
        public static UPChangeHints ChangeHintsWithHintDetailHints(string hint, Dictionary<string, object> detailHints)
        {
            return new UPChangeHints(hint, detailHints);
        }
    }
}
