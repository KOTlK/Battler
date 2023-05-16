using Game.Runtime.Characters.Components;
using Game.Runtime.Squads.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Runtime.Squads.Systems
{
    public class DisablePreviewSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Squad, DisablePreview>> _filter = default;
        private readonly EcsPoolInject<Squad> _squads = default;
        private readonly EcsPoolInject<CharacterPreview> _characterPreviews = default;
        private readonly EcsPoolInject<EnablePreview> _enablePreivewCommands = default;
        private readonly EcsPoolInject<DisablePreview> _disablePreviewCommands = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var squad = ref _squads.Value.Get(entity);

                foreach (var characterEntity in squad.AllMembers)
                {
                    if (_enablePreivewCommands.Value.Has(characterEntity))
                    {
                        ref var preview = ref _characterPreviews.Value.Get(characterEntity);
                        preview.Instance.Hide();
                        _enablePreivewCommands.Value.Del(characterEntity);
                    }
                }

                _disablePreviewCommands.Value.Del(entity);
            }
        }
    }
}