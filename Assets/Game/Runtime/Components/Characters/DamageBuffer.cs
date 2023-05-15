using System;
using Unity.Collections;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    public struct DamageBuffer : IDisposable
    {
        public NativeQueue<float> Buffer;

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}