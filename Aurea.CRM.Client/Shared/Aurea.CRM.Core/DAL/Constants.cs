// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Aurea Software Gmbh">
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
//   The type of the link
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// The type of the link
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// The ident.
        /// </summary>
        IDENT = 0,

        /// <summary>
        /// The parent.
        /// </summary>
        PARENT = 1,

        /// <summary>
        /// The child.
        /// </summary>
        CHILD = 2,

        /// <summary>
        /// The generic.
        /// </summary>
        GENERIC = 3,

        /// <summary>
        /// The unknown.
        /// </summary>
        UNKNOWN = -1,

        /// <summary>
        /// The onetoone.
        /// </summary>
        ONETOONE = 4,

        /// <summary>
        /// The onetomany.
        /// </summary>
        ONETOMANY = 2,

        /// <summary>
        /// The manytoone.
        /// </summary>
        MANYTOONE = 1
    }

    /// <summary>
    /// The field id type
    /// </summary>
    public enum FieldIdType
    {
        /// <summary>
        /// The base.
        /// </summary>
        BASE = 99000,

        /// <summary>
        /// The upddate.
        /// </summary>
        UPDDATE = 99003,

        /// <summary>
        /// The syncdate.
        /// </summary>
        SYNCDATE = 99002,

        /// <summary>
        /// The infoareaid.
        /// </summary>
        INFOAREAID = 99001,

        /// <summary>
        /// The recordid.
        /// </summary>
        RECORDID = 99000,

        /// <summary>
        /// The empty.
        /// </summary>
        EMPTY = 99009,

        /// <summary>
        /// The lookup.
        /// </summary>
        LOOKUP = 99004
    }

    /// <summary>
    /// Constant values related to data access
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The max catalog tablename.
        /// </summary>
        public const int MaxCatalogTablename = 20;

        /// <summary>
        /// The max catalog valuelen.
        /// </summary>
        public const int MaxCatalogValuelen = 80;

        /// <summary>
        /// The max infoareaid len.
        /// </summary>
        public const int MaxInfoareaidLen = 6;

        /// <summary>
        /// The max infoarea tablename.
        /// </summary>
        public const int MaxInfoareaTablename = 12;

        /// <summary>
        /// The max link column name.
        /// </summary>
        public const int MaxLinkColumnName = 18;
    }
}
