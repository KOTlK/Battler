using Game.Runtime.Application;
using Game.Runtime.Components.Camera;
using Game.Runtime.MonoHell.Configs;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.GameCamera
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class CameraMovementSystem : UpdateSystem
    {
        private readonly Camera _camera;
        private readonly CameraConfig _config;

        private Filter _input;
        private float _rotationX;
        private float _rotationY;
        private float _height = 15f;
        private RaycastHit _hit;

        private const float MaxRotationX = 90f;
        private const float MinHeight = 5f;
        
        public CameraMovementSystem(World world, Camera camera, CameraConfig config) : base(world)
        {
            _camera = camera;
            _config = config;
            _rotationX = camera.transform.rotation.eulerAngles.x;
            _rotationY = camera.transform.rotation.eulerAngles.y;
        }

        public override void OnAwake()
        {
            _input = World.Filter.With<CameraInput>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _input)
            {
                ref var input = ref entity.GetComponent<CameraInput>();
                var cameraTransform = _camera.transform;

                _height = Mathf.Clamp(_height - input.MovementDirection.y * deltaTime * _config.VerticalSpeed, MinHeight, _config.MaxHeight);
                
                cameraTransform.position = CalculateTotalPosition(cameraTransform, ref input, deltaTime);

                _rotationX += input.RotationDirection.x * _config.Sensitivity * deltaTime;
                _rotationY += input.RotationDirection.y * _config.Sensitivity * deltaTime;

                _rotationX = Mathf.Clamp(_rotationX, -MaxRotationX, MaxRotationX);

                cameraTransform.rotation = Quaternion.Euler(new Vector3(_rotationX, _rotationY, 0));
            }
        }

        public override void Dispose()
        {

        }

        private Vector3 CalculateTotalPosition(Transform cameraTransform, ref CameraInput input, float deltaTime)
        {
            var position = cameraTransform.position;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            position += forward * input.MovementDirection.z * deltaTime * _config.Speed +
                                        right * input.MovementDirection.x * deltaTime * _config.Speed;
            
            if (Physics.Raycast(cameraTransform.position, Vector3.down, out _hit, Mathf.Infinity))
            {
                position.y = _hit.point.y + _height;
                cameraTransform.position = position;
            }

            return position;
        }
    }
}