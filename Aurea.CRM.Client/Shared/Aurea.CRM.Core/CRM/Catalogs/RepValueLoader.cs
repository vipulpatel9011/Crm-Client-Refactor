// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepValueLoader.cs" company="Aurea Software Gmbh">
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
//   Catalog value provider interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Catalogs
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Rep value loader
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueLoaderBase" />
    public class UPRepValueLoader : UPCatalogValueLoaderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPRepValueLoader"/> class.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        public UPRepValueLoader(UPCRMFieldInfo fieldInfo)
            : base(fieldInfo)
        {
        }

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public override Dictionary<string, string> PossibleValues()
        {
            return UPCRMDataStore.DefaultStore.Reps.RepNameDictionaryWithTypeString(this.FieldInfo.RepType);
        }
    }
}
