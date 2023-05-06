using System;
using Scellecs.Morpeh;
using Unity.Collections;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Components.Characters
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct DamageBuffer : IComponent, IDisposable
    {
        public NativeQueue<float> Buffer;

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}