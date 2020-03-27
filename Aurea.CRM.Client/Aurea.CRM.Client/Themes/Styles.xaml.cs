// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Styles.xaml.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Ioan Armenean (Nelutu)
// </author>
// <summary>
//   The styles resource dictionary
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Aurea.CRM.Client.UI.Themes
{
    using Xamarin.Forms;

    /// <inheritdoc />
    /// <summary>
    /// The styles resource dictionary
    /// </summary>
    /// <seealso cref="T:Xamarin.Forms.ResourceDictionary" />
    public partial class Styles : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Styles"/> class.
        /// </summary>
        public Styles()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}
