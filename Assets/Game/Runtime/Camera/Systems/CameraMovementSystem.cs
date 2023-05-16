using Game.Runtime.Camera.Components;
using Game.Runtime.MonoHell.Configs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Time = Game.Runtime.Application.Time;

namespace Game.Runtime.Camera.Systems
{
    public class CameraMovementSystem : IEcsRunSystem, IEcsInitSystem
    {

        private readonly EcsFilterInject<Inc<CameraInput>> _filter = default;
        private readonly EcsPoolInject<CameraInput> _inputs = default;
        private readonly EcsCustomInject<Time> _time = default;
        private readonly EcsCustomInject<Config> _config;
        private readonly EcsCustomInject<UnityEngine.Camera> _camera;

        private float _rotationX;

        private float _rotationY;

        private float _height = 15f;

        private RaycastHit _hit;

        private const float MaxRotationX = 90f;

        private const float MinHeight = 5f;

        public void Init(IEcsSystems systems)
        {
            _rotationX = _camera.Value.transform.rotation.eulerAngles.x;
            _rotationY = _camera.Value.transform.rotation.eulerAngles.y;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var input = ref _inputs.Value.Get(entity);
                var cameraTransform = _camera.Value.transform;

                _height = Mathf.Clamp(_height - input.MovementDirection.y * _time.Value.DeltaTime * _config.Value.CameraConfig.VerticalSpeed, MinHeight, _config.Value.CameraConfig.MaxHeight);
                
                cameraTransform.position = CalculateTotalPosition(cameraTransform, ref input, _time.Value.DeltaTime);

                _rotationX += input.RotationDirection.x * _config.Value.CameraConfig.Sensitivity * _time.Value.DeltaTime;
                _rotationY += input.RotationDirection.y * _config.Value.CameraConfig.Sensitivity * _time.Value.DeltaTime;

                _rotationX = Mathf.Clamp(_rotationX, -MaxRotationX, MaxRotationX);

                cameraTransform.rotation = Quaternion.Euler(new Vector3(_rotationX, _rotationY, 0));
            }
        }

        private Vector3 CalculateTotalPosition(Transform cameraTransform, ref CameraInput input, float deltaTime)
        {
            var position = cameraTransform.position;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            position += forward * input.MovementDirection.z * deltaTime * _config.Value.CameraConfig.Speed +
                                        right * input.MovementDirection.x * deltaTime * _config.Value.CameraConfig.Speed;
            
            if (Physics.Raycast(cameraTransform.position, Vector3.down, out _hit, Mathf.Infinity))
            {
                position.y = _hit.point.y + _height;
                cameraTransform.position = position;
            }

            return position;
        }
    }
}