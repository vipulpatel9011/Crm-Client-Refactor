// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PList.cs" company="Aurea Software Gmbh">
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
//   Defines the root types of a plist
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the root types of a plist
    /// </summary>
    public static class ListRootType
    {
        /// <summary>
        /// The array root
        /// </summary>
        public const string Array = "array";

        /// <summary>
        /// The dictionary root
        /// </summary>
        public const string Dictionary = "dict";
    }

    /// <summary>
    /// Defines a plist property
    /// </summary>
    public class PListValue
    {
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public string PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public dynamic Value { get; set; }
    }

    /// <summary>
    /// Implements a plist with a dictionary root
    /// </summary>
    /// <seealso cref="string" />
    public class PList : Dictionary<string, PListValue>
    {
    }

    /// <summary>
    /// Implements a plist with array root
    /// </summary>
    /// <seealso cref="PListValue" />
    public class ArrayPList : List<PListValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPList"/> class.
        /// </summary>
        public ArrayPList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPList"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ArrayPList(IEnumerable<PListValue> collection)
            : base(collection)
        {
        }
    }
}
