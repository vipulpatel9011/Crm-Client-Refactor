// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogValue.cs" company="Aurea Software Gmbh">
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
//   Catalog value implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Catalogs
{
    using System.Collections.Generic;
    using System.Text;

    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Catalog value implementation
    /// </summary>
    public class UPCatalogValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogValue"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        public UPCatalogValue(List<object> def)
        {
            if (def == null)
            {
                return;
            }

            this.Code = JObjectExtensions.ToInt(def[0]);
            this.Text = (string)def[1];
            this.ExtKey = (string)def[2];
            this.Tenant = JObjectExtensions.ToInt(def[3]);
            this.ParentCode = def.Count > 4 ? JObjectExtensions.ToInt(def[4]) : 0;

            if (def.Count > 6)
            {
                this.Sortinfo = JObjectExtensions.ToInt(def[5]);
                this.Access = JObjectExtensions.ToInt(def[6]);
            }
            else
            {
                this.Sortinfo = 0;
                this.Access = 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogValue"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPCatalogValue(CatalogValue value)
        {
            if (value == null)
            {
                return;
            }

            this.Code = value.Code;
            this.Text = value.Text;
            this.Sortinfo = value.SortInfo;
            this.Access = value.Access;
            if (value.IsFixedValue)
            {
                return;
            }

            var varValue = (VariableCatalogValue)value;
            this.ExtKey = varValue.ExtKey;
            this.Tenant = varValue.GetTenantNo();
        }

        /// <summary>
        /// Gets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public int Access { get; private set; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; private set; }

        /// <summary>
        /// Gets the code key.
        /// </summary>
        /// <value>
        /// The code key.
        /// </value>
        public virtual string CodeKey => $"{this.Code}";

        /// <summary>
        /// Gets the ext key.
        /// </summary>
        /// <value>
        /// The ext key.
        /// </value>
        public string ExtKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed value; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFixedValue => false;

        /// <summary>
        /// Gets the parent code.
        /// </summary>
        /// <value>
        /// The parent code.
        /// </value>
        public int ParentCode { get; private set; }

        /// <summary>
        /// Gets the sortinfo.
        /// </summary>
        /// <value>
        /// The sortinfo.
        /// </value>
        public int Sortinfo { get; private set; }

        /// <summary>
        /// Gets the tenant.
        /// </summary>
        /// <value>
        /// The tenant.
        /// </value>
        public int Tenant { get; private set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var desc = new StringBuilder($"{this.Text} ({this.Code})");
            if (!string.IsNullOrEmpty(this.ExtKey))
            {
                desc.Append($", extkey={this.ExtKey}");
            }

            if (this.ParentCode > 0)
            {
                desc.Append($", parentCode={this.ParentCode}");
            }

            if (this.Tenant > 0)
            {
                desc.Append($", tenant={this.Tenant}");
            }

            return desc.ToString();
        }
    }
}
