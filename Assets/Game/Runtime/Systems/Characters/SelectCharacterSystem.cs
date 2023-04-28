using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Characters.Movement;
using Game.Runtime.MonoHell.View.Selection;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Game.Runtime.Systems.Characters
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class SelectCharacterSystem : UpdateSystem
    {
        private readonly CharacterSelectionArea _view;
        private readonly Camera _camera;
        
        private Filter _unselectedEntities;
        private Filter _selectedEntities;
        private Vector3 _startDrag = Vector3.zero;
        private float _delay = 0;
        
        private const float Threshold = 0.1f;
        
        public SelectCharacterSystem(World world, CharacterSelectionArea view) : base(world)
        {
            _view = view;
            _camera = Camera.main;
        }

        public override void OnAwake()
        {
            _unselectedEntities = World.Filter.With<Character>().With<MovableCharacter>().Without<Selected>();
            _selectedEntities = World.Filter.With<Character>().With<MovableCharacter>().With<Selected>();
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
                    var size = new Vector3(
                        Math.Abs(endDrag.x - _startDrag.x),
                        Math.Abs(endDrag.y - _startDrag.y));
                    var rect = new SelectArea()
                    {
                        Position = center,
                        HalfExtents = size * 0.5f
                    };

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
                
                if (_delay < Threshold)
                {
                    _delay = 0f;
                    _startDrag = Vector3.zero;
                    return;
                }

                var endDrag = _camera.ScreenToViewportPoint(Input.mousePosition);
                var center = _startDrag + (endDrag - _startDrag) * 0.5f;
                var size = new Vector3(
                    Math.Abs(endDrag.x - _startDrag.x),
                    Math.Abs(endDrag.y - _startDrag.y));
                var rect = new SelectArea()
                {
                    Position = center,
                    HalfExtents = size * 0.5f
                };
                
                foreach (var entity in _unselectedEntities)
                {
                    ref var movable = ref entity.GetComponent<MovableCharacter>();
                    var viewportPosition = _camera.WorldToViewportPoint(movable.Position);
                    viewportPosition.z = 0;
                    if (rect.Contains(viewportPosition))
                    {
                        ref var view = ref entity.GetComponent<CharacterView>();
                        view.Instance.Highlight();
                        entity.AddComponent<Selected>();
                    }
                }

                _delay = 0f;
                _startDrag = Vector3.zero;
            }
        }

        public override void Dispose()
        {

        }

        private void RemoveSelection()
        {
            foreach (var entity in _selectedEntities)
            {
                ref var view = ref entity.GetComponent<CharacterView>();
                view.Instance.StopHighlighting();
                entity.RemoveComponent<Selected>();
            }
            _view.Clear();
        }
    }
}