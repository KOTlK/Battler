using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class RectangleMovementSystem : UpdateSystem
    {
        private Filter _squads;
        
        public RectangleMovementSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _squads = World.Filter.With<Squad>().With<RectangleFormation>().With<MoveCommand>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _squads)
            {
                ref var squad = ref entity.GetComponent<Squad>();
                ref var formation = ref entity.GetComponent<RectangleFormation>();
                ref var command = ref entity.GetComponent<MoveCommand>();
                var startPosition = command.Position;
                var localPosition = Vector3.zero;

                foreach (var characterEntity in squad.Members)
                {
                    ref var health = ref characterEntity.GetComponent<Health>();
                    if (health.Current <= 0)
                    {
                        continue;
                    }
                    
                    ref var characterCommand = ref characterEntity.AddComponent<MoveCommand>();
                    characterCommand.Position = localPosition + startPosition;
                    UnityEngine.Debug.DrawRay(localPosition + startPosition, Vector3.up * 5f, Color.green, 5f);
                    
                    localPosition.x++;
                    if (localPosition.x >= formation.MaxColumns)
                    {
                        localPosition.x = 0;
                        localPosition.z--;
                    }
                }

                entity.RemoveComponent<MoveCommand>();
            }
        }

        public override void Dispose()
        {

        }
    }
}