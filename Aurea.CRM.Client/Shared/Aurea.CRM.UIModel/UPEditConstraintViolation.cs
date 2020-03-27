// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPEditConstraintViolation.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The violation type of the edit field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// The violation type of the edit field
    /// </summary>
    public enum EditConstraintViolationType
    {
        /// <summary>
        /// The must field.
        /// </summary>
        MustField = 1,

        /// <summary>
        /// The client constraint.
        /// </summary>
        ClientConstraint
    }

    /// <summary>
    /// Edit constrain violation
    /// </summary>
    public class UPEditConstraintViolation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditConstraintViolation"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="violationType">
        /// Type of the violation.
        /// </param>
        public UPEditConstraintViolation(UPEditFieldContext context, EditConstraintViolationType violationType)
            : this(context, violationType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditConstraintViolation"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="violationType">
        /// Type of the violation.
        /// </param>
        /// <param name="violationKey">
        /// The violation key.
        /// </param>
        public UPEditConstraintViolation(
            UPEditFieldContext context,
            EditConstraintViolationType violationType,
            string violationKey)
        {
            this.EditFieldContext = context;
            this.ViolationType = violationType;
            this.ViolationKey = violationKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditConstraintViolation"/> class.
        /// </summary>
        /// <param name="editFieldContexts">
        /// The edit field contexts.
        /// </param>
        /// <param name="violationType">
        /// Type of the violation.
        /// </param>
        /// <param name="violationKey">
        /// The violation key.
        /// </param>
        public UPEditConstraintViolation(
            List<UPEditFieldContext> editFieldContexts,
            EditConstraintViolationType violationType,
            string violationKey)
            : this(editFieldContexts?.FirstOrDefault(), violationType, violationKey)
        {
            if (editFieldContexts == null)
            {
                return;
            }

            var length = editFieldContexts.Count - 1;
            if (length <= 0)
            {
                return;
            }

            var tempList = new List<UPEditFieldContext>();
            for (var i = 0; i < length; i--)
            {
                tempList.Add(editFieldContexts[i + 1]);
            }

            this.AdditionalEditFieldContexts = tempList;
        }

        /// <summary>
        /// Gets the additional edit field contexts.
        /// </summary>
        /// <value>
        /// The additional edit field contexts.
        /// </value>
        public List<UPEditFieldContext> AdditionalEditFieldContexts { get; private set; }

        /// <summary>
        /// Gets the edit field context.
        /// </summary>
        /// <value>
        /// The edit field context.
        /// </value>
        public UPEditFieldContext EditFieldContext { get; private set; }

        /// <summary>
        /// Gets or sets the localized description.
        /// </summary>
        /// <value>
        /// The localized description.
        /// </value>
        public string LocalizedDescription { get; set; }

        /// <summary>
        /// Gets the violation key.
        /// </summary>
        /// <value>
        /// The violation key.
        /// </value>
        public string ViolationKey { get; private set; }

        /// <summary>
        /// Gets the type of the violation.
        /// </summary>
        /// <value>
        /// The type of the violation.
        /// </value>
        public EditConstraintViolationType ViolationType { get; private set; }
    }
}
