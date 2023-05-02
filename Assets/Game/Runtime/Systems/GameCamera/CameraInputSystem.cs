using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Camera;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.GameCamera
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CameraInputSystem : UpdateSystem
    {
        private Filter _filter;

        public CameraInputSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<CameraInput>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var input = ref entity.GetComponent<CameraInput>();
                var x = Input.GetAxis("Horizontal");
                var y = Input.GetAxis("Vertical");
                var scroll = Input.mouseScrollDelta.y;
                input.MovementDirection = new Vector3(x, scroll, y);
                
                if (Input.GetKey(KeyCode.Mouse2))
                {
                    var mouseX = Input.GetAxis("Mouse X");
                    var mouseY = Input.GetAxis("Mouse Y");
                    input.RotationDirection = new Vector3(-mouseY, mouseX, 0);
                }
                else
                {
                    input.RotationDirection = Vector3.zero;
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}