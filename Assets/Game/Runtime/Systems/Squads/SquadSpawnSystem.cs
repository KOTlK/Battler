using System.Collections.Generic;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.MonoHell.Configs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Collections;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    public class SquadSpawnSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<Config> _config;
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<SpawnSquadCommand>> _filter = default;
        private readonly EcsPoolInject<SpawnSquadCommand> _spawnSquadCommands = default;
        private readonly EcsPoolInject<SpawnCharacterCommand> _spawnCharacterCommands = default;
        private readonly EcsPoolInject<Formation> _formations = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<DamageBuffer> _damageBuffers = default;
        private readonly EcsPoolInject<Character> _characters = default;
        private readonly EcsPoolInject<Health> _healths = default;
        private readonly EcsPoolInject<CharacterView> _characterViews = default;
        private readonly EcsPoolInject<MovableCharacter> _movableCharacters = default;
        private readonly EcsPoolInject<CharacterPreview> _characterPreviews = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                var squadEntity = _world.Value.NewEntity();
                var localPosition = Vector3.zero;
                var formationPosition = Vector2Int.zero;
                ref var formation = ref _formations.Value.Add(squadEntity);
                ref var squad = ref _squads.Value.Add(squadEntity);
                ref var damageBuffer = ref _damageBuffers.Value.Add(squadEntity);
                ref var command = ref _spawnSquadCommands.Value.Get(entity);

                formation.Graph = new FormationGraph();
                formation.Graph.NeighboursMap = new Dictionary<FormationNode, IEnumerable<FormationNode>>();
                formation.Graph.AllNodes = new List<FormationNode>();
                formation.MaxColumns = command.MinColumnsCount;
                formation.Forward = Vector3.forward;
                damageBuffer.Buffer = new NativeQueue<float>(Allocator.Persistent);
                squad.AliveMembers = new List<int>();
                squad.DeadMembers = new List<int>();
                squad.AllMembers = new int[command.Count];
                squad.TotalCount = command.Count;
                squad.MinColumnsCount = command.MinColumnsCount;
                squad.MaxColumnsCount = command.MaxColumnsCount;
                squad.AttackMode = command.AttackMode;
                squad.HaveRangedAttack = command.HaveRangedAttack;
                squad.DistanceBetweenUnits = command.DistanceBetweenUnits;

                for (var i = 0; i < command.Count; i++)
                {
                    var characterEntity = _world.Value.NewEntity();
                    var spawnCharacterEntity = _world.Value.NewEntity();
                    ref var spawnCharacterCommand = ref _spawnCharacterCommands.Value.Add(spawnCharacterEntity);

                    spawnCharacterCommand.Prefab = command.CharacterPrefab;
                    spawnCharacterCommand.MaxHealth = command.CharacterHealth;
                    spawnCharacterCommand.Damage = command.CharactersDamage;
                    spawnCharacterCommand.Speed = command.CharactersSpeed;
                    spawnCharacterCommand.Position = localPosition + command.Position;

                    SpawnCharacter(characterEntity, spawnCharacterCommand);
                    
                    formation.Graph.AllNodes.Add(new FormationNode()
                    {
                        Entity = characterEntity,
                        Position = formationPosition
                    });
                    squad.AliveMembers.Add(characterEntity);
                    squad.AllMembers[i] = characterEntity;

                    localPosition.x += command.DistanceBetweenUnits;
                    formationPosition.x++;
                    if (formationPosition.x >= formation.MaxColumns)
                    {
                        formationPosition.x = 0;
                        formationPosition.y--;
                    }

                    if (localPosition.x >= formation.MaxColumns)
                    {
                        localPosition.x = 0;
                        localPosition.z -= command.DistanceBetweenUnits;
                    }
                }

                formation.Graph.RecalculateMap();


                _world.Value.DelEntity(entity);
            }
        }

        private void SpawnCharacter(int entity, SpawnCharacterCommand command)
        {
            ref var character = ref _characters.Value.Add(entity);
            ref var health = ref _healths.Value.Add(entity);
            ref var view = ref _characterViews.Value.Add(entity);
            ref var movable = ref _movableCharacters.Value.Add(entity);
            ref var preview = ref _characterPreviews.Value.Add(entity);
            var instance = Object.Instantiate(command.Prefab);
            var previewInstance = Object.Instantiate(_config.Value.PreviewPrefab);
                
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
        }
    }
}