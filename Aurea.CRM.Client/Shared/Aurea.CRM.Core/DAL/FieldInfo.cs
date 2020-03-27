// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldInfo.cs" company="Aurea Software Gmbh">
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
//   Field info implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Linq;

    /// <summary>
    /// Field info implementation
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="xmlName">
        /// Name of the XML.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fieldType">
        /// Type of the field.
        /// </param>
        /// <param name="fieldLen">
        /// Length of the field.
        /// </param>
        /// <param name="cat">
        /// The cat.
        /// </param>
        /// <param name="ucat">
        /// The ucat.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="repMode">
        /// The rep mode.
        /// </param>
        /// <param name="rights">
        /// The rights.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arrayFieldCount">
        /// The array field count.
        /// </param>
        /// <param name="arrayFieldIndices">
        /// The array field indices.
        /// </param>
        public FieldInfo(
            string infoAreaId,
            int fieldId,
            string xmlName,
            string name,
            char fieldType,
            int fieldLen,
            int cat,
            int ucat,
            int attributes,
            string repMode,
            int rights,
            int format,
            int arrayFieldCount,
            int[] arrayFieldIndices)
        {
            this.InfoAreaId = infoAreaId;
            this.FieldId = fieldId;
            this.XmlName = xmlName;
            this.Name = name;
            this.FieldType = fieldType;
            this.Format = format;
            this.Attributes = attributes;
            this.Cat = cat;
            this.UCat = ucat;
            this.FieldLen = fieldLen;
            this.Rights = rights;
            this.ZField = -1;
            this.ArrayFieldCount = arrayFieldCount;
            if (this.ArrayFieldCount > 0 && arrayFieldIndices[0] == this.FieldId)
            {
                this.ArrayFieldIndices = arrayFieldIndices.ToArray();
            }
            else
            {
                // PVCS #81410 - Array-Search only for first field (array selection field), not all
                this.ArrayFieldCount = 0;
                this.ArrayFieldIndices = null;
            }

            this.RepMode = repMode;
            this.DatabaseFieldName = $"F{fieldId}";
        }

        /// <summary>
        /// Gets the array field count.
        /// </summary>
        /// <value>
        /// The array field count.
        /// </value>
        public int ArrayFieldCount { get; private set; }

        /// <summary>
        /// Gets the array field indices.
        /// </summary>
        /// <value>
        /// The array field indices.
        /// </value>
        public int[] ArrayFieldIndices { get; private set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public int Attributes { get; set; }

        /// <summary>
        /// Gets or sets the cat.
        /// </summary>
        /// <value>
        /// The cat.
        /// </value>
        public int Cat { get; set; }

        /// <summary>
        /// Gets the name of the database field.
        /// </summary>
        /// <value>
        /// The name of the database field.
        /// </value>
        public string DatabaseFieldName { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets or sets the length of the field.
        /// </summary>
        /// <value>
        /// The length of the field.
        /// </value>
        public int FieldLen { get; set; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public char FieldType { get; private set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public int Format { get; set; }

        /// <summary>
        /// Gets a value indicating whether [four decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [four decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool FourDecimalDigits => this.Format == 512;

        /// <summary>
        /// Gets a value indicating whether this instance has grouping separator.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has grouping separator; otherwise, <c>false</c>.
        /// </value>
        public bool HasGroupingSeparator => this.Format == 1;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is amount.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is amount; otherwise, <c>false</c>.
        /// </value>
        public bool IsAmount => this.Format == 0x100;

        /// <summary>
        /// Gets a value indicating whether this instance is catalog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is catalog; otherwise, <c>false</c>.
        /// </value>
        public bool IsCatalog => "XK".IndexOf(this.FieldType) > -1;

        /// <summary>
        /// Gets a value indicating whether this instance is date.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is date; otherwise, <c>false</c>.
        /// </value>
        public bool IsDate => this.FieldType == 'D';

        /// <summary>
        /// Gets a value indicating whether this instance is HTML.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is HTML; otherwise, <c>false</c>.
        /// </value>
        public bool IsHtml => this.Attributes == 0x40;

        /// <summary>
        /// Gets a value indicating whether this instance is numeric.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is numeric; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumeric => "NLS".IndexOf(this.FieldType) > -1;

        /// <summary>
        /// Gets a value indicating whether this instance is participants field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is participants field; otherwise, <c>false</c>.
        /// </value>
        public bool IsParticipantsField => !string.IsNullOrEmpty(this.RepMode) && this.FieldType == 'C';

        /// <summary>
        /// Gets a value indicating whether this instance is percent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is percent; otherwise, <c>false</c>.
        /// </value>
        public bool IsPercent => (this.Format & 8) != 0;

        /// <summary>
        /// Gets a value indicating whether this instance is time.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is time; otherwise, <c>false</c>.
        /// </value>
        public bool IsTime => this.FieldType == 'T';

        /// <summary>
        /// Gets a value indicating whether this instance is variable catalog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is variable catalog; otherwise, <c>false</c>.
        /// </value>
        public bool IsVariableCatalog => this.FieldType == 'K';

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [no decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool NoDecimalDigits => this.Format == 4;

        /// <summary>
        /// Gets a value indicating whether [one decimal digit].
        /// </summary>
        /// <value>
        /// <c>true</c> if [one decimal digit]; otherwise, <c>false</c>.
        /// </value>
        public bool OneDecimalDigit => this.Format == 2;

        /// <summary>
        /// Gets the rep mode.
        /// </summary>
        /// <value>
        /// The rep mode.
        /// </value>
        public string RepMode { get; private set; }

        /// <summary>
        /// Gets or sets the rights.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        public int Rights { get; set; }

        /// <summary>
        /// Gets a value indicating whether to [show zero].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show zero]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowZero => this.Format == 32;

        /// <summary>
        /// Gets a value indicating whether [three decimal digits].
        /// </summary>
        /// <value>
        /// <c>true</c> if [three decimal digits]; otherwise, <c>false</c>.
        /// </value>
        public bool ThreeDecimalDigits => this.Format == 64;

        /// <summary>
        /// Gets or sets the u cat.
        /// </summary>
        /// <value>
        /// The u cat.
        /// </value>
        public int UCat { get; set; }

        /// <summary>
        /// Gets the name of the XML.
        /// </summary>
        /// <value>
        /// The name of the XML.
        /// </value>
        public string XmlName { get; private set; }

        /// <summary>
        /// Gets or sets the z field.
        /// </summary>
        /// <value>
        /// The z field.
        /// </value>
        public int ZField { get; set; }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo CreateCopy()
        {
            return this.MemberwiseClone() as FieldInfo;
        }
    }
}
