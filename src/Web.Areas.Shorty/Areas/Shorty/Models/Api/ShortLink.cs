using System;

namespace MvcPlugin.Shorty.Areas.Shorty.Models.Api
{
    public class ShortLink
    {
        public DateTime Created { get; set; }
        public string Token { get; set; }
        public string ShortenedUrl { get; set; }
        public string OriginalUrl { get; set; }
        public string QrCodeImagePath { get; set; }
        public int Clicks { get; set; }
        public DateTime? LastClicked { get; set; }
    }
}
