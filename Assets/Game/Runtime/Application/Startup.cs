using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.Configs;
using Game.Runtime.MonoHell.View.Selection;
using Game.Runtime.Systems.Characters;
using Game.Runtime.Systems.GameCamera;
using Game.Runtime.Systems.Squads;
using Scellecs.Morpeh;
using UnityEngine;

namespace Game.Runtime.Application
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private Config _config;
        [SerializeField] private SelectionArea _debugRect;
        [SerializeField] private Camera _camera;
        [SerializeField] private MonoHell.View.Characters.CharacterView _characterPrefab;
        [SerializeField] private SelectionArea _view;
        
        private World _world;
        
        private void Start()
        {
            UnityEngine.Application.targetFrameRate = 120;
            
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
            command.Count = 200;
            command.SquadConfig.AttackMode = AttackMode.Melee;
            command.SquadConfig.HaveRangedAttack = false;
            command.SquadConfig.MinColumnsCount = 20;
            command.SquadConfig.DistanceBetweenUnits = 1.4f;
            command.SquadConfig.MaxColumnsCount = 50;
            command.Position = Vector3.zero;


            //add initializers
            
            //add update systems
            systems.AddSystem(new PlayerInputSystem(_world));
            systems.AddSystem(new CameraInputSystem(_world));
            systems.AddSystem(new CameraMovementSystem(_world, _camera, _config.CameraConfig));
            systems.AddSystem(new SquadSpawnSystem(_world));
            systems.AddSystem(new CharacterSpawnSystem(_world));
            systems.AddSystem(new SelectSquadSystem(_world, _view));
            systems.AddSystem(new RectangleFormationPreviewSystem(_world, _config));
            systems.AddSystem(new DisablePreviewSystem(_world));
            systems.AddSystem(new RectangleMovementSystem(_world));
            systems.AddSystem(new CharacterMovementSystem(_world));
            
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