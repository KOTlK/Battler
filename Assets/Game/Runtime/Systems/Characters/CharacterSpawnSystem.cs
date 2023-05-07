using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.MonoHell.Configs;
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
        private readonly Config _config;
        private Filter _commands;
        
        public CharacterSpawnSystem(World world, Config config) : base(world)
        {
            _config = config;
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
                ref var preview = ref command.TargetEntity.AddComponent<CharacterPreview>();
                var instance = Object.Instantiate(command.Prefab);
                var previewInstance = Object.Instantiate(_config.PreviewPrefab);
                
                previewInstance.Hide();
                preview.Transform = previewInstance.transform;
                preview.Position = command.Position;
                preview.Instance = previewInstance;
                character.MaxHealth = command.MaxHealth;
                character.Speed = command.Speed;
                health.Max = command.MaxHealth;
                health.Current = health.Max;
                view.Instance = instance;
                view.Instance.transform.position = command.Position;
                movable.Speed = command.Speed;
                movable.Agent = instance.NavMeshAgent;

                entity.RemoveComponent<SpawnCharacterCommand>();
            }
        }

        public override void Dispose()
        {

        }
    }
}