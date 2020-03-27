// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MElement.cs" company="Aurea Software Gmbh">
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
//   Defines the font styles
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// Defines the font styles
    /// </summary>
    public enum AureaFontStyle
    {
        /// <summary>
        /// The plain.
        /// </summary>
        Plain = 0,

        /// <summary>
        /// The bold.
        /// </summary>
        Bold,

        /// <summary>
        /// The italic.
        /// </summary>
        Italic,

        /// <summary>
        /// The default.
        /// </summary>
        Default
    }

    /// <summary>
    /// The upm element.
    /// </summary>
    public class UPMElement : ViewModelBase, IUPMElement
    {
        // : StackPanel
        /// <summary>
        /// The help identifier
        /// </summary>
        private IIdentifier helpIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMElement"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMElement(IIdentifier identifier)
        {
            this.Identifier = identifier;
            this.helpIdentifier = null;

            // this.ItemCopyFlyout = new MenuFlyout();
            // this.ItemCopyFlyout.Items.Add(new MenuFlyoutItem() { Text = "Copy" });
        }

        /// <summary>
        /// Gets or sets the help identifier.
        /// </summary>
        /// <value>
        /// The help identifier.
        /// </value>
        public IIdentifier HelpIdentifier
        {
            get
            {
                return this.helpIdentifier ?? this.Identifier;
            }

            set
            {
                this.helpIdentifier = value;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public IIdentifier Identifier { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMElement"/> is invalid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if invalid; otherwise, <c>false</c>.
        /// </value>
        public bool Invalid { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <value>
        /// The parent element.
        /// </value>
        public UPMElement Parent { get; set; }

        ///// <summary>
        ///// Gets the item copy flyout.
        ///// </summary>
        ///// <value>
        ///// The item copy flyout.
        ///// </value>
        // public MenuFlyout ItemCopyFlyout { get; }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">
        /// The list of identifiers.
        /// </param>
        /// <param name="appliedIdentifiers">
        /// The applied identifiers.
        /// </param>
        public virtual void ProcessChangesAppliedIdentifiers(
            List<IIdentifier> listOfIdentifiers,
            List<IIdentifier> appliedIdentifiers)
        {
            foreach (var identifier in listOfIdentifiers)
            {
                if (!identifier.MatchesIdentifier(this.Identifier))
                {
                    continue;
                }

                appliedIdentifiers.Add(identifier);
                this.Invalid = true;
                return;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{base.ToString()}, identifier: {this.Identifier}, parent: {this.Parent}, invalid: {this.Invalid}]";
        }
    }
}
