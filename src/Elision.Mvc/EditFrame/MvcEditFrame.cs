using System;
using System.IO;
using System.Web.UI;
using Sitecore.Web.UI.WebControls;

namespace Elision.Mvc
{
    public class MvcEditFrame : IDisposable
    {
        public const string DefaultEditButtons = "/sitecore/content/Applications/WebEdit/Edit Frame Buttons/Default";
        private readonly EditFrame _frame;
        private readonly HtmlTextWriter _writer;

        public MvcEditFrame(string buttons, TextWriter writer, string dataSource = DefaultEditButtons)
        {
            this._frame = new EditFrame
                {
                    DataSource = dataSource, 
                    Buttons = buttons
                };
            this._writer = new HtmlTextWriter(writer);
        }

        public MvcEditFrame(EditFrame frame)
        {
            this._frame = frame;
        }

        public void RenderFirstPart()
        {
            this._frame.RenderFirstPart(this._writer);
        }

        public void Dispose()
        {
            this._frame.RenderLastPart(this._writer);
            this._writer.Dispose();
        }
    }
}
