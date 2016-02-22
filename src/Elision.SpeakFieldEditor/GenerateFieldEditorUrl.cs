using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;

namespace Elision.SpeakFieldEditor
{
    public class GenerateFieldEditorUrl : PipelineProcessorRequest<ItemContext>
    {
        public string GenerateUrl()
        {
            var parameters = WebUtil.ParseQueryString(RequestContext.Argument);
            var fieldeditorOption = new FieldEditorOptions(CreateFieldDescriptors(parameters["fieldnames"], parameters["datasource"]));

            if (!string.IsNullOrWhiteSpace(parameters["dialogtitle"]))
                fieldeditorOption.DialogTitle = parameters["dialogtitle"];

            bool boolValue;
            fieldeditorOption.PreserveSections = !bool.TryParse(parameters["preservesections"], out boolValue) || boolValue;

            fieldeditorOption.SaveItem = true;
            return fieldeditorOption.ToUrlString().ToString();
        }

        private IEnumerable<FieldDescriptor> CreateFieldDescriptors(string fields, string datasourceString)
        {
            Item item = null;
            var db = Factory.GetDatabase(RequestContext.Database);
            if (ID.IsID(datasourceString))
            {
                var itemId = ID.Parse(datasourceString);
                if (!ID.IsNullOrEmpty(itemId))
                {
                    item = db.GetItem(itemId);
                }
            }
            else if (!string.IsNullOrWhiteSpace(datasourceString) && datasourceString.StartsWith("query:"))
            {
                item = RequestContext.Item.Axes.SelectSingleItem(datasourceString.Substring("query:".Length));
            }
            if (item == null)
                item = RequestContext.Item;

            var fieldString = new ListString(fields);
            return new ListString(fieldString)
                .Where(x => item.Fields[x] != null)
                .Select(field => new FieldDescriptor(item, field))
                .ToList();
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var response = new PipelineProcessorResponseValue();
            try
            {
                response.Value = GenerateUrl();
            }
            catch (Exception ex)
            {
                response.AbortMessage = ex.ToString();
            }
            return response;
        }
    }
}