using System;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.View.Selection;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Time = Game.Runtime.Application.Time;

namespace Game.Runtime.Systems.Squads
{
    public class SelectSquadSystem : IEcsRunSystem
    {
        private readonly SelectionArea _view;
        private readonly SelectedSquads _selectedSquads;
        private readonly Camera _camera;
        private readonly EcsFilterInject<Inc<Squad>, Exc<Selected, Dead>> _unselectedSquadsFilter = default;
        private readonly EcsFilterInject<Inc<Squad, Selected>> _selectedSquadsFilter = default;
        private readonly EcsCustomInject<Time> _time = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<Selected> _selected = default;
        private readonly EcsPoolInject<MovableCharacter> _movableCharacters = default;
        private readonly EcsPoolInject<CharacterView> _characterViews = default;

        private Vector3 _startDrag = Vector3.zero;
        private float _delay = 0;
        
        private const float Threshold = 0.1f;
        
        public SelectSquadSystem(SelectionArea view, SelectedSquads selectedSquads)
        {
            _view = view;
            _selectedSquads = selectedSquads;
            _camera = Camera.main;
        }

        public void Run(IEcsSystems systems)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RemoveSelection();
            }
            
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (_delay >= Threshold)
                {
                    if (_startDrag == Vector3.zero)
                    {
                        _startDrag = _camera.ScreenToViewportPoint(Input.mousePosition);
                        _startDrag.z = 0;
                    }
                    
                    var endDrag = _camera.ScreenToViewportPoint(Input.mousePosition);
                    var center = _startDrag + (endDrag - _startDrag) * 0.5f;
                    var size = new Vector2(
                        Math.Abs(endDrag.x - _startDrag.x),
                        Math.Abs(endDrag.y - _startDrag.y));
                    var rect = new Area(center, size * 0.5f);

                    _view.DrawRect(rect);
                }
                else
                {
                    _delay += _time.Value.DeltaTime;
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                RemoveSelection();
                _selectedSquads.MinDistance = 0;
                _selectedSquads.Squads.Clear();
                
                if (_delay < Threshold)
                {
                    _delay = 0f;
                    _startDrag = Vector3.zero;
                    return;
                }

                var endDrag = _camera.ScreenToViewportPoint(Input.mousePosition);
                var center = _startDrag + (endDrag - _startDrag) * 0.5f;
                var size = new Vector2(
                    Math.Abs(endDrag.x - _startDrag.x),
                    Math.Abs(endDrag.y - _startDrag.y));
                var rect = new Area(center, size * 0.5f);
                
                foreach (var squadEntity in _unselectedSquadsFilter.Value)
                {
                    ref var squad = ref _squads.Value.Get(squadEntity);

                    foreach (var characterEntity in squad.AliveMembers)
                    {
                        ref var movableCharacter = ref _movableCharacters.Value.Get(characterEntity);
                        var viewportPosition = _camera.WorldToViewportPoint(movableCharacter.Position);
                    
                        if (rect.Contains(viewportPosition))
                        {
                            Select(ref squad);
                            _selected.Value.Add(squadEntity);
                            _selectedSquads.MinDistance += squad.DistanceBetweenUnits * squad.MinColumnsCount;
                            _selectedSquads.Squads.Add(squad);
                            break;
                        }
                    }
                }

                _delay = 0f;
                _startDrag = Vector3.zero;
            }
        }

        private void Select(ref Squad squad)
        {
            foreach (var entity in squad.AliveMembers)
            {
                ref var view = ref _characterViews.Value.Get(entity);
                view.Instance.Highlight();
            }
        }

        private void RemoveSelection()
        {
            foreach (var entity in _selectedSquadsFilter.Value)
            {
                ref var squad = ref _squads.Value.Get(entity);
                
                foreach (var characterEntity in squad.AllMembers)
                {
                    ref var view = ref _characterViews.Value.Get(characterEntity);
                    view.Instance.StopHighlighting();
                }

                _selected.Value.Del(entity);
            }
            _view.Clear();
        }
    }
}