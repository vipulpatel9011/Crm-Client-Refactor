// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmTiles.cs" company="Aurea Software Gmbh">
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
//   The CRM Tiles class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Delegates;

    /// <summary>
    /// Tiles class
    /// </summary>
    public class UPCRMTiles
    {
        private int openTileCount;

        /// <summary>
        /// Gets the name of the tile.
        /// </summary>
        /// <value>
        /// The name of the tile.
        /// </value>
        public string TileName { get; private set; }

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        /// <value>
        /// The tiles.
        /// </value>
        public List<UPCRMTile> Tiles { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPCRMTilesDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMTiles"/> class.
        /// </summary>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCRMTiles(string tileName, string recordIdentification, Dictionary<string, object> parameters, UPCRMTilesDelegate theDelegate)
        {
            this.TileName = tileName;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            Menu menu = configStore.MenuByName(tileName);
            List<UPCRMTile> tileArray = new List<UPCRMTile>();

            foreach (string menuActionName in menu.Items)
            {
                Menu menuAction = configStore.MenuByName(menuActionName);
                ViewReference viewReference = menuAction.ViewReference?.ViewReferenceWith(recordIdentification);
                if (viewReference == null)
                {
                    continue;
                }

                UPCRMTile tile = UPCRMTile.TileFromViewReference(viewReference, parameters, menuAction, this);
                if (tile != null)
                {
                    tileArray.Add(tile);
                }
            }

            if (tileArray.Count == 0)
            {
                throw new Exception("Tile Array count zero.");
            }

            this.Tiles = tileArray;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.openTileCount = this.Tiles.Count;
            foreach (UPCRMTile tile in this.Tiles)
            {
                tile.Load();
            }
        }

        /// <summary>
        /// Tiles finished.
        /// </summary>
        public void TilesFinished()
        {
            this.TheDelegate?.TilesDidFinishWithSuccess(this, this.Tiles);
        }

        /// <summary>
        /// Tiles finished.
        /// </summary>
        /// <param name="tile">The tile.</param>
        public void TileFinished(UPCRMTile tile)
        {
            bool finished = false;
            lock (this)
            {
                if (--this.openTileCount == 0)
                {
                    finished = true;
                }
            }

            if (finished)
            {
                this.TilesFinished();
            }
        }

        /// <summary>
        /// Tiles the finished with error.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="error">The error.</param>
        public void TileFinishedWithError(UPCRMTile tile, Exception error)
        {
            this.TileFinished(tile);
        }
    }
}
