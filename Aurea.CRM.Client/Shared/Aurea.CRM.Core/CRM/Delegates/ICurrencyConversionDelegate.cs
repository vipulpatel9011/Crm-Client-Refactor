// <copyright file="ICurrencyConversionDelegate.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;
    using Features;

    /// <summary>
    /// Currency conversion delegate interface
    /// </summary>
    public interface ICurrencyConversionDelegate
    {
        /// <summary>
        /// Currency conversion did finish with result
        /// </summary>
        /// <param name="currencyConversion">Conversion object</param>
        /// <param name="result">Result</param>
        void CurrencyConversionDidFinishWithResult(CurrencyConversion currencyConversion, object result);

        /// <summary>
        /// Currency conversion did finish with error
        /// </summary>
        /// <param name="currencyConversion">Conversion object</param>
        /// <param name="error">Error</param>
        void CurrencyConversionDidFailWithError(CurrencyConversion currencyConversion, Exception error);
    }
}
