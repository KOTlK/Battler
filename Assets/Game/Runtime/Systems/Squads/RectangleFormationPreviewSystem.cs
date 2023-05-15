using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Game.Runtime.Components.Squads.Formations;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Runtime.Systems.Squads
{
    public class RectangleFormationPreviewSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Squad, Formation, DisplayPreview>> _squads = default;
        private readonly EcsPoolInject<CharacterPreview> _characterPreviews = default;
        private readonly EcsPoolInject<Squad> _squadsStash = default;
        private readonly EcsPoolInject<DisplayPreview> _previews = default;
        private readonly EcsPoolInject<EnablePreview> _enablePreviewCommands = default;


        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _squads.Value)
            {
                ref var squad = ref _squadsStash.Value.Get(entity);
                ref var command = ref _previews.Value.Get(entity);
                var lookDirection = command.Forward;
                var startPosition = command.StartPosition;
                var offset = Vector3.Cross(lookDirection, Vector3.down).normalized * squad.DistanceBetweenUnits;
                var backwards = -lookDirection * squad.DistanceBetweenUnits;
                var previousLocalPosition = Vector3.zero;
                var localPosition = Vector3.zero;
                var currentColumn = 0;

                foreach (var characterEntity in squad.AliveMembers)
                {
                    ref var preview = ref _characterPreviews.Value.Get(characterEntity);
                    
                    if (_enablePreviewCommands.Value.Has(characterEntity) == false)
                    {
                        _enablePreviewCommands.Value.Add(characterEntity);
                    }
                    
                    if (preview.Instance.Hidden)
                    {
                        preview.Instance.Show();
                    }
                    preview.Position = localPosition + startPosition;

                    currentColumn++;
                    localPosition += offset;
                    if (currentColumn >= command.MaxColumns)
                    {
                        currentColumn = 0;
                        localPosition = previousLocalPosition;
                        localPosition += backwards;
                        previousLocalPosition = localPosition;
                    }
                }
            }
        }
    }
}