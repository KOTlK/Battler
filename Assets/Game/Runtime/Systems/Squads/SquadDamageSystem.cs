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
    public class SquadDamageSystem : UpdateSystem
    {
        private Filter _filter;
        
        public SquadDamageSystem(World world) : base(world)
        {
        }

        public override void OnAwake()
        {
            _filter = World.Filter.With<Squad>().With<DamageBuffer>().Without<Dead>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var squad = ref entity.GetComponent<Squad>();
                ref var buffer = ref entity.GetComponent<DamageBuffer>();

                if (buffer.Buffer.Count == 0)
                {
                    continue;
                }

                var aliveCount = squad.AliveMembers.Count;

                if (aliveCount == 0)
                {
                    entity.AddComponent<Dead>();
                    continue;
                }

                if (aliveCount == 1)
                {
                    var characterEntity = squad.AliveMembers[0];
                    
                    ref var health = ref characterEntity.GetComponent<Health>();
                    
                    while (health.Current > 0 && buffer.Buffer.Count > 0)
                    {
                        health.Current -= buffer.Buffer.Dequeue();
                    }
                    
                    if (health.Current <= 0)
                    {
                        ref var view = ref characterEntity.GetComponent<CharacterView>();
                        view.Instance.PlayDeathAnimation();
                        squad.DeadMembers.Add(characterEntity);
                        squad.AliveMembers.RemoveAt(0);
                    }
                }

                for (var i = 0; i < aliveCount - 1; i++)
                {
                    var characterEntity = squad.AliveMembers[i];
                    
                    if (buffer.Buffer.Count == 0)
                    {
                        break;
                    }
                    
                    ref var health = ref characterEntity.GetComponent<Health>();

                    while (health.Current > 0 && buffer.Buffer.Count > 0)
                    {
                        health.Current -= buffer.Buffer.Dequeue();
                    }
                    
                    if (health.Current <= 0)
                    {
                        ref var view = ref characterEntity.GetComponent<CharacterView>();
                        view.Instance.PlayDeathAnimation();
                        squad.DeadMembers.Add(characterEntity);
                        squad.AliveMembers.RemoveAt(i);
                    }
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}