using Game.Runtime.Components.Camera;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.Configs;
using Game.Runtime.MonoHell.View.Selection;
using Game.Runtime.Systems.Application;
using Game.Runtime.Systems.Characters;
using Game.Runtime.Systems.Debug;
using Game.Runtime.Systems.GameCamera;
using Game.Runtime.Systems.Squads;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.UnityEditor;
using TMPro;
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
        [SerializeField] private TMP_Text _debugText;
        [SerializeField] private SpawnSquadCommand[] _squadsToSpawn;
        
        private EcsWorld _world;
        private IEcsSystems _systems;
        
        private void Start()
        {
            UnityEngine.Application.targetFrameRate = 120;

            _world = new EcsWorld();
            _systems = new EcsSystems(_world);

            var cameraEntity = _world.NewEntity();
            _world.GetPool<CameraInput>().Add(cameraEntity);

            var commandsPool = _world.GetPool<SpawnSquadCommand>();

            foreach (var squadSpawnCommand in _squadsToSpawn)
            {
                var spawnEntity = _world.NewEntity();

                ref var command = ref commandsPool.Add(spawnEntity);
                command = squadSpawnCommand;
            }

            SelectedSquads selectedSquads;

            _systems
                .Add(new TimeSystem())
                .Add(new CameraInputSystem())
                .Add(new CameraMovementSystem())
                .Add(new SquadSpawnSystem())
                .Add(new SquadsPlacementSystem(selectedSquads = new SelectedSquads()))
                .Add(new SelectSquadSystem(_view, selectedSquads))
                .Add(new DebugDamageSystem(_debugDamageView))
                .Add(new RebuildFormationSystem())
                .Add(new RectangleFormationPreviewSystem())
                .Add(new DisablePreviewSystem())
                .Add(new RectangleMovementSystem())
                .Add(new CharacterMovementSystem())
                .Add(new SquadDamageSystem())
                //.Add(new FormationDebugSystem(_debugText))
                .Add(new ApplyPreviewPositions())
                .Add(new EcsWorldDebugSystem())
                .Inject(new Time(), _config, _camera)
                .Init();
        }

        private void Update()
        {
            _systems.Run();
        }
    }
}