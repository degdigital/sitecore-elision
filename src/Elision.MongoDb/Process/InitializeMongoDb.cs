using Sitecore.Pipelines;

namespace Elision.MongoDb.Process
{
    public class InitializeMongoDb
    {
        public void Process(PipelineArgs args)
        {
            CorePipeline.Run("startMongoDb", new PipelineArgs());
        }
    }
}
