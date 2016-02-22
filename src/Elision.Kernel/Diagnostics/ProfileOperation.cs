using System;

namespace Elision.Diagnostics
{
    public class ProfileOperation : IDisposable
    {
        private readonly string _name;

        public ProfileOperation(string name)
        {
            _name = name;
            Sitecore.Diagnostics.Profiler.StartOperation(_name);
        }

        public void Dispose()
        {
            Sitecore.Diagnostics.Profiler.EndOperation();
        }
    }
}
