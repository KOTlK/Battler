using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Characters
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class PlayerInputSystem : UpdateSystem
    {
        private readonly Camera _camera;
        
        private Filter _selectedSquads;
        private RaycastHit _hit;

        public PlayerInputSystem(World world) : base(world)
        {
            _camera = Camera.main;
        }

        public override void OnAwake()
        {
            _selectedSquads = World.Filter.With<Squad>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    foreach (var entity in _selectedSquads)
                    {
                        ref var command = ref entity.AddComponent<MoveCommand>();
                        command.Position = _hit.point;
                        command.LookDirection = _camera.transform.forward;
                        command.LookDirection.y = 0;
                    }
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}