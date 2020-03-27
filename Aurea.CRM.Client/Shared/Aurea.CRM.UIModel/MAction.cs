// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MAction.cs" company="Aurea Software Gmbh">
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
//   Implements an UI Action
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using GalaSoft.MvvmLight.Ioc;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// Implements an UI Action
    /// </summary>
    /// <seealso cref="UPMElement" />
    public class UPMAction : UPMElement
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The action.
        /// </summary>
        private Action<object, object> action;

        /// <summary>
        /// The target.
        /// </summary>
        private object target;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMAction"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMAction(IIdentifier identifier)
            : base(identifier)
        {
            this.Enabled = true;
            this.MainAction = false;
            this.logger = SimpleIoc.Default.GetInstance<ILogger>();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public virtual object Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMAction"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the icon.
        /// </summary>
        /// <value>
        /// The name of the icon.
        /// </value>
        public virtual string IconName { get; set; }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        public virtual string LabelText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [main action].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [main action]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool MainAction { get; set; }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public virtual void PerformAction(object sender)
        {
            try
            {
                this.action?.Invoke(sender, this.Context);
            }
            catch (Exception e)
            {
                this.logger?.LogError(e);
                this.MessengerInstance?.Send(new ToastrMessage { MessageText = e.Message, DetailedMessage = e.StackTrace });
            }
        }

        /// <summary>
        /// Sets the target action.
        /// </summary>
        /// <param name="target">
        /// The _target.
        /// </param>
        /// <param name="action">
        /// The _action.
        /// </param>
        public virtual void SetTargetAction(object target, Action<object, object> action)
        {
            this.target = target;
            this.action = action;
        }

        /// <summary>
        /// Sets the target action.
        /// </summary>
        /// <param name="_target">
        /// The _target.
        /// </param>
        /// <param name="_action">
        /// The _action.
        /// </param>
        public virtual void SetTargetAction(object _target, Action<object> _action)
        {
            this.target = _target;
            this.action = (a, b) => _action(a);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Target: {this.target}, Action: {nameof(this.action)}";
        }
    }
}
