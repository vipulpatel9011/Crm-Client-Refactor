// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmTile.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The Tile class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Configuration;

    /// <summary>
    /// Tile class
    /// </summary>
    public class UPCRMTile
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="UpcrmTile"/> is clickable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if clickable; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Clickable => false;

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        /// <value>
        /// The tiles.
        /// </value>
        public UPCRMTiles Tiles { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the menu action.
        /// </summary>
        /// <value>
        /// The menu action.
        /// </value>
        public Menu MenuAction { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpcrmTile"/> class.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="menuAction">The menu action.</param>
        public UPCRMTile(UPCRMTiles tiles, ViewReference viewReference, Dictionary<string, object> parameters, Menu menuAction)
        {
            this.Tiles = tiles;
            this.ViewReference = viewReference;
            this.Parameters = parameters;
            this.RequestOption = UPRequestOption.FastestAvailable;
            this.MenuAction = menuAction;
        }

        /// <summary>
        /// Tiles from view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="menuAction">The menu action.</param>
        /// <param name="crmTiles">The CRM tiles.</param>
        /// <returns></returns>
        public static UPCRMTile TileFromViewReference(ViewReference viewReference, Dictionary<string, object> parameters, Menu menuAction, UPCRMTiles crmTiles)
        {
            if (viewReference.ViewName == "Record")
            {
                return new UPCRMRecordTile(crmTiles, viewReference, parameters, menuAction);
            }

            if (viewReference.ViewName == "Search")
            {
                return new UPCRMSearchTile(crmTiles, viewReference, parameters, menuAction);
            }

            return null;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public virtual void Load()
        {
            this.Tiles.TileFinishedWithError(this, new Exception("Not implemented - generic tile cannot be loaded"));
        }

        /// <summary>
        /// Loads the with result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public virtual bool LoadWithResult(UPCRMResult result)
        {
            return false;
        }

        /// <summary>
        /// Loads the with result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public virtual bool LoadWithResultRow(UPCRMResultRow resultRow)
        {
            return false;
        }
    }
}
