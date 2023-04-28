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
    public class PlayerInputSystem : UpdateSystem
    {
        private readonly Camera _camera;
        
        private Filter _selectedCharacters;
        private RaycastHit _hit;

        public PlayerInputSystem(World world) : base(world)
        {
            _camera = Camera.main;
        }

        public override void OnAwake()
        {
            _selectedCharacters = World.Filter.With<Character>().With<MovableCharacter>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    foreach (var entity in _selectedCharacters)
                    {
                        ref var movement = ref entity.GetComponent<MovableCharacter>();
                        movement.Agent.SetDestination(_hit.point);
                    }
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}