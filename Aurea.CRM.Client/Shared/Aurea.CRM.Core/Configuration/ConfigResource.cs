// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigResource.cs" company="Aurea Software Gmbh">
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
//   Defines resource configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Defines resource configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigResource : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigResource"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        public UPConfigResource(List<object> defArray)
        {
            this.UnitName = defArray[0] as string;
            this.FileName = (string)defArray[1];
            this.Label = (string)defArray[2];
            this.ConfigId = defArray.Count > 3 ? JObjectExtensions.ToInt(defArray[3]) : -1;
        }

        /// <summary>
        /// Gets the configuration identifier.
        /// </summary>
        /// <value>
        /// The configuration identifier.
        /// </value>
        public int ConfigId { get; private set; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the file name2 x.
        /// </summary>
        /// <value>
        /// The file name2 x.
        /// </value>
        public string FileName2X
        {
            get
            {
                if (string.IsNullOrEmpty(this.FileName))
                {
                    return this.FileName;
                }

                var extensionPoint = this.FileName.LastIndexOf('.');
                var pathExtension = this.FileName.Substring(extensionPoint);
                var fileBase = this.FileName.Substring(0, extensionPoint);
                return $"{fileBase}@2x.{pathExtension}";
            }
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }
    }
}
