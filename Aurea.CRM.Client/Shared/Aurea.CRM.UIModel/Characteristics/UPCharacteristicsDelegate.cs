// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCharacteristicsDelegate.cs" company="Aurea Software Gmbh">
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
//   UPCharacteristicsDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Characteristics
{
    using System;

    /// <summary>
    /// UPCharacteristicsDelegate
    /// </summary>
    public interface UPCharacteristicsDelegate
    {
        /// <summary>
        /// Characteristicses the did finish with result.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="result">The result.</param>
        void CharacteristicsDidFinishWithResult(UPCharacteristics characteristics, object result);

        /// <summary>
        /// Characteristicses the did fail with error.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="error">The error.</param>
        void CharacteristicsDidFailWithError(UPCharacteristics characteristics, Exception error);
    }
}
