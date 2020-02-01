using System;
using System.Collections.Generic;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace Modules.General
{
    public interface ICompositeInitializable : IInitializable
    {
        List<IInitializable> Initializables { get; }
        List<IDisposable> Disposables { get; }
    }
}