// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalOperationManager.cs" company="Aurea Software Gmbh">
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
//   Implements a manager to handle local operations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements a manager to handle local operations
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LocalOperationManager : IDisposable
    {
        /// <summary>
        /// The local operations.
        /// </summary>
        protected List<LocalOperation> LocalOperations;

        /// <summary>
        /// The can start operation.
        /// </summary>
        protected bool CanStartOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalOperationManager"/> class.
        /// </summary>
        public LocalOperationManager()
        {
            this.LocalOperations = new List<LocalOperation>();
            this.CanStartOperation = true;
        }

        /// <summary>
        /// Queues the local operation.
        /// </summary>
        /// <param name="localOperation">
        /// The local operation.
        /// </param>
        public void QueueLocalOperation(LocalOperation localOperation)
        {
            this.LocalOperations.Add(localOperation);
            this.StartNextOperation();
        }

        /// <summary>
        /// Starts the next operation.
        /// </summary>
        public void StartNextOperation()
        {
            if (this.LocalOperations.Count == 0 || !this.CanStartOperation)
            {
                return;
            }

            var cancelledOperations = new List<LocalOperation>();
            foreach (var localOperation in this.LocalOperations)
            {
                if (localOperation.Canceled)
                {
                    cancelledOperations.Add(localOperation);
                }
            }

            foreach (var cancelledOperation in cancelledOperations)
            {
                this.LocalOperations.Remove(cancelledOperation);
            }

            if (this.LocalOperations.Count == 0)
            {
                return;
            }

            var currentLocalOperation = this.LocalOperations[0];

            this.StartProcessingOfCurrentLocalOperation(currentLocalOperation);

            this.LocalOperations.Remove(currentLocalOperation);
            this.CanStartOperation = true;
            this.StartNextOperation();
        }

        /// <summary>
        /// Starts the processing of current local operation.
        /// </summary>
        /// <param name="operationToStart">
        /// The operation To Start.
        /// </param>
        public void StartProcessingOfCurrentLocalOperation(LocalOperation operationToStart)
        {
            this.CanStartOperation = false;
            operationToStart?.PerformOperation();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.CancelAllOperations();
        }

        /// <summary>
        /// Cancels all operations.
        /// </summary>
        public void CancelAllOperations()
        {
            Task.Factory.StartNew(
                () =>
                    {
                        foreach (var localOperation in this.LocalOperations)
                        {
                            localOperation.Cancel();
                        }

                        this.CanStartOperation = true;
                    });
        }

#if PORTING
        void ObserveValueForKeyPathOfObjectChangeContext(string keyPath, object theObject, NSDictionary change, ref void context)
        {
            if ((keyPath.Equals("finished") || keyPath.Equals("cancelled")) && theObject == currentLocalOperation)
            {
                __block bool startNextOperation = false;
                dispatch_sync(synchronizationQueue, delegate ()
                {
                    currentLocalOperation.RemoveObserverForKeyPath(this, "finished");
                    currentLocalOperation.RemoveObserverForKeyPath(this, "cancelled");
                    localOperations.Remove(currentLocalOperation);
                    currentLocalOperation = null;
                    if (localOperations.Count > 0)
                    {
                        startNextOperation = true;
                    }

                });
                if (startNextOperation)
                {
                    StartNextOperation();
                }
            }
        }
#endif
    }
}
