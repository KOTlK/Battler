using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.Configs;
using Game.Runtime.MonoHell.View.Selection;
using Game.Runtime.Systems.Characters;
using Game.Runtime.Systems.Debug;
using Game.Runtime.Systems.GameCamera;
using Game.Runtime.Systems.Squads;
using Scellecs.Morpeh;
using UnityEngine;

namespace Game.Runtime.Application
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private Config _config;
        [SerializeField] private Camera _camera;
        [SerializeField] private MonoHell.View.Characters.CharacterView _characterPrefab;
        [SerializeField] private SelectionArea _view;
        [SerializeField] private DebugDamage _debugDamageView;
        [SerializeField] private SpawnSquadCommand[] _squadsToSpawn;
        
        private World _world;
        
        private void Start()
        {
            UnityEngine.Application.targetFrameRate = 120;
            
            _world = World.Default;
            var systems = _world.CreateSystemsGroup();

            foreach (var squadSpawnCommand in _squadsToSpawn)
            {
                var spawnEntity = _world.CreateEntity();

                ref var command = ref spawnEntity.AddComponent<SpawnSquadCommand>();
                command = squadSpawnCommand;
            }

            SelectedSquads selectedSquads;

            //add initializers
            
            //add update systems
            systems.AddSystem(new SquadsPlacementSystem(_world, selectedSquads = new SelectedSquads()));
            systems.AddSystem(new CameraInputSystem(_world));
            systems.AddSystem(new CameraMovementSystem(_world, _camera, _config.CameraConfig));
            systems.AddSystem(new SquadSpawnSystem(_world));
            systems.AddSystem(new CharacterSpawnSystem(_world, _config));
            systems.AddSystem(new SelectSquadSystem(_world, _view, selectedSquads));
            systems.AddSystem(new DebugDamageSystem(_world, _debugDamageView));
            systems.AddSystem(new RectangleFormationPreviewSystem(_world));
            systems.AddSystem(new DisablePreviewSystem(_world));
            systems.AddSystem(new RectangleMovementSystem(_world));
            systems.AddSystem(new CharacterMovementSystem(_world));
            systems.AddSystem(new SquadDamageSystem(_world));
            systems.AddSystem(new ApplyPreviewPositions(_world));
            
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