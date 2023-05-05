﻿using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class SquadsPlacementSystem : UpdateSystem
    {
        private readonly SelectedSquads _selectedSquads;
        private readonly Camera _camera;

        private Vector3 _startDragPoint;
        private Filter _selected;
        private RaycastHit _hit;
        private float _dragTime;

        private const float DistanceBetweenSquads = 2f;
        private const float DragThreshold = 0.1f;

        public SquadsPlacementSystem(World world, SelectedSquads selectedSquads) : base(world)
        {
            _selectedSquads = selectedSquads;
            _camera = Camera.main;
        }

        public override void OnAwake()
        {
            _selected = World.Filter.With<Squad>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    _startDragPoint = _hit.point;
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                _dragTime += deltaTime;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    var direction = _hit.point - _startDragPoint;
                    var forward = Vector3.Cross(direction, Vector3.up).normalized;
                    var squadsCount = _selectedSquads.Squads.Count;
                    var directionPerSquads = direction / squadsCount;
                    var position = _startDragPoint;
                    var distancePerSquad = directionPerSquads.magnitude;

                    foreach (var entity in _selected)
                    {
                        ref var squad = ref entity.GetComponent<Squad>();
                        ref var rectangle = ref entity.GetComponent<RectangleFormation>();
                        var squadLength = squad.MinColumnsCount * squad.DistanceBetweenUnits;
                        var normalizedDirection = direction.normalized;

                        if (distancePerSquad < squadLength)
                        {
                            rectangle.MaxColumns = squad.MinColumnsCount;
                            
                            entity.SetComponent(new DisplayPreview()
                            {
                                Forward = forward,
                                StartPosition = position
                            });
                            
                            position += (normalizedDirection * squadLength) +
                                        normalizedDirection * DistanceBetweenSquads;
                        }
                        else
                        {
                            var availableDistance = distancePerSquad - squadLength;
                            var unitsCapacity = (int)Math.Floor(availableDistance / squad.DistanceBetweenUnits);

                            if (unitsCapacity == 0)
                            {
                                rectangle.MaxColumns = squad.MinColumnsCount;
                            } 
                            else if (unitsCapacity + squad.MinColumnsCount >= squad.MaxColumnsCount)
                            {
                                rectangle.MaxColumns = squad.MaxColumnsCount;
                            }
                            else
                            {
                                rectangle.MaxColumns = squad.MinColumnsCount + unitsCapacity;
                            }

                            var totalLength = squad.DistanceBetweenUnits * rectangle.MaxColumns;

                            
                            entity.SetComponent(new DisplayPreview()
                            {
                                Forward = forward,
                                StartPosition = position
                            });
                            
                            position += normalizedDirection * totalLength + normalizedDirection * DistanceBetweenSquads;
                        }
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (_dragTime >= DragThreshold)
                {
                    foreach (var entity in _selected)
                    {
                        ref var command = ref entity.AddComponent<MoveCommand>();
                        ref var preview = ref entity.GetComponent<DisplayPreview>();

                        command.Position = preview.StartPosition;
                        command.LookDirection = preview.Forward;

                        entity.RemoveComponent<DisplayPreview>();
                        entity.AddComponent<DisablePreview>();
                    }
                }
                else
                {
                    var forward = _camera.transform.forward;
                    var right = Vector3.Cross(forward, Vector3.down).normalized;
                    forward.y = 0;
                    var position = _startDragPoint;

                    foreach (var entity in _selected)
                    {
                        ref var command = ref entity.AddComponent<MoveCommand>();
                        ref var squad = ref entity.GetComponent<Squad>();
                        ref var formation = ref entity.GetComponent<RectangleFormation>();
                        var squadLength = squad.MinColumnsCount * squad.DistanceBetweenUnits;

                        command.Position = position;
                        command.LookDirection = forward;
                        formation.MaxColumns = squad.MinColumnsCount;

                        entity.RemoveComponent<DisplayPreview>();
                        entity.AddComponent<DisablePreview>();
                        position += right * squadLength + right * DistanceBetweenSquads;
                    }
                }

                _dragTime = 0f;
            }
        }

        public override void Dispose()
        {

        }
    }
}