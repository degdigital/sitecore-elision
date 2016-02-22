using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.Security.Accounts;

namespace Elision.Publishing
{
    public class PublishAgent
    {
        public PublishMode Mode { get; protected set; }
        public string SourceDatabase { get; protected set; }
        public string TargetDatabase { get; protected set; }
        public string Username { get; protected set; }

        public PublishAgent(string sourceDatabase, string targetDatabase, string mode, string username)
        {
            Assert.ArgumentNotNullOrEmpty(sourceDatabase, "sourceDatabase");
            Assert.ArgumentNotNullOrEmpty(targetDatabase, "targetDatabase");
            Assert.ArgumentNotNullOrEmpty(mode, "mode");
            SourceDatabase = sourceDatabase;
            TargetDatabase = targetDatabase;
            Mode = ParseMode(mode);
            Username = username;
        }

        public virtual void Run()
        {
            Log.Info(string.Format("PublishAgent started (source: {0}, target: {1}, mode: {2}, user: {3})",
                                   SourceDatabase, TargetDatabase, Mode, Username), this);

            var user = string.IsNullOrWhiteSpace(Username)
                           ? User.Current
                           : User.FromName(Username, false);

            if (user == null || user == User.Current)
                StartPublish();
            else
                using (new UserSwitcher(user))
                    StartPublish();
        }

        protected virtual void StartPublish()
        {
            var sourceDatabase = Factory.GetDatabase(SourceDatabase);
            var targetDatabase = Factory.GetDatabase(TargetDatabase);
            foreach (var language in LanguageManager.GetLanguages(sourceDatabase))
            {
                try
                {
                    var languageItem = sourceDatabase.GetItem(LanguageManager.GetLanguageItemId(language, sourceDatabase));

                    CheckboxField autoPublish = languageItem.Fields["AutoPublish"];
                    if (autoPublish == null || autoPublish.Checked)
                        StartPublish(language, sourceDatabase, targetDatabase);
                }
                catch (Exception ex)
                {
                    Log.Error("PublishingAgent error initiating publishing for language " + language.Name, ex, this);
                }
            }
        }

        protected PublishMode ParseMode(string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                case "full": return PublishMode.Full;
                case "incremental":return  PublishMode.Incremental;
                case "smart":return  PublishMode.Smart;
                default: return PublishMode.Unknown;
            }
        }

        protected virtual void StartPublish(Language language, Database sourceDatabase, Database targetDatabase)
        {
            Assert.ArgumentNotNull(language, "language");
            Assert.IsNotNull(sourceDatabase, "Unknown database: {0}", SourceDatabase);
            Assert.IsNotNull(targetDatabase, "Unknown database: {0}", TargetDatabase);

            var options = new PublishOptions(sourceDatabase, targetDatabase, Mode, language, DateTime.Now)
                {
                    Deep = Mode != PublishMode.Incremental
                };
            if (Mode == PublishMode.Full)
                options.CompareRevisions = false;

            var publisher = new Publisher(options);
            var willBeQueued = publisher.WillBeQueued;
            publisher.PublishAsync();

            Log.Info(string.Format("Asynchronous publishing {0}. Job name: {1}", willBeQueued ? "queued" : "started", publisher.GetJobName()), this);
            TaskCounters.Publishings.Increment();
        }
    }
}
