using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.Components.Squads;
using Game.Runtime.MonoHell.View.Selection;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class SelectSquadSystem : UpdateSystem
    {
        private readonly SelectionArea _view;
        private readonly SelectedSquads _selectedSquads;
        private readonly Camera _camera;
        
        private Filter _unselected;
        private Filter _selected;
        private Vector3 _startDrag = Vector3.zero;
        private float _delay = 0;
        
        private const float Threshold = 0.1f;
        
        public SelectSquadSystem(World world, SelectionArea view, SelectedSquads selectedSquads) : base(world)
        {
            _view = view;
            _selectedSquads = selectedSquads;
            _camera = Camera.main;
        }

        public override void OnAwake()
        {
            _unselected = World.Filter.With<Squad>().Without<Selected>();
            _selected = World.Filter.With<Squad>().With<Selected>();
        }

        public override void OnUpdate(float deltaTime)
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
                    _delay += deltaTime;
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
                
                foreach (var squadEntity in _unselected)
                {
                    ref var squad = ref squadEntity.GetComponent<Squad>();

                    foreach (var characterEntity in squad.Members)
                    {
                        ref var health = ref characterEntity.GetComponent<Health>();

                        if (health.Current <= 0)
                        {
                            continue;
                        }
                        
                        ref var movableCharacter = ref characterEntity.GetComponent<MovableCharacter>();
                        var viewportPosition = _camera.WorldToViewportPoint(movableCharacter.Position);
                    
                        if (rect.Contains(viewportPosition))
                        {
                            Select(ref squad);
                            squadEntity.AddComponent<Selected>();
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

        public override void Dispose()
        {

        }

        private void Select(ref Squad squad)
        {
            foreach (var entity in squad.Members)
            {
                ref var health = ref entity.GetComponent<Health>();

                if (health.Current > 0)
                {
                    ref var view = ref entity.GetComponent<CharacterView>();
                    view.Instance.Highlight();
                }
            }
        }

        private void RemoveSelection()
        {
            foreach (var entity in _selected)
            {
                ref var squad = ref entity.GetComponent<Squad>();
                
                foreach (var characterEntity in squad.Members)
                {
                    ref var view = ref characterEntity.GetComponent<CharacterView>();
                    view.Instance.StopHighlighting();
                }
                
                entity.RemoveComponent<Selected>();
            }
            _view.Clear();
        }
    }
}