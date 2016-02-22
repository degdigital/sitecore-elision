using System.Collections.Specialized;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;

namespace Elision.CascadingRenderings
{
    public class UncascadePlaceholderCommand : Sitecore.Shell.Applications.WebEdit.Commands.WebEditCommand
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items == null || !context.Items.Any())
                return;

            var placeholderKey = context.Parameters.Get("placeholder");
            if (string.IsNullOrWhiteSpace(placeholderKey))
                return;

            var item = context.Items[0];
            var parameters = new NameValueCollection();
            parameters["id"] = item.ID.ToString();
            parameters["database"] = item.Database.ToString();
            parameters["language"] = item.Language.ToString();
            parameters["version"] = item.Version.ToString();
            parameters["ph"] = placeholderKey;
            Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
        {
            if (!Sitecore.Web.UI.Sheer.SheerResponse.CheckModified()) 
                return;

            if (!args.IsPostBack)
            {
                Sitecore.Web.UI.Sheer.SheerResponse.Confirm(
                    Sitecore.Globalization.Translate.Text("Cancel cascading all renderings from this placeholder to child pages?")
                    );
                args.WaitForPostBack();
            }

            if (!args.HasResult) return;
            if (args.Result != "yes") return;

            var database = Sitecore.Configuration.Factory.GetDatabase(args.Parameters["database"]);
            var language = Sitecore.Globalization.Language.Parse(args.Parameters["language"]);
            var version = Sitecore.Data.Version.Parse(args.Parameters["version"]);
            var item = database.GetItem(args.Parameters["id"], language, version);
            var placeholderKey = args.Parameters["ph"];

            if (item.Fields[LayoutIDs.CascadePlaceholdersField] == null)
                return;

            using (new EditContext(item, SecurityCheck.Disable))
            {
                var value = item.Fields[LayoutIDs.CascadePlaceholdersField].Value ?? "";
                value = string.Join("|", value.Split('|')
                                              .Where(x => !string.IsNullOrWhiteSpace(x))
                                              .Except(new[] {placeholderKey}));
                item.Fields[LayoutIDs.CascadePlaceholdersField].Value = value;
            }
            Sitecore.Web.UI.Sheer.SheerResponse.Redraw();
        }
    }
}
