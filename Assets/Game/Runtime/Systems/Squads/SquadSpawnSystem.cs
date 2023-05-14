using System;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.Extensions;
using Scellecs.Morpeh;
using Unity.Collections;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class SquadSpawnSystem : UpdateSystem
    {
        private Filter _commands;

        public SquadSpawnSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _commands = World.Filter.With<SpawnSquadCommand>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _commands)
            {
                var squadEntity = World.CreateEntity();
                var localPosition = Vector3.zero;
                var formationPosition = Vector2Int.zero;
                ref var formation = ref squadEntity.AddComponent<Formation>();
                ref var squad = ref squadEntity.AddComponent<Squad>();
                ref var damageBuffer = ref squadEntity.AddComponent<DamageBuffer>();
                ref var command = ref entity.GetComponent<SpawnSquadCommand>();

                formation.Graph = new FormationGraph();
                formation.Graph.NeighboursMap = new Dictionary<FormationNode, IEnumerable<FormationNode>>();
                formation.Graph.AllNodes = new List<FormationNode>();
                formation.MaxColumns = command.MinColumnsCount;
                formation.Forward = Vector3.forward;
                damageBuffer.Buffer = new NativeQueue<float>(Allocator.Persistent);
                squad.AliveMembers = new List<Entity>();
                squad.DeadMembers = new List<Entity>();
                squad.AllMembers = new Entity[command.Count];
                squad.TotalCount = command.Count;
                squad.MinColumnsCount = command.MinColumnsCount;
                squad.MaxColumnsCount = command.MaxColumnsCount;
                squad.AttackMode = command.AttackMode;
                squad.HaveRangedAttack = command.HaveRangedAttack;
                squad.DistanceBetweenUnits = command.DistanceBetweenUnits;
                
                for (var i = 0; i < command.Count; i++)
                {
                    var characterEntity = World.CreateEntity();
                    var spawnCharacterEntity = World.CreateEntity();
                    ref var spawnCharacterCommand = ref spawnCharacterEntity.AddComponent<SpawnCharacterCommand>();

                    spawnCharacterCommand.TargetEntity = characterEntity;
                    spawnCharacterCommand.Prefab = command.CharacterPrefab;
                    spawnCharacterCommand.MaxHealth = command.CharacterHealth;
                    spawnCharacterCommand.Damage = command.CharactersDamage;
                    spawnCharacterCommand.Speed = command.CharactersSpeed;
                    spawnCharacterCommand.Position = localPosition + command.Position;
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
                entity.RemoveComponent<SpawnSquadCommand>();
            }
        }

        public override void Dispose()
        {

        }
    }
}