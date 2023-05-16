using System.Linq;
using Game.Runtime.Characters.Components.Movement;
using Game.Runtime.Extensions;
using Game.Runtime.Squads.Components;
using Game.Runtime.Squads.Components.Formations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Runtime.Squads.Systems
{
    public class RectangleMovementSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Squad, Formation, MoveCommand>> _squadsFilter = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<Formation> _formations = default;
        private readonly EcsPoolInject<MoveCommand> _moveCommands = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _squadsFilter.Value)
            {
                ref var squad = ref _squads.Value.Get(entity);
                ref var formation = ref _formations.Value.Get(entity);
                ref var command = ref _moveCommands.Value.Get(entity);
                var startPosition = command.Position;
                var offset = Vector3.Cross(command.LookDirection, Vector3.down).normalized * squad.DistanceBetweenUnits;
                var forward = command.LookDirection;

                for (var i = 0; i < formation.Graph.AllNodes.Count; i++)
                {
                    var node = formation.Graph.AllNodes.ElementAt(i);
                    ref var moveCommand = ref _moveCommands.Value.Add(node.Entity);
                    var localPosition = node.Position.FromXZ();

                    moveCommand.Position = startPosition + offset * localPosition.x + forward * localPosition.z;
                }

                _moveCommands.Value.Del(entity);
            }
        }
    }
}