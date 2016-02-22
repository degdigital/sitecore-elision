using Sitecore.Diagnostics;
using Sitecore.Jobs;

namespace Elision.Web.sitecore.admin
{
    public abstract class AdminJobRunner
    {
        public abstract void Run();
        public abstract string JobName { get; }

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
            Log.Info(JobName + ": " + message, this);
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
                Log.Info(JobName + " error: " + message, this);
            }
        }

        public void LogSection(string message)
        {
            LogMessage("##section: " + message, false);
        }

    }
}