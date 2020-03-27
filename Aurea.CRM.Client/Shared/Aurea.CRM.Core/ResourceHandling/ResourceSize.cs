// <copyright file="ResourceSize.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.ResourceHandling
{
    using System;

    /// <summary>
    /// Resource size class implementation.
    /// </summary>
    public class ResourceSize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSize"/> class.
        /// Creates an instance of <see cref="ResourceSize"/> by given byte count.
        /// </summary>
        /// <param name="bytes">
        /// Data byte count
        /// </param>
        public ResourceSize(ulong bytes)
        {
            this.Bytes = bytes;
        }

        /// <summary>
        /// Gets or sets gets current byte count
        /// </summary>
        protected ulong Bytes { get; set; }

        /// <summary>
        /// Gets GigaByte representation of current byte value
        /// </summary>
        private float GigaBytes => this.MegaBytes / 1024.0f;

        /// <summary>
        /// Gets KiloByte representation of current byte value
        /// </summary>
        private float KiloBytes => this.Bytes / 1024.0f;

        /// <summary>
        /// Gets MegaByte representation of current byte value
        /// </summary>
        private float MegaBytes => this.KiloBytes / 1024.0f;

        /// <summary>
        /// Returns a short string representation for given <see cref="ResourceSizeUnit"/>
        /// </summary>
        /// <param name="unit">
        /// <see cref="ResourceSizeUnit"/>
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RenderableForUnit(ResourceSizeUnit unit)
        {
            string unitString = null;
            switch (unit)
            {
                case ResourceSizeUnit.KiloBytes:
                    unitString = "KB";
                    break;
                case ResourceSizeUnit.MegaBytes:
                    unitString = "MB";
                    break;
                case ResourceSizeUnit.GigaBytes:
                    unitString = "GB";
                    break;
                default:
                    unitString = string.Empty;
                    break;
            }

            return unitString;
        }

        /// <summary>
        /// Returns a suggested unit by current size
        /// </summary>
        /// <returns>
        /// The <see cref="ResourceSizeUnit"/>.
        /// </returns>
        public ResourceSizeUnit SuggestedUnit()
        {
            float byteCount = this.Bytes;
            if (byteCount < 1023)
            {
                return ResourceSizeUnit.Bytes;
            }

            byteCount = byteCount / 1024;
            if (byteCount < 1023)
            {
                return ResourceSizeUnit.KiloBytes;
            }

            byteCount = byteCount / 1024;
            if (byteCount < 1023)
            {
                return ResourceSizeUnit.MegaBytes;
            }

            return ResourceSizeUnit.GigaBytes;
        }

        /// <summary>
        /// Returns converted size value for given <see cref="ResourceSizeUnit"/>
        /// </summary>
        /// <param name="unit">
        /// <see cref="ResourceSizeUnit"/>
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public float ValueConvertedTo(ResourceSizeUnit unit)
        {
            switch (unit)
            {
                case ResourceSizeUnit.Bytes:
                    return this.Bytes;

                case ResourceSizeUnit.KiloBytes:
                    return this.KiloBytes;

                case ResourceSizeUnit.MegaBytes:
                    return this.MegaBytes;

                case ResourceSizeUnit.GigaBytes:
                    return this.GigaBytes;
            }

            return 0.0f;
        }
    }
}
