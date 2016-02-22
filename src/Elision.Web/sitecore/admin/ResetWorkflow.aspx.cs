using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.sitecore.admin;
using System;
using System.Linq;
using System.Text;

namespace Elision.Web.sitecore.admin
{
    public partial class ResetWorkflow : AdminPage
    {
        public Handle JobHandle
        {
            get
            {
                return ViewState["ResetWorkflowJobHandle"] as Handle;
            }
            set
            {
                ViewState["ResetWorkflowJobHandle"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            CheckSecurity();
            base.OnInit(e);
        }

        protected void Start(object sender, EventArgs e)
        {
            ProgressTimer.Enabled = true;
            StartButton.Enabled = false;
            Output.Text = "<p>Starting job...</p>";

            var options = new ResetWorkflowRunnerOptions
            {
                IncludePending = IncludePending.Checked,
                StartingItem = StartingItem.Text,
                ClearEmptyWf = ClearWfWhenNoDefault.Checked,
                Preview = PreviewOnly.Checked
            };
            var job = JobManager.Start(new JobOptions("Reset item workflows",
                "maintenance", Sitecore.Context.Site.Name,
                this, "StartWorkflowReset",
                new object[1] { options }));
            job.Status.Processed = 0;
            JobHandle = job.Handle;
        }
        
        protected void StartWorkflowReset(ResetWorkflowRunnerOptions options)
        {
            new ResetWorkflowRunner(options).Run();
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
                    StartButton.Enabled = true;
                }
            }
        }

        public class ResetWorkflowRunner
        {
            private readonly ResetWorkflowRunnerOptions _options;
            public ResetWorkflowRunner(ResetWorkflowRunnerOptions options)
            {
                _options = options;
            }
            public void Run()
            {
                if (_options == null)
                {
                    LogError("Options not set. Aborting job.");
                    return;
                }

                var db = GetDatabase();
                var rootItem = db.ResolveDatasource(_options.StartingItem);
                if (rootItem == null)
                {
                    LogError("Cannot find starting item '" + _options.StartingItem + "' in " + db.Name + " database. Aborting job.");
                    return;
                }
                using (new BulkUpdateContext())
                {
                    ResetItemWorkflow(rootItem);
                }
            }

            protected void ResetItemWorkflow(Item item)
            {
                if (item == null) return;

                var workflow = item.State.GetWorkflow();
                var workflowState = item.State.GetWorkflowState();

                var defaultWorkflowId = item.Fields[FieldIDs.DefaultWorkflow].Value;
                var workflowId = item.Fields[FieldIDs.Workflow].Value;
                var workflowStateId = item.Fields[FieldIDs.WorkflowState].Value;

                var newWorkflowId = workflowId;
                var newWorkflowStateId = workflowStateId;

                if (string.IsNullOrWhiteSpace(defaultWorkflowId))
                {
                    if (_options.ClearEmptyWf)
                    {
                        if (!string.IsNullOrWhiteSpace(workflowId) || !string.IsNullOrWhiteSpace(workflowStateId))
                        {
                            newWorkflowId = string.Empty;
                            newWorkflowStateId = string.Empty;
                        }
                    }
                }
                else
                {
                    var defaultWorkflow = item.Database.WorkflowProvider.GetWorkflow(defaultWorkflowId);
                    var defaultWorkflowFinalState = defaultWorkflow.GetStates().First(x => x.FinalState);
                    
                    if (workflowId != defaultWorkflowId)
                        newWorkflowId = defaultWorkflowId;

                    if (workflowState == null)
                        newWorkflowStateId = defaultWorkflowFinalState.StateID;
                    else if (_options.IncludePending && !workflowState.FinalState)
                        newWorkflowStateId = defaultWorkflowFinalState.StateID;
                }

                if (newWorkflowId != workflowId || newWorkflowStateId != workflowStateId)
                {
                    LogMessage($"Updating workflow for '{item.Paths.FullPath}' (Workflow '{workflowId.Or("null")}' => '{newWorkflowId.Or("null")}', State '{workflowStateId.Or("null")}' => '{newWorkflowStateId.Or("null")}')", false);
                    if (!_options.Preview)
                    {
                        using (new EditContext(item, Sitecore.SecurityModel.SecurityCheck.Disable))
                        {
                            item[FieldIDs.Workflow] = newWorkflowId;
                            item[FieldIDs.WorkflowState] = newWorkflowStateId;
                        }
                        IncrementProcessed();
                    }
                }

                foreach (Item child in item.GetChildren(Sitecore.Collections.ChildListOptions.IgnoreSecurity | Sitecore.Collections.ChildListOptions.SkipSorting))
                {
                    ResetItemWorkflow(child);
                }
            }

            private Database GetDatabase()
            {
                return Factory.GetDatabase("master");
            }

            public void IncrementProcessed()
            {
                Job job = Sitecore.Context.Job;
                if (job == null)
                    return;
                job.Status.IncrementProcessed();
            }

            public void LogMessage(string message, bool logOnly = true)
            {
                if (!logOnly)
                {
                    Job job = Sitecore.Context.Job;
                    if (job == null)
                    {
                        Log.Warn("No job found during workflow reset.", this);
                        return;
                    }
                    lock (job.Status.Messages)
                      job.Status.Messages.Add(message);
                }
                Log.Info("ResetWorkflow: " + message, this);
            }

            public void LogError(string message)
            {
                Job job = Sitecore.Context.Job;
                if (job == null)
                {
                    Log.Warn("No job found during workflow reset.", this);
                }
                else
                {
                    lock (job.Status.Messages)
                      job.Status.LogError(message);
                    Log.Info("ResetWorkflow error: " + message, this);
                }
            }

            public void LogSection(string message)
            {
                LogMessage("##section: " + message, false);
            }

        }
        public class ResetWorkflowRunnerOptions
        {
            public bool ClearEmptyWf { get; set; }
            public bool IncludePending { get; set; }
            public bool Preview { get; set; }
            public string StartingItem { get; set; }
        }
    }
}