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

        private const float Indent = 1.5f;
        
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
                var offset = Vector3.Cross(command.LookDirection, Vector3.down).normalized * Indent;
                var backwards = -command.LookDirection * Indent;
                var previousLocalPosition = Vector3.zero;
                var localPosition = Vector3.zero;
                var currentColumn = 0;

                foreach (var characterEntity in squad.Members)
                {
                    ref var health = ref characterEntity.GetComponent<Health>();
                    if (health.Current <= 0)
                    {
                        continue;
                    }
                    
                    ref var characterCommand = ref characterEntity.AddComponent<MoveCommand>();
                    characterCommand.Position = localPosition + startPosition;

                    currentColumn++;
                    localPosition += offset;
                    if (currentColumn >= formation.MaxColumns)
                    {
                        currentColumn = 0;
                        localPosition = previousLocalPosition;
                        localPosition += backwards;
                        previousLocalPosition = localPosition;
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