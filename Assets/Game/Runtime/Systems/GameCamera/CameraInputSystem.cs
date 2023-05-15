using Game.Runtime.Components.Camera;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Runtime.Systems.GameCamera
{
    public class CameraInputSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraInput>> _filter = default;
        private readonly EcsPoolInject<CameraInput> _inputsPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var input = ref _inputsPool.Value.Get(entity);
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
    }
}