// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuestionnaireDelegate.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Questionnaire
{
    using System;

    /// <summary>
    /// Questionnaire delegate
    /// </summary>
    public interface IQuestionnaireDelegate
    {
        /// <summary>
        /// Questionnaire the did finish with result.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        /// <param name="result">The result.</param>
        void QuestionnaireDidFinishWithResult(UPQuestionnaire questionnaire, object result);

        /// <summary>
        /// Questionnaire the did fail with error.
        /// </summary>
        /// <param name="questionnaire">The questionnaire.</param>
        /// <param name="error">The error.</param>
        void QuestionnaireDidFailWithError(UPQuestionnaire questionnaire, Exception error);
    }
}
