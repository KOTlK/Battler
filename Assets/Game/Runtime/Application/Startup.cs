using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.View.Selection;
using Game.Runtime.Systems.Characters;
using Game.Runtime.Systems.Squads;
using Scellecs.Morpeh;
using UnityEngine;

namespace Game.Runtime.Application
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private MonoHell.View.Characters.CharacterView _characterPrefab;
        [SerializeField] private CharacterSelectionArea _view;
        
        private World _world;
        
        private void Start()
        {
            _world = World.Default;
            var systems = _world.CreateSystemsGroup();

            var spawnEntity = _world.CreateEntity();

            ref var command = ref spawnEntity.AddComponent<SpawnSquadCommand>();
            var spawnCharacterCommand = new SpawnCharacterCommand
            {
                Prefab = _characterPrefab,
                Config = new Character()
                {
                    MaxHealth = 100,
                    Speed = 5f
                },
                TargetEntity = _world.CreateEntity()
            };

            command.CharacterConfig = spawnCharacterCommand;
            command.Count = 20;


            //add initializers
            
            //add update systems
            systems.AddSystem(new PlayerInputSystem(_world));
            systems.AddSystem(new SquadSpawnSystem(_world));
            systems.AddSystem(new CharacterSpawnSystem(_world));
            systems.AddSystem(new SelectCharacterSystem(_world, _view));
            
            //add fixed update systems
            
            //add late update systems
            
            //add cleanup systems
            
            
            _world.AddSystemsGroup(0, systems);
            _world.UpdateByUnity = false;
        }

        private void Update()
        {
            _world.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _world.FixedUpdate(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _world.LateUpdate(Time.deltaTime);
            _world.CleanupUpdate(Time.deltaTime);
        }
    }
}