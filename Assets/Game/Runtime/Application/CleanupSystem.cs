using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Application
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class CleanupSystem : ICleanupSystem 
    {
        protected CleanupSystem(World world)
        {
            World = world;
        }

        public World World { get; set; }

        public abstract void OnAwake();
        public abstract void OnUpdate(float deltaTime);
        public abstract void Dispose();
    }
}