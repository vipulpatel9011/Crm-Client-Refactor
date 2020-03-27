// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineMultiRequest.cs" company="Aurea Software Gmbh">
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
//   Offline Multi Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Offline Multi Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRequest" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    public class UPOfflineMultiRequest : UPOfflineRequest, UPOfflineRequestDelegate
    {
        /// <summary>
        /// The request array
        /// </summary>
        protected List<UPOfflineRequest> requestArray;

        /// <summary>
        /// The current index
        /// </summary>
        protected int currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineMultiRequest"/> class.
        /// </summary>
        /// <param name="requestDelegate">The request delegate.</param>
        public UPOfflineMultiRequest(UPOfflineRequestDelegate requestDelegate)
        {
            this.RequestDelegate = requestDelegate;
        }

        /// <summary>
        /// Gets the request delegate.
        /// </summary>
        /// <value>
        /// The request delegate.
        /// </value>
        public UPOfflineRequestDelegate RequestDelegate { get; private set; }

        /// <summary>
        /// Adds the request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void AddRequest(UPOfflineRequest request)
        {
            if (this.requestArray == null)
            {
                this.requestArray = new List<UPOfflineRequest>();
            }

            this.requestArray.Add(request);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            bool first = true;
            int rootRequestId = 0;

            foreach (UPOfflineRequest request in this.requestArray)
            {
                if (first)
                {
                    rootRequestId = request.SaveToOfflineStorageAsFollowUpRoot();
                    first = false;
                }
                else
                {
                    request.SaveToOfflineStorage(rootRequestId);
                }

                request.OnPreStartMultiRequest();
            }

            this.currentIndex = 0;
            this.StartNextRequest();
        }

        /// <summary>
        /// Starts the next request.
        /// </summary>
        private void StartNextRequest()
        {
            if (this.requestArray.Count > this.currentIndex)
            {
                UPOfflineRequest request = this.requestArray[this.currentIndex++];
                request.StartSync(this, false);
            }
            else
            {
                this.RequestDelegate.OfflineRequestDidFinishMultiRequest(this);
            }
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            if (online)
            {
                request.DeleteRequest(false);
                for (int i = this.currentIndex; i < this.requestArray.Count; i++)
                {
                    UPOfflineRequest followUpRequest = this.requestArray[i];
                    followUpRequest.RedoRecordOperations();
                }

                this.RequestDelegate.OfflineRequestDidFinishWithResult(request, data, online, context, result);
                this.StartNextRequest();
            }
            else
            {
                this.RequestDelegate.OfflineRequestDidFinishWithResult(request, data, online, context, result);
                for (int i = this.currentIndex; i < this.requestArray.Count; i++)
                {
                    UPOfflineRequest followUpRequest = this.requestArray[i];
                    if (!followUpRequest.StoreBeforeRequest)
                    {
                        followUpRequest.RedoRecordOperations();
                    }

                    this.RequestDelegate.OfflineRequestDidFinishWithResult(followUpRequest, data, false, null, null);
                }

                this.RequestDelegate.OfflineRequestDidFinishMultiRequest(this);
            }
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            if (error.IsConnectionOfflineError())
            {
                for (int i = this.currentIndex; i < this.requestArray.Count; i++)
                {
                    UPOfflineRequest followUpRequest = this.requestArray[i];
                    if (!followUpRequest.StoreBeforeRequest)
                    {
                        followUpRequest.RedoRecordOperations();
                    }
                }

                this.RequestDelegate.OfflineRequestDidFailWithError(request, data, context, error);
                this.RequestDelegate.OfflineRequestDidFinishMultiRequest(this);
            }
            else
            {
                request.DeleteRequest(false);
                while (this.currentIndex < this.requestArray.Count)
                {
                    UPOfflineRequest req = this.requestArray[this.currentIndex++];
                    req.DeleteRequest(false);
                }

                this.RequestDelegate.OfflineRequestDidFailWithError(request, data, context, error);
            }
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
