using System;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Time = Game.Runtime.Application.Time;

namespace Game.Runtime.Systems.Squads
{
    public class SquadsPlacementSystem : IEcsRunSystem
    {
        private readonly SelectedSquads _selectedSquads;
        private readonly Camera _camera;
        
        private readonly EcsFilterInject<Inc<Squad, Selected>> _selected = default;
        private readonly EcsCustomInject<Time> _time = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<DisplayPreview> _displayPreviewCommands = default;
        private readonly EcsPoolInject<DisablePreview> _disablePreviewCommands = default;
        private readonly EcsPoolInject<Formation> _formations = default;
        private readonly EcsPoolInject<MoveCommand> _moveCommands = default;
        private readonly EcsPoolInject<RebuildFormation> _rebuildFormationCommands = default;

        private Vector3 _startDragPoint;
        private RaycastHit _hit;
        private float _dragTime;

        private const float DistanceBetweenSquads = 2f;
        private const float DragThreshold = 0.1f;

        public SquadsPlacementSystem(SelectedSquads selectedSquads)
        {
            _selectedSquads = selectedSquads;
            _camera = Camera.main;
        }

        public void Run(IEcsSystems systems)
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
                _dragTime += _time.Value.DeltaTime;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit))
                {
                    var direction = _hit.point - _startDragPoint;
                    var forward = Vector3.Cross(direction, Vector3.up).normalized;
                    var squadsCount = _selectedSquads.Squads.Count;
                    var directionPerSquads = direction / squadsCount;
                    var position = _startDragPoint;
                    var distancePerSquad = directionPerSquads.magnitude;

                    foreach (var entity in _selected.Value)
                    {
                        ref var squad = ref _squads.Value.Get(entity);
                        var squadLength = squad.MinColumnsCount * squad.DistanceBetweenUnits;
                        var normalizedDirection = direction.normalized;
                        var maxColumns = squad.MinColumnsCount;

                        if (distancePerSquad < squadLength)
                        {
                            SetPreview(entity, new DisplayPreview()
                            {
                                Forward = forward,
                                StartPosition = position,
                                FormationType = FormationType.Rectangle,
                                MaxColumns = maxColumns
                            });

                            position += (normalizedDirection * squadLength) + normalizedDirection * DistanceBetweenSquads;
                        }
                        else
                        {
                            var availableDistance = distancePerSquad - squadLength;
                            var unitsCapacity = (int)Math.Floor(availableDistance / squad.DistanceBetweenUnits);

                            if (unitsCapacity == 0)
                            {
                                maxColumns = squad.MinColumnsCount;
                            } 
                            else if (unitsCapacity + squad.MinColumnsCount >= squad.MaxColumnsCount)
                            {
                                maxColumns = squad.MaxColumnsCount;
                            }
                            else
                            {
                                maxColumns = squad.MinColumnsCount + unitsCapacity;
                            }

                            var totalLength = squad.DistanceBetweenUnits * maxColumns;
                            
                            
                            SetPreview(entity, new DisplayPreview()
                            {
                                Forward = forward,
                                StartPosition = position,
                                MaxColumns = maxColumns,
                                FormationType = FormationType.Rectangle
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
                    foreach (var entity in _selected.Value)
                    {
                        ref var command = ref _moveCommands.Value.Add(entity);
                        ref var preview = ref _displayPreviewCommands.Value.Get(entity);
                        ref var formation = ref _formations.Value.Get(entity);

                        command.Position = preview.StartPosition;
                        command.LookDirection = preview.Forward;

                        if (formation.MaxColumns != preview.MaxColumns)
                        {
                            ref var rebuildCommand = ref _rebuildFormationCommands.Value.Add(entity);
                            rebuildCommand.FormationType = FormationType.Rectangle;
                            rebuildCommand.Columns = preview.MaxColumns;
                            formation.Forward = preview.Forward;
                        }

                        _displayPreviewCommands.Value.Del(entity);
                        _disablePreviewCommands.Value.Add(entity);
                    }
                }
                else
                {
                    var forward = _camera.transform.forward;
                    var right = Vector3.Cross(forward, Vector3.down).normalized;
                    var position = _startDragPoint;
                    forward.y = 0;

                    foreach (var entity in _selected.Value)
                    {
                        ref var command = ref _moveCommands.Value.Add(entity);
                        ref var rebuildCommand = ref _rebuildFormationCommands.Value.Add(entity);
                        ref var squad = ref _squads.Value.Get(entity);
                        ref var formation = ref _formations.Value.Get(entity);
                        var squadLength = squad.MinColumnsCount * squad.DistanceBetweenUnits;

                        formation.Forward = forward;
                        
                        command.Position = position;
                        command.LookDirection = forward;

                        rebuildCommand.FormationType = FormationType.Rectangle;
                        rebuildCommand.Columns = squad.MinColumnsCount;

                        _displayPreviewCommands.Value.Del(entity);
                        _disablePreviewCommands.Value.Add(entity);
                        position += right * squadLength + right * DistanceBetweenSquads;
                    }
                }

                _dragTime = 0f;
            }
        }

        private void SetPreview(int entity, DisplayPreview targetPreview)
        {
            if (_displayPreviewCommands.Value.Has(entity))
            {
                ref var preview = ref _displayPreviewCommands.Value.Get(entity);
                preview = targetPreview;
            }
            else
            {
                ref var preview = ref _displayPreviewCommands.Value.Add(entity);
                preview = targetPreview;
            }
        }
    }
}