﻿using System;
using System.Web;
using System.Web.Mvc;

namespace CaptchaExample.Helpers
{
    public static class CaptchaHelper
    {
        internal const string SessionKeyPrefix = "__Captcha";
        private const string ImgFormat = "<img src=\"{0}\" />"
                            + @"<input type=""hidden"" name=""{1}"" value=""{2}"" />";

        public static MvcHtmlString Captcha(this HtmlHelper html, string name)
        {
            // Pick a GUID to represent this challenge
            string challengeGuid = Guid.NewGuid().ToString();
            // Generate and store a random solution text
            var session = html.ViewContext.HttpContext.Session;
            session[SessionKeyPrefix + challengeGuid] = MakeRandomSolution();

            // Render an <IMG> tag for the distorted text,
            // plus a hidden field to contain the challenge GUID
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            string url = urlHelper.Action("Render", "CaptchaImage", new { challengeGuid });
            string htmlToDisplay = string.Format(ImgFormat, url, name, challengeGuid);
            return MvcHtmlString.Create(htmlToDisplay);
        }

        public static bool VerifyAndExpireSolution(HttpContextBase context,
                                           string challengeGuid,
                                           string attemptedSolution)
        {
            // Immediately remove the solution from Session to prevent replay attacks
            string solution = (string)context.Session[SessionKeyPrefix + challengeGuid];
            context.Session.Remove(SessionKeyPrefix + challengeGuid);

            return ((solution != null) && (attemptedSolution == solution));
        }

        private static string MakeRandomSolution()
        {
            Random rng = new Random();
            int length = rng.Next(5, 7);
            char[] buf = new char[length];
            for (int i = 0; i < length; i++)
                buf[i] = (char)('a' + rng.Next(26));
            return new string(buf);
        }
    }

}