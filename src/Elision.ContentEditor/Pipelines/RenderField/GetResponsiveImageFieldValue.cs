using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sitecore.Pipelines.RenderField;
using Sitecore.Web;

namespace Elision.ContentEditor.Pipelines.RenderField
{
    public class GetResponsiveImageFieldValue : GetImageFieldValue
    {
        public new void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "image")
                return;

            var useSrcSet = args.Parameters.ContainsKey("usesrcset") && args.Parameters["usesrcset"] == "True";
            args.Parameters.Remove("usesrcset");
            var suppressSrcParameters = args.Parameters.ContainsKey("suppressSrcParameters")
                ? (args.Parameters["suppressSrcParameters"] ?? "").Split(",|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                : new string[0];
            args.Parameters.Remove("suppressSrcParameters");

            base.Process(args);

            if (string.IsNullOrWhiteSpace(args.Result.FirstPart))
                return;

            args.Result.FirstPart = RemoveSuppressedSrcParameters(suppressSrcParameters, args.Result.FirstPart);

            if (!useSrcSet)
                return;

            args.Result.FirstPart = args.Result.FirstPart.Replace(" src=", " srcset=");
            args.Result.FirstPart = Regex.Replace(args.Result.FirstPart, @"(\swidth=""[\w\d]+"")|(\sheight=""[\w\d]+"")", "");
        }

        private string RemoveSuppressedSrcParameters(IEnumerable<string> suppressSrcParameters, string htmlTag)
        {
            var match = Regex.Match(htmlTag, @"\ssrc=""(?<src>[^""]*)""");
            if (!match.Success)
                return htmlTag;

            var src = match.Groups["src"].Value;
            var queryValues = WebUtil.ParseQueryString(src);
            foreach (var suppressSrcParameter in suppressSrcParameters)
            {
                if (queryValues.ContainsKey(suppressSrcParameter))
                    queryValues.Remove(suppressSrcParameter);
            }
            var queryString = WebUtil.BuildQueryString(queryValues, false);

            return Regex.Replace(htmlTag,
                                 @"\ssrc=""(?<path>[^\?]*)(?<query>\?[^""]*)?""",
                                 " src=\"$1?" + queryString + "\"");
        }
    }
}