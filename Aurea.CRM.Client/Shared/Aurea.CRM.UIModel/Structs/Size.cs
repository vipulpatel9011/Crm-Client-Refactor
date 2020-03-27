// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Size.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Generic Size data type for UIModels
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Structs
{
    /// <summary>
    /// Generic Size data type for UIModels
    /// </summary>
    public struct Size
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">
        /// Width parameter
        /// </param>
        /// <param name="height">
        /// Height parameter
        /// </param>
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets width of size
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets height of size
        /// </summary>
        public int Height { get; }
    }
}
