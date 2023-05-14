using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
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
    public class RectangleFormationPreviewSystem : UpdateSystem
    {
        private Filter _squads;
        private Stash<CharacterPreview> _characterPreviews;
        private Stash<Squad> _squadsStash;
        private Stash<Formation> _formations;
        private Stash<DisplayPreview> _previews;

        public RectangleFormationPreviewSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _squads = World.Filter.With<Squad>().With<Formation>().With<DisplayPreview>();
            _squadsStash = World.GetStash<Squad>();
            _formations = World.GetStash<Formation>();
            _previews = World.GetStash<DisplayPreview>();
            _characterPreviews = World.GetStash<CharacterPreview>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _squads)
            {
                ref var squad = ref _squadsStash.Get(entity);
                ref var command = ref _previews.Get(entity);
                var lookDirection = command.Forward;
                var startPosition = command.StartPosition;
                var offset = Vector3.Cross(lookDirection, Vector3.down).normalized * squad.DistanceBetweenUnits;
                var backwards = -lookDirection * squad.DistanceBetweenUnits;
                var previousLocalPosition = Vector3.zero;
                var localPosition = Vector3.zero;
                var currentColumn = 0;

                foreach (var characterEntity in squad.AliveMembers)
                {
                    ref var preview = ref _characterPreviews.Get(characterEntity);
                    characterEntity.SetComponent(new EnablePreview());
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

        public override void Dispose()
        {

        }
    }
}