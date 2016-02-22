using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Web.UI.WebControls;

namespace Elision.Mvc
{
    public static class MvcEditFrameExtensions
    {
        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper)
        {
            return BeginEditFrame(htmlHelper, "");
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, ID dataSource)
        {
            return BeginEditFrame(htmlHelper, dataSource.ToString());
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, string dataSource)
        {
            return BeginEditFrame(htmlHelper, dataSource, "/sitecore/content/Applications/WebEdit/Edit Frame Buttons/Default");
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, ID dataSource, ID buttons)
        {
            return BeginEditFrame(htmlHelper, dataSource.ToString(), buttons.ToString());
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, ID dataSource, string buttons)
        {
            return BeginEditFrame(htmlHelper, dataSource.ToString(), buttons);
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, string dataSource, ID buttons)
        {
            return BeginEditFrame(htmlHelper, dataSource, buttons.ToString());
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, string dataSource, string buttons)
        {
            var glassEditFrame = new MvcEditFrame(buttons, htmlHelper.ViewContext.Writer, dataSource);
            glassEditFrame.RenderFirstPart();
            return glassEditFrame;
        }

        public static MvcEditFrame BeginEditFrame(this HtmlHelper htmlHelper, EditFrame editFrame)
        {
            var output = new HtmlTextWriter(htmlHelper.ViewContext.Writer);
            editFrame.RenderFirstPart(output);
            return new MvcEditFrame(editFrame);
        }
    }
}