// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceArgument.cs" company="Aurea Software Gmbh">
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
//   Implements the reference argument object
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements the reference argument object
    /// </summary>
    public class ReferenceArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceArgument"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ReferenceArgument(IReadOnlyList<string> definition)
        {
            this.Name = definition[0];
            this.Type = definition[1];
            this.Value = definition[2];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceArgument"/> class.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ReferenceArgument(ReferenceArgument argument, string value)
        {
            this.Name = argument.Name;
            this.Type = argument.Type;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceArgument"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public ReferenceArgument(string name, string value)
        {
            this.Name = name;
            this.Type = "wurscht";
            this.Value = value;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
