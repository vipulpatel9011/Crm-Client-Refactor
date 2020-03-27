// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMQuestionnaireGroup.cs" company="Aurea Software Gmbh">
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
//   The Questionnaire Group
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Questionnaire;

    /// <summary>
    /// Questionnaire Progress Type Enum
    /// </summary>
    public enum QuestionnaireProgressType
    {
        /// <summary>
        /// No questions
        /// </summary>
        NoQuestions = 0,

        /// <summary>
        /// Not all mandatory
        /// </summary>
        NotAllMandatory,

        /// <summary>
        /// All mandatory
        /// </summary>
        AllMandatory,

        /// <summary>
        /// All questions
        /// </summary>
        AllQuestions
    }

    /// <summary>
    /// Questionnaire Group
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMQuestionnaireGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMQuestionnaireGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMQuestionnaireGroup(IIdentifier identifier)
             : base(identifier)
        {
        }

        /// <summary>
        /// Questionses this instance.
        /// </summary>
        /// <returns></returns>
        public List<UPMQuestionnaireQuestion> Questions => this.Children.Cast<UPMQuestionnaireQuestion>().ToList();

        /// <summary>
        /// Adds the question.
        /// </summary>
        /// <param name="question">The question.</param>
        public void AddQuestion(UPMQuestionnaireQuestion question)
        {
            this.AddChild(question);
        }
    }
}
