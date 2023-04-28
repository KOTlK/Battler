using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Characters
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CharacterSpawnSystem : UpdateSystem
    {
        private Filter _commands;
        
        public CharacterSpawnSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _commands = World.Filter.With<SpawnCharacterCommand>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _commands)
            {
                ref var command = ref entity.GetComponent<SpawnCharacterCommand>();
                ref var character = ref command.TargetEntity.AddComponent<Character>();
                ref var health = ref command.TargetEntity.AddComponent<Health>();
                ref var view = ref command.TargetEntity.AddComponent<CharacterView>();
                ref var movable = ref command.TargetEntity.AddComponent<MovableCharacter>();
                var instance = Object.Instantiate(command.Prefab);

                character.MaxHealth = command.Config.MaxHealth;
                character.Speed = command.Config.Speed;
                health.Max = command.Config.MaxHealth;
                health.Current = health.Max;
                view.Prefab = command.Prefab;
                view.Instance = instance;
                view.Instance.transform.position = command.Position;
                movable.Speed = command.Config.Speed;
                movable.Agent = instance.NavMeshAgent;

                entity.RemoveComponent<SpawnCharacterCommand>();
            }
        }

        public override void Dispose()
        {

        }
    }
}