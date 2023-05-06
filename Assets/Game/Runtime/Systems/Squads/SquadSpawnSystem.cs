﻿using System.Collections.Generic;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
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
                ref var formation = ref squadEntity.AddComponent<RectangleFormation>();
                ref var squad = ref squadEntity.AddComponent<Squad>();
                ref var damageBuffer = ref squadEntity.AddComponent<DamageBuffer>();
                ref var command = ref entity.GetComponent<SpawnSquadCommand>();
                var squadConfig = command.SquadConfig;

                formation.MaxColumns = 20;
                damageBuffer.Buffer = new NativeQueue<float>(Allocator.Persistent);
                squad = squadConfig;
                squad.AliveMembers = new List<Entity>();
                squad.DeadMembers = new List<Entity>();
                squad.AllMembers = new Entity[command.Count];
                squad.TotalCount = command.Count;
                command.CharacterConfig.Config.Squad = squad;
                
                for (var i = 0; i < command.Count; i++)
                {
                    var characterEntity = World.CreateEntity();
                    var spawnCharacterEntity = World.CreateEntity();
                    ref var spawnCharacterCommand = ref spawnCharacterEntity.AddComponent<SpawnCharacterCommand>();

                    spawnCharacterCommand = command.CharacterConfig;
                    spawnCharacterCommand.TargetEntity = characterEntity;
                    spawnCharacterCommand.Position = localPosition + command.Position;
                    squad.AliveMembers.Add(characterEntity);
                    squad.AllMembers[i] = characterEntity;

                    localPosition.x++;
                    if (localPosition.x >= formation.MaxColumns)
                    {
                        localPosition.x = 0;
                        localPosition.z--;
                    }
                }

                entity.RemoveComponent<SpawnSquadCommand>();
            }
        }

        public override void Dispose()
        {

        }
    }
}