// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerLanguage.cs" company="Aurea Software Gmbh">
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
//   Defines the server language
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    /// <summary>
    /// Defines the server language
    /// </summary>
    public class ServerLanguage
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public readonly string Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLanguage"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="mmlangid">
        /// The mmlangid.
        /// </param>
        /// <param name="langid">
        /// The langid.
        /// </param>
        /// <param name="imageName">
        /// Name of the image.
        /// </param>
        public ServerLanguage(string key, string name, int mmlangid, int langid, string imageName)
        {
            this.Name = name;
            this.Key = key;
            this.Mmlangid = mmlangid;
            this.Langid = langid;
            this.ImageName = imageName;
        }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the langid.
        /// </summary>
        /// <value>
        /// The langid.
        /// </value>
        public int Langid { get; private set; }

        /// <summary>
        /// Gets the mmlangid.
        /// </summary>
        /// <value>
        /// The mmlangid.
        /// </value>
        public int Mmlangid { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var lang = obj as ServerLanguage;
            return lang != null && this.Key.Equals(lang.Key);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = this.Key?.GetHashCode() ?? 0;
            return hashCode;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Name} ({this.Key})";
        }
    }
}
