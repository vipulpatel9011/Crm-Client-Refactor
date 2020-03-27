// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseIdentifier.cs" company="Aurea Software Gmbh">
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
//   The abstract implementation of a UI control identifier
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Identifiers
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The abstract implementation of a UI control identifier
    /// </summary>
    /// <seealso cref="IIdentifier" />
    public abstract class BaseIdentifier : object, IIdentifier
    {
        /// <summary>
        /// The identifier description
        /// </summary>
        protected string identifierDescription;

        /// <summary>
        /// Gets a value indicating whether this instance has wildcard suffix.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has wildcard suffix; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasWildcardSuffix { get; set; }

        /// <summary>
        /// Gets the identifier as string.
        /// </summary>
        /// <value>
        /// The identifier as string.
        /// </value>
        public string IdentifierAsString => this.identifierDescription;

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var otherObject = obj as BaseIdentifier;
            return this.ToString().Equals(otherObject?.ToString());
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Matcheses the identifier.
        /// </summary>
        /// <param name="otherIdentifier">
        /// The other identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool MatchesIdentifier(IIdentifier otherIdentifier)
        {
            if (otherIdentifier is MultipleIdentifier)
            {
                return otherIdentifier.MatchesIdentifier(this);
            }

            var directMatch = $"{otherIdentifier}" == $"{this}";
            if (directMatch)
            {
                return true;
            }

            var otherDescription = otherIdentifier?.ToString();
            var myDescription = this.ToString();

            if (this.ToString() == "*" || otherIdentifier?.ToString() == "*")
            {
                // Super Joker
                return true;
            }

            if (otherIdentifier?.HasWildcardSuffix == true && this.HasWildcardSuffix)
            {
                otherDescription = otherDescription.Substring(otherDescription.Length - 2);
                myDescription = myDescription.Substring(myDescription.Length - 2);
                return myDescription.StartsWith(otherDescription) || otherDescription.StartsWith(myDescription);
            }

            if (otherIdentifier?.HasWildcardSuffix == true)
            {
                otherDescription = otherDescription.Substring(otherDescription.Length - 2);
                return this.ToString().StartsWith(otherDescription);
            }

            if (!this.HasWildcardSuffix)
            {
                return false;
            }

            myDescription = myDescription.Substring(myDescription.Length - 2);
            return otherDescription?.StartsWith(myDescription) == true;
        }

        /// <summary>
        /// Sets the has wildcard suffix.
        /// </summary>
        public void SetHasWildcardSuffix()
        {
            this.HasWildcardSuffix = this.identifierDescription?.EndsWith(".*") ?? false;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.identifierDescription;
    }
}
