using Game.Runtime.Components.Characters.Movement;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Runtime.Systems.Characters
{
    public class CharacterMovementSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<MovableCharacter, MoveCommand>> _characters = default;
        private readonly EcsPoolInject<MovableCharacter> _movableCharacters = default;
        private readonly EcsPoolInject<MoveCommand> _moveCommands = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _characters.Value)
            {
                ref var movable = ref _movableCharacters.Value.Get(entity);
                ref var command = ref _moveCommands.Value.Get(entity);

                if (movable.Position != command.Position)
                {
                    movable.Agent.speed = movable.Speed;
                    movable.Agent.SetDestination(command.Position);
                    _moveCommands.Value.Del(entity);
                }
            }
        }
    }
}