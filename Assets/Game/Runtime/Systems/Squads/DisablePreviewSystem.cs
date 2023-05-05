using Game.Runtime.Application;
using Game.Runtime.Components.Characters;
using Game.Runtime.Components.Squads;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Game.Runtime.Systems.Squads
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class DisablePreviewSystem : UpdateSystem
    {
        private Filter _filter;
        
        public DisablePreviewSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Squad>().With<DisablePreview>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var squad = ref entity.GetComponent<Squad>();

                foreach (var characterEntity in squad.Members)
                {
                    ref var preview = ref characterEntity.GetComponent<CharacterPreview>();
                    preview.Instance.Hide();
                    characterEntity.RemoveComponent<EnablePreview>();
                }

                entity.RemoveComponent<DisablePreview>();
            }
        }

        public override void Dispose()
        {

        }
    }
}