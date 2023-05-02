using System;
using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Game.Runtime.MonoHell.Configs;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class RectangleFormationPreviewSystem : UpdateSystem
    {
        private readonly Config _config;
        private Filter _squads;
        
        public RectangleFormationPreviewSystem(World world, Config config) : base(world)
        {
            _config = config;
        }

        public override void OnAwake()
        {
            _squads = World.Filter.With<Squad>().With<RectangleFormation>().With<DisplayPreview>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _squads)
            {
                ref var squad = ref entity.GetComponent<Squad>();
                ref var formation = ref entity.GetComponent<RectangleFormation>();
                ref var command = ref entity.GetComponent<DisplayPreview>();
                var direction = command.EndPosition - command.StartPosition;
                var lookDirection = Vector3.Cross(direction, Vector3.up).normalized;
                var startPosition = command.StartPosition;
                var offset = Vector3.Cross(lookDirection, Vector3.down).normalized * squad.DistanceBetweenUnits;
                var backwards = -lookDirection * squad.DistanceBetweenUnits;
                var previousLocalPosition = Vector3.zero;
                var localPosition = Vector3.zero;
                var currentColumn = 0;

                formation.MaxColumns = (int)Math.Floor(Vector3.Distance(command.EndPosition, command.StartPosition) / squad.DistanceBetweenUnits);

                foreach (var characterEntity in squad.Members)
                {
                    ref var health = ref characterEntity.GetComponent<Health>();
                    if (health.Current <= 0)
                    {
                        continue;
                    }

                    ref var view = ref characterEntity.GetComponent<CharacterView>();
                    if (view.PositionPreview == null)
                    {
                        var effect = Object.Instantiate(_config.PositionPreviewEffect, localPosition + startPosition,
                            Quaternion.identity);
                        view.PositionPreview = effect;
                        effect.Play();
                    }
                    else
                    {
                        view.PositionPreview.transform.position = localPosition + startPosition;
                        view.PositionPreview.Play();
                    }

                    currentColumn++;
                    localPosition += offset;
                    if (currentColumn >= formation.MaxColumns)
                    {
                        currentColumn = 0;
                        localPosition = previousLocalPosition;
                        localPosition += backwards;
                        previousLocalPosition = localPosition;
                    }
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}