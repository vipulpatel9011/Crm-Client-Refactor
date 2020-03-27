// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoAreaCondition.cs" company="Aurea Software Gmbh">
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
//   Info area condition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Query
{
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Info area condition
    /// </summary>
    public class UPInfoAreaCondition
    {
        /// <summary>
        /// Conditions to object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object ConditionToObject() => null;

        /// <summary>
        /// Creates the query condition options.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="TreeItemCondition"/>.
        /// </returns>
        public virtual TreeItemCondition CreateQueryConditionOptions(CRMDatabase crmDatabase, int options) => null;

        /// <summary>
        /// Dates the time condition for information area parent.
        /// </summary>
        /// <param name="infoArea">
        /// The information area.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaDateTimeCondition"/>.
        /// </returns>
        public virtual UPInfoAreaDateTimeCondition DateTimeConditionForInfoAreaParent(
            UPContainerInfoAreaMetaInfo infoArea,
            UPInfoAreaConditionTree parent) => null;

        // creates an internal cpp querycondition object
        // creates an json object for server requests
        /// <summary>
        /// Informations the area condition by appending and condition.
        /// </summary>
        /// <param name="cond">
        /// The cond.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public virtual UPInfoAreaCondition InfoAreaConditionByAppendingAndCondition(UPInfoAreaCondition cond)
            => new UPInfoAreaConditionTree("AND", this, cond);

        /// <summary>
        /// Informations the area condition by appending or condition.
        /// </summary>
        /// <param name="cond">
        /// The cond.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public virtual UPInfoAreaCondition InfoAreaConditionByAppendingOrCondition(UPInfoAreaCondition cond)
            => new UPInfoAreaConditionTree("OR", this, cond);
    }
}
