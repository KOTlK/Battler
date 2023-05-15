using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = System.Random;

namespace Game.Runtime.Systems.Debug
{
    public class DebugDamageSystem : IEcsRunSystem
    {
        private readonly DebugDamage _debugDamageView;
        private readonly EcsFilterInject<Inc<Squad, Selected, DamageBuffer>> _filter = default;
        private readonly EcsPoolInject<DamageBuffer> _damageBuffers = default;
        private readonly Random _random = new Random();
        
        public DebugDamageSystem(DebugDamage debugDamageView)
        {
            _debugDamageView = debugDamageView;
        }

        public void Run(IEcsSystems systems)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                foreach (var entity in _filter.Value)
                {
                    ref var buffer = ref _damageBuffers.Value.Get(entity);
                    buffer.Buffer.Enqueue((float)_random.NextDouble() * float.Parse(_debugDamageView.InputField.text));
                }
            }
        }
    }
}