using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.sitecore.admin;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Sitecore.Security.AccessControl;

namespace Elision.Web.sitecore.admin
{
    public partial class BackupSecurity : AdminPage
    {
        public Handle JobHandle
        {
            get { return ViewState["BackupSecurityJobHandle"] as Handle; }
            set { ViewState["BackupSecurityJobHandle"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            CheckSecurity();
            RefreshFilesList();
            base.OnInit(e);
        }

        private void RefreshFilesList()
        {
            var selectedValue = BackupFiles.SelectedValue;

            if (Directory.Exists(ApplySecurityRunner.BackupFolder))
            {
                var files = Directory
                    .GetFiles(ApplySecurityRunner.BackupFolder, "*.xml", SearchOption.TopDirectoryOnly)
                    .Select(x => new KeyValuePair<string, string>(x, Path.GetFileNameWithoutExtension(x)));
                if (files.Any())
                    BackupFiles.DataSource = files;
            }
            BackupFiles.DataSource = BackupFiles.DataSource
                ?? new[] { new KeyValuePair<string, string>("", "No backup files found.") };

            if (!string.IsNullOrWhiteSpace(selectedValue))
                BackupFiles.SelectedValue = selectedValue;

            BackupFiles.DataBind();
        }

        protected void Start(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BackupFiles.SelectedValue) || !File.Exists(BackupFiles.SelectedValue))
            {
                Output.Text = "<p>You must select a file to restore.</p>";
                return;
            }

            ProgressTimer.Enabled = true;
            StartRestoreButton.Enabled = StartBackupButton.Enabled = false;
            Output.Text = "<p>Starting job...</p>";

            var options = new ApplySecurityRunnerOptions
            {
                Action = "Restore",
                Filename = BackupFiles.SelectedValue,
                Preview = PreviewOnly.Checked,
                StartingItem = StartingItem.Text,
                SkipPathIntegrityCheck = SkipPathIntegrityCheck.Checked
            };
            var job = JobManager.Start(new JobOptions("Restore security",
                "maintenance", Sitecore.Context.Site.Name,
                this, "StartRestoreSecurity",
                new object[1] { options }));
            job.Status.Processed = 0;
            JobHandle = job.Handle;
        }
        protected void StartBackup(object sender, EventArgs e)
        {
            ProgressTimer.Enabled = true;
            StartRestoreButton.Enabled = StartBackupButton.Enabled = false;
            Output.Text = "<p>Starting job...</p>";

            var options = new ApplySecurityRunnerOptions
            {
                Action = "Backup"
            };
            var job = JobManager.Start(new JobOptions("Backup security",
                "maintenance", Sitecore.Context.Site.Name,
                this, "StartRestoreSecurity",
                new object[1] { options }));
            job.Status.Processed = 0;
            JobHandle = job.Handle;
        }

        protected void StartRestoreSecurity(ApplySecurityRunnerOptions options)
        {
            new ApplySecurityRunner(options).Run();
        }

        protected void ProgressTimer_Tick(object sender, EventArgs e)
        {
            var jobHandle = JobHandle;
            if (jobHandle == null)
            {
                Output.Text += "<p>Failed to find job handle</p>";
            }
            else
            {
                Job job = JobManager.GetJob(jobHandle);
                if (job == null)
                {
                    Output.Text += "<p>Failed to find job</p>";
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string errorText = Translate.Text("#Error: ");
                    string[] array = null;
                    lock (job.Status.Messages)
                    {
                        array = new string[job.Status.Messages.Count];
                        job.Status.Messages.CopyTo(array, 0);
                    }
                    if (array != null)
                    {
                        foreach (string str2 in array)
                        {
                            if (str2.StartsWith(errorText))
                            {
                                stringBuilder.Append("<p class=\"error\">");
                                stringBuilder.Append(str2.Substring(errorText.Length));
                                stringBuilder.Append("</p>");
                            }
                            else if (str2.StartsWith("##section: "))
                            {
                                stringBuilder.Append("<h2><span>");
                                stringBuilder.Append(str2.Substring("##section: ".Length));
                                stringBuilder.Append("</span></h2>");
                            }
                            else
                            {
                                stringBuilder.Append("<p>");
                                stringBuilder.Append(str2);
                                stringBuilder.Append("</p>");
                            }
                        }
                    }
                    Output.Text += stringBuilder.ToString();
                    if (!job.IsDone)
                        return;
                    Output.Text += "<h2><span>Job complete</span></h2>";
                    ProgressTimer.Enabled = false;
                    StartRestoreButton.Enabled = StartBackupButton.Enabled = true;
                    RefreshFilesList();
                }
            }
        }

        public class ApplySecurityRunner : AdminJobRunner
        {
            private readonly ApplySecurityRunnerOptions _options;
            public ApplySecurityRunner(ApplySecurityRunnerOptions options)
            {
                _options = options;
            }

            public override string JobName { get { return "ApplySecurity"; } }

            public static string BackupFolder { get { return Path.Combine(Settings.DataFolder, "security"); } }

            public override void Run()
            {
                if (_options == null)
                {
                    LogError("Options not set. Aborting job.");
                    return;
                }

                var db = GetDatabase();
                if (_options.Action == "Restore")
                {
                    using (new BulkUpdateContext())
                    {
                        var rootItem = db.ResolveDatasource(_options.StartingItem);
                        if (rootItem == null)
                        {
                            LogError($"Cannot find starting item '{_options.StartingItem}' in '{db.Name}' database. Aborting job.");
                            return;
                        }

                        var securityXml = new XmlDocument();
                        securityXml.Load(_options.Filename);

                        var entries = securityXml
                            .SelectNodes("/security/items/item")
                            .Cast<XmlElement>()
                            .ToDictionary(
                            x => x.Attributes["id"].Value,
                            x => new SecurityEntry
                            {
                                Id = x.Attributes["id"].Value,
                                Path = x.Attributes["path"].Value,
                                Security = x.Attributes["security"].Value
                            });

                        ApplySecurity(rootItem, entries);
                    }
                }
                else
                {
                    var output = new StringBuilder();
                    var writer = new StringWriter(output);

                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    writer.WriteLine("<security>");
                    writer.WriteLine("\t<items>");
                    BackupSecurity(db.GetRootItem(), writer);
                    writer.WriteLine("\t</items>");
                    writer.WriteLine("</security>");

                    writer.Flush();
                    if (!Directory.Exists(BackupFolder))
                        Directory.CreateDirectory(BackupFolder);
                    File.WriteAllText(Path.Combine(BackupFolder, $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml"), output.ToString(), Encoding.UTF8);
                }
            }

            private void ApplySecurity(Item item, Dictionary<string, SecurityEntry> entries)
            {
                if (item == null) return;

                var securityString = item.Fields[FieldIDs.Security].ContainsStandardValue
                    ? null
                    : item.Security.GetAccessRules().ToString();
                var newSecurityString = securityString;

                if (entries.ContainsKey(item.ID.ToString()))
                {
                    var entry = entries[item.ID.ToString()];

                    if (!_options.SkipPathIntegrityCheck && !entry.Path.Equals(item.Paths.FullPath, StringComparison.InvariantCultureIgnoreCase))
                        LogError($"Skipping item that failed path integrity check '{item.ID}'. Item path '{item.Paths.FullPath}' does not match entry path '{entry.Path}'");
                    else if (entry.Security != securityString)
                        newSecurityString = entry.Security;
                }
                else if (!string.IsNullOrWhiteSpace(securityString))
                {
                    newSecurityString = null;
                }

                if (securityString != newSecurityString)
                {
                    LogMessage($"Updating security for item '{item.Paths.FullPath}' ('{(securityString == null ? "null" : securityString)}' => '{(newSecurityString == null ? "null" : newSecurityString)}')", false);
                    if (!_options.Preview)
                    {
                        using (new EditContext(item, Sitecore.SecurityModel.SecurityCheck.Disable))
                        {
                            if (newSecurityString == null)
                                item.Fields[FieldIDs.Security].Reset();
                            else
                                item.Security.SetAccessRules(AccessRuleCollection.FromString(newSecurityString));
                        }
                        IncrementProcessed();
                    }
                }

                foreach (Item child in item.GetChildren(Sitecore.Collections.ChildListOptions.IgnoreSecurity | Sitecore.Collections.ChildListOptions.SkipSorting))
                {
                    ApplySecurity(child, entries);
                }
            }

            protected void BackupSecurity(Item item, TextWriter writer)
            {
                if (item == null) return;

                if (HasValue(item))
                {
                    var accessRules = item.Security.GetAccessRules();
                    writer.WriteLine("\t\t<item id=\"{0}\" path=\"{1}\" security=\"{2}\" />", item.ID, item.Paths.FullPath, accessRules);
                    IncrementProcessed();
                }
                foreach (Item child in item.GetChildren(Sitecore.Collections.ChildListOptions.IgnoreSecurity | Sitecore.Collections.ChildListOptions.SkipSorting))
                {
                    BackupSecurity(child, writer);
                }
            }

            private static bool HasValue(Item item)
            {
                return item.Fields[FieldIDs.Security].GetValue(false, false, false, false, true) != null;
            }

            private Database GetDatabase()
            {
                return Factory.GetDatabase("master");
            }
        }
        public class SecurityEntry
        {
            public string Id { get; set; }
            public string Path { get; set; }
            public string Security { get; set; }
        }
        public class ApplySecurityRunnerOptions
        {
            public string Action { get; set; }
            public string Filename { get; set; }
            public bool Preview { get; set; }
            public bool SkipPathIntegrityCheck { get; set; }
            public string StartingItem { get; set; }
        }
    }
}