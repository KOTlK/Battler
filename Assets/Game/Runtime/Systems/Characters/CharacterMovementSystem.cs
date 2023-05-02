using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters.Movement;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.AI;

namespace Game.Runtime.Systems.Characters
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CharacterMovementSystem : UpdateSystem
    {
        private Filter _characters;
        
        public CharacterMovementSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _characters = World.Filter.With<MovableCharacter>().With<MoveCommand>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _characters)
            {
                ref var movable = ref entity.GetComponent<MovableCharacter>();
                ref var command = ref entity.GetComponent<MoveCommand>();

                if (movable.Position != command.Position)
                {
                    movable.Agent.SetDestination(command.Position);
                    entity.RemoveComponent<MoveCommand>();
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}