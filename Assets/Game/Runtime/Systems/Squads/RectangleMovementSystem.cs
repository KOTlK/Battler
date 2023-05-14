using System.Linq;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.Extensions;
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
            _squads = World.Filter.With<Squad>().With<Formation>().With<MoveCommand>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _squads)
            {
                ref var squad = ref entity.GetComponent<Squad>();
                ref var formation = ref entity.GetComponent<Formation>();
                ref var command = ref entity.GetComponent<MoveCommand>();
                var startPosition = command.Position;
                var offset = Vector3.Cross(command.LookDirection, Vector3.down).normalized * squad.DistanceBetweenUnits;
                var forward = command.LookDirection;

                for (var i = 0; i < formation.Graph.AllNodes.Count; i++)
                {
                    var node = formation.Graph.AllNodes.ElementAt(i);
                    ref var moveCommand = ref node.Entity.AddComponent<MoveCommand>();
                    var localPosition = node.Position.FromXZ();

                    moveCommand.Position = startPosition + offset * localPosition.x + forward * localPosition.z;
                }

                entity.RemoveComponent<MoveCommand>();
            }
        }

        public override void Dispose()
        {
        }
    }
}