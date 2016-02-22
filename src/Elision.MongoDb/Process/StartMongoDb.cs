using System;
using System.Diagnostics;
using System.IO;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Elision.MongoDb.Process
{
    public class StartMongoDb
    {
        private readonly string _exePath;
        private readonly string _dbFolderPath;

        public StartMongoDb(string exePath, string dbFolderPath)
        {
            _exePath = exePath;
            _dbFolderPath = dbFolderPath;
        }

        public void Process(PipelineArgs args)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = _exePath, //@"C:\Program Files\MongoDB 2.6 Standard\bin\mongod.exe",
                Arguments = @"--dbpath " + _dbFolderPath,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            if (!Directory.Exists(_dbFolderPath))
                Directory.CreateDirectory(_dbFolderPath);

            try
            {
                Log.Audit("Trying to start mongo with command line " + startInfo.FileName + " " + startInfo.Arguments, this);
                int pid = 0;
                using (var exeProcess = System.Diagnostics.Process.Start(startInfo))
                {
                    pid = exeProcess.Id;
                    exeProcess.WaitForExit(50);
                }
                Log.Audit("Mongo started. PID: " + pid, this);
            }
            catch (Exception exception)
            {
                Log.Error("Could not start mongo", exception, this);
            }
        }
    }
}
