using System;
using UnityEngine;

namespace Modules.Tools
{
    public static class DisposeAfter
    {
        private static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        
        public static async void DisposeAfterFrame(this IDisposable disposable, int frames = 0)
        {
            for (int i = 0; i < frames; i++)
            {
                await WaitForEndOfFrame;
            }
            
            disposable.Dispose();
        }
    }
}