using System.Configuration;
using Sitecore.Analytics.Data.DataAccess.MongoDb;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Elision.MongoDb.Process
{
    public class AbortPipelineIfMongoDbIsRunning
    {
        private readonly string _connectionStringName;

        public AbortPipelineIfMongoDbIsRunning(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        public void Process(PipelineArgs args)
        {
            Log.Audit("Testing mongodb connection", this);

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[_connectionStringName];
            if (connectionStringSettings == null)
            {
                Log.SingleError(string.Format("Unable to determine MongoDB status because connection string name '{0}' was not found in the web.config.", _connectionStringName), this);
                args.AbortPipeline();
                return;
            }

            var driver = new MongoDbDriver(connectionStringSettings.ConnectionString);
            try
            {
                if (driver.DatabaseAvailable)
                {
                    Log.Audit("Mongo Already running", this);
                    args.AbortPipeline();
                    return;
                }
            }
            catch{}

            Log.Audit("Mongo Not Running", this);
        }
    }
}
