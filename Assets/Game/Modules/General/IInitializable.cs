using System;

namespace Modules.General
{
    public interface IInitializable : IDisposable
    {
        // ReSharper disable once UnusedParameter.Global
        void Initialize(object data = null);
    }
}