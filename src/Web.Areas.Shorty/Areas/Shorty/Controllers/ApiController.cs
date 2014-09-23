using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPlugin.Shorty.Areas.Shorty.Models.Api;
using Raven.Client;

namespace MvcPlugin.Shorty.Areas.Shorty.Controllers
{
    // TODO: Configure controller with its own settings class instead of hard coded members
    public class ApiController : RavenDataController
    {
        // TODO: Take TokenAllowedChars out of this class (into configClass)
        private const string TokenAllowedChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // TODO: Take TokenLength out of this class (into configClass)
        private const int TokenLength = 5;

        private readonly Random _rng;

        private static IEnumerable<string> RandomStrings(string allowedChars, int minLength, int maxLength, int count, Random rng)
        {
            var chars = new char[maxLength];

            var setLength = allowedChars.Length;

            while (count-- > 0)
            {
                var length = rng.Next(minLength, maxLength + 1);

                for (var i = 0; i < length; ++i)
                {
                    chars[i] = allowedChars[rng.Next(setLength)];
                }

                yield return new string(chars, 0, length);
            }
        }

        public ApiController(IDocumentStore store)
            : base(store)
        {
            this._rng = new Random();
        }

        // ~/shorty/api/item/Gua8A
        public ActionResult Item(string token)
        {
            // ReSharper disable once ReplaceWithSingleCallToFirst
            var result = this.DocumentSession.Query<ShortLink>().Where(x => String.Equals(x.Token, token, StringComparison.InvariantCulture)).First();
            return View(result);
        }

        // ~/shorty/api/
        // ~/shorty/api/list
        public ActionResult List()
        {
            var result = this.DocumentSession.Query<ShortLink>().AsEnumerable();
            return View(result);
        }

        
        // ~/Gua8A
        // ~/shorty/api/index?token=Gua8A => http://www.google.com
        public ActionResult Index(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new RedirectResult("~/start", true);
            }

            var result = GetUrlFromToken(token);

            return new RedirectResult(result, true);
        }

        protected string GetUrlFromToken(string token)
        {
            // ReSharper disable once ReplaceWithSingleCallToFirst
            var result = this.DocumentSession.Query<ShortLink>().Where(x => x.Token == token).First();

            result.Clicks += 1;

            result.LastClicked = DateTime.UtcNow.Date;

            this.DocumentSession.Store(result);

            return result.OriginalUrl;
        }

        // ~/shorty/api/create?url=https://www.google.com
        public ActionResult Create(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return RedirectPermanent("~/start");
            }

            // TODO: Figure out what the heck is going on here, why dots, slashes and the like are getting eaten by RouteMap
            url = HttpUtility.UrlEncode(url);

            if (Request.QueryString != null)
            {
                // If we don't do this loop, legit parameters of passed url will get eaten. So...
                // TODO: Figure out why and get rid of this loop
                for (var i = 0; i < Request.QueryString.Count; i++)
                {
                    if (i == 0)
                    {
                        url += "?";
                    }
                    else
                    {
                        url += "&";
                    }

                    url += Request.QueryString.GetKey(i) + "=" + HttpUtility.UrlEncode(Request.QueryString.Get(i));
                    
                }
            }

            var result = this.CreateShortyFromUrl(url);

            return View("Item", result.Token);
        }

        protected ShortLink CreateShortyFromUrl(string url)
        {
            // TODO: Check if url already exists and return that token in favor to just store another document
            var token = RandomStrings(TokenAllowedChars, TokenLength, TokenLength, 1, this._rng).First();
            
            var result = new ShortLink
            {
                Created = DateTime.UtcNow,
                OriginalUrl = url,
                Token = token,
                Clicks = 0,
                LastClicked = null,
                QrCodeImagePath = null,
                ShortenedUrl = this.BaseUrl + "/" + token
            };

            this.DocumentSession.Store(result);

            return result;
        }
    }
}
