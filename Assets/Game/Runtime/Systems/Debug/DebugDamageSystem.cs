using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using Random = System.Random;

namespace Game.Runtime.Systems.Debug
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class DebugDamageSystem : UpdateSystem
    {
        private readonly DebugDamage _debugDamageView;
        private Filter _filter;
        private readonly Random _random = new Random();
        
        public DebugDamageSystem(World world, DebugDamage debugDamageView) : base(world)
        {
            _debugDamageView = debugDamageView;
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Squad>().With<Selected>().With<DamageBuffer>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                foreach (var entity in _filter)
                {
                    ref var buffer = ref entity.GetComponent<DamageBuffer>();
                    buffer.Buffer.Enqueue((float)_random.NextDouble() * float.Parse(_debugDamageView.InputField.text));
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}