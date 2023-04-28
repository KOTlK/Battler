using System;
using System.Linq;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Debug
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class LogSelectedCharacters : UpdateSystem
    {
        private Filter _filter;
        
        public LogSelectedCharacters(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Character>().With<MovableCharacter>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
        {
            UnityEngine.Debug.Log(_filter.Count());
        }

        public override void Dispose()
        {

        }
    }
}