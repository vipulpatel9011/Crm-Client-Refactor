// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieStorage.cs" company="Aurea Software Gmbh">
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
//   Implements a coockie storage
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Net;

    /// <summary>
    /// Implements a coockie storage
    /// </summary>
    public class CookieStorage
    {
        /// <summary>
        /// The container.
        /// </summary>
        private CookieContainer container;

        /// <summary>
        /// The domain cookies.
        /// </summary>
        private CookieCollection domainCookies;

        /// <summary>
        /// Adds the cookies to request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void AddCookiesToRequest(HttpWebRequest request)
        {
            var path = this.PathFromUrl(request.RequestUri);

            // Host cookies
            var cookieArray = this.container.GetCookies(request.RequestUri);
            var sendCookies = cookieArray;

            // Domain cookies
            // domainCookies.Where(domainCookie => CookieMatchesRequest(domainCookie, request);
            // sendCookies.Where<Cookie>(domainCookie => CookieMatchesRequest(domainCookie, request)));

#if PORTING
            var headerFields = NSHTTPCookie.RequestHeaderFieldsWithCookies(sendCookies);
            foreach (string key in headerFields)
            {
                if (request.Headers[key] == null)
                {
                    request.Headers.AddValueForHTTPHeaderField(headerFields.ObjectForKey(key), key);
                }
            }
#endif
        }

        /// <summary>
        /// Cookies the matches request.
        /// </summary>
        /// <param name="cookie">
        /// The cookie.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CookieMatchesRequest(Cookie cookie, WebRequest request)
        {
            if (cookie == null || request == null)
            {
                return false;
            }

            var path = this.PathFromUrl(request.RequestUri);
            return cookie.Domain.ToLower().Contains(path);
        }

        /// <summary>
        /// Gets the cookies from response for URL.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="url">
        /// The URL.
        /// </param>
        public void GetCookiesFromResponseForUrl(HttpWebResponse response, Uri url)
        {
            var newCookies = response?.Cookies;
            if (newCookies == null || newCookies.Count == 0)
            {
                return;
            }

            var path = this.PathFromUrl(url);
            foreach (Cookie cookie in newCookies)
            {
                if (cookie.Domain.ToLower().Contains(path))
                {
                    // Handle domain cookies Needed for Revolution / amazon enviroment
                    if (this.domainCookies == null)
                    {
                        this.domainCookies = new CookieCollection();
                    }

                    this.domainCookies.Add(cookie);
                }
                else
                {
                    // Handle host cookies
                    if (this.container == null)
                    {
                        this.container = new CookieContainer();
                    }

                    this.container.Add(new Uri(url.Host), cookie);
                }
            }
        }

        /// <summary>
        /// Pathes from URL.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string PathFromUrl(Uri url)
        {
            return url?.Host?.ToLower();
        }
    }
}
