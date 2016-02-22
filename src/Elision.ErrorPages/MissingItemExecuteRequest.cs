using System;
using Sitecore.Configuration;

namespace Elision.ErrorPages
{
    public class MissingItemExecuteRequest : Sitecore.Pipelines.HttpRequest.ExecuteRequest
    {
        protected override void RedirectOnItemNotFound(string url)
        {
            var errorCode = Settings.GetIntSetting("ItemNotFoundUrlErrorCode", 404);
            WriteResponse(Settings.GetSetting("ItemNotFoundUrlManaged", url), errorCode,
                          () => base.RedirectOnItemNotFound(url));
        }

        protected override void RedirectOnLayoutNotFound(string url)
        {
            var errorCode = Settings.GetIntSetting("LayoutNotFoundUrlErrorCode", 500);
            WriteResponse(Settings.GetSetting("LayoutNotFoundUrlManaged", url), errorCode,
                          () => base.RedirectOnLayoutNotFound(url));
        }

        protected override void RedirectOnNoAccess(string url)
        {
            var errorCode = Settings.GetIntSetting("NoAccessUrlErrorCode", 403);
            WriteResponse(Settings.GetSetting("NoAccessUrlManaged", url), errorCode,
                          () => base.RedirectOnNoAccess(url));
        }

        protected override void RedirectOnSiteAccessDenied(string url)
        {
            var errorCode = Settings.GetIntSetting("NoAccessUrlErrorCode", 403);
            WriteResponse(Settings.GetSetting("NoAccessUrlManaged", url), errorCode,
                          () => base.RedirectOnSiteAccessDenied(url));
        }

        private static void WriteResponse(string errorPageUrl, int statusCode, Action fallbackAction)
        {
            var context = System.Web.HttpContext.Current;
            try
            {
                var domain = context.Request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped);
                var content = Sitecore.Web.WebUtil.ExecuteWebPage(string.Concat(domain, errorPageUrl));

                context.Response.TrySkipIisCustomErrors = true;
                context.Response.StatusCode = statusCode;
                context.Response.Write(content);
            }
            catch (Exception)
            {
                fallbackAction();
            }

            context.Response.End();
        }
    }
}
