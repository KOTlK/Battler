using System;
using Unity.Collections;

namespace Game.Runtime.Characters.Components
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