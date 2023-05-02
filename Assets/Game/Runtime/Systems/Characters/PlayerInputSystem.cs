using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
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

        private Vector3 _startDragPoint;
        private Filter _selectedSquads;
        private RaycastHit _hit;
        private float _minDistance;

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
                    _startDragPoint = _hit.point;
                    _minDistance = 0f;

                    foreach (var entity in _selectedSquads)
                    {
                        ref var squad = ref entity.GetComponent<Squad>();
                        _minDistance += squad.DistanceBetweenUnits * squad.MinColumnsCount;
                        UnityEngine.Debug.Log($"MinDistance: {_minDistance}");
                    }
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    foreach (var entity in _selectedSquads)
                    {
                        entity.SetComponent(new DisplayPreview()
                        {
                            StartPosition = _startDragPoint,
                            EndPosition = _hit.point
                        });
                    }
                    UnityEngine.Debug.DrawRay(_startDragPoint, Vector3.up * 2f, Color.red);
                    UnityEngine.Debug.DrawRay(_hit.point, Vector3.up * 2f, Color.red);
                    UnityEngine.Debug.DrawLine(_startDragPoint, _hit.point, Color.blue);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out _hit))
                {
                    var endDragPoint = _hit.point;
                    var direction = endDragPoint - _startDragPoint;
                    var forward = Vector3.Cross(direction, Vector3.up).normalized;
                    forward.y = 0;
                    var distance = Vector3.Distance(endDragPoint, _startDragPoint);

                    foreach (var entity in _selectedSquads)
                    {
                        ref var squad = ref entity.GetComponent<Squad>();
                        var columnsCount = distance / squad.DistanceBetweenUnits;
                        UnityEngine.Debug.Log($"Distance: {distance}, ColumnsCount: {(int)columnsCount}");
                        
                        if (columnsCount > squad.MinColumnsCount)
                        {
                            ref var command = ref entity.AddComponent<MoveCommand>();

                            command.LookDirection = forward;
                            command.Position = _startDragPoint;
                            
                            entity.SetComponent(new RectangleFormation()
                            {
                                MaxColumns = (int)Math.Floor(columnsCount)
                            });
                        }
                        else
                        {
                            ref var command = ref entity.AddComponent<MoveCommand>();
                            var forwardDirection = _camera.transform.forward;
                            forwardDirection.y = 0;

                            command.LookDirection = forwardDirection;
                            command.Position = _startDragPoint;
                            
                            entity.SetComponent(new RectangleFormation()
                            {
                                MaxColumns = squad.MinColumnsCount
                            });
                        }

                        entity.RemoveComponent<DisplayPreview>();
                        entity.AddComponent<DisablePreview>();
                    }
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}